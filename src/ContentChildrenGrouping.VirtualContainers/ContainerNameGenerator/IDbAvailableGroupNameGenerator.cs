using System.Collections.Generic;

namespace ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator
{
    /// <summary>
    /// When generator class implements this interface, then generator is available as DB plugin
    /// </summary>
    public interface IDbAvailableGroupNameGenerator
    {
        /// <summary>
        /// unique generator name
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Readonly generator settings
        /// </summary>
        Dictionary<string, string> Settings { get; }

        IGroupNameGenerator CreateGenerator(Dictionary<string, string> settings);
    }
}