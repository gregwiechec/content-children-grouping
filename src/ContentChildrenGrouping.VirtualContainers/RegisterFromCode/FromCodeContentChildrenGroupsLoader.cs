using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.VirtualContainers;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Core.RegisterFromCode
{
    /// <summary>
    /// Containers configuration loader that loads configurations registered from code
    /// </summary>
    [ServiceConfiguration(typeof(IContentChildrenGroupsLoader))]
    public class FromCodeContentChildrenGroupsLoader : IContentChildrenGroupsLoader
    {
        private readonly IContentChildrenGroupsRegistration _contentChildrenGroupsRegistration;

        public FromCodeContentChildrenGroupsLoader(IContentChildrenGroupsRegistration contentChildrenGroupsRegistration)
        {
            _contentChildrenGroupsRegistration = contentChildrenGroupsRegistration;
        }

        public int Rank { get; } = 200;

        public IEnumerable<ContainerConfiguration> GetConfigurations()
        {
            if (_contentChildrenGroupsRegistration is ContentChildrenGroupsRegistration
                contentChildrenGroupsRegistration)
            {
                return contentChildrenGroupsRegistration.Configurations;
            }

            return Enumerable.Empty<ContainerConfiguration>();
        }
    }
}