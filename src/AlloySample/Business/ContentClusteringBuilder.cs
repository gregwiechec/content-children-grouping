using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.ServiceLocation;

//TODO: group move to another assembly

namespace EPiServer.Labs.ContentManager.ContentClustering
{
    public class ContentClusteringBuilder
    {
        private readonly ContentReference _rootLink;
        private readonly Type _contentType;

        public ContentClusteringBuilder(ContentReference rootLink, Type contentType)
        {
            _rootLink = rootLink;
            if (!typeof(IContent).IsAssignableFrom(contentType) && !typeof(BlockData).IsAssignableFrom(contentType))
            {
                throw new ArgumentOutOfRangeException(contentType.Name + " must implement IContent");
            }
            _contentType = contentType;
        }

        public IContinuationBuilder ClusterByCreatedDate(string format)
        {
            return new ContinuationBuilder(_rootLink, _contentType, new CreatedDateClusterGenerator(format));
        }

        public IContinuationBuilder ClusterByCreatedYear()
        {
            return new ContinuationBuilder(_rootLink, _contentType, new CreatedDateClusterGenerator("yyyy"));
        }

        public IContinuationBuilder ClusterByName(int numberOfLetters = 1, string defaultName = "Default")
        {
            return new ContinuationBuilder(_rootLink, _contentType, new NameClusterGenerator(0, numberOfLetters, defaultName));
        }

        public IContinuationBuilder ClusterByCustomExpression(Func<IContent, string> expression)
        {
            return new ContinuationBuilder(_rootLink, _contentType, new ExpressionClusterGenerator(expression));
        }

        public static ContentClusteringBuilder For<T>(ContentReference rootLink) where T : IContent
        {
            return For(rootLink, typeof(T));
        }

        public static ContentClusteringBuilder For(ContentReference rootLink, Type contentType)
        {
            return new ContentClusteringBuilder(rootLink, contentType);
        }
    }

    public interface IContinuationBuilder
    {
        IContinuationBuilder ThenByCreatedDate(string format);
        IContinuationBuilder ThenByCreatedMonth();
        IContinuationBuilder ThenByCreatedDay();
        IContinuationBuilder ThenByName(int numberOfLetters, string defaultName);
        IContinuationBuilder ThenByNameSubstr(int startIndex, int count, string defaultName);
        IContinuationBuilder ThenByExpression(Func<IContent, string> expression);
        EventHandler<ContentEventArgs> Register(IContentEvents contentEvents);
    }

    internal class ContinuationBuilder : IContinuationBuilder
    {
        private readonly Type _contentType;
        private readonly ContentReference _rootLink;
        private readonly List<IClusterNameGenerator> _generators = new List<IClusterNameGenerator>();

        public ContinuationBuilder(ContentReference rootLink, Type contentType, IClusterNameGenerator defaultClusterNameGenerator)
        {
            _contentType = contentType;
            _rootLink = rootLink.ToReferenceWithoutVersion();
            this._generators.Add(defaultClusterNameGenerator);
        }

        public IContinuationBuilder ThenByCreatedDate(string format = "yyyy-mm")
        {
            _generators.Add(new CreatedDateClusterGenerator(format));
            return this;
        }

        public IContinuationBuilder ThenByCreatedMonth()
        {
            _generators.Add(new CreatedDateClusterGenerator("MM"));
            return this;
        }

        public IContinuationBuilder ThenByCreatedDay()
        {
            _generators.Add(new CreatedDateClusterGenerator("dd"));
            return this;
        }

        public IContinuationBuilder ThenByName(int numberOfLetters, string defaultName = "Default")
        {
            _generators.Add(new NameClusterGenerator(0, numberOfLetters, defaultName));
            return this;
        }

        public IContinuationBuilder ThenByNameSubstr(int startIndex, int countLetters, string defaultName = "Default")
        {
            _generators.Add(new NameClusterGenerator(startIndex, countLetters, defaultName));
            return this;
        }

        public IContinuationBuilder ThenByExpression(Func<IContent, string> expression)
        {
            _generators.Add(new ExpressionClusterGenerator(expression));
            return this;
        }

        public EventHandler<ContentEventArgs> Register(IContentEvents contentEvents)
        {
            contentEvents.SavingContent += ContentEvents_SavingContent;
            return ContentEvents_SavingContent;
        }

        private void ContentEvents_SavingContent(object sender, EPiServer.ContentEventArgs e)
        {
            if (e.Content == null || ContentReference.IsNullOrEmpty(e.Content.ParentLink))
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
