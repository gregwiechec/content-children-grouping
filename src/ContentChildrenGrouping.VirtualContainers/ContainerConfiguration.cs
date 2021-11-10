using System;
using System.Collections.Generic;
using ContentChildrenGrouping.Core.ContainerNameGenerator;
using EPiServer.Core;

namespace ContentChildrenGrouping.Core
{
    /// <summary>
    /// Configuration for container content
    /// </summary>
    public class ContainerConfiguration
    {
        /// <summary>
        /// Id of container content
        /// </summary>
        public ContentReference ContainerContentLink { get; set; }

        /// <summary>
        /// Configurations for groups
        /// </summary>
        public IEnumerable<IGroupNameGenerator> GroupLevelConfigurations { get; set; }
    }
}