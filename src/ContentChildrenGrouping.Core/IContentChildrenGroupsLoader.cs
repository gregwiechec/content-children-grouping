using System.Collections.Generic;

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