using System;
using System.Collections.Generic;
using System.Linq;
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

    public class ContentStructureModifier: IContentStructureModifier
    {
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;

        public ContentStructureModifier(IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders)
        {
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
        }

        public void UpdateContentParent(IContent content)
        {
            if (content == null || ContentReference.IsNullOrEmpty(content.ParentLink))
            {
                return;
            }

            if (!_contentType.IsAssignableFrom(e.Content.GetType()))
            {
                return;
            }

            if (e.Content.ParentLink.ToReferenceWithoutVersion() != _rootLink.ToReferenceWithoutVersion())
            {
                return;
            }

            var parentLink = _rootLink;
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            foreach (var clusterNameGenerator in this._generators)
            {
                var clusterName = clusterNameGenerator.GetName(e.Content);
                var parent = contentRepository.GetChildren<IContent>(parentLink).FirstOrDefault(x =>
                    string.Compare(x.Name, clusterName, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (parent == null)
                {
                    var contentFolder = contentRepository.GetDefault<ContentFolder>(parentLink);
                    contentFolder.Name = clusterName;
                    parentLink = contentRepository.Save(contentFolder, SaveAction.Publish);
                }
                else
                {
                    parentLink = parent.ContentLink;
                }
            }

            if (e.Content.ParentLink.ToReferenceWithoutVersion() != parentLink.ToReferenceWithoutVersion())
            {
                e.Content.ParentLink = parentLink.ToReferenceWithoutVersion();
            }
        }
    }
}
