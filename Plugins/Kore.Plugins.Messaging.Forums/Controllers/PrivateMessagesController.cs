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

namespace Kore.Plugins.Messaging.Forums.Controllers
{
    [Authorize]
    [RouteArea("")]
    [RoutePrefix("pm")]
    public class PrivateMessagesController : KoreController
    {
        private readonly IForumService forumService;
        private readonly ForumSettings forumSettings;
        private readonly IMembershipService membershipService;

        public PrivateMessagesController(IForumService forumService,
            IMembershipService membershipService,
            ForumSettings forumSettings)
        {
            this.forumService = forumService;
            this.membershipService = membershipService;
            this.forumSettings = forumSettings;
        }

        [Route("create/{id}/{toUserId?}")]
        public async Task<ActionResult> PMCreate(int id, string toUserId)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var blockedUser = await forumService.GetBlockedUserById(toUserId, WorkContext.CurrentUser.Id, true);

            if (blockedUser != null)
            {
                return RedirectToHomePage();
            }

            var pmUser = await membershipService.GetUserById(toUserId);

            var model = new PrivateMessageModel
            {
                Id = 0,
                ToUserId = toUserId,
                ToUser = pmUser.UserName,
                ForumEditor = forumSettings.ForumEditor
            };

            return View(model);
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

                    return RedirectToAction("MailBox");
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

            var pm = await forumService.GetPrivateMessageById(id);

            var model = new PrivateMessageModel
            {
                Id = pm.Id,
                FromUser = membershipService.GetUserById(pm.FromUserId).Result.UserName,
                FromUserId = pm.FromUserId,
                ToUserId = pm.ToUserId,
                Subject = pm.Subject,
                Text = pm.Text,
                CreatedOnUtc = pm.CreatedOnUtc
            };

            pm.IsRead = true;
            await forumService.UpdatePrivateMessage(pm);

            return View(model);
        }

        [Route("mailbox/delete/{id}")]
        public async Task<ActionResult> PMDelete(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var pm = await forumService.GetPrivateMessageById(id);

            await forumService.DeletePrivateMessage(pm);

            return RedirectToAction("MailBox");
        }

        [Route("mailbox/sentmessage/{id}")]
        public async Task<ActionResult> SentPMView(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var spm = await forumService.GetPrivateMessageById(id);

            var model = new PrivateMessageModel
            {
                Id = spm.Id,
                ToUser = membershipService.GetUserById(spm.ToUserId).Result.UserName,
                ToUserId = spm.ToUserId,
                Subject = spm.Subject,
                Text = spm.Text,
                CreatedOnUtc = spm.CreatedOnUtc
            };

            return View(model);
        }

        [Route("mailbox")]
        public async Task<ActionResult> MailBox()
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var model = new MailBoxModel();

            var privateMessages = await forumService.GetAllPrivateMessages("", WorkContext.CurrentUser.Id, null, null, null, "");
            var sentMessages = await forumService.GetAllPrivateMessages(WorkContext.CurrentUser.Id, "", null, null, null, "");

            foreach (PrivateMessage pm in privateMessages)
            {
                var pmModel = new PrivateMessageModel
                {
                    Id = pm.Id,
                    FromUser = membershipService.GetUserById(pm.FromUserId).Result.UserName,
                    ToUser = membershipService.GetUserById(WorkContext.CurrentUser.Id).Result.UserName,
                    Subject = pm.Subject,
                    IsRead = pm.IsRead,
                    IsDeletedByAuthor = pm.IsDeletedByAuthor,
                    CreatedOnUtc = pm.CreatedOnUtc
                };
                model.AllPrivateMessages.Add(pmModel);
            }

            foreach (PrivateMessage spm in sentMessages)
            {
                var sPMModel = new PrivateMessageModel
                {
                    Id = spm.Id,
                    FromUser = membershipService.GetUserById(spm.FromUserId).Result.UserName,
                    ToUser = membershipService.GetUserById(spm.ToUserId).Result.UserName,
                    Subject = spm.Subject,
                    IsRead = spm.IsRead,
                    IsDeletedByAuthor = spm.IsDeletedByAuthor,
                    CreatedOnUtc = spm.CreatedOnUtc
                };
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

            var blockedUser = await forumService.GetBlockedUserById(WorkContext.CurrentUser.Id, blockedUserId, null);

            if (blockedUser == null)
            {
                var newBU = new BlockedUser()
                {
                    BlockedByUserId = WorkContext.CurrentUser.Id,
                    BlockedUserId = blockedUserId,
                    IsBlocked = true
                };
                await forumService.InsertBlockedUser(newBU);
            }
            else
            {
                var bu = new BlockedUser()
                {
                    Id = blockedUser.Id,
                    BlockedByUserId = WorkContext.CurrentUser.Id,
                    BlockedUserId = blockedUserId,
                    IsBlocked = isBlocked
                };
                await forumService.UpdateBlockedUser(bu);
            }

            string url = Request.UrlReferrer.AbsolutePath;
            return Redirect(url);
        }
    }
}


