using System.Collections.Generic;
using EPiServer.Core;

namespace ContentChildrenGrouping.Core
{
    /// <summary>
    /// Generating name for structure
    /// </summary>
    public interface IGroupNameGenerator
    {
        /// <summary>
        /// unique generator name
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Readonly generator settings
        /// </summary>
        Dictionary<string, string> Settings { get; }

        string GetName(IContent content);
    }
}