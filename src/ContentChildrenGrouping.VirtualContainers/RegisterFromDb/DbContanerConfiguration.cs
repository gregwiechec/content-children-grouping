using System;
using ContentChildrenGrouping.VirtualContainers;

namespace ContentChildrenGrouping.Core
{
    /// <summary>
    /// Configuration with extended fields
    /// </summary>
    public class DbContainerConfiguration : ContainerConfiguration
    {
        /// <summary>
        /// login of user who edited edited/configuration
        /// </summary>
        public string ChangedBy { get; set; }

        /// <summary>
        /// Date when configuration was changed lastTime
        /// </summary>
        public DateTime? ChangedOn { get; set; }
    }
}