using System.Collections.Generic;
using EPiServer.Core;

namespace ContentChildrenGrouping
{
    /// <summary>
    /// Container content that has grouping configured
    /// </summary>
    public class ContainerConfiguration
    {
        public ContentReference ContainerContentLink { get; set; }

        public IEnumerable<IGroupNameGenerator> GroupLevelConfigurations { get; set; }
    }
}