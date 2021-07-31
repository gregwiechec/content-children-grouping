using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace ContentChildrenGrouping.ContainerModel
{
    /// <summary>
    /// Content type used to logically group pages in the page tree
    /// </summary>
    [ContentType(
        GUID = "C5495780-3BC7-4DC3-A5AC-8444617F530C",
        GroupName = "Administration",
        AvailableInEditMode = false)]
    //[SiteImageUrl] //TODO: [grouping] add image for new type
    public class GroupingContainerPage : PageData
    {

    }
}
