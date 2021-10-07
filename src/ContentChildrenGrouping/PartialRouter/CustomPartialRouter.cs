using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Extensions;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace ContentChildrenGrouping.PartialRouter
{
    /// <summary>
    /// Partial router responsible for remoing container segments from URL
    /// </summary>
    public class CustomPartialRouter: IPartialRouter<PageData, PageData>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;

        public CustomPartialRouter(IContentLoader contentLoader, IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders)
        {
            _contentLoader = contentLoader;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            /* We don't need partial router. Routing is handled by ExternalURL*/

            return null;
            /*if (string.IsNullOrWhiteSpace(segmentContext.RemainingPath))
            {
                return null;
            }
            var contentName = segmentContext.RemainingPath;
            if (contentName.EndsWith("/"))
            {
                contentName = contentName.Substring(0, contentName.Length - 1);
            }
            if (string.IsNullOrWhiteSpace(contentName))
            {
                return null;
            }

            var configuration = _contentChildrenGroupsLoaders
                .GellAllConfigurations()
                .FirstOrDefault(x =>
                    x.ContainerContentLink == segmentContext.RoutedContentLink.ToReferenceWithoutVersion());
            if (configuration == null || !configuration.RoutingEnabled)
            {
                return null;
            }

            // we have only alloy/container/article-1
            // and don't know where to look in structure
            // we need to traverse all children
            PageData GetChildrenAtLevel(string segmentName, int currentLevelIndex, List<PageData> children)
            {
                if (currentLevelIndex == 0)
                {
                    var result = children.FirstOrDefault(x => string.Compare(x.URLSegment, segmentName, StringComparison.InvariantCultureIgnoreCase) == 0);
                    return result;
                }

                //TODO: groups optimization if config[currentLevelIndex] == nameGenerator then find child faster

                foreach (var child in children)
                {
                    var result = GetChildrenAtLevel(segmentName, currentLevelIndex - 1, _contentLoader.GetChildren<PageData>(child.ContentLink).ToList());
                    if (result != null)
                    {
                        return result;
                    }
                }

                return null;
            }

            var numberOfLevels = configuration.GroupLevelConfigurations.Count();
            var containerChildren = _contentLoader.GetChildren<PageData>(segmentContext.RoutedContentLink);
            
            var resolvedContent = GetChildrenAtLevel(contentName, numberOfLevels, containerChildren.ToList());
            if (resolvedContent == null)
            {
                return null;
            }
            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);
            segmentContext.RemainingPath = nextSegment.Remaining;
            return resolvedContent;*/
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, string language, RouteValueDictionary routeValues,
            RequestContext requestContext)
        {
            if (requestContext.GetContextMode() != ContextMode.Default)
            {
                return null;
            }

            var configurations = _contentChildrenGroupsLoaders.GetAllContainersConfigurations().ToList();
            ContainerConfiguration configuration = null;
            foreach (var ancestor in _contentLoader.GetAncestors(content.ContentLink))
            {
                foreach (var containerConfiguration in configurations)
                {
                    if (ancestor.ContentLink.ToReferenceWithoutVersion() ==
                        containerConfiguration.ContainerContentLink.ToReferenceWithoutVersion())
                    {
                        configuration = containerConfiguration;
                        if (!configuration.RoutingEnabled)
                        {
                            return null;
                        }
                        break;
                    }
                }

                if (configuration != null)
                {
                    break;
                }
            }
            if (configuration == null)
            {
                return null;
            }

            // the url is just content URLSegment
            return new PartialRouteData
            {
                BasePathRoot = configuration.ContainerContentLink.ToReferenceWithoutVersion(),
                PartialVirtualPath = content.URLSegment
            };
        }
    }
}