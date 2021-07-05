using EPiServer.Core;

namespace ContentChildrenGrouping
{
    /// <summary>
    /// Generating name for structure
    /// </summary>
    public interface IGroupNameGenerator
    {
        string GetName(IContent content);
    }
}