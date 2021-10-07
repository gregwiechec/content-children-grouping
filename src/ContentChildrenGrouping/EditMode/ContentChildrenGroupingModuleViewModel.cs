using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Extensions;
using ContentChildrenGrouping.VirtualContainers;
using EPiServer.Cms.Shell.Internal;
using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell;
using EPiServer.Shell.Modules;

namespace ContentChildrenGrouping.EditMode
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
            ContentChildrenGroupingOptions childrenGroupingOptions, VirtualContainersOptions virtualContainersOptions) :
            base(shellModule, clientResourceService, contentRepositoryDescriptors)
        {
            ConfigurationContainerLinks = contentChildrenGroupsLoaders.GetAllConfigurations()
                .Select(x => x.ContainerContentLink.ToReferenceWithoutVersion().ToString());
            CustomIconsEnabled = childrenGroupingOptions.CustomIconsEnabled;
            SearchCommandEnabled = childrenGroupingOptions.SearchCommandEnabled;
            VirtualContainersEnabled = virtualContainersOptions.Enabled;
        }
    }
}