using System;
using System.Collections.Generic;
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
        /// When true then routing for configuration is enabled
        /// </summary>
        public bool RoutingEnabled { get; set; } = true;

        /// <summary>
        /// Type of container page used to group pages
        /// for block and media it will be always ContentFolder
        /// when null, then GroupingContainerPage is used
        /// </summary>
        public Type ContainerType { get; set; } = null;

        /// <summary>
        /// When true, then container is virtual
        /// Container is not stored in repository, just displayed on the UI
        /// </summary>
        public bool IsVirtualContainer { get; set; } = false;

        /// <summary>
        /// Configurations for groups
        /// </summary>
        public IEnumerable<IGroupNameGenerator> GroupLevelConfigurations { get; set; }

        /// <summary>
        /// login of user who edited edited/configuration
        /// </summary>
        public string ChangedBy { get; set; } //TODO: remove

        /// <summary>
        /// Date when configuration was changed lastTime
        /// </summary>
        public DateTime? ChangedOn { get; set; }
    }
}