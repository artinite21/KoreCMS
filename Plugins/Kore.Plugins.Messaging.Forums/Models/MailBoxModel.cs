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

        public IList<PrivateMessageModel> AllPrivateMessages { get; set; }

        public IList<PrivateMessageModel> SentMessages { get; set; }
    }
}
