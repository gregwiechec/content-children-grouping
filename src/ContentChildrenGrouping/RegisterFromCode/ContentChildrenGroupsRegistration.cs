using System;
using System.Collections.Generic;
using ContentChildrenGrouping.ContainerModel;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.RegisterFromCode
{
    public interface IContentChildrenGroupsRegistration
    {
        void Register(ContainerConfiguration configuration);

        void RegisterByLetter(ContentReference containerId, Type containerType = null);

        void RegisterByCreateDate(ContentReference containerId, Type containerType = null);
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
    }
}