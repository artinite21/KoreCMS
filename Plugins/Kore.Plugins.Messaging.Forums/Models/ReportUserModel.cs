using System.ComponentModel.DataAnnotations;


namespace Kore.Plugins.Messaging.Forums.Models
{
    public class ReportUserModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string ReportedUserId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Details { get; set; }
    }
}
