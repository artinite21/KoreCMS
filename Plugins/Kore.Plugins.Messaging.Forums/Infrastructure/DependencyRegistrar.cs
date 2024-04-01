using Autofac;
using Kore.Infrastructure;
using Kore.Localization;
using Kore.Plugins.Messaging.Forums.Services;
using Kore.Web.Configuration;
using Kore.Web.ContentManagement.Infrastructure;
using Kore.Web.Infrastructure;
using Kore.Web.Mvc.Themes;
using Kore.Web.Navigation;
using Kore.Web.Plugins;
using Kore.Web.Security.Membership.Permissions;
using Kore.Web.Services;

namespace Kore.Plugins.Messaging.Forums.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar<ContainerBuilder> Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
            {
                return;
            }

            builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();
            builder.RegisterType<WebApiRegistrar>().As<IWebApiRegistrar>().SingleInstance();

            builder.RegisterType<ForumSettings>().As<ISettings>().InstancePerLifetimeScope();
            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();

            builder.RegisterType<ForumPermissions>().As<IPermissionProvider>().SingleInstance();
            builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();

            builder.RegisterType<AutoMenuProvider>().As<IAutoMenuProvider>().SingleInstance();
            builder.RegisterType<ForumService>().As<IForumService>().InstancePerDependency();
            builder.RegisterType<UserManagementService>().As<IUserManagementService>().InstancePerDependency();

            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerDependency();
        }

        public int Order => 9999;

        #endregion IDependencyRegistrar<ContainerBuilder> Members
    }
}