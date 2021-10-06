using EPiServer.Core;

namespace ContentChildrenGrouping.VirtualContainers
{
    public static class VirtualContainerExtensions
    {
        public static bool IsVirtualContainer(this ContentReference contentLink)
        {
            return contentLink.ProviderName?.StartsWith("VirtualContainers") == true;
        }
        
        public static bool IsVirtualContainer(this IContent content)
        {
            return IsVirtualContainer(content.ContentLink);
        }
    }
}