using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers
{
    [ModuleDependency(typeof(EPiServer.Shell.UI.InitializationModule))]
    public class VirtualContainersInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.Intercept<IHasChildrenEvaluator>((locator, defaultCache) => new CustomHasChildrenEvaluator(defaultCache));

            //context.Services.Intercept<IModelTransform>(
            //    (locator, defaultCache) => defaultCache.GetType() == typeof(ContentDataModelBaseTransform) ? new CustomContentDataModelBaseTransform() : defaultCache);
            context.Services.AddTransient<MissingContentLanguageInformationResolver, ExtendedMissingContentLanguageInformationResolver>();

            context.Services.Intercept<IContentProviderManager>(
                (locator, defaultCache) => new CustomContentProviderManager(defaultCache));
            context.Services.Intercept<IContentLanguageSettingsHandler>(
                (locator, defaultCache) => new CustomContentLanguageSettingsHandler(defaultCache));
            context.Services.AddTransient<IContentQueryHelper, ExtendedDefaultContentQueryHelper>();

            context.Services.Intercept<IContentCapability>((locator, capability) =>
                new ExtendedCapability(capability) );
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}