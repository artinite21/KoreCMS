namespace Kore.Web.Models
{
    public class FriendModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FriendId { get; set; }

        public string AvatarUrl { get; set; }

        public string UserName { get; set; }

        public bool IsBlocked { get; set; }

        public bool BlockedByFriend { get; set; }

        public bool IsAvatarApproved { get; set; }
    }
}
