﻿using System.Collections.Generic;
using Kore.Localization;

namespace KoreCMS.Infrastructure
{
    public class DefaultLocalizableStringsProvider : IDefaultLocalizableStringsProvider
    {
        #region IDefaultLocalizableStringsProvider Members

        public ICollection<Translation> GetTranslations()
        {
            return new[]
            {
                new Translation
                {
                    CultureCode = null,
                    LocalizedStrings = new Dictionary<string, string>
                    {
                        { LocalizableStrings.Account.ConfirmPassword, "Confirm Password" },
                        { LocalizableStrings.Account.ConfirmNewPassword, "Confirm New Password" },
                        { LocalizableStrings.Account.Email, "Email" },
                        { LocalizableStrings.Account.NewPassword, "New Password" },
                        { LocalizableStrings.Account.OldPassword, "Current Password" },
                        { LocalizableStrings.Account.Password, "Password" },
                        { LocalizableStrings.Account.RememberMe, "Remember Me?" },
                        { LocalizableStrings.Dashboard.Title, "Dashboard" },
                    }
                }
            };
        }

        #endregion IDefaultLocalizableStringsProvider Members
    }
}