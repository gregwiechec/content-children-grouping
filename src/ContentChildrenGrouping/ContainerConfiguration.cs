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

        /// <summary>
        /// Type of container page used to group pages
        /// for block and media it will be always ContentFolder
        /// when null, then GroupingContainerPage is used
        /// </summary>
        public Type ContainerType { get; set; }

        /// <summary>
        /// Configurations for groups
        /// </summary>
        public IEnumerable<IGroupNameGenerator> GroupLevelConfigurations { get; set; }
    }
}