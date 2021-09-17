using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Extensions;
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

        public bool CustomIconsEnabled { get; set; } = true;

        public bool SearchCommandEnabled { get; set; } = true;

        public ContentChildrenGroupingModuleViewModel(ShellModule shellModule,
            IClientResourceService clientResourceService,
            IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders,
            ContentChildrenGroupingOptions childrenGroupingOptions) :
            base(shellModule, clientResourceService, contentRepositoryDescriptors)
        {
            ConfigurationContainerLinks = contentChildrenGroupsLoaders.GetAllConfigurations()
                .Select(x => x.ContainerContentLink.ToReferenceWithoutVersion().ToString());
            CustomIconsEnabled = childrenGroupingOptions.CustomIconsEnabled;
            SearchCommandEnabled = childrenGroupingOptions.SearchCommandEnabled;
        }
    }
}