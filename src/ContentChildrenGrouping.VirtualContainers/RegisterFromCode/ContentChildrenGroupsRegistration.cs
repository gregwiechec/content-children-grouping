using System;
using System.Collections.Generic;
using ContentChildrenGrouping.Core.ContainerNameGenerator;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Core.RegisterFromCode
{
    public interface IContentChildrenGroupsRegistration
    {
        void Register(ContainerConfiguration configuration);
    }

    public static class ContentChildrenGroupsRegistrationExtensions
    {
        public static void RegisterByLetter(this IContentChildrenGroupsRegistration childrenGroupsRegistration,
            int containerId, int countLetters = 1)
        {
            childrenGroupsRegistration.Register(new ContainerConfiguration
            {
                ContainerContentLink = new ContentReference(containerId),
                GroupLevelConfigurations = new[] {new ByNameGroupNameGenerator(0, countLetters)}
            });
        }

        public static void RegisterByCreateDate(this IContentChildrenGroupsRegistration childrenGroupsRegistration,
            int containerId, string dateFormat = ByCreateDateGroupNameGenerator.DefaultFormat)
        {
            childrenGroupsRegistration.Register(new ContainerConfiguration
            {
                ContainerContentLink = new ContentReference(containerId),
                GroupLevelConfigurations = new[] {new ByCreateDateGroupNameGenerator(dateFormat)}
            });
        }
    }

    [ServiceConfiguration(typeof(IContentChildrenGroupsRegistration), Lifecycle = ServiceInstanceScope.Singleton)]
    internal class ContentChildrenGroupsRegistration : IContentChildrenGroupsRegistration
    {
        internal List<ContainerConfiguration> Configurations { get; private set; } = new List<ContainerConfiguration>();

        public void Register(ContainerConfiguration configuration)
        {
            if (ContentReference.IsNullOrEmpty(configuration.ContainerContentLink))
            {
                throw new ArgumentException("Configuration contentLink cannot be empty");
            }
            Configurations.Add(configuration);
        }
    }
}