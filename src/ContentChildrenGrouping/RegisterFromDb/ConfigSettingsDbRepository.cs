﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Extensions;
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
        private static string DateFormat = "yyyy-MM-dd HH:mm:ss";

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
            return result.Select(x =>
            {
                DateTime.TryParseExact(x.ChangedOn, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date);
                return new ContainerConfiguration
                {
                    ContainerContentLink = x.ContainerContentLink,
                    ContainerType = string.IsNullOrWhiteSpace(x.ContainerType) ? null : Type.GetType(x.ContainerType),
                    RoutingEnabled = x.RoutingEnabled,
                    GroupLevelConfigurations = (x.GroupLevelConfigurations ?? "").Split(',')
                        .Select(str => _groupNameGenerators.FirstOrDefault(g => g.Key == str)).Where(g => g != null),
                    ChangedBy = x.ChangedBy,
                    ChangedOn = date
                };
            });
        }

        public void Save(IEnumerable<ContainerConfiguration> configurations)
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
                    string.Join(",", userConfig.GroupLevelConfigurations.Select(x => x.Key));
                ddsConfig.RoutingEnabled = userConfig.RoutingEnabled;
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
                    GroupLevelConfigurations =
                        string.Join(",", userConfig.GroupLevelConfigurations.Select(x => x.Key)),
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