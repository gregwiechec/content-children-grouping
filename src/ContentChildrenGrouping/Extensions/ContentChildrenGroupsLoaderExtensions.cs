using System.Collections.Generic;
using System.Linq;

namespace ContentChildrenGrouping.Extensions
{
    internal static class ContentChildrenGroupsLoaderExtensions
    {
        public static List<ContainerConfiguration> GetAllConfigurations(
            this IEnumerable<IContentChildrenGroupsLoader> loaders)
        {
            return loaders.OrderBy(x => x.Rank).SelectMany(x => x.GetConfigurations()).ToList();
        }
    }
}