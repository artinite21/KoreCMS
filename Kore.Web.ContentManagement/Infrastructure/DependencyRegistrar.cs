﻿using Autofac;
using Kore.Infrastructure;
using Kore.Localization;
using Kore.Tasks;
using Kore.Web.Configuration;
using Kore.Web.ContentManagement.Areas.Admin.Blog;
using Kore.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks;
using Kore.Web.ContentManagement.Areas.Admin.Blog.Services;
using Kore.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Kore.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Kore.Web.ContentManagement.Areas.Admin.Localization;
using Kore.Web.ContentManagement.Areas.Admin.Media.ContentBlocks;
using Kore.Web.ContentManagement.Areas.Admin.Media.Services;
using Kore.Web.ContentManagement.Areas.Admin.Menus.Services;
using Kore.Web.ContentManagement.Areas.Admin.Messaging;
using Kore.Web.ContentManagement.Areas.Admin.Messaging.Services;
using Kore.Web.ContentManagement.Areas.Admin.Newsletters;
using Kore.Web.ContentManagement.Areas.Admin.Newsletters.ContentBlocks;
using Kore.Web.ContentManagement.Areas.Admin.Newsletters.Services;
using Kore.Web.ContentManagement.Areas.Admin.Pages;
using Kore.Web.ContentManagement.Areas.Admin.Pages.Services;
using Kore.Web.Indexing.Services;
using Kore.Web.Infrastructure;
using Kore.Web.Mvc.Themes;
using Kore.Web.Navigation;
using Kore.Web.Security.Membership;
using Kore.Web.Security.Membership.Permissions;
using Kore.Web.Services;

namespace Kore.Web.ContentManagement.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();

            #region Services

            // Blog
            builder.RegisterType<BlogCategoryService>().As<IBlogCategoryService>().InstancePerDependency();
            builder.RegisterType<BlogPostService>().As<IBlogPostService>().InstancePerDependency();
            builder.RegisterType<BlogTagService>().As<IBlogTagService>().InstancePerDependency();
            builder.RegisterType<BlogPostTagService>().As<IBlogPostTagService>().InstancePerDependency();

            // Menus
            builder.RegisterType<MenuService>().As<IMenuService>().InstancePerDependency();
            builder.RegisterType<MenuItemService>().As<IMenuItemService>().InstancePerDependency();

            // Messaging
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerDependency();
            builder.RegisterType<MessageService>().As<IQueuedMessageProvider>().InstancePerDependency();
            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerDependency();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerDependency();

            // Pages
            builder.RegisterType<PageService>().As<IPageService>().InstancePerDependency();
            builder.RegisterType<PageTypeService>().As<IPageTypeService>().InstancePerDependency();
            builder.RegisterType<PageVersionService>().As<IPageVersionService>().InstancePerDependency();

            // Content Blocks
            builder.RegisterType<EntityTypeContentBlockService>().As<IEntityTypeContentBlockService>().InstancePerDependency();
            builder.RegisterType<ContentBlockService>().As<IContentBlockService>().InstancePerDependency();
            builder.RegisterType<ZoneService>().As<IZoneService>().InstancePerDependency();

            builder.RegisterType<NewsletterService>().As<INewsletterService>().InstancePerDependency();

            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerDependency();

            #endregion Services

            #region Localization

            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();

            #endregion Localization

            #region Navigation

            builder.RegisterType<CmsNavigationProvider>().As<INavigationProvider>().SingleInstance();

            #endregion Navigation

            #region Security

            // Permissions
            builder.RegisterType<CmsPermissions>().As<IPermissionProvider>().SingleInstance();

            // User Profile Providers
            builder.RegisterType<NewsletterUserProfileProvider>().As<IUserProfileProvider>().SingleInstance();

            #endregion Security

            #region Themes

            builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();

            #endregion Themes

            #region Configuration

            builder.RegisterType<BlogSettings>().As<ISettings>().InstancePerLifetimeScope();
            builder.RegisterType<PageSettings>().As<ISettings>().InstancePerLifetimeScope();

            #endregion Configuration

            #region Content Blocks

            // Blogs
            builder.RegisterType<FilteredPostsBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<LastNPostsBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<TagCloudBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<CategoriesBlock>().As<IContentBlock>().InstancePerDependency();

            // Other
            builder.RegisterType<FormBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<HtmlBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<LanguageSwitchBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<NewsletterSubscriptionBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<VideoBlock>().As<IContentBlock>().InstancePerDependency();

            #endregion Content Blocks

            #region Other: Content Blocks
            
            builder.RegisterType<DefaultContentBlockProvider>().As<IContentBlockProvider>().InstancePerDependency();
            builder.RegisterType<DefaultEntityTypeContentBlockProvider>().As<IEntityTypeContentBlockProvider>().InstancePerDependency();

            #endregion Other: Content Blocks

            #region Other: Media
            
            builder.RegisterType<FileSystemStorageProvider>().As<IStorageProvider>().InstancePerDependency();
            builder.RegisterType<MediaService>().As<IMediaService>().InstancePerDependency();
            builder.RegisterType<MediaPathProvider>().As<IMediaPathProvider>().InstancePerDependency();

            #endregion Other: Media

            #region Other: Messaging

            //builder.RegisterType<SimpleTextParserEngine>().As<IParserEngine>().InstancePerDependency();
            //builder.RegisterType<UrlContentParserEngine>().As<IParserEngine>().InstancePerDependency();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerDependency();

            #endregion Other: Messaging

            // Other
            builder.RegisterType<ResourceBundleRegistrar>().As<IResourceBundleRegistrar>().SingleInstance();
            builder.RegisterType<WebApiRegistrar>().As<IWebApiRegistrar>().SingleInstance();

            // Scheduled Tasks
            builder.RegisterType<ProcessQueuedMailTask>().As<ITask>().SingleInstance();

            // Indexing
            builder.RegisterType<PagesIndexingContentProvider>().As<IIndexingContentProvider>().InstancePerDependency();
            builder.RegisterType<BlogIndexingContentProvider>().As<IIndexingContentProvider>().InstancePerDependency();

            builder.RegisterType<NewsletterMessageTemplates>().As<IMessageTemplatesProvider>().InstancePerDependency();
        }

        public int Order => 1;

        #endregion IDependencyRegistrar Members
    }
}