namespace Kore.Plugins.Messaging.Forums.Models
{
    public class BlockedUserModel
    {
        public int Id { get; set; }

        public string BlockedByUserId { get; set; }

        public string BlockedUserId { get; set; }

        public bool IsBlocked { get; set; }
    }
}
