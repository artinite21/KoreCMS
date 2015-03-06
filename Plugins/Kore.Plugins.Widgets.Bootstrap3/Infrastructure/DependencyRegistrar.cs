﻿using Autofac;
using Kore.Infrastructure;
using Kore.Localization;
using Kore.Plugins.Widgets.Bootstrap3.Widgets;
using Kore.Web.ContentManagement.Areas.Admin.Widgets;
using Kore.Web.Plugins;

namespace Kore.Plugins.Widgets.Bootstrap3.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar<ContainerBuilder> Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (!PluginManager.IsPluginInstalled("Kore.Plugins.Widgets.Bootstrap3"))
            {
                return;
            }

            builder.RegisterType<DefaultLocalizableStringsProvider>().As<IDefaultLocalizableStringsProvider>().SingleInstance();
            builder.RegisterType<Bootstrap3CarouselWidget>().As<IWidget>().InstancePerDependency();
            builder.RegisterType<Bootstrap3ImageGalleryWidget>().As<IWidget>().InstancePerDependency();
        }

        public int Order
        {
            get { return 9999; }
        }

        #endregion IDependencyRegistrar<ContainerBuilder> Members
    }
}