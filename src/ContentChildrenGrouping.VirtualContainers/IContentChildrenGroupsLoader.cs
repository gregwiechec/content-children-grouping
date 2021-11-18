using System.Collections.Generic;
using ContentChildrenGrouping.VirtualContainers;

namespace ContentChildrenGrouping.Core
{
    /// <summary>
    /// Load configurations for containers
    /// </summary>
    public interface IContentChildrenGroupsLoader
    {
        int Rank { get; }

        IEnumerable<ContainerConfiguration> GetConfigurations();
    }
}