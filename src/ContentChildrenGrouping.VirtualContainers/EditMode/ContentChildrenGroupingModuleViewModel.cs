using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.Extensions;
using EPiServer.Cms.Shell.Internal;
using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell;
using EPiServer.Shell.Modules;

namespace ContentChildrenGrouping.VirtualContainers.EditMode
{
    public class ContentChildrenGroupingModuleViewModel : CmsModuleViewModel
    {
        /// <summary>
        /// List of all configured containers
        /// </summary>
        public IEnumerable<string> ConfigurationContainerLinks { get; set; }

        public bool CustomIconsEnabled { get; set; } = false;

        public bool SearchCommandEnabled { get; set; } = false;

        public bool VirtualContainersEnabled { get; set; } = false;

        public ContentChildrenGroupingModuleViewModel(ShellModule shellModule,
            IClientResourceService clientResourceService,
            IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders,
            VirtualContainersOptions virtualContainersOptions) :
            base(shellModule, clientResourceService, contentRepositoryDescriptors)
        {
            var loaders = contentChildrenGroupsLoaders.ToList();
            ConfigurationContainerLinks = loaders.GetAllConfigurations().Select(x => x.ContainerContentLink.ToReferenceWithoutVersion().ToString());
            CustomIconsEnabled = virtualContainersOptions.CustomIconsEnabled;
            SearchCommandEnabled = virtualContainersOptions.SearchCommandEnabled;
            VirtualContainersEnabled = virtualContainersOptions.Enabled;
        }
    }
}