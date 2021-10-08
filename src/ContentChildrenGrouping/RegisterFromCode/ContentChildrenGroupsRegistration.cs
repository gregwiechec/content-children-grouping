using System;
using System.Collections.Generic;
using ContentChildrenGrouping.ContainerModel;
using ContentChildrenGrouping.Core;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.RegisterFromCode
{
    public interface IContentChildrenGroupsRegistration
    {
        void Register(ContainerConfiguration configuration);

        void RegisterByLetter(ContentReference containerId, Type containerType = null);

        void RegisterByCreateDate(ContentReference containerId, Type containerType = null);

        void RegisterVirtualContainerByLetter(ContentReference containerId, int countLetters = 1);
    }

    public static class ContentChildrenGroupsRegistrationExtensions
    {
        public static void RegisterByLetter(this IContentChildrenGroupsRegistration childrenGroupsRegistration,
            int containerId, Type containerType = null)
        {
            childrenGroupsRegistration.RegisterByLetter(new ContentReference(containerId), containerType);
        }

        public static void RegisterVirtualContainerByLetter(
            this IContentChildrenGroupsRegistration childrenGroupsRegistration, int containerId, int countLetters = 1)
        {
            childrenGroupsRegistration.RegisterVirtualContainerByLetter(new ContentReference(containerId), countLetters);
        }
    }

    [ServiceConfiguration(typeof(IContentChildrenGroupsRegistration), Lifecycle = ServiceInstanceScope.Singleton)]
    internal class ContentChildrenGroupsRegistration : IContentChildrenGroupsRegistration
    {
        internal List<ContainerConfiguration> Configurations { get; private set; } = new List<ContainerConfiguration>();

        public void Register(ContainerConfiguration configuration)
        {
            Configurations.Add(configuration);
        }

        public void RegisterByLetter(ContentReference containerId, Type containerType = null)
        {
            Register(new ContainerConfiguration
            {
                ContainerContentLink = containerId,
                ContainerType = containerType,
                GroupLevelConfigurations = new[] {new ByNameGroupNameGenerator(0, 1, "_no_category")}
            });
        }

        public void RegisterByCreateDate(ContentReference containerId, Type containerType = null)
        {
            Register(new ContainerConfiguration
            {
                ContainerContentLink = containerId,
                ContainerType = containerType,
                GroupLevelConfigurations = new[] {new ByCreateDateGroupNameGenerator("yy-mm-dd", "no_date")}
            });
        }

        public void RegisterVirtualContainerByLetter(ContentReference containerId, int countLetters)
        {
            Register(new ContainerConfiguration
            {
                ContainerContentLink = containerId,
                IsVirtualContainer = true,
                GroupLevelConfigurations = new[] { new ByNameGroupNameGenerator(0, countLetters, "_no_category") }
            });
        }
   }
}