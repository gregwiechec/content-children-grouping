using EPiServer.Core;

namespace ContentChildrenGrouping.VirtualContainers
{
    public static class VirtualContainerExtensions
    {
        public const string ProviderPrefix = "VirtualContainers";

        public static bool IsVirtualContainer(this ContentReference contentLink)
        {
            return contentLink.ProviderName?.StartsWith(ProviderPrefix) == true;
        }
        
        public static bool IsVirtualContainer(this IContent content)
        {
            return IsVirtualContainer(content.ContentLink);
        }
    }
}