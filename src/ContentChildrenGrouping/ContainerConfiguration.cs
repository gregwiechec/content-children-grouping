using System;
using System.Collections.Generic;
using EPiServer.Core;

namespace ContentChildrenGrouping
{
    /// <summary>
    /// Configuration for container content that has groups
    /// </summary>
    public class ContainerConfiguration
    {
        /// <summary>
        /// Id of container content
        /// </summary>
        public ContentReference ContainerContentLink { get; set; }

        // for assets it should be ContentFolder
        public Type ContainerType { get; set; }

        /// <summary>
        /// Configurations for groups
        /// </summary>
        public IEnumerable<IGroupNameGenerator> GroupLevelConfigurations { get; set; }
    }
}