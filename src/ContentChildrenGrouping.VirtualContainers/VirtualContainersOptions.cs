using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.VirtualContainers
{
    /// <summary>
    /// Configuration for virtual containers
    /// </summary>
    [Options]
    public class VirtualContainersOptions
    {
        /// <summary>
        /// When true then Virtual containers are enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// When true, then containers can be configured using admin plugin.
        /// Default false
        /// </summary>
        public bool DatabaseConfigurationsEnabled { get; set; } = true;

        /// <summary>
        /// When true, then container icon is replaced with custom icon.
        /// Default true
        /// </summary>
        public bool CustomIconsEnabled { get; set; } = true;

        /// <summary>
        /// When true, then search commands are available in page tree and assets.
        /// Default true
        /// </summary>
        public bool SearchCommandEnabled { get; set; } = true;
    }
}