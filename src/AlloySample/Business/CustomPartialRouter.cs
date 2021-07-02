using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using AlloySample.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace AlloySample.Business
{
    //TODO: group move to another assembly

    public class CustomPartialRouter: IPartialRouter<NewsPage, ArticlePage>
    {
        private readonly IContentLoader _contentLoader;

        public CustomPartialRouter(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public object RoutePartial(NewsPage content, SegmentContext segmentContext)
        {
            //Use helper method GetNextValue to get the next part from the URL
            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);

            string letter = nextSegment.Next;
            var children = _contentLoader.GetChildren<IContent>(content.ContentLink).ToList();
            var containerPage = children.FirstOrDefault(x => x.Name.ToLowerInvariant() == letter[0].ToString().ToLowerInvariant());
            if (containerPage == null)
            {
                return null;
            }

            var articles = _contentLoader.GetChildren<IContent>(containerPage.ContentLink);
            var article = articles.FirstOrDefault(x => x. == letter);
            if (article == null)
            {
                return null;
            }

            segmentContext.RemainingPath = nextSegment.Remaining;
            return article;
        }

        public PartialRouteData GetPartialVirtualPath(ArticlePage content, string language, RouteValueDictionary routeValues,
            RequestContext requestContext)
        {
            return null;

            var _newsContainer = new ContentReference(113);

            if (ContentReference.IsNullOrEmpty(_newsContainer))
            {
                throw new InvalidOperationException("property NewsContainer must be set on start page");
            }
            return new PartialRouteData()
            {
                BasePathRoot = _newsContainer,
                PartialVirtualPath = String.Format("{0}/{1}/",
                    content.Name[0],
                    HttpUtility.UrlPathEncode(content.Name))
            };
        }
    }

    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class InitializationModule : IInitializableModule
    {
        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
            var partialRouter = new CustomPartialRouter(context.Locate.ContentLoader());
            RouteTable.Routes.RegisterPartialRouter<NewsPage, ArticlePage>(partialRouter);
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
        }
    }
}