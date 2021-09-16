using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping
{
    [Options]
    public class ContentChildrenGroupingOptions
    {
        /// <summary>
        /// When true, then routing is globally enabled. Default true
        /// For complex scenarios, because of the performance it's better to implement custom PartialRouter.
        /// </summary>
        public bool RouterEnabled { get; set; } = true;

        /// <summary>
        /// When true, then during content saving the structure is updated, default true
        /// </summary>
        public bool StructureUpdateEnabled { get; set; } = false;

        /// <summary>
        /// When true, then containers can be configured using admin plugin
        /// </summary>
        public bool DatabaseConfigurationsEnabled { get; set; } = false;

        /// <summary>
        /// When true, then container icon is replaced with custom icon. Default true
        /// </summary>
        public bool CustomIconsEnabled { get; set; } = true;

        /// <summary>
        /// When true, then search commands are available in page tree and assets. Default true
        /// </summary>
        public bool SearchCommandEnabled { get; set; } = true;
    }
}
