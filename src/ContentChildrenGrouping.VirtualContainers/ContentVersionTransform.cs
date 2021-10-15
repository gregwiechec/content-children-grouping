using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.Extensions;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Cms.Shell.UI.Rest.Models;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace ContentChildrenGrouping.VirtualContainers
{
    [ServiceConfiguration(typeof(IModelTransform))]
    public class ExtendedContentVersionFilter: ContentVersionFilter
    {
        private readonly VirtualContainersOptions _options;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;
        private readonly IContentLoader _contentLoader;

        public ExtendedContentVersionFilter(
            MissingContentLanguageInformationResolver contentLanguageInformationResolver,
            IContentRepository contentRepository, LanguageResolver languageResolver,
            ServiceAccessor<SiteDefinition> currentSiteDefinition, VirtualContainersOptions options,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders, IContentLoader contentLoader) : base(
            contentLanguageInformationResolver,
            contentRepository, languageResolver, currentSiteDefinition)
        {
            _options = options;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
            _contentLoader = contentLoader;
        }

        public override void TransformInstance(IContent source, StructureStoreContentDataModel target,
            IModelTransformContext context)
        {
            if (!_options.Enabled)
            {
                return;
            }

            AssignVirtualContainerParents(source, target);

            //if (source.IsVirtualContainer() && target.MissingLanguageBranch != null)
            //{
            //    target.MissingLanguageBranch.Reason = LanguageSelectionSource.None;
            //}
        }

        private void AssignVirtualContainerParents(IContent content, StructureStoreContentDataModel target)
        {
            if (content.IsVirtualContainer())
            {
                return;
            }

            var configurations = _contentChildrenGroupsLoaders.GetAllVirtualContainersConfigurations().ToList();
            if (!configurations.Any())
            {
                return;
            }
            var ancestors = _contentLoader.GetAncestors(content.ContentLink).ToList().Select(x => x.ContentLink.ToReferenceWithoutVersion()).ToList();

            var configuration = configurations.FirstOrDefault(x => ancestors.Contains(x.ContainerContentLink.ToReferenceWithoutVersion()));
            if (configuration == null)
            {
                return;
            }

            var result = new List<ContentReference>();
            foreach (var generator in configuration.GroupLevelConfigurations)
            {
                var name = generator.GetName(content);
                result.Add(ExtendedGetChildrenQuery.GetVirtualContentLink(content.ParentLink, name.Replace("_", "-")));

            }

            target.Properties["VirtualContainerParents"] = string.Join(",", result.Select(x => x.ToString()));
        }

        public override TransformOrder Order => TransformOrder.OutputFilter;
    }
}