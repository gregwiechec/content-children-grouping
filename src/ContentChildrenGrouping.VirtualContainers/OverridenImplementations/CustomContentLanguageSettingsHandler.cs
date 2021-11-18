using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace ContentChildrenGrouping.VirtualContainers
{
    public class CustomContentLanguageSettingsHandler : IContentLanguageSettingsHandler
    {
        private readonly IContentLanguageSettingsHandler _contentLanguageSettingsHandler;

        public CustomContentLanguageSettingsHandler(IContentLanguageSettingsHandler contentLanguageSettingsHandler)
        {
            _contentLanguageSettingsHandler = contentLanguageSettingsHandler;
        }

        public ContentLanguageSetting Get(ContentReference contentLink, string languageBranch)
        {
            return _contentLanguageSettingsHandler.Get(contentLink, languageBranch);
        }

        public IEnumerable<ContentLanguageSetting> Get(ContentReference contentLink)
        {
            return _contentLanguageSettingsHandler.Get(contentLink);
        }

        public string GetDefaultAllowedLanguage(ContentReference contentLink)
        {
            return _contentLanguageSettingsHandler.GetDefaultAllowedLanguage(contentLink);
        }

        public string[] GetFallbackLanguages(ContentReference conentLink, string languageBranch)
        {
            return _contentLanguageSettingsHandler.GetFallbackLanguages(conentLink, languageBranch);
        }

        public bool IsLanguageAllowed(ContentReference contentLink, string languageBranch)
        {
            return _contentLanguageSettingsHandler.IsLanguageAllowed(contentLink, languageBranch);
        }

        public bool IsLanguageAllowedForCreation(ContentReference contentLink, string languageBranch)
        {
            if (contentLink.IsVirtualContainer())
            {
                return false;
            }
            return _contentLanguageSettingsHandler.IsLanguageAllowedForCreation(contentLink, languageBranch);
        }

        public bool IsSettingsDefined(ContentReference contentLink)
        {
            return _contentLanguageSettingsHandler.IsSettingsDefined(contentLink);
        }

        public LanguageSelectionSource MatchLanguageSettings(IContent content, string requestedLanguage)
        {
            return _contentLanguageSettingsHandler.MatchLanguageSettings(content, requestedLanguage);

        }
    }
}