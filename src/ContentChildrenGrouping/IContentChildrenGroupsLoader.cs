using System.Collections.Generic;

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
}