using Kore.Plugins.Messaging.Forums.Data.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kore.Plugins.Messaging.Forums.Models
{
    public class PrivateMessageModel
    {
        public int Id { get; set; }

        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public string ToUser { get; set; }

        public string FromUser { get; set; }

        [Required]
        public string Subject { get; set; }

        [AllowHtml]
        [Required]
        public string Text { get; set; }

        public bool IsFromUserAvatarApproved { get; set; }

        public bool IsToUserAvatarApproved { get; set; }

        public string FromUserAvatarUrl { get; set; }

        public string ToUserAvatarUrl { get; set; }

        public bool IsRead { get; set; }

        public bool IsDeletedByAuthor { get; set; }

        public bool IsDeletedByRecipient { get; set; }

        public bool BlockedByUser { get; set; }

        public bool HasBlockedUser { get; set; }

        public bool IsFriend { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public EditorType ForumEditor { get; set; }
    }
}
