﻿using System.Collections.Generic;
using Kore.Localization;

namespace Kore.Web.Infrastructure
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
                        { KoreWebLocalizableStrings.General.Actions, "Actions" },
                        { KoreWebLocalizableStrings.General.Back, "Back" },
                        { KoreWebLocalizableStrings.General.Cancel, "Cancel" },
                        { KoreWebLocalizableStrings.General.Clear, "Clear" },
                        { KoreWebLocalizableStrings.General.Close, "Close" },
                        { KoreWebLocalizableStrings.General.Configuration, "Configuration" },
                        { KoreWebLocalizableStrings.General.Create, "Create" },
                        { KoreWebLocalizableStrings.General.CreateFormat, "Create {0}" },
                        { KoreWebLocalizableStrings.General.Delete, "Delete" },
                        { KoreWebLocalizableStrings.General.Edit, "Edit" },
                        { KoreWebLocalizableStrings.General.EditFormat, "Edit {0}" },
                        { KoreWebLocalizableStrings.General.Enabled, "Enabled" },
                        { KoreWebLocalizableStrings.General.Home, "Home" },
                        { KoreWebLocalizableStrings.General.Install, "Install" },
                        { KoreWebLocalizableStrings.General.Miscellaneous, "Miscellaneous" },
                        { KoreWebLocalizableStrings.General.OK, "OK" },
                        { KoreWebLocalizableStrings.General.OnOff, "On/Off" },
                        { KoreWebLocalizableStrings.General.Preview, "Preview" },
                        { KoreWebLocalizableStrings.General.Save, "Save" },
                        { KoreWebLocalizableStrings.General.SaveAndContinue, "Save & Continue" },
                        { KoreWebLocalizableStrings.General.Search, "Search" },
                        { KoreWebLocalizableStrings.General.Set, "Set" },
                        { KoreWebLocalizableStrings.General.SetDefault, "Set Default" },
                        { KoreWebLocalizableStrings.General.Settings, "Settings" },
                        { KoreWebLocalizableStrings.General.Submit, "Submit" },
                        { KoreWebLocalizableStrings.General.Themes, "Themes" },
                        { KoreWebLocalizableStrings.General.Toggle, "Toggle" },
                        { KoreWebLocalizableStrings.General.Uninstall, "Uninstall" },
                        { KoreWebLocalizableStrings.General.View, "View" },
                        { KoreWebLocalizableStrings.General.ViewFormat, "View {0}" },
                        { KoreWebLocalizableStrings.Indexing.InvalidSearchQuery, "Invalid search query: {0}" },
                        { KoreWebLocalizableStrings.Indexing.NoSearchIndexToManage, "This site does not have a search index to manage." },
                        { KoreWebLocalizableStrings.Indexing.NoSearchIndexToRebuild, "There is no search index to rebuild." },
                        { KoreWebLocalizableStrings.Indexing.Rebuild, "Rebuild" },
                        { KoreWebLocalizableStrings.Indexing.RebuildForFreshStart, "Rebuild the search index for a fresh start:" },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexContainsNDocuments, "The search index contains {0} document/s." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexContainsNoFields, "The search index does not contain any fields." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexContainsTheseFields, "The search index contains the following fields: {0}." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexCurrentlyBeingRebuilt, "The index is currently being rebuilt." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexCurrentlyBeingUpdated, "The index is currently being updated." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexLastUpdatedOn, "The search index was last updated {0}." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexMayBeCorrupted, "The index might be corrupted. If you can't recover click on Rebuild." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexNoDocuments, "The search index does not contain any documents." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexNotBuiltYet, "The search index has not been built yet." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexRebuildFail, "The index, '{0}' could not be rebuilt. It might already be in use. Please try again later." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexRebuilt, "The index, '{0}' has been rebuilt." },
                        { KoreWebLocalizableStrings.Indexing.SearchIndexUpdated, "The search index has been updated." },
                        { KoreWebLocalizableStrings.Indexing.SearchMatchedNDocuments, "Your search for, '<strong>{0}</strong>' resulted in {1} documents." },
                        { KoreWebLocalizableStrings.Indexing.SearchMatchedNoDocuments, "Your search for, '<strong>{0}</strong>' did not match any documents." },
                        { KoreWebLocalizableStrings.Indexing.Title, "Search Index" },
                        { KoreWebLocalizableStrings.Messages.ConfirmDeleteRecord, "Are you sure that you want to delete this record?" },
                        { KoreWebLocalizableStrings.Messages.DeleteRecordError, "There was an error when deleting the record." },
                        { KoreWebLocalizableStrings.Messages.DeleteRecordErrorFormat, "There was an error when deleting the record. Additional information as follows: {0}" },
                        { KoreWebLocalizableStrings.Messages.DeleteRecordSuccess, "Successfully deleted record." },
                        { KoreWebLocalizableStrings.Messages.GetRecordError, "There was an error when retrieving the record." },
                        { KoreWebLocalizableStrings.Messages.InsertRecordError, "There was an error when inserting the record." },
                        { KoreWebLocalizableStrings.Messages.InsertRecordSuccess, "Successfully inserted record." },
                        { KoreWebLocalizableStrings.Messages.InstallPluginError, "There was an error when installing the plugin." },
                        { KoreWebLocalizableStrings.Messages.InstallPluginSuccess, "Successfully installed plugin." },
                        { KoreWebLocalizableStrings.Messages.ResetLocalizableStringsError, "There was an error when resetting the localized strings." },
                        { KoreWebLocalizableStrings.Messages.ResetLocalizableStringsSuccess, "Successfully reset the localized strings." },
                        { KoreWebLocalizableStrings.Messages.SetDesktopThemeError, "Error when setting default desktop theme." },
                        { KoreWebLocalizableStrings.Messages.SetDesktopThemeSuccess, "Successfully set default desktop theme." },
                        { KoreWebLocalizableStrings.Messages.SetMobileThemeError, "Error when setting default mobile theme." },
                        { KoreWebLocalizableStrings.Messages.SetMobileThemeSuccess, "Successfully set default mobile theme." },
                        { KoreWebLocalizableStrings.Messages.UninstallPluginError, "There was an error when uninstalling the plugin." },
                        { KoreWebLocalizableStrings.Messages.UninstallPluginSuccess, "Successfully uninstalled plugin." },
                        { KoreWebLocalizableStrings.Messages.UpdateRecordError, "There was an error when updating the record." },
                        { KoreWebLocalizableStrings.Messages.UpdateRecordErrorFormat, "There was an error when updating the record. Additional information as follows: {0}" },
                        { KoreWebLocalizableStrings.Messages.UpdateRecordSuccess, "Successfully updated record." },
                        { KoreWebLocalizableStrings.Models.ScheduledTasks.Enabled, "Is Enabled" },
                        { KoreWebLocalizableStrings.Models.ScheduledTasks.LastEndUtc, "Last End (UTC)" },
                        { KoreWebLocalizableStrings.Models.ScheduledTasks.LastStartUtc, "Last Start (UTC)" },
                        { KoreWebLocalizableStrings.Models.ScheduledTasks.LastSuccessUtc, "Last Success (UTC)" },
                        { KoreWebLocalizableStrings.Models.ScheduledTasks.Name, "Name" },
                        { KoreWebLocalizableStrings.Models.ScheduledTasks.Seconds, "Seconds" },
                        { KoreWebLocalizableStrings.Models.ScheduledTasks.StopOnError, "Stop On Error" },
                        { KoreWebLocalizableStrings.Models.Settings.Name, "Name" },
                        { KoreWebLocalizableStrings.Models.Themes.PreviewImageUrl, "Preview" },
                        { KoreWebLocalizableStrings.Models.Themes.SupportRtl, "Supports RTL" },
                        { KoreWebLocalizableStrings.Models.Themes.MobileTheme, "For Mobile?" },
                        { KoreWebLocalizableStrings.Models.Themes.IsDefaultDesktopTheme, "Default Desktop Theme" },
                        { KoreWebLocalizableStrings.Models.Themes.IsDefaultMobileTheme, "Default Mobile Theme" },
                        { KoreWebLocalizableStrings.Plugins.ManagePlugins, "Manage Plugins" },
                        { KoreWebLocalizableStrings.Plugins.Title, "Plugins" },
                        { KoreWebLocalizableStrings.ScheduledTasks.ExecutedTaskError, "There was an error when executing the task." },
                        { KoreWebLocalizableStrings.ScheduledTasks.ExecutedTaskSuccess, "Successfully executed task." },
                        { KoreWebLocalizableStrings.ScheduledTasks.ManageScheduledTasks, "Manage Scheduled Tasks" },
                        { KoreWebLocalizableStrings.ScheduledTasks.Title, "Scheduled Tasks" },
                        { KoreWebLocalizableStrings.Settings.Captcha.PrivateKey, "Private Key" },
                        { KoreWebLocalizableStrings.Settings.Captcha.PublicKey, "Public Key" },
                        { KoreWebLocalizableStrings.Settings.Membership.GeneratedPasswordLength, "Length" },
                        { KoreWebLocalizableStrings.Settings.Membership.GeneratedPasswordNumberOfNonAlphanumericChars, "# Non-Alphanumeric Characters" },
                        { KoreWebLocalizableStrings.Settings.Site.AdminLayoutPath, "Admin Layout Path" },
                        { KoreWebLocalizableStrings.Settings.Site.AllowUserToSelectTheme, "Allow User To Select Theme" },
                        { KoreWebLocalizableStrings.Settings.Site.DefaultDesktopTheme, "Default Desktop Theme" },
                        { KoreWebLocalizableStrings.Settings.Site.DefaultFrontendLayoutPath, "Default Frontend Layout Path" },
                        { KoreWebLocalizableStrings.Settings.Site.DefaultGridPageSize, "Default Grid Page Size" },
                        { KoreWebLocalizableStrings.Settings.Site.DefaultLanguage, "Default Language" },
                        { KoreWebLocalizableStrings.Settings.Site.DefaultMetaDescription, "Default Meta Description" },
                        { KoreWebLocalizableStrings.Settings.Site.DefaultMetaKeywords, "Default Meta Keywords" },
                        { KoreWebLocalizableStrings.Settings.Site.DefaultMobileTheme, "Default Mobile Theme" },
                        { KoreWebLocalizableStrings.Settings.Site.HomePageTitle, "Home Page Title" },
                        { KoreWebLocalizableStrings.Settings.Site.SiteName, "Site Name" },
                        { KoreWebLocalizableStrings.Settings.Smtp.DisplayName, "Display Name" },
                        { KoreWebLocalizableStrings.Settings.Smtp.EnableSsl, "Enable SSL" },
                        { KoreWebLocalizableStrings.Settings.Smtp.Host, "Host" },
                        { KoreWebLocalizableStrings.Settings.Smtp.MaxTries, "Max Tries" },
                        { KoreWebLocalizableStrings.Settings.Smtp.MessagesPerBatch, "Messages Per Batch" },
                        { KoreWebLocalizableStrings.Settings.Smtp.Password, "Password" },
                        { KoreWebLocalizableStrings.Settings.Smtp.Port, "Port" },
                        { KoreWebLocalizableStrings.Settings.Smtp.UseDefaultCredentials, "Use Default Credentials" },
                        { KoreWebLocalizableStrings.Settings.Smtp.Username, "Username" },
                        { KoreWebLocalizableStrings.UserProfile.Account.FamilyName, "Family Name" },
                        { KoreWebLocalizableStrings.UserProfile.Account.GivenNames, "Given Name/s" },
                        { KoreWebLocalizableStrings.UserProfile.Account.ShowFamilyNameFirst, "Show Family Name First" },
                        { KoreWebLocalizableStrings.UserProfile.Localization.PreferredLanguage, "Preferred Language" },
                        { KoreWebLocalizableStrings.UserProfile.Mobile.DontUseMobileVersion, "Don't Use Mobile Version" },
                        { KoreWebLocalizableStrings.UserProfile.Theme.PreferredTheme, "Preferred Theme" },
                        { KoreWebLocalizableStrings.Validation.Date, "Please enter a valid date." },
                        { KoreWebLocalizableStrings.Validation.Digits, "Please enter only digits." },
                        { KoreWebLocalizableStrings.Validation.Email, "Please enter a valid email address." },
                        { KoreWebLocalizableStrings.Validation.EqualTo, "Please enter the same value again." },
                        { KoreWebLocalizableStrings.Validation.MaxLength, "Please enter no more than {0} characters." },
                        { KoreWebLocalizableStrings.Validation.MinLength, "Please enter at least {0} characters." },
                        { KoreWebLocalizableStrings.Validation.Number, "Please enter a valid number." },
                        { KoreWebLocalizableStrings.Validation.Range, "Please enter a value between {0} and {1}." },
                        { KoreWebLocalizableStrings.Validation.RangeLength, "Please enter a value between {0} and {1} characters long." },
                        { KoreWebLocalizableStrings.Validation.RangeMax, "Please enter a value less than or equal to {0}." },
                        { KoreWebLocalizableStrings.Validation.RangeMin, "Please enter a value greater than or equal to {0}." },
                        { KoreWebLocalizableStrings.Validation.Required, "This field is required." },
                        { KoreWebLocalizableStrings.Validation.Url, "Please enter a valid URL." },
                    }
                }
            };
        }

        #endregion IDefaultLocalizableStringsProvider Members
    }
}