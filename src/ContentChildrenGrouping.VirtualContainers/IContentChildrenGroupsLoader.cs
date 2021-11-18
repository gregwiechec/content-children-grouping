using System.Collections.Generic;

namespace ContentChildrenGrouping.VirtualContainers
{
    /// <summary>
    /// Load configurations for containers
    /// </summary>
    public interface IContentChildrenGroupsLoader
    {
        /// <summary>
        /// Rank is used to sort all instances of IContentChildrenGroupsLoader
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Return list of registered configurations
        /// </summary>
        /// <returns></returns>
        IEnumerable<ContainerConfiguration> GetConfigurations();
    }
}