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
using Kore.Collections.Generic;
using Kore.Web.ContentManagement.Areas.Admin.Messaging;
using Kore.Web.ContentManagement.Areas.Admin.Messaging.Services;

namespace Kore.Plugins.Messaging.Forums.Controllers
{
    [Authorize]
    [RouteArea("")]
    [RoutePrefix("pm")]
    public class PrivateMessagesController : KoreController
    {
        private readonly IFriendService friendService;
        private readonly IForumService forumService;
        private readonly ForumSettings forumSettings;
        private readonly IMembershipService membershipService;
        private readonly IMessageService messageService;

        public PrivateMessagesController(
            IFriendService friendService,
            IForumService forumService,
            IMembershipService membershipService,
            IMessageService messageService,
            ForumSettings forumSettings)
        {
            this.friendService = friendService;
            this.forumService = forumService;
            this.membershipService = membershipService;
            this.messageService = messageService;
            this.forumSettings = forumSettings;

            WorkContext.Breadcrumbs.Add("MailBox");
        }

        [Route("create/{toUserId?}")]
        public async Task<ActionResult> PMCreate(string toUserId)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var blockedUser = await forumService.GetBlockedUserById(WorkContext.CurrentUser.Id, toUserId, true);
            var blockedByUser = await forumService.GetBlockedUserById(toUserId, WorkContext.CurrentUser.Id, true);

            if (blockedUser != null || blockedByUser != null)
            {
                return new HttpUnauthorizedResult();
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
            return await PMSendPost(model);
        }

        [HttpPost]
        [FrontendAntiForgery]
        [Route("confirm")]
        [ValidateInput(false)]
        public async Task<ActionResult> PMSendPost(PrivateMessageModel model)
        {
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
                    var toUser = await membershipService.GetUserById(model.ToUserId);

                    var tokens = new List<Token>
                    {
                        new Token("[FromUserName]", await membershipService.GetUserDisplayName(fromUser)),
                        new Token("[UserName]", await membershipService.GetUserDisplayName(toUser)),

                        // May need to map image path to https://www.godofwonders.co.uk
                        new Token("[GOWImage]", Url.Content(Server.MapPath("~/Media/GOW_Logo.png")))
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
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            WorkContext.Breadcrumbs.Add("Inbox");
            WorkContext.Breadcrumbs.Add("PM");

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

                var blockedByUser = await forumService.GetBlockedUserById(pm.FromUserId, WorkContext.CurrentUser.Id, true);
                model.BlockedByUser = blockedByUser != null && blockedByUser.IsBlocked;

                var blockedUser = await forumService.GetBlockedUserById(WorkContext.CurrentUser.Id, pm.FromUserId);
                model.HasBlockedUser = blockedUser != null && blockedUser.IsBlocked;

                var friend = await friendService.GetFriendByIdAsync(WorkContext.CurrentUser.Id, pm.FromUserId);
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
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            WorkContext.Breadcrumbs.Add("Sent");
            WorkContext.Breadcrumbs.Add("PM");

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

                var blockedByUser = await forumService.GetBlockedUserById(spm.ToUserId, WorkContext.CurrentUser.Id, true);
                model.BlockedByUser = blockedByUser != null && blockedByUser.IsBlocked;

                var blockedUser = await forumService.GetBlockedUserById(WorkContext.CurrentUser.Id, spm.ToUserId);
                model.HasBlockedUser = blockedUser != null && blockedUser.IsBlocked;

                var friend = await friendService.GetFriendByIdAsync(WorkContext.CurrentUser.Id, spm.ToUserId);
                model.IsFriend = friend != null;

                return View(model);
            }
            return RedirectToAction("MailBox");
        }

        [Route("mailbox")]
        public async Task<ActionResult> MailBox()
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var model = new MailBoxModel();

            var privateMessages = await forumService.GetAllPrivateMessages("", WorkContext.CurrentUser.Id, null, null, false, "");
            var sentMessages = await forumService.GetAllPrivateMessages(WorkContext.CurrentUser.Id, "", null, false, null, "");

            foreach (PrivateMessage pm in privateMessages)
            {
                var pmModel = new PrivateMessageModel
                {
                    Id = pm.Id,
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

                string isAvatarApprovedValue = await membershipService.GetProfileEntry(pm.FromUserId, "IsAvatarApproved");
                pmModel.IsFromUserAvatarApproved = bool.Parse(isAvatarApprovedValue);

                pmModel.FromUserAvatarUrl = Kore.Web.Html.HtmlHelper.FormatText(pmModel.FromUserAvatarUrl, false, true, false, true, false, false);

                model.AllPrivateMessages.Add(pmModel);
            }

            foreach (PrivateMessage spm in sentMessages)
            {
                var sPMModel = new PrivateMessageModel
                {
                    Id = spm.Id,
                    Subject = spm.Subject,
                    IsRead = spm.IsRead,
                    IsDeletedByAuthor = spm.IsDeletedByAuthor,
                    CreatedOnUtc = spm.CreatedOnUtc,
                    ToUserAvatarUrl = await membershipService.GetProfileEntry(spm.ToUserId, "AvatarUrl")
                };

                var fromUser = await membershipService.GetUserById(spm.FromUserId);
                sPMModel.FromUser = await membershipService.GetUserDisplayName(fromUser);

                var toUser = await membershipService.GetUserById(spm.ToUserId);
                sPMModel.ToUser = await membershipService.GetUserDisplayName(toUser);

                string isAvatarApprovedValue = await membershipService.GetProfileEntry(spm.ToUserId, "IsAvatarApproved");
                sPMModel.IsToUserAvatarApproved = bool.Parse(isAvatarApprovedValue);

                sPMModel.ToUserAvatarUrl = Kore.Web.Html.HtmlHelper.FormatText(sPMModel.ToUserAvatarUrl, false, true, false, true, false, false);

                model.SentMessages.Add(sPMModel);
            }
            return View(model);
        }

        [Route("block/user/{blockedUserId?}/{isBlocked}")]
        public async Task<ActionResult> BlockUser(string blockedUserId, bool isBlocked)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var blockedUser = await forumService.GetBlockedUserById(WorkContext.CurrentUser.Id, blockedUserId);

            if (blockedUser == null)
            {
                var bu = new BlockedUser()
                {
                    BlockedByUserId = WorkContext.CurrentUser.Id,
                    BlockedUserId = blockedUserId,
                    IsBlocked = true
                };
                await forumService.InsertBlockedUser(bu);
            }
            else
            {
                blockedUser.IsBlocked = isBlocked;
                await forumService.UpdateBlockedUser(blockedUser);
            }

            string url = Request.UrlReferrer.AbsolutePath;
            return Redirect(url);
        }
    }
}


