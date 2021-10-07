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

        string GetName(IContent content);
    }
}