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
        public bool Enabled { get; set; } = false;
    }
}