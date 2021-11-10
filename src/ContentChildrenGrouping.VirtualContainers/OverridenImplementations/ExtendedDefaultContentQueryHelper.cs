using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery.Internal;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Shell;

namespace ContentChildrenGrouping.VirtualContainers
{
    class ExtendedDefaultContentQueryHelper : DefaultContentQueryHelper
    {
        private readonly IContentProviderManager _contentProviderManager;

        public override IEnumerable<IContent> FilterWasteBasket(IEnumerable<IContent> items)
        {
            if (items == null)
            {
                return items;
            }

            return items.Where(m => m.IsVirtualContainer() || !_contentProviderManager.IsWastebasket(m.ContentLink));
        }

        public ExtendedDefaultContentQueryHelper(IContentRepository contentRepository,
            IContentProviderManager contentProviderManager,
            MissingContentLanguageInformationResolver contentLanguageInformationResolver,
            UIDescriptorRegistry uiDescriptorRegistry, LanguageResolver languageResolver) : base(contentRepository,
            contentProviderManager, contentLanguageInformationResolver, uiDescriptorRegistry, languageResolver)
        {
            _contentProviderManager = contentProviderManager;
        }
    }
}