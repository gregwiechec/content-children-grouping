using System.Collections.Generic;
using System.Linq;

namespace ContentChildrenGrouping.Core.Extensions
{
    public static class ContentChildrenGroupsLoaderExtensions
    {
        private static List<ContainerConfiguration> GetAllConfigurations(
            this IEnumerable<IContentChildrenGroupsLoader> loaders)
        {
            return loaders.OrderBy(x => x.Rank).SelectMany(x => x.GetConfigurations()).ToList();
        }

        public static List<ContainerConfiguration> GetAllContainersConfigurations(
            this IEnumerable<IContentChildrenGroupsLoader> loaders)
        {
            return loaders.GetAllConfigurations().Where(x => x.IsVirtualContainer == false).ToList();
        }

        public static List<ContainerConfiguration> GetAllVirtualContainersConfigurations(
            this IEnumerable<IContentChildrenGroupsLoader> loaders)
        {
            return loaders.GetAllConfigurations().Where(x => x.IsVirtualContainer).ToList();
        }
    }
}