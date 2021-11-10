using System.Collections.Generic;
using System.Linq;

namespace ContentChildrenGrouping.Core.Extensions
{
    public static class ContentChildrenGroupsLoaderExtensions
    {
        public static List<ContainerConfiguration> GetAllConfigurations(
            this IEnumerable<IContentChildrenGroupsLoader> loaders)
        {
            return loaders.OrderBy(x => x.Rank).SelectMany(x => x.GetConfigurations()).ToList();
        }
    }
}