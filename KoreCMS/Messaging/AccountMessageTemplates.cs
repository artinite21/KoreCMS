﻿using System.Collections.Generic;
using System.Linq;
using Kore.Web.ContentManagement.Areas.Admin.Messaging;

namespace KoreCMS.Messaging
{
    public class AccountMessageTemplates : IMessageTemplatesProvider
    {
        public const string Account_Registered = "Account: Registered";
        public const string Account_Banned = "Account: Banned";
        public const string Account_Confirmed = "Account: Confirmed";
        public const string Account_PasswordReset = "Account: Password Reset";
        public const string Account_ProfileChanged = "Account: Profile Changed";
        public const string Account_Deleted = "Account: Deleted";
        public const string Mailbox_Received = "Mailbox: Received";

        #region IMessageTemplatesProvider Members

        public IEnumerable<MessageTemplate> GetTemplates()
        {
            yield return new MessageTemplate(Account_Registered, "Welcome");
            yield return new MessageTemplate(Account_Banned, "Account Banned");
            yield return new MessageTemplate(Account_Confirmed, "Account Confirmed");
            yield return new MessageTemplate(Account_PasswordReset, "Password Reset");
            yield return new MessageTemplate(Account_ProfileChanged, "Profile Changed");
            yield return new MessageTemplate(Account_Deleted, "Account Deleted");
            yield return new MessageTemplate(Mailbox_Received, "New Message Received!");
        }

        #endregion IMessageTemplatesProvider Members

        #region IMessageTokensProvider Members

        public IEnumerable<string> GetAvailableTokens(string template)
        {
            return Enumerable.Empty<string>();

            //switch (template)
            //{
            //    case Account_PasswordChanged:
            //        break;
            //    case Account_Registered:
            //        break;
            //    case Account_Confirmed:
            //        break;
            //    case Account_PasswordReset:
            //        break;
            //    case Account_ProfileChanged:
            //        break;
            //    default:
            //        return Enumerable.Empty<string>();
            //}
        }

        public void GetTokens(string template, IEnumerable<Token> tokens)
        {
        }

        #endregion IMessageTokensProvider Members
    }
}