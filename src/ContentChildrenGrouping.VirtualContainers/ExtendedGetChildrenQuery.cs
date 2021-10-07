using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;

namespace ContentChildrenGrouping.VirtualContainers
{
    [ServiceConfiguration(typeof(IContentQuery))]
    public class ExtendedGetChildrenQuery: GetChildrenQuery
    {
        private readonly IContentQueryHelper _queryHelper;
        private readonly IContentRepository _contentRepository;
        private readonly LanguageSelectorFactory _languageSelectorFactory;
        private readonly VirtualContainersOptions _options;
        public override int Rank => base.Rank + (_options.Enabled ? 1 : -1);

        public ExtendedGetChildrenQuery(IContentQueryHelper queryHelper, IContentRepository contentRepository,
            LanguageSelectorFactory languageSelectorFactory, VirtualContainersOptions options) : base(queryHelper, contentRepository,
            languageSelectorFactory)
        {
            _queryHelper = queryHelper;
            _contentRepository = contentRepository;
            _languageSelectorFactory = languageSelectorFactory;
            _options = options;
        }

        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            if (!ContentReference.IsNullOrEmpty(parameters.ReferenceId))
            {
                var selector = _languageSelectorFactory.AutoDetect(true);

                if (parameters.ReferenceId.IsVirtualContainer())
                {
                    var contentReference = new ContentReference(parameters.ReferenceId.ID);
                    var filteredChildren = GetChildren(parameters, contentReference, selector).ToList();
                    var startLetter = ((char)parameters.ReferenceId.WorkID).ToString();
                    var result = filteredChildren.Where(x => x.Name.ToLowerInvariant().StartsWith(startLetter)).ToList();
                    return result;
                }

                var children = GetChildren(parameters, parameters.ReferenceId, selector).ToList();
                if (children.Count > 10)
                {
                    var fakeContents = children.Select(x => x.Name[0].ToString()).ToList().Distinct().OrderBy(x => x);
                    var result = fakeContents.Select(x =>
                    {
                        var virtualContainerPage = _contentRepository.GetDefault<VirtualContainerPage>(parameters.ReferenceId);
                        virtualContainerPage.Name = x.ToUpperInvariant();
                        virtualContainerPage.ContentLink =
                            new ContentReference(parameters.ReferenceId.ID, (int)(x[0]), "VirtualContainers1");
                        return virtualContainerPage;
                    }).ToList();
                    return result;
                }

                return children;
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
