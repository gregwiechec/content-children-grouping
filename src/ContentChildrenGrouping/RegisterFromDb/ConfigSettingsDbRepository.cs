using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ContentChildrenGrouping.Containers.RegisterFromDb;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.ContainerNameGenerator;
using ContentChildrenGrouping.Extensions;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.RegisterFromDb
{
    public interface IConfigSettingsDbRepository
    {
        IEnumerable<DbContainerConfiguration> LoadAll();

        void Save(IEnumerable<DbContainerConfiguration> configurations);
    }

    /// <summary>
    /// Load and save config from DDS
    /// </summary>
    [ServiceConfiguration(typeof(IConfigSettingsDbRepository))]
    public class ConfigSettingsDbRepository : IConfigSettingsDbRepository
    {
        private static string DateFormat = "yyyy-MM-dd HH:mm:ss";

        private readonly DynamicDataStoreFactory _dataStoreFactory;
        private readonly IReadOnlyList<IGroupNameGenerator> _groupNameGenerators;
        private readonly GroupNameSerializer _nameGeneratorSerializer;

        public ConfigSettingsDbRepository(DynamicDataStoreFactory dataStoreFactory,
            IEnumerable<IGroupNameGenerator> groupNameGenerators, GroupNameSerializer nameGeneratorSerializer)
        {
            _dataStoreFactory = dataStoreFactory;
            _nameGeneratorSerializer = nameGeneratorSerializer;
            _groupNameGenerators = groupNameGenerators.ToList();
        }

        public IEnumerable<DbContainerConfiguration> LoadAll()
        {
            var result = GetStore().Items<ConfigurationSettingsDds>().ToList();
            return result.Select(x =>
            {
                DateTime.TryParseExact(x.ChangedOn, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date);
                return new DbContainerConfiguration
                {
                    ContainerContentLink = x.ContainerContentLink,
                    ContainerType = string.IsNullOrWhiteSpace(x.ContainerType) ? null : Type.GetType(x.ContainerType),
                    RoutingEnabled = x.RoutingEnabled,
                    IsVirtualContainer = x.IsVirtualContainer,
                    GroupLevelConfigurations = _nameGeneratorSerializer.Deserialize(x.GroupLevelConfigurations),
                    ChangedBy = x.ChangedBy,
                    ChangedOn = date
                };
            });
        }

        public void Save(IEnumerable<DbContainerConfiguration> configurations)
        {
            var store = GetStore();

            var ddsConfigs = store.Items<ConfigurationSettingsDds>().ToList();

            var containerConfigurations = configurations.ToList();
            foreach (var ddsConfig in ddsConfigs)
            {
                var userConfig = containerConfigurations.FirstOrDefault(x => x.ContainerContentLink == ddsConfig.ContainerContentLink);
                if (userConfig == null)
                {
                    store.Delete(ddsConfig);
                    continue;
                }

                ddsConfig.ContainerType = userConfig.ContainerType.TypeToString();
                ddsConfig.GroupLevelConfigurations =
                    _nameGeneratorSerializer.Serialize(userConfig.GroupLevelConfigurations);
                ddsConfig.RoutingEnabled = userConfig.RoutingEnabled;
                ddsConfig.IsVirtualContainer = userConfig.IsVirtualContainer;
                store.Save(ddsConfig);
                containerConfigurations.Remove(userConfig);
            }

            foreach (var userConfig in containerConfigurations)
            {
                var dds = new ConfigurationSettingsDds
                {
                    ContainerContentLink = userConfig.ContainerContentLink,
                    ContainerType = userConfig.ContainerType.TypeToString() ?? "",
                    RoutingEnabled = userConfig.RoutingEnabled,
                    IsVirtualContainer = userConfig.IsVirtualContainer,
                    GroupLevelConfigurations = _nameGeneratorSerializer.Serialize(userConfig.GroupLevelConfigurations),
                    ChangedBy = userConfig.ChangedBy,
                    ChangedOn = userConfig.ChangedOn?.ToString(DateFormat)
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
}