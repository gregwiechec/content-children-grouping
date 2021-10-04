using EPiServer;
using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace ContentChildrenGrouping.VirtualContainers
{
    public class ExtendedMissingContentLanguageInformationResolver : MissingContentLanguageInformationResolver
    {
        public ExtendedMissingContentLanguageInformationResolver(
            IContentLanguageSettingsHandler contentLanguageSettingsHandler,
            ILanguageBranchRepository languageBranchRepository, IContentProviderManager contentProviderManager,
            IContentLoader contentLoader, LanguageResolver languageResolver,
            ServiceAccessor<SiteDefinition> currentSiteDefinition, LocalizationService localizationService) : base(
            contentLanguageSettingsHandler, languageBranchRepository, contentProviderManager, contentLoader,
            languageResolver, currentSiteDefinition, localizationService)
        {
        }

        public override ContentLanguageInformation Resolve(IContent content, string preferredLanguage)
        {
            if (content.ContentLink.ProviderName?.StartsWith("VirtualContainers") == true)
            {
                return new ContentLanguageInformation
                {
                    Language = "en",
                    HasTranslationAccess = false,
                    IsPreferredLanguageAvailable = false,
                    IsTranslationNeeded = false,
                    PreferredLanguage = "en"
                };
            }

            return base.Resolve(content, preferredLanguage);
        }
    }

    /*
     public class ExtendedContentLanguageInformationResolver : ContentLanguageInformationResolver
    {
        public ExtendedContentLanguageInformationResolver(
            IContentLanguageSettingsHandler contentLanguageSettingsHandler,
            ILanguageBranchRepository languageBranchRepository, IContentProviderManager contentProviderManager,
            IContentLoader contentLoader, LanguageResolver languageResolver,
            ServiceAccessor<SiteDefinition> currentSiteDefinition, LocalizationService localizationService) : base(
            contentLanguageSettingsHandler, languageBranchRepository, contentProviderManager, contentLoader,
            languageResolver, currentSiteDefinition, localizationService)
        {
        }

        public override ContentLanguageInformation Resolve(IContent content, string preferredLanguage)
        {
            if (content.ContentLink.ProviderName?.StartsWith("VirtualContainers") == true)
            {
                return new ContentLanguageInformation
                {
                    Language = "en",
                    HasTranslationAccess = false,
                    IsPreferredLanguageAvailable = false,
                    IsTranslationNeeded = false,
                    PreferredLanguage = "en"
                };
            }

            return base.Resolve(content, preferredLanguage);
        }
    }
    */
}