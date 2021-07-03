using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace EPiServer.Labs.ContentManager.ContentClustering
{
    /*
    public static class ClusteringExtensions
    {
        public static void RegisterClusterByNameForAllViews(this ContentManagerOptions configuration)
        {
            foreach (var view in ServiceLocator.Current.GetInstance<ExternalViewsResolver>().GetViews())
            {
                view.RegisterClusterByName();
            }
        }

        public static void RegisterClusterByDateForAllViews(this ContentManagerOptions configuration)
        {
            foreach (var view in ServiceLocator.Current.GetInstance<ExternalViewsResolver>().GetViews())
            {
                view.RegisterClusterByDate();
            }
        }

        public static void RegisterClusterByName(this ContentManagerViewDefinition viewConfiguration)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();

            foreach (var contentType in viewConfiguration.AllowedTypesToAdd)
            {
                ContentClusteringBuilder.For(viewConfiguration.Root, contentType.ModelType).ClusterByName(2).Register(contentEvents);
            }
        }

        public static void RegisterClusterByDate(this ContentManagerViewDefinition viewConfiguration)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();

            foreach (var contentType in viewConfiguration.AllowedTypesToAdd)
            {
                ContentClusteringBuilder.For(viewConfiguration.Root, contentType.ModelType).ClusterByCreatedDate("yy_MM")
                    .Register(contentEvents);
            }
        }
    }
    */

    public class ContainerConfiguration
    {
        public ContentReference ContentLink { get; set; }

        public IEnumerable<IClusterNameGenerator> GroupLevelConfigurations { get; set; }
    }

    /// <summary>
    /// Load configurations for containers
    /// </summary>
    public interface IContentChildrenGroupsLoader
    {
        int Rank { get; }

        IEnumerable<ContainerConfiguration> GetConfigurations();
    }

    public class ContentChildrenGroupsRegistration
    {


        public void RegisterByLetter(ContentReference parentPage)
        {

        }

        public void RegisterByCreateDate(ContentReference parentPage)
        {

        }
    }
}