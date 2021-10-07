using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Extensions;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.RegisterFromDb
{
    /// <summary>
    /// Clear content structure
    /// </summary>
    [ServiceConfiguration(typeof(ContentStructureCleaner))]
    public class ContentStructureCleaner
    {
        private readonly IContentRepository _contentRepository;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _childrenGroupsLoaders;
        private readonly ContentChildrenGroupingOptions _childrenGroupingOptions;

        public ContentStructureCleaner(IContentRepository contentRepository,
            IEnumerable<IContentChildrenGroupsLoader> childrenGroupsLoaders,
            ContentChildrenGroupingOptions childrenGroupingOptions
            )
        {
            _contentRepository = contentRepository;
            _childrenGroupsLoaders = childrenGroupsLoaders;
            _childrenGroupingOptions = childrenGroupingOptions;
        }

        public bool ClearContainers(ContentReference containerContentLink, out string message)
        {
            if (_childrenGroupingOptions.StructureUpdateEnabled)
            {
                //TODO: allow to configure structure clear for configuration
                message = "Cannot clear containers when StructureUpdateEnabled is set";
                return false;
            }

            var containerConfigurations = _childrenGroupsLoaders.GetAllContainersConfigurations();
            var containerConfiguration = containerConfigurations.FirstOrDefault(x=> x.ContainerContentLink.CompareToIgnoreWorkID(containerContentLink));
            if (containerConfiguration == null)
            {
                message = "Configuration not found";
                return false;
            }

            var containerType = containerConfiguration.GetContainerType();

            var totalMoved = 0;
            var totalDeleted = 0;

            void Clean(IContent content)
            {
                var children = _contentRepository.GetChildren<IContent>(content.ContentLink).ToList();
                foreach (var child in children)
                {
                    Clean(child);
                }

                if (containerType.IsInstanceOfType(content))
                {
                    totalDeleted++;
                    _contentRepository.Delete(content.ContentLink, true, AccessLevel.NoAccess);
                }
                else
                {
                    if (!content.ContentLink.CompareToIgnoreWorkID(containerContentLink))
                    {
                        totalMoved++;
                        _contentRepository.Move(content.ContentLink, containerContentLink, AccessLevel.NoAccess,
                            AccessLevel.NoAccess);
                    }
                }
            }

            var containerChildren = _contentRepository.GetChildren<IContent>(containerContentLink).ToList();
            foreach (var child in containerChildren)
            {
                Clean(child);
            }

            message = $"Structure cleared. Total moved: {totalMoved}, total deleted: {totalDeleted}";
            return false;
        }
    }
}