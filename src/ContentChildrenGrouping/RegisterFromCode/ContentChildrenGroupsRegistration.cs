using System;
using System.Collections.Generic;
using ContentChildrenGrouping.Core;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Containers.RegisterFromCode
{
    public interface IContentChildrenGroupsRegistration
    {
        void Register(ContainerConfiguration configuration);
    }

    public static class ContentChildrenGroupsRegistrationExtensions
    {
        public static void RegisterByLetter(this IContentChildrenGroupsRegistration childrenGroupsRegistration,
            int containerId, Type containerType = null, int countLetters = 1)
        {
            childrenGroupsRegistration.Register(new ContainerConfiguration
            {
                ContainerContentLink = new ContentReference(containerId),
                ContainerType = containerType,
                GroupLevelConfigurations = new[] {new ByNameGroupNameGenerator(0, countLetters)}
            });
        }

        public static void RegisterByCreateDate(this IContentChildrenGroupsRegistration childrenGroupsRegistration,
            int containerId, string dateFormat = ByCreateDateGroupNameGenerator.DefaultFormat,
            Type containerType = null)
        {
            childrenGroupsRegistration.Register(new ContainerConfiguration
            {
                ContainerContentLink = new ContentReference(containerId),
                ContainerType = containerType,
                GroupLevelConfigurations = new[] {new ByCreateDateGroupNameGenerator(dateFormat)}
            });
        }

        public static void RegisterVirtualContainerByLetter(
            this IContentChildrenGroupsRegistration childrenGroupsRegistration, int containerId, int countLetters = 1)
        {
            childrenGroupsRegistration.Register(new ContainerConfiguration
            {
                ContainerContentLink = new ContentReference(containerId),
                IsVirtualContainer = true,
                GroupLevelConfigurations = new[] {new ByNameGroupNameGenerator(0, countLetters)}
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