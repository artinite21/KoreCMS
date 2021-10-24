namespace Kore.Plugins.Messaging.Forums.Models
{
    public class BlockedUserModel
    {
        public int Id { get; set; }

        public string BlockedUserId { get; set; }

        public string BlockedUserName { get; set; }

        public string BlockedUserAvatarUrl { get; set; }

        public bool IsBlockedUserAvatarApproved { get; set; }

        public bool BlockedByUser { get; set; }

        public bool IsBlocked { get; set; }
    }
}
