using System.Collections.Generic;

namespace Kore.Plugins.Messaging.Forums.Models
{
    public class MailBoxModel
    {
        public MailBoxModel()
        {
            this.AllPrivateMessages = new List<PrivateMessageModel>();
            this.SentMessages = new List<PrivateMessageModel>();
        }

        public int InboxPageIndex { get; set; }

        public int SentPageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalInboxHits { get; set; }

        public int TotalSentHits { get; set; }

        public IList<PrivateMessageModel> AllPrivateMessages { get; set; }

        public IList<PrivateMessageModel> SentMessages { get; set; }
    }
}
