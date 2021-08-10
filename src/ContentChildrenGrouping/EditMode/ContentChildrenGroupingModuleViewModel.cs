using System.Collections.Generic;
using System.Linq;
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


        public ContentChildrenGroupingModuleViewModel(ShellModule shellModule, IClientResourceService clientResourceService,
            IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors, IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders) :
            base(shellModule, clientResourceService, contentRepositoryDescriptors)
        {
            this.ConfigurationContainerLinks = contentChildrenGroupsLoaders.GellAllConfigurations().Select(x=> x.ContainerContentLink.ToReferenceWithoutVersion().ToString());
        }
    }
}