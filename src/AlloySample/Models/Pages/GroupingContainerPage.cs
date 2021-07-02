using AlloySample.Business.Rendering;
using EPiServer.Core;

namespace AlloySample.Models.Pages
{
    //TODO: group move to another assembly
    /// <summary>
    /// Used to logically group pages in the page tree
    /// </summary>
    [SiteContentType(
        GUID = "C5495780-3BC7-4DC3-A5AC-8444617F530C",
        GroupName = Global.GroupNames.Specialized)]
    //[SiteImageUrl]
    public class GroupingContainerPage : PageData, IContainerPage
    {

    }
}
