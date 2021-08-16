using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.Search;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Search;
using EPiServer.Shell.UI.Rest.Internal;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace AlloySample.Business
{
    internal abstract class BasicSearchProvider<T> : ContentSearchProviderBase<IContent, ContentType>, ISortable
    {
        protected IContentLoader _contentLoader;

        protected ISearchRootsResolver _searchRootsResolver;

        protected BasicSearchProvider(LocalizationService localizationService,
            ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository<ContentType> contentTypeRepository,
            EditUrlResolver editUrlResolver, ServiceAccessor<SiteDefinition> currentSiteDefinition,
            LanguageResolver languageResolver, UrlResolver urlResolver, TemplateResolver templateResolver,
            UIDescriptorRegistry uiDescriptorRegistry, IContentLoader contentLoader,
            ISearchRootsResolver searchRootsResolver) : base(localizationService, siteDefinitionResolver,
            contentTypeRepository, editUrlResolver, currentSiteDefinition, languageResolver, urlResolver,
            templateResolver, uiDescriptorRegistry)
        {
            _contentLoader = contentLoader;
            _searchRootsResolver = searchRootsResolver;
        }

        public override IEnumerable<SearchResult> Search(Query query)
        {
            var searchPhrase = query.SearchQuery?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(searchPhrase))
            {
                return Enumerable.Empty<SearchResult>();
            }

            var contentReferences = new List<ContentReference>();
            var searchRoots = query.SearchRoots.Any() ? query.SearchRoots : _searchRootsResolver.GetSearchRoots(this.Area);
            foreach (var querySearchRoot in searchRoots)
            {
                contentReferences.AddRange(_contentLoader.GetDescendents(ContentReference.Parse(querySearchRoot)));
            }

            var result = query.FilterOnCulture
                ? _contentLoader.GetItems(contentReferences, LanguageResolver.GetPreferredCulture())
                : _contentLoader.GetItems(contentReferences, CultureInfo.InvariantCulture)
                    .Where(x => x is T)
                    .Where(x => x.Name.ToLowerInvariant().StartsWith(searchPhrase) ||
                                x.ContentLink.ToString().Equals(searchPhrase));
            if (query.Parameters.ContainsKey("filterOnDeleted"))
            {
                result = result.Where(x => !x.IsDeleted);
            }
            return result.Select(CreateSearchResult);
        }

        public int SortOrder => int.MinValue;
    }


    [ServiceConfiguration(typeof(ISearchProvider))]
    internal class BasicPageSearchProvider : BasicSearchProvider<PageData>
    {
        public override string Area => "CMS/pages";

        public override string Category => "Page provider";

        protected override string IconCssClass => "epi-resourceIcon epi-resourceIcon-page";

        public BasicPageSearchProvider(LocalizationService localizationService,
            ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository<ContentType> contentTypeRepository,
            EditUrlResolver editUrlResolver, ServiceAccessor<SiteDefinition> currentSiteDefinition,
            LanguageResolver languageResolver, UrlResolver urlResolver, TemplateResolver templateResolver,
            UIDescriptorRegistry uiDescriptorRegistry, IContentLoader contentLoader,
            ISearchRootsResolver searchRootsResolver) : base(localizationService,
            siteDefinitionResolver, contentTypeRepository, editUrlResolver, currentSiteDefinition, languageResolver,
            urlResolver, templateResolver, uiDescriptorRegistry, contentLoader, searchRootsResolver)
        {
        }
    }

    [ServiceConfiguration(typeof(ISearchProvider))]
    internal class BasicBlockSearchProvider : BasicSearchProvider<BlockData>
    {
        public override string Area => "CMS/blocks";

        public override string Category => "Block provider";

        protected override string IconCssClass => "epi-resourceIcon epi-resourceIcon-block";

        public BasicBlockSearchProvider(LocalizationService localizationService,
            ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository<ContentType> contentTypeRepository,
            EditUrlResolver editUrlResolver, ServiceAccessor<SiteDefinition> currentSiteDefinition,
            LanguageResolver languageResolver, UrlResolver urlResolver, TemplateResolver templateResolver,
            UIDescriptorRegistry uiDescriptorRegistry, IContentLoader contentLoader,
            ISearchRootsResolver searchRootsResolver) : base(localizationService,
            siteDefinitionResolver, contentTypeRepository, editUrlResolver, currentSiteDefinition, languageResolver,
            urlResolver, templateResolver, uiDescriptorRegistry, contentLoader, searchRootsResolver)
        {
        }
    }

    [ServiceConfiguration(typeof(ISearchProvider))]
    internal class BasicFileSearchProvider : BasicSearchProvider<MediaData>
    {
        public override string Area => "CMS/files";

        public override string Category => "File provider";

        protected override string IconCssClass => "epi-resourceIcon epi-resourceIcon-jpg";

        public BasicFileSearchProvider(LocalizationService localizationService,
            ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository<ContentType> contentTypeRepository,
            EditUrlResolver editUrlResolver, ServiceAccessor<SiteDefinition> currentSiteDefinition,
            LanguageResolver languageResolver, UrlResolver urlResolver, TemplateResolver templateResolver,
            UIDescriptorRegistry uiDescriptorRegistry, IContentLoader contentLoader,
            ISearchRootsResolver searchRootsResolver) : base(localizationService,
            siteDefinitionResolver, contentTypeRepository, editUrlResolver, currentSiteDefinition, languageResolver,
            urlResolver, templateResolver, uiDescriptorRegistry, contentLoader, searchRootsResolver)
        {
            _searchRootsResolver = searchRootsResolver;
        }
    }
}