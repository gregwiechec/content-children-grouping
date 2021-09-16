using System.Collections.Generic;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Modules;

namespace ContentChildrenGrouping.EditMode
{
    public class ContentChildrenGroupingModuleViewModelModule : ShellModule
    {
        private readonly IEnumerable<IContentRepositoryDescriptor> _contentRepositoryDescriptors;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;
        private readonly ContentChildrenGroupingOptions _childrenGroupingOptions;

        public ContentChildrenGroupingModuleViewModelModule(string name, string routeBasePath, string resourceBasePath) :
            base(name, routeBasePath, resourceBasePath)
        {
            _childrenGroupingOptions = ServiceLocator.Current.GetInstance<ContentChildrenGroupingOptions>();
            _contentRepositoryDescriptors = ServiceLocator.Current.GetAllInstances<IContentRepositoryDescriptor>();
            _contentChildrenGroupsLoaders = ServiceLocator.Current.GetAllInstances<IContentChildrenGroupsLoader>();
        }

        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable,
            IClientResourceService clientResourceService)
        {
            return new ContentChildrenGroupingModuleViewModel(this, clientResourceService, _contentRepositoryDescriptors,  _contentChildrenGroupsLoaders, _childrenGroupingOptions);
        }
    }
}