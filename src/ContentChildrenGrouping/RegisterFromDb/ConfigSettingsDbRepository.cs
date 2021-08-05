using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.RegisterFromDb
{
    public interface IConfigSettingsDbRepository
    {
        IEnumerable<ContainerConfiguration> LoadAll();

        void Save(IEnumerable<ContainerConfiguration> configurations);
    }

    /// <summary>
    /// Load and save config from DDS
    /// </summary>
    [ServiceConfiguration(typeof(IConfigSettingsDbRepository))]
    public class ConfigSettingsDbRepository : IConfigSettingsDbRepository
    {
        private readonly DynamicDataStoreFactory _dataStoreFactory;
        private readonly IReadOnlyList<IGroupNameGenerator> _groupNameGenerators;

        public ConfigSettingsDbRepository(DynamicDataStoreFactory dataStoreFactory,
            IEnumerable<IGroupNameGenerator> groupNameGenerators)
        {
            _dataStoreFactory = dataStoreFactory;
            _groupNameGenerators = groupNameGenerators.ToList();
        }

        public IEnumerable<ContainerConfiguration> LoadAll()
        {
            var result = GetStore().Items<ConfigurationSettingsDds>().ToList();
            return result.Select(x => new ContainerConfiguration
            {
                ContainerContentLink = x.ContainerContentLink,
                ContainerType = Type.GetType(x.ContainerType),
                RoutingEnabled = x.RoutingEnabled,
                GroupLevelConfigurations = (x.GroupLevelConfigurations ?? "").Split(',')
                    .Select(str => _groupNameGenerators.FirstOrDefault(g => g.Key == str)).Where(g => g != null)
            });
        }

        public void Save(IEnumerable<ContainerConfiguration> configurations)
        {
            var store = GetStore();

            var allConfigs = store.Items<ConfigurationSettingsDds>().ToList();

            var containerConfigurations = configurations.ToList();
            foreach (var containerConfiguration in allConfigs)
            {
                var userConfig= containerConfigurations.FirstOrDefault(x => x.ContainerContentLink == containerConfiguration.ContainerContentLink);
                if (userConfig == null)
                {
                    store.Delete(containerConfiguration.RoutingEnabled);
                    continue;
                }

                containerConfiguration.ContainerType = userConfig.ContainerType.TypeToString();
                containerConfiguration.GroupLevelConfigurations =
                    string.Join(",", userConfig.GroupLevelConfigurations.Select(x => x.Key));
                containerConfiguration.RoutingEnabled = userConfig.RoutingEnabled;
                store.Save(containerConfiguration);
                containerConfigurations.Remove(userConfig);
            }

            foreach (var userConfig in containerConfigurations)
            {
                var dds = new ConfigurationSettingsDds
                {
                    ContainerContentLink = userConfig.ContainerContentLink,
                    ContainerType = userConfig.ContainerType.TypeToString() ?? "",
                    RoutingEnabled = userConfig.RoutingEnabled,
                    GroupLevelConfigurations =
                        string.Join(",", userConfig.GroupLevelConfigurations.Select(x => x.Key))
                };
                store.Save(dds);
            }
        }

        private DynamicDataStore GetStore()
        {
            return _dataStoreFactory.GetStore(typeof(ConfigurationSettingsDds)) ??
                   _dataStoreFactory.CreateStore(typeof(ConfigurationSettingsDds));
        }
    }


    public static class ContainerTypeExtensions
    {
        public static string TypeToString(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            return $"{type.FullName}, {type.Assembly.GetName().Name}";

        }
    }
}