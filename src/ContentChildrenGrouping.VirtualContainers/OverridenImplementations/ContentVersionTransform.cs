using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.Extensions;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Models;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers
{
    [ServiceConfiguration(typeof(IModelTransform))]
    public class VirtualContainersTransform: TransformBase<StructureStoreContentDataModel>
    {
        private readonly VirtualContainersOptions _options;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;
        private readonly IContentLoader _contentLoader;

        public VirtualContainersTransform(VirtualContainersOptions options,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders, IContentLoader contentLoader)
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

            if (source.IsVirtualContainer())
            {
                return;
            }

            var configurations = _contentChildrenGroupsLoaders.GetAllConfigurations().ToList();
            if (!configurations.Any())
            {
                return;
            }
            var ancestors = _contentLoader.GetAncestors(source.ContentLink).ToList().Select(x => x.ContentLink.ToReferenceWithoutVersion()).ToList();

            var configuration = configurations.FirstOrDefault(x => ancestors.Contains(x.ContainerContentLink.ToReferenceWithoutVersion()));
            if (configuration == null)
            {
                return;
            }

            var result = new List<ContentReference>();
            var generators = configuration.GroupLevelConfigurations.ToList();
            for (var index = 0; index < generators.Count; index++)
            {
                result.Add(VirtualNamesParser.GetVirtualContentLink(source.ParentLink, source, generators, index));
            }

            target.Properties["VirtualContainerParents"] = string.Join(",", result.Select(x => x.ToString()));
        }

        public override TransformOrder Order => TransformOrder.OutputFilter;
    }
}