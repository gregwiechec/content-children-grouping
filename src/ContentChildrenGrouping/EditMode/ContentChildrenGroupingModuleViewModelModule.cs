using System;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Routing;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Notifications.Feature.Internal;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Configuration;
using EPiServer.DataAbstraction;
using EPiServer.Framework.TypeScanner;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Modules;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace ContentChildrenGrouping.EditMode
{
    public class ContentChildrenGroupingModuleViewModelModule : CmsModule
    {
        private readonly IEnumerable<IContentRepositoryDescriptor> _contentRepositoryDescriptors;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;

        public ContentChildrenGroupingModuleViewModelModule(string name, string routeBasePath, string resourceBasePath) :
            base(name, routeBasePath, resourceBasePath)
        {
            _contentRepositoryDescriptors = ServiceLocator.Current.GetAllInstances<IContentRepositoryDescriptor>();
            _contentChildrenGroupsLoaders = ServiceLocator.Current.GetAllInstances<IContentChildrenGroupsLoader>();
        }

        public ContentChildrenGroupingModuleViewModelModule(string name, string routeBasePath, string resourceBasePath,
            Uri uiUrl, Uri utilUrl, Func<string, string> absolutePathConverter, ITypeScannerLookup typeScannerLookup,
            VirtualPathProvider vpp, IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors,
            CmsUIDefaults cmsUiDefaults, CategoryRepository categoryRepository,
            DisplayResolutionService displayResolutionService, ServiceAccessor<SiteDefinition> currentSiteDefinition,
            EPiServerSection episerverSection, Settings settings, IFrameRepository frameRepository,
            IProjectService projectService, ServiceAccessor<RequestContext> requestContext,
            IFeatureNotificationService featureNotificationService,
            ServiceAccessor<RoutingOptions> routingOptionsAccessor,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders) : base(name, routeBasePath, resourceBasePath, uiUrl,
            utilUrl, absolutePathConverter, typeScannerLookup, vpp, contentRepositoryDescriptors, cmsUiDefaults,
            categoryRepository, displayResolutionService, currentSiteDefinition, episerverSection, settings,
            frameRepository, projectService, requestContext, featureNotificationService, routingOptionsAccessor)
        {
            _contentRepositoryDescriptors = contentRepositoryDescriptors;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
        }

        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
        {
            return new ContentChildrenGroupingModuleViewModel(this, clientResourceService, _contentRepositoryDescriptors, _contentChildrenGroupsLoaders);
        }
    }
}