using Kore.Plugins.Messaging.Forums.Data.Domain;
using Kore.Plugins.Messaging.Forums.Models;
using Kore.Plugins.Messaging.Forums.Services;
using Kore.Security.Membership;
using Kore.Web.Mvc;
using Kore.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using Kore.Web.ContentManagement.Areas.Admin.Messaging;
using Kore.Web.ContentManagement.Areas.Admin.Messaging.Services;
using Kore.Web.Security;
using Kore.Web.Services;

namespace Kore.Plugins.Messaging.Forums.Controllers
{
    [JwtToken]
    [Authorize]
    [RouteArea("")]
    [RoutePrefix("pm")]
    public class PrivateMessagesController : KoreController
    {
        private readonly IUserManagementService userManagementService;
        private readonly IForumService forumService;
        private readonly ForumSettings forumSettings;
        private readonly IMembershipService membershipService;
        private readonly IMessageService messageService;
        private readonly IAuthenticationService authenticationService;

        public PrivateMessagesController(
            IUserManagementService userManagementService,
            IForumService forumService,
            IMembershipService membershipService,
            IMessageService messageService,
            IAuthenticationService authenticationService,
            ForumSettings forumSettings)
        {
            this.userManagementService = userManagementService;
            this.forumService = forumService;
            this.membershipService = membershipService;
            this.messageService = messageService;
            this.authenticationService = authenticationService;
            this.forumSettings = forumSettings;
        }

        #region Utilities

        [NonAction]
        private async Task<bool> IsForumModerator(KoreUser user)
        {
            var roles = await membershipService.GetRolesForUser(user.Id);
            return roles.Any(x => x.Name == Constants.Roles.ForumModerators || x.Name == KoreConstants.Roles.Administrators);
        }

        #endregion Utilities

        [Route("create/{toUserId?}")]
        public async Task<ActionResult> PMCreate(string toUserId)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            WorkContext.Breadcrumbs.Add("MailBox", Url.Action("Mailbox"));
            WorkContext.Breadcrumbs.Add("New Message");

            var blockedUser = await userManagementService.GetBlockedUserByIdAsync(WorkContext.CurrentUser.Id, toUserId, true);
            var blockedByUser = await userManagementService.GetBlockedUserByIdAsync(toUserId, WorkContext.CurrentUser.Id, true);

            if (blockedUser != null || blockedByUser != null)
            {
                return Redirect("/account/blocked");
            }

            var pmUser = await membershipService.GetUserById(toUserId);

            if (pmUser != null)
            {
                var model = new PrivateMessageModel
                {
                    Id = 0,
                    ToUserId = toUserId,
                    ForumEditor = forumSettings.ForumEditor
                };

                model.ToUser = await membershipService.GetUserDisplayName(pmUser);

                return View(model);
            }
            return RedirectToAction("MailBox");
        }

        [HttpPost]
        [FrontendAntiForgery]
        [Route("send")]
        [ValidateInput(false)]
        public async Task<ActionResult> PMSend(PrivateMessageModel model)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            return await PMSendPost(model);
        }

        [HttpPost]
        [FrontendAntiForgery]
        [Route("confirm")]
        [ValidateInput(false)]
        public async Task<ActionResult> PMSendPost(PrivateMessageModel model)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string toUserId = model.ToUserId;
                    string subject = model.Subject;

                    int maxPMSubjectLength = forumSettings.PMSubjectMaxLength;

                    if (maxPMSubjectLength > 0 && subject.Length > maxPMSubjectLength)
                    {
                        subject = subject.Substring(0, maxPMSubjectLength);
                    }

                    string text = model.Text;
                    var maxPMTextLength = forumSettings.PMTextMaxLength;

                    if (maxPMTextLength > 0 && text.Length > maxPMTextLength)
                    {
                        text = text.Substring(0, maxPMTextLength);
                    }

                    DateTime nowUtc = DateTime.UtcNow;

                    var pm = new PrivateMessage
                    {
                        FromUserId = WorkContext.CurrentUser.Id,
                        ToUserId = toUserId,
                        Subject = subject,
                        Text = text,
                        IsRead = false,
                        IsDeletedByAuthor = false,
                        IsDeletedByRecipient = false,
                        CreatedOnUtc = nowUtc
                    };
                    await forumService.InsertPrivateMessage(pm);

                    var fromUser = await membershipService.GetUserById(WorkContext.CurrentUser.Id);
                    var fromUserDisplayName = await membershipService.GetUserDisplayName(fromUser);

                    var toUser = await membershipService.GetUserById(model.ToUserId);
                    var toUserDisplayName = await membershipService.GetUserDisplayName(toUser);

                    var tokens = new List<Token>
                    {
                        new Token("[FromUserDisplayName]", fromUserDisplayName.TrimStart()),
                        new Token("[ToUserDisplayName]", toUserDisplayName.TrimStart()),
                        new Token("[ToUserName]", toUser.UserName.TrimStart())

                        // May need to map image path to https://www.godofwonders.co.uk
                        //new Token("[GOWImage]", Url.Content(Server.MapPath("~/Media/GOW_Logo.png")))
                    };

                    messageService.SendEmailMessage(WorkContext.CurrentTenant.Id, "Mailbox: Received", tokens, toUser.Email);

                    string url = Url.Action("MailBox");
                    return Redirect(string.Format("{0}#{1}", url, "SentButton"));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View("PMCreate", model);
        }

        [Route("mailbox/message/{id}")]
        public async Task<ActionResult> PMView(int id)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            WorkContext.Breadcrumbs.Add("MailBox", Url.Action("Mailbox"));
            WorkContext.Breadcrumbs.Add("Inbox", Url.Action("MailBox") + "#InboxButton");
            WorkContext.Breadcrumbs.Add("Message");

            var pm = await forumService.GetPrivateMessageById(id);

            if (pm != null)
            {
                if (pm.ToUserId != WorkContext.CurrentUser.Id)
                {
                    return new HttpUnauthorizedResult();
                }

                var model = new PrivateMessageModel
                {
                    Id = pm.Id,
                    FromUserId = pm.FromUserId,
                    ToUserId = pm.ToUserId,
                    Subject = pm.Subject,
                    Text = pm.FormatPrivateMessageText(),
                    CreatedOnUtc = pm.CreatedOnUtc,
                    FromUserAvatarUrl = await membershipService.GetProfileEntry(pm.FromUserId, "AvatarUrl")
                };

                var fromUser = await membershipService.GetUserById(pm.FromUserId);
                model.FromUser = await membershipService.GetUserDisplayName(fromUser);

                model.FromUserAvatarUrl = Kore.Web.Html.HtmlHelper.FormatText(model.FromUserAvatarUrl, false, true, false, true, false, false);

                var blockedByUser = await userManagementService.GetBlockedUserByIdAsync(pm.FromUserId, WorkContext.CurrentUser.Id, true);
                model.BlockedByUser = blockedByUser != null && blockedByUser.IsBlocked;

                var blockedUser = await userManagementService.GetBlockedUserByIdAsync(WorkContext.CurrentUser.Id, pm.FromUserId);
                model.HasBlockedUser = blockedUser != null && blockedUser.IsBlocked;

                var friend = await userManagementService.GetFriendByIdAsync(WorkContext.CurrentUser.Id, pm.FromUserId);
                model.IsFriend = friend != null;

                pm.IsRead = true;
                await forumService.UpdatePrivateMessage(pm);

                return View(model);
            }
            return RedirectToAction("MailBox");
        }

        [Route("mailbox/delete/{id}")]
        public async Task<ActionResult> PMDelete(int id)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var pm = await forumService.GetPrivateMessageById(id);

            if (pm != null)
            {
                if (pm.ToUserId != WorkContext.CurrentUser.Id)
                {
                    return new HttpUnauthorizedResult();
                }

                pm.IsDeletedByRecipient = true;
                await forumService.UpdatePrivateMessage(pm);

                if (pm.IsDeletedByAuthor && pm.IsDeletedByRecipient)
                {
                    await forumService.DeletePrivateMessage(pm);
                }
            }
            return RedirectToAction("MailBox");
        }

        [Route("mailbox/deletesent/{id}")]
        public async Task<ActionResult> SentPMDelete(int id)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var spm = await forumService.GetPrivateMessageById(id);

            if (spm != null)
            {
                if (spm.FromUserId != WorkContext.CurrentUser.Id)
                {
                    return new HttpUnauthorizedResult();
                }

                if (!spm.IsRead)
                {
                    await forumService.DeletePrivateMessage(spm);
                }
                else
                {
                    spm.IsDeletedByAuthor = true;
                    await forumService.UpdatePrivateMessage(spm);

                    if (spm.IsDeletedByAuthor && spm.IsDeletedByRecipient)
                    {
                        await forumService.DeletePrivateMessage(spm);
                    }
                }
            }
            return RedirectToAction("MailBox");
        }

        [Route("mailbox/sentmessage/{id}")]
        public async Task<ActionResult> SentPMView(int id)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            WorkContext.Breadcrumbs.Add("MailBox", Url.Action("Mailbox"));
            WorkContext.Breadcrumbs.Add("Sent", Url.Action("MailBox") + "#SentButton");
            WorkContext.Breadcrumbs.Add("Sent Message");

            var spm = await forumService.GetPrivateMessageById(id);

            if (spm != null)
            {
                if (spm.FromUserId != WorkContext.CurrentUser.Id)
                {
                    return new HttpUnauthorizedResult();
                }

                var model = new PrivateMessageModel
                {
                    Id = spm.Id,
                    ToUserId = spm.ToUserId,
                    Subject = spm.Subject,
                    Text = spm.FormatPrivateMessageText(),
                    CreatedOnUtc = spm.CreatedOnUtc
                };

                var toUser = await membershipService.GetUserById(spm.ToUserId);
                model.ToUser = await membershipService.GetUserDisplayName(toUser);

                var blockedByUser = await userManagementService.GetBlockedUserByIdAsync(spm.ToUserId, WorkContext.CurrentUser.Id, true);
                model.BlockedByUser = blockedByUser != null && blockedByUser.IsBlocked;

                var blockedUser = await userManagementService.GetBlockedUserByIdAsync(WorkContext.CurrentUser.Id, spm.ToUserId);
                model.HasBlockedUser = blockedUser != null && blockedUser.IsBlocked;

                var friend = await userManagementService.GetFriendByIdAsync(WorkContext.CurrentUser.Id, spm.ToUserId);
                model.IsFriend = friend != null;

                return View(model);
            }
            return RedirectToAction("MailBox");
        }

        [Route("mailbox")]
        public async Task<ActionResult> MailBox(int page = 1)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            WorkContext.Breadcrumbs.Add("MailBox");

            const int pageSize = 10;

            var model = new MailBoxModel();
            model.PageSize = pageSize;

            var privateMessages = await forumService.GetAllPrivateMessages("", WorkContext.CurrentUser.Id, null, null, false, "", page - 1, pageSize);
            var sentMessages = await forumService.GetAllPrivateMessages(WorkContext.CurrentUser.Id, "", null, false, null, "", page - 1, pageSize);

            model.TotalInboxHits = privateMessages.ItemCount;
            model.InboxPageIndex = privateMessages.PageIndex;
            model.TotalSentHits = sentMessages.ItemCount;
            model.SentPageIndex = sentMessages.PageIndex;

            foreach (PrivateMessage pm in privateMessages)
            {
                var pmModel = new PrivateMessageModel
                {
                    Id = pm.Id,
                    FromUserId = pm.FromUserId,
                    Subject = pm.Subject,
                    IsRead = pm.IsRead,
                    IsDeletedByAuthor = pm.IsDeletedByAuthor,
                    IsDeletedByRecipient = pm.IsDeletedByRecipient,
                    CreatedOnUtc = pm.CreatedOnUtc,
                    FromUserAvatarUrl = await membershipService.GetProfileEntry(pm.FromUserId, "AvatarUrl")
                };

                var fromUser = await membershipService.GetUserById(pm.FromUserId);
                pmModel.FromUser = await membershipService.GetUserDisplayName(fromUser);

                var toUser = await membershipService.GetUserById(WorkContext.CurrentUser.Id);
                pmModel.ToUser = await membershipService.GetUserDisplayName(toUser);

                var blockedByUser = await userManagementService.GetBlockedUserByIdAsync(pm.FromUserId, WorkContext.CurrentUser.Id, true);
                pmModel.BlockedByUser = blockedByUser != null && blockedByUser.IsBlocked;

                string isAvatarApprovedValue = await membershipService.GetProfileEntry(pm.FromUserId, "IsAvatarApproved");
                pmModel.IsFromUserAvatarApproved = bool.Parse(isAvatarApprovedValue);

                pmModel.FromUserAvatarUrl = Kore.Web.Html.HtmlHelper.FormatText(pmModel.FromUserAvatarUrl, false, true, false, true, false, false);

                model.AllPrivateMessages.Add(pmModel);
            }

            foreach (PrivateMessage spm in sentMessages)
            {
                var spmModel = new PrivateMessageModel
                {
                    Id = spm.Id,
                    ToUserId = spm.ToUserId,
                    Subject = spm.Subject,
                    IsRead = spm.IsRead,
                    IsDeletedByAuthor = spm.IsDeletedByAuthor,
                    CreatedOnUtc = spm.CreatedOnUtc,
                    ToUserAvatarUrl = await membershipService.GetProfileEntry(spm.ToUserId, "AvatarUrl")
                };

                var fromUser = await membershipService.GetUserById(spm.FromUserId);
                spmModel.FromUser = await membershipService.GetUserDisplayName(fromUser);

                var toUser = await membershipService.GetUserById(spm.ToUserId);
                spmModel.ToUser = await membershipService.GetUserDisplayName(toUser);

                var blockedByUser = await userManagementService.GetBlockedUserByIdAsync(spm.ToUserId, WorkContext.CurrentUser.Id, true);
                spmModel.BlockedByUser = blockedByUser != null && blockedByUser.IsBlocked;

                string isAvatarApprovedValue = await membershipService.GetProfileEntry(spm.ToUserId, "IsAvatarApproved");
                spmModel.IsToUserAvatarApproved = bool.Parse(isAvatarApprovedValue);

                spmModel.ToUserAvatarUrl = Kore.Web.Html.HtmlHelper.FormatText(spmModel.ToUserAvatarUrl, false, true, false, true, false, false);

                model.SentMessages.Add(spmModel);
            }
            return View(model);
        }

        [Route("block/user/{blockedUserId?}/{isBlocked}")]
        public async Task<ActionResult> BlockUser(string blockedUserId, bool isBlocked)
        {
            if (!authenticationService.ValidateToken(HttpContext))
            {
                return RedirectToRefreshToken();
            }

            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var userToBlock = await membershipService.GetUserById(blockedUserId);

            if (await IsForumModerator(userToBlock))
            {
                return new HttpUnauthorizedResult();
            }

            var blockedUser = await userManagementService.GetBlockedUserByIdAsync(WorkContext.CurrentUser.Id, blockedUserId);

            if (blockedUser == null)
            {
                var bu = new BlockedUser()
                {
                    BlockedByUserId = WorkContext.CurrentUser.Id,
                    BlockedUserId = blockedUserId,
                    IsBlocked = isBlocked
                };
                await userManagementService.InsertBlockedUserAsync(bu);
            }
            else
            {
                await userManagementService.DeleteBlockedUsersByUserIdAsync(blockedUserId);
            }

            string url = Request.UrlReferrer.AbsolutePath;
            return Redirect(url);
        }

        private RedirectToRouteResult RedirectToRefreshToken()
        {
            return RedirectToAction("RefreshToken", "Account", new { returnUrl = Request.Url.ToString() });
        }
    }
}


