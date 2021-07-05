using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.RegisterFromCode
{
    [ServiceConfiguration(typeof(IContentChildrenGroupsLoader))]
    public class FromCodeContentChildrenGroupsLoader : IContentChildrenGroupsLoader
    {
        private readonly IContentChildrenGroupsRegistration _contentChildrenGroupsRegistration;

        public FromCodeContentChildrenGroupsLoader(IContentChildrenGroupsRegistration contentChildrenGroupsRegistration)
        {
            _contentChildrenGroupsRegistration = contentChildrenGroupsRegistration;
        }

        public int Rank { get; } = 100;

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