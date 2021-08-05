using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.ContainerModel;
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

            var containerConfigurations = _contentChildrenGroupsLoaders.GellAllConfigurations();

            ContainerConfiguration containerConfiguration = null;
            containerConfiguration = containerConfigurations.FirstOrDefault(x =>
                x.ContainerContentLink.ToReferenceWithoutVersion() == content.ParentLink.ToReferenceWithoutVersion());

            if (containerConfiguration == null)
            {
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
            }

            if (containerConfiguration == null)
            {
                return;
            }

            if (containerConfiguration.ContainerType.IsInstanceOfType(content))
            {
                return;
            }

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

            /* TODO: not working
            if (content is PageData pageData)
            {
                var ancestors = _contentRepository.GetAncestors(containerConfiguration.ContainerContentLink).Where(x =>
                        x.ContentLink != ContentReference.StartPage && x.ContentLink != ContentReference.RootPage)
                    .Reverse()
                    .Cast<PageData>();
                var ancestorUrls = string.Join("/", ancestors.Select(x => x.URLSegment));
                var container = _contentRepository.Get<PageData>(containerConfiguration.ContainerContentLink);


                var url = pageData.URLSegment;
                if (string.IsNullOrWhiteSpace(url))
                {
                    // TODO: IAggregatedSimpleAddressResolver
                    // TODO: InvalidUrlMessageGenerator


                    var isNewContent = pageData.ContentLink == null ||
                                       pageData.ContentLink == ContentReference.EmptyReference;
                    var provider = GetProvider(isNewContent ? pageData.ParentLink : pageData.ContentLink, isNewContent);
                    url = provider.GetUniqueUrlSegment(pageData, pageData.ParentLink);
                }


                pageData.URLSegment = ancestorUrls + "/" + container.URLSegment + "/" + url;
            }
            */
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