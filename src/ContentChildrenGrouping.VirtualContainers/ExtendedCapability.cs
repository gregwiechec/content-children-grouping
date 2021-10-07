using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Core;

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
}