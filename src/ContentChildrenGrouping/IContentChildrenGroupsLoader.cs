using System.Collections.Generic;
using System.Linq;

namespace ContentChildrenGrouping
{
    /// <summary>
    /// Load configurations for containers
    /// </summary>
    public interface IContentChildrenGroupsLoader
    {
        int Rank { get; }

        IEnumerable<ContainerConfiguration> GetConfigurations();
    }

    internal static class ContentChildrenGroupsLoaderExtensions
    {
        public static List<ContainerConfiguration> GellAllConfigurations(
            this IEnumerable<IContentChildrenGroupsLoader> loaders)
        {
            return loaders.OrderBy(x => x.Rank).SelectMany(x => x.GetConfigurations()).ToList();
        }
    }
}