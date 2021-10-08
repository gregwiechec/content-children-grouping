using System;
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

        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            if (!ContentReference.IsNullOrEmpty(parameters.ReferenceId))
            {
                var selector = _languageSelectorFactory.AutoDetect(true);
                
                //TODO: vc allow multiple levels of virtual containers
                //TODO: vs should be available only for pages

                // get children of virtual container
                if (parameters.ReferenceId.IsVirtualContainer())
                {
                    return GetVirtualContainerChildren(parameters, selector);
                }

                if (GetVirtualContainers(parameters, selector, out var result))
                {
                    return result;
                }

                return base.GetContent(parameters);
            }

            return base.GetContent(parameters);
        }

        /// <summary>
        /// Get Virtual Containers for selected content
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="selector"></param>
        /// <param name="virtualContainers"></param>
        /// <returns></returns>
        private bool GetVirtualContainers(ContentQueryParameters parameters, LanguageSelector selector,
            out IEnumerable<IContent> virtualContainers)
        {
            var virtualContainer = GetContainerConfiguration(parameters.ReferenceId);
            if (virtualContainer == null)
            {
                virtualContainers = null;
                return false;
            }

            var children = GetChildren(parameters, parameters.ReferenceId, selector).ToList();

            var groupNameGenerator = virtualContainer.GroupLevelConfigurations.First();
            var virtualContainerNames = children.Select(x => groupNameGenerator.GetName(x))
                .Select(x => x.Replace("_", "-")) // ContentReference provider name cannot contains "_", because it's a separator
                .Select(x => x.ToLowerInvariant())
                .ToList()
                .Distinct()
                .OrderBy(x => x);

            virtualContainers = virtualContainerNames.Select(x =>
            {
                var virtualContainerPage = _contentRepository.GetDefault<VirtualContainerPage>(parameters.ReferenceId);
                virtualContainerPage.Name = x.ToUpperInvariant();
                virtualContainerPage.ContentLink =
                    new ContentReference(parameters.ReferenceId.ID, 0,
                        VirtualContainerExtensions.ProviderPrefix + "-" + x);
                return virtualContainerPage;
            }).ToList();
            return true;
        }

        /// <summary>
        /// Get children of virtual content
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="languageSelector"></param>
        /// <returns></returns>
        private IEnumerable<IContent> GetVirtualContainerChildren(ContentQueryParameters parameters, LanguageSelector languageSelector)
        {
            var configuration = GetContainerConfiguration(parameters.ReferenceId);

            if (configuration == null)
            {
                throw new Exception("Configuration not found for " + parameters.ReferenceId);
            }

            string ParseFilter(ContentReference virtualContainerReference)
            {
                if (string.IsNullOrWhiteSpace(virtualContainerReference.ProviderName))
                {
                    return string.Empty;
                }

                var pageFilter = virtualContainerReference.ProviderName.Substring((VirtualContainerExtensions.ProviderPrefix + "-").Length);
                return pageFilter;
            }

            var groupNameGenerator = configuration.GroupLevelConfigurations.First();

            var contentReference = new ContentReference(parameters.ReferenceId.ID);
            var filteredChildren = GetChildren(parameters, contentReference, languageSelector).ToList();
            var filterText = ParseFilter(parameters.ReferenceId).ToLowerInvariant();
            var result = filteredChildren.Where(x => groupNameGenerator.GetName(x).ToLowerInvariant() == filterText).ToList();
            return result;
        }

        /// <summary>
        /// Returns Virtual Container configuration for content
        /// </summary>
        /// <param name="contentLink"></param>
        /// <returns></returns>
        private ContainerConfiguration GetContainerConfiguration(ContentReference contentLink)
        {
            // get virtual containers for configured container
            var containerContentLink = new ContentReference(contentLink.ID);
            var result = _contentChildrenGroupsLoaders.GetAllVirtualContainersConfigurations()
                .FirstOrDefault(x => x.ContainerContentLink.ToReferenceWithoutVersion() == containerContentLink);
            return result;
        }

        /// <summary>
        /// Get all content children
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="parentId"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
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
