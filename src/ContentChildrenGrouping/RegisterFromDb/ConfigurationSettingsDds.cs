using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace ContentChildrenGrouping.RegisterFromDb
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class ConfigurationSettingsDds : IDynamicData
    {
        public Identity Id { get; set; }

        [EPiServerDataIndex]
        public ContentReference ContainerContentLink { get; set; }

        public bool RoutingEnabled { get; set; }

        public string ContainerType { get; set; }

        public string GroupLevelConfigurations { get; set; }
    }
}