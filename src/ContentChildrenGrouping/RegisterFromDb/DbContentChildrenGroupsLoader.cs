using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.RegisterFromDb
{
    [ServiceConfiguration(typeof(DbContentChildrenGroupsLoader))]
    [ServiceConfiguration(typeof(IContentChildrenGroupsLoader))]
    public class DbContentChildrenGroupsLoader : IContentChildrenGroupsLoader
    {
        private readonly IConfigSettingsDbRepository _configSettingsDbRepository;
        private readonly ContentChildrenGroupingOptions _options;
        private readonly object _lock = new object();

        public int Rank => 100;

        private const string CacheKey = "Configurations";

        public DbContentChildrenGroupsLoader(IConfigSettingsDbRepository configSettingsDbRepository,
            ContentChildrenGroupingOptions options)
        {
            _configSettingsDbRepository = configSettingsDbRepository;
            _options = options;
        }

        public IEnumerable<ContainerConfiguration> GetConfigurations()
        {
            if (!_options.DatabaseConfigurationsEnabled)
            {
                return Enumerable.Empty<ContainerConfiguration>();
            }

            if (EPiServer.CacheManager.Get(CacheKey) is IEnumerable<ContainerConfiguration> containerConfigurations)
            {
                return containerConfigurations;
            }

            lock (_lock)
            {
                var configurations = _configSettingsDbRepository.LoadAll().ToList();
                EPiServer.CacheManager.Insert(CacheKey, configurations);
                return configurations;
            }
        }

        public void ClearCache()
        {
            EPiServer.CacheManager.Remove(CacheKey);
        }
    }
}

//TODO: grouping remove hash from javascripts