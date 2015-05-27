﻿using System.Web.Optimization;
using Kore.Web.Infrastructure;

namespace Kore.Web.ContentManagement.Infrastructure
{
    public class ResourceBundleRegistrar : IResourceBundleRegistrar
    {
        #region IResourceBundleRegistrar Members

        public void Register(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/bootbox")
                .Include("~/Scripts/Kore.Web.ContentManagement.Scripts.bootbox.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/bootpag")
                .Include("~/Scripts/Kore.Web.ContentManagement.Scripts.jquery.bootpag.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/blog")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/blog-content")
                .Include("~/Scripts/Kore.Web.ContentManagement.Scripts.jquery.bootpag.min.js")
                .Include("~/Scripts/Kore.Web.ContentManagement.Scripts.moment.js")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Blog.Scripts.blogContent.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/content-blocks")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.index.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/languages")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Localization.Scripts.index.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/localizable-strings")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Localization.Scripts.localizableStrings.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/membership")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Membership.Scripts.membership.js")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Membership.Scripts.permissions.js")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Membership.Scripts.roles.js")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Membership.Scripts.users.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/menus")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Menus.Scripts.menus.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/message-templates")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Messaging.Scripts.messageTemplates.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/newsletters/subscribers")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/pages")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Pages.Scripts.index.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/pages-history")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Pages.Scripts.history.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/kore-cms/queued-emails")
                .Include("~/Scripts/Kore.Web.ContentManagement.Areas.Admin.Messaging.Scripts.queuedEmails.js"));
        }

        #endregion IResourceBundleRegistrar Members
    }
}