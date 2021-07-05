using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.SavingContent += ContentEvents_SavingContent;
            _log.Information($"Structure SavingContent registered");

        }

        public void Uninitialize(InitializationEngine context)
        {
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
    }

    /// <summary>
    /// Update content parent page to match the structure tree
    /// </summary>
    [ServiceConfiguration(typeof(IContentStructureModifier))]
    public class ContentStructureModifier: IContentStructureModifier
    {
        private readonly IContentRepository _contentRepository;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;

        public ContentStructureModifier(IContentRepository contentRepository, IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders)
        {
            _contentRepository = contentRepository;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
        }

        public void UpdateContentParent(IContent content)
        {
            if (content == null || ContentReference.IsNullOrEmpty(content.ParentLink))
            {
                return;
            }


            var containerConfigurations = _contentChildrenGroupsLoaders.SelectMany(x => x.GetConfigurations()).ToList();

            ContainerConfiguration containerConfiguration = null;
            containerConfiguration = containerConfigurations.FirstOrDefault(x => x.ContainerContentLink.ToReferenceWithoutVersion() == content.ParentLink.ToReferenceWithoutVersion());

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
                var parent = _contentRepository.GetChildren<IContent>(parentLink).FirstOrDefault(x =>
                    string.Compare(x.Name, groupName, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (parent == null)
                {
                    MethodInfo getDefault = typeof(IContentRepository).GetMethod(nameof(IContentRepository.GetDefault),
                        new[] {typeof(ContentReference)});
                    MethodInfo generic = getDefault.MakeGenericMethod(containerConfiguration.ContainerType);
                    var container = generic.Invoke(_contentRepository, new [] {parentLink}) as IContent;
                    container.Name = groupName;
                    parentLink = _contentRepository.Save(container, SaveAction.Publish);
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
    }
}
