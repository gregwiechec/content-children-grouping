using EPiServer.Core;

namespace ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator
{
    /// <summary>
    /// Generating name for structure
    /// </summary>
    public interface IGroupNameGenerator
    {
        /// <summary>
        /// Generates name for the container
        /// </summary>
        /// <param name="content">The content for which the container name should be specified</param>
        /// <returns>Container name</returns>
        string GetName(IContent content);
    }
}