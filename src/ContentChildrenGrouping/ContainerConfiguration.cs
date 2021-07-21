using System;
using System.Collections.Generic;
using EPiServer.Core;

namespace ContentChildrenGrouping
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
        /// When true then routing for configuration is enabled
        /// </summary>
        public bool RoutingEnabled { get; set; } = true;

        // for assets it should be ContentFolder
        public Type ContainerType { get; set; }

        /// <summary>
        /// Configurations for groups
        /// </summary>
        public IEnumerable<IGroupNameGenerator> GroupLevelConfigurations { get; set; }
    }
}