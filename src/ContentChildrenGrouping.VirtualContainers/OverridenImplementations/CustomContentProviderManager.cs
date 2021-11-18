using EPiServer.Core;

namespace ContentChildrenGrouping.VirtualContainers
{
    public class CustomContentProviderManager : IContentProviderManager
    {
        private readonly IContentProviderManager _contentProviderManager;

        public CustomContentProviderManager(IContentProviderManager contentProviderManager)
        {
            _contentProviderManager = contentProviderManager;
        }

        public ContentProvider GetProvider(string providerName)
        {
            return _contentProviderManager.GetProvider(providerName);
        }

        public bool IsCapabilitySupported(ContentReference contentLink, ContentProviderCapabilities capability)
        {
            return _contentProviderManager.IsCapabilitySupported(contentLink, capability);
        }

        public bool HasEntryPointChild(ContentReference contentLink)
        {
            return _contentProviderManager.HasEntryPointChild(contentLink);
        }

        public bool IsWastebasket(ContentReference contentLink)
        {
            if (contentLink.IsVirtualContainer())
            {
                return false;
            }
            return _contentProviderManager.IsWastebasket(contentLink);
        }

        public ContentProviderMap ProviderMap => _contentProviderManager.ProviderMap;
    }
}