using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.ContainerModel;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Extensions;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ChangeEventInitialization : IInitializableModule
    {
        private ILogger _log = LogManager.GetLogger(typeof(ChangeEventInitialization));

        public void Initialize(InitializationEngine context)
        {
            var structureUpdateEnabled = ServiceLocator.Current.GetInstance<ContentChildrenGroupingOptions>()
                .StructureUpdateEnabled;
            if (!structureUpdateEnabled)
            {
                return;
            }

            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.SavingContent += ContentEvents_SavingContent;
            _log.Information($"Structure SavingContent registered");
        }

        public void Uninitialize(InitializationEngine context)
        {
            var structureUpdateEnabled = ServiceLocator.Current.GetInstance<ContentChildrenGroupingOptions>()
                .StructureUpdateEnabled;
            if (!structureUpdateEnabled)
            {
                return;
            }

            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.SavingContent -= ContentEvents_SavingContent;
            _log.Information($"Structure SavingContent unregistered");
        }

        private void ContentEvents_SavingContent(object sender, EPiServer.ContentEventArgs e)
        {
            ServiceLocator.Current.GetInstance<IContentStructureModifier>().UpdateContentParent(e.Content);
        }
    }

    public interface IContentStructureModifier
    {
        void UpdateContentParent(IContent content);

        ContentReference CreateParent(ContainerConfiguration containerConfiguration, string parentName,
            ContentReference parentParentContentLink, IContent content);
    }

    /// <summary>
    /// Update content parent page to match the structure tree
    /// </summary>
    [ServiceConfiguration(typeof(IContentStructureModifier))]
    public class ContentStructureModifier : IContentStructureModifier
    {
        private readonly IContentRepository _contentRepository;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;
        private readonly IContentProviderManager _providerManager;

        public ContentStructureModifier(IContentRepository contentRepository,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders,
            IContentProviderManager providerManager)
        {
            _contentRepository = contentRepository;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
            _providerManager = providerManager;
        }

        public void UpdateContentParent(IContent content)
        {
            if (content == null || ContentReference.IsNullOrEmpty(content.ParentLink))
            {
                return;
            }

            // Containers works only for Pages, blocks and media
            if (!(content is PageData || content is BlockData || content is MediaData))
            {
                return;
            }

            var containerConfigurations = _contentChildrenGroupsLoaders.GetAllContainersConfigurations();

            var containerConfiguration = FindConfiguration(content, containerConfigurations);
            if (containerConfiguration == null)
            {
                return;
            }

            var containerType = containerConfiguration.GetContainerType();
            if (containerType.IsInstanceOfType(content))
            {
                return;
            }

            UpdateParentLink(content, containerConfiguration);
            UpdateExternalUrl(content, containerConfiguration);
        }

        /// <summary>
        /// Find container configuration for content. It should be parent page or one of the ascendants
        /// </summary>
        /// <param name="content"></param>
        /// <param name="containerConfigurations"></param>
        /// <returns></returns>
        private ContainerConfiguration FindConfiguration(IContent content,
            IReadOnlyCollection<ContainerConfiguration> containerConfigurations)
        {
            // Try to find configuration directly in parent
            var containerConfiguration = containerConfigurations.FirstOrDefault(x =>
                x.ContainerContentLink.ToReferenceWithoutVersion() == content.ParentLink.ToReferenceWithoutVersion());

            if (containerConfiguration != null)
            {
                return containerConfiguration;
            }

            var ancestors = _contentRepository.GetAncestors(content.ParentLink);
            foreach (var ancestor in ancestors)
            {
                var parentContentLink = ancestor.ContentLink.ToReferenceWithoutVersion();
                containerConfiguration = containerConfigurations.FirstOrDefault(x =>
                    x.ContainerContentLink.ToReferenceWithoutVersion() == parentContentLink);
                if (containerConfiguration != null)
                {
                    break;
                }
            }

            return containerConfiguration;
        }

        // update ParentLink when adding or moving content
        private void UpdateParentLink(IContent content, ContainerConfiguration containerConfiguration)
        {
            var parentLink = containerConfiguration.ContainerContentLink;
            foreach (var nameGenerator in containerConfiguration.GroupLevelConfigurations)
            {
                var groupName = nameGenerator.GetName(content);
                var parent = _contentRepository.GetChildren<IContent>(parentLink)
                    .FirstOrDefault(x => x.Name.CompareStrings(groupName));
                if (parent == null)
                {
                    parentLink = CreateParent(containerConfiguration, groupName, parentLink, content);
                }
                else
                {
                    parentLink = parent.ContentLink;
                }
            }

            if (content.ParentLink.ToReferenceWithoutVersion() != parentLink.ToReferenceWithoutVersion())
            {
                content.ParentLink = parentLink.ToReferenceWithoutVersion();
            }
        }

        /// <summary>
        /// Updating ExternalURL to remove content structure segments
        /// </summary>
        private void UpdateExternalUrl(IContent content, ContainerConfiguration containerConfiguration)
        {
            if (containerConfiguration == null)
            {
                return;
            }

            var pageData = content as PageData;
            if (pageData == null)
            {
                return;
            }

            var ancestors = _contentRepository.GetAncestors(containerConfiguration.ContainerContentLink).Where(
                    x =>
                        x.ContentLink != ContentReference.StartPage &&
                        x.ContentLink != ContentReference.RootPage)
                .Reverse()
                .Cast<PageData>();
            var ancestorUrls = string.Join("/", ancestors.Select(x => x.URLSegment));
            var container = _contentRepository.Get<PageData>(containerConfiguration.ContainerContentLink);

            var url = pageData.URLSegment;
            if (string.IsNullOrWhiteSpace(url))
            {
                var isNewContent = pageData.ContentLink == null ||
                                   pageData.ContentLink == ContentReference.EmptyReference;
                var provider = GetProvider(isNewContent ? pageData.ParentLink : pageData.ContentLink,
                    isNewContent);
                url = provider.GetUniqueUrlSegment(pageData, pageData.ParentLink);
            }


            pageData.ExternalURL = ancestorUrls + "/" + container.URLSegment + "/" + url;
        }

        private ContentProvider GetProvider(ContentReference contentLink, bool isNewContent)
        {
            return isNewContent || !this._providerManager.ProviderMap.IsEntryPoint(contentLink)
                ? this._providerManager.ProviderMap.GetProvider(contentLink)
                : this._providerManager.GetProvider(contentLink.ProviderName);
        }

        public ContentReference CreateParent(ContainerConfiguration containerConfiguration, string parentName,
            ContentReference parentParentContentLink, IContent content)
        {
            IContent container;
            if (content is BlockData || content is MediaData)
            {
                container = _contentRepository.GetDefault<ContentFolder>(parentParentContentLink);
            }
            else if (containerConfiguration.ContainerType == null ||
                     containerConfiguration.ContainerType == typeof(GroupingContainerPage))
            {
                container = _contentRepository.GetDefault<GroupingContainerPage>(parentParentContentLink);
            }
            else
            {
                var getDefault = typeof(IContentRepository).GetMethod(nameof(IContentRepository.GetDefault),
                    new[] { typeof(ContentReference) });
                var generic = getDefault.MakeGenericMethod(containerConfiguration.ContainerType);
                container = (IContent)generic.Invoke(_contentRepository, new[] { parentParentContentLink });
            }

            container.Name = parentName;
            return _contentRepository.Save(container, SaveAction.Publish);
        }
    }
}