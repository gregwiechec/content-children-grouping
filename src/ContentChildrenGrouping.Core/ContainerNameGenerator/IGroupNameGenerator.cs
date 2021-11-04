using EPiServer.Core;

namespace ContentChildrenGrouping.Core.ContainerNameGenerator
{
    /// <summary>
    /// Generating name for structure
    /// </summary>
    public interface IGroupNameGenerator
    {
        /// <summary>
        /// Generates name for the container
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Container name</returns>
        string GetName(IContent content);
    }
}