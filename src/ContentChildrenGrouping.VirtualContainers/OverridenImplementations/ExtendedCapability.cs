using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.Extensions;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers
{
    /// <summary>
    /// Wrapper for all capabilities
    /// </summary>
    public class ExtendedCapability : IContentCapability
    {
        private readonly IContentCapability _capability;

        public ExtendedCapability(IContentCapability capability)
        {
            _capability = capability;
        }

        public bool IsCapable(IContent content)
        {
            if (_capability.Key == "isPage")
            {
                return true;
            }

            if (_capability.Key == IsInVirtualContainer.CapabilityKey)
            {
                return _capability.IsCapable(content);
            }

            // For virtual container capabilities are false
            if (content.IsVirtualContainer())
            {
                return false;
            }

            return _capability.IsCapable(content);
        }

        public string Key => _capability.Key;
        public int SortOrder => _capability.SortOrder;
    }

    /// <summary>
    /// Returns true when content is in virtual container
    /// </summary>
    [ServiceConfiguration(typeof(IContentCapability))]
    public class IsInVirtualContainer : IContentCapability
    {
        public static string CapabilityKey = nameof(IsInVirtualContainer);
        private readonly IContentLoader _contentLoader;
        private readonly VirtualContainersOptions _options;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;

        public IsInVirtualContainer(IContentLoader contentLoader, VirtualContainersOptions options,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders)
        {
            _contentLoader = contentLoader;
            _options = options;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
        }

        public bool IsCapable(IContent content)
        {
            if (!_options.Enabled)
            {
                return false;
            }

            if (content.IsVirtualContainer())
            {
                return false;
            }

            var configurations = _contentChildrenGroupsLoaders.GetAllConfigurations().ToList();
            if (!configurations.Any())
            {
                return false;
            }

            var ancestors = _contentLoader.GetAncestors(content.ContentLink).ToList().Select(x=>x.ContentLink.ToReferenceWithoutVersion()).ToList();

            var hasConfiguration = configurations.Any(x => ancestors.Contains(x.ContainerContentLink.ToReferenceWithoutVersion()));
            return hasConfiguration;
        }

        public string Key => CapabilityKey;
        public int SortOrder => 100;
    }
}