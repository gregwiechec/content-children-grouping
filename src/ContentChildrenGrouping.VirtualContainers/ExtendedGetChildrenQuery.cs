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
            if (!_options.Enabled)
            {
                return base.GetContent(parameters);
            }

            if (!ContentReference.IsNullOrEmpty(parameters.ReferenceId))
            {
                var selector = _languageSelectorFactory.AutoDetect(true);

                // get children of virtual container
                if (parameters.ReferenceId.IsVirtualContainer())
                {
                    return GetVirtualContainerChildren(parameters, selector);
                }

                // virtual containers works only for PageData
                var parent = _contentRepository.Get<IContent>(parameters.ReferenceId);
                if (!(parent is PageData))
                {
                    return base.GetContent(parameters);
                }

                var virtualContainer = GetContainerConfiguration(parameters.ReferenceId);
                if (virtualContainer == null)
                {
                    return base.GetContent(parameters);
                }

                var children = GetChildren(parameters, parameters.ReferenceId, selector).ToList();
                if (GetVirtualContainers(parameters, selector, children, new List<string>(), virtualContainer, out var result))
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
        /// <param name="children"></param>
        /// <param name="generatedNames"></param>
        /// <param name="containerConfiguration"></param>
        /// <param name="virtualContainers"></param>
        /// <returns></returns>
        private bool GetVirtualContainers(ContentQueryParameters parameters, LanguageSelector selector,
            List<IContent> children,
            List<string> generatedNames, ContainerConfiguration containerConfiguration,
            out IEnumerable<IContent> virtualContainers)
        {
            var groupNameGenerators = containerConfiguration.GroupLevelConfigurations.ToList();

            // filter children that match all above filters
            var filteredChildren = new List<IContent>();
            if (generatedNames.Any())
            {
                foreach (var content in children)
                {
                    var allMatch = true;
                    for (var generatorIndex = 0; generatorIndex < generatedNames.Count; generatorIndex++)
                    {
                        var groupNameGenerator = groupNameGenerators[generatorIndex];
                        if (groupNameGenerator.GetName(content).ToLowerInvariant() != generatedNames[generatorIndex])
                        {
                            allMatch = false;
                            break;
                        }
                    }
                    if (allMatch)
                    {
                        filteredChildren.Add(content);
                    }
                }
            }
            else
            {
                filteredChildren = children;
            }
            
            var generatorLevel = generatedNames.Count;

            var result = new List<IContent>();
            foreach (var filteredChild in filteredChildren)
            {
                var contentName = groupNameGenerators[generatorLevel].GetName(filteredChild).ToUpperInvariant();
                if (result.Any(x => x.Name == contentName))
                {
                    continue;
                }
                var virtualContainerPage = _contentRepository.GetDefault<VirtualContainerPage>(new ContentReference(parameters.ReferenceId.ID));
                virtualContainerPage.Name = contentName;
                virtualContainerPage.ContentLink = VirtualNamesParser.GetVirtualContentLink(parameters.ReferenceId, filteredChild, groupNameGenerators, generatorLevel);
                result.Add(virtualContainerPage);
            }
            virtualContainers = result.OrderBy(x=>x.Name).ToList();
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

            var contentReference = new ContentReference(parameters.ReferenceId.ID);
            var filteredChildren = GetChildren(parameters, contentReference, languageSelector).ToList();
            var generatedNames = VirtualNamesParser.ParseGeneratorValues(parameters.ReferenceId)
                .Select(x => x.ToLowerInvariant()).ToList();

            var groupNameGenerators = configuration.GroupLevelConfigurations.ToList();
            
            if (generatedNames.Count == groupNameGenerators.Count)
            {
                // should return children of virtual container
                var result = new List<IContent>();
                foreach (var filteredChild in filteredChildren)
                {
                    var allMatch = true;
                    for (var generatorIndex = 0; generatorIndex < generatedNames.Count; generatorIndex++)
                    {
                        var groupNameGenerator = groupNameGenerators[generatorIndex];
                        if (groupNameGenerator.GetName(filteredChild).ToLowerInvariant() != generatedNames[generatorIndex])
                        {
                            allMatch = false;
                            break;
                        }
                    }
                    if (allMatch)
                    {
                        result.Add(filteredChild); 
                    }
                }
                return result;
            }
            else
            {
                // we should return virtual containers on specific level
                if (GetVirtualContainers(parameters, languageSelector, filteredChildren, generatedNames, configuration, out var result))
                {
                    return result;
                }
                throw new Exception("Cannot get virtual containers for " + parameters.ReferenceId);
            }
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
