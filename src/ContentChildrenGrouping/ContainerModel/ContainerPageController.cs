using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;

namespace ContentChildrenGrouping.ContainerModel
{
    public class GroupingContainerPageController : PageController<GroupingContainerPage>
    {
        private readonly IContentLoader _contentLoader;

        public GroupingContainerPageController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public ActionResult Index(GroupingContainerPage currentPage)
        {
            var ancestors = _contentLoader.GetAncestors(currentPage.ContentLink).OfType<PageData>();
            foreach (var ancestor in ancestors)
            {
                if (!(ancestor is GroupingContainerPage))
                {
                    return Redirect(ancestor.LinkURL);
                }
            }

            return Redirect(_contentLoader.Get<PageData>(ContentReference.StartPage).LinkURL);
        }
    }
}