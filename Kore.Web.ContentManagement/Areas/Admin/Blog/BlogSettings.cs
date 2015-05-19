﻿using Kore.ComponentModel;
using Kore.Web.Configuration;

namespace Kore.Web.ContentManagement.Areas.Admin.Blog
{
    public class BlogSettings : ISettings
    {
        public BlogSettings()
        {
            DateFormat = "YYYY-MM-DD HH:mm:ss";
            ItemsPerPage = 5;
            PageTitle = "Blog";
            ShowOnMenus = true;
            MenuPosition = 0;
        }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.BlogSettings.PageTitle)]
        public string PageTitle { get; set; }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.BlogSettings.DateFormat)]
        public string DateFormat { get; set; }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.BlogSettings.ItemsPerPage)]
        public byte ItemsPerPage { get; set; }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.BlogSettings.ShowOnMenus)]
        public bool ShowOnMenus { get; set; }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.BlogSettings.MenuPosition)]
        public byte MenuPosition { get; set; }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.BlogSettings.UseAjax)]
        public bool UseAjax { get; set; }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.BlogSettings.AccessRestrictions)]
        public string AccessRestrictions { get; set; }

        #region ISettings Members

        public string Name
        {
            get { return "CMS: Blog Settings"; }
        }

        public string EditorTemplatePath
        {
            get { return "Kore.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.EditorTemplates.BlogSettings"; }
        }

        #endregion ISettings Members
    }
}