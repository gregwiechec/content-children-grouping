using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Configuration;
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

            //context.Services.AddTransient<IContentLanguageInformationResolver, ExtendedContentLanguageInformationResolver>();
            //context.Services.AddTransient<IContentLanguageInformationResolver, ExtendedContentLanguageInformationResolver>();

            context.Services.Intercept<IContentCapability>((locator, capability) =>
                new CustomCapability(capability) );
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }

    public class CustomCapability : IContentCapability
    {
        private readonly IContentCapability _capability;

        public CustomCapability(IContentCapability capability)
        {
            _capability = capability;
        }

        public bool IsCapable(IContent content)
        {
            if (_capability.Key == "isPage")
            {
                return true;
            }
            
            if (content.ContentLink?.ProviderName?.Contains("VirtualContainers") == true)
            {
                return false;
            }

            return _capability.IsCapable(content);
        }

        public string Key => _capability.Key;
        public int SortOrder => _capability.SortOrder;
    }
}