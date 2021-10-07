using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.Extensions;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;

namespace ContentChildrenGrouping.VirtualContainers
{
    /// <summary>
    /// Customized GetChildren query that returns virtual containers
    /// </summary>
    [ServiceConfiguration(typeof(IContentQuery))]
    public class ExtendedGetChildrenQuery: GetChildrenQuery
    {
        private readonly IContentQueryHelper _queryHelper;
        private readonly IContentRepository _contentRepository;
        private readonly LanguageSelectorFactory _languageSelectorFactory;
        private readonly VirtualContainersOptions _options;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;
        public override int Rank => base.Rank + (_options.Enabled ? 1 : -1);

        public ExtendedGetChildrenQuery(IContentQueryHelper queryHelper, IContentRepository contentRepository,
            LanguageSelectorFactory languageSelectorFactory, VirtualContainersOptions options, 
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders) : base(queryHelper,
            contentRepository,
            languageSelectorFactory)
        {
            _queryHelper = queryHelper;
            _contentRepository = contentRepository;
            _languageSelectorFactory = languageSelectorFactory;
            _options = options;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
        }

        private string ParseFilter(ContentReference virtualContainerReference)
        {
            if (string.IsNullOrWhiteSpace(virtualContainerReference.ProviderName))
            {
                return string.Empty;
            }

            var result = virtualContainerReference.ProviderName.Substring((VirtualContainerExtensions.ProviderPrefix + "-").Length);
            return result;
        }

        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            if (!ContentReference.IsNullOrEmpty(parameters.ReferenceId))
            {
                var selector = _languageSelectorFactory.AutoDetect(true);

                // get children of virtual container
                if (parameters.ReferenceId.IsVirtualContainer())
                {
                    //TODO: vc allow multiple levels of virtual containers
                    //TODO: vs should be available only for pages
                    var contentReference = new ContentReference(parameters.ReferenceId.ID);
                    var filteredChildren = GetChildren(parameters, contentReference, selector).ToList();
                    var startLetter = ParseFilter(parameters.ReferenceId).ToLowerInvariant();
                    var result = filteredChildren.Where(x => x.Name.ToLowerInvariant().StartsWith(startLetter)).ToList();
                    return result;
                }

                // get virtual containers for configured container
                var containerContentLink = parameters.ReferenceId.ToReferenceWithoutVersion();
                var virtualContainer = _contentChildrenGroupsLoaders.GetAllVirtualContainersConfigurations()
                    .FirstOrDefault(x => x.ContainerContentLink.ToReferenceWithoutVersion() == containerContentLink);
                if (virtualContainer != null)
                {
                    var children = GetChildren(parameters, parameters.ReferenceId, selector).ToList();
                    var fakeContents = children.Select(x => x.Name[0].ToString().ToLowerInvariant()).ToList().Distinct().OrderBy(x => x);
                    var result = fakeContents.Select(x =>
                    {
                        var virtualContainerPage = _contentRepository.GetDefault<VirtualContainerPage>(parameters.ReferenceId);
                        virtualContainerPage.Name = x.ToUpperInvariant();
                        virtualContainerPage.ContentLink =
                            new ContentReference(parameters.ReferenceId.ID, 0, VirtualContainerExtensions.ProviderPrefix + "-" + x[0]);
                        return virtualContainerPage;
                    }).ToList();
                    return result;
                }

                return base.GetContent(parameters);
            }

            return base.GetContent(parameters);
        }

        private IEnumerable<IContent> GetChildren(ContentQueryParameters parameters, ContentReference parentId, LanguageSelector selector)
        {
            IEnumerable<IContent> children = null;
            // If the parent is wasteBasket and don't have READ access right then shouldn't return children.
            var parent = _contentRepository.Get<IContent>(parentId);
            if (_queryHelper.FilterAccess(new[] { parent }, AccessLevel.Read).Any())
            {
                if (parameters.TypeIdentifiers != null && parameters.TypeIdentifiers.Any())
                {
                    // Get by type and concatenate into one list
                    children =
                        parameters.TypeIdentifiers.Select(i => TypeResolver.GetType(i, false))
                            .Where(type => type != null)
                            .SelectMany(type => GetChildrenByType(type, parentId, selector))
                            .Distinct();
                }
                else
                {
                    children = _contentRepository.GetChildren<IContent>(parentId, selector);
                }
            }

            return children ?? Enumerable.Empty<IContent>();
        }

    }
}
