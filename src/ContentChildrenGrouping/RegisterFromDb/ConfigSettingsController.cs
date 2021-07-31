using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;

namespace ContentChildrenGrouping.RegisterFromDb
{
    public class ConfigSettingsController : Controller
    {
        private readonly IConfigSettingsDbRepository _configSettingsDbRepository;
        private readonly DbContentChildrenGroupsLoader _dbContentChildrenGroupsLoader;
        private readonly IEnumerable<IGroupNameGenerator> _groupNameGenerators;

        public ConfigSettingsController(IConfigSettingsDbRepository configSettingsDbRepository,
            DbContentChildrenGroupsLoader dbContentChildrenGroupsLoader,
            IEnumerable<IGroupNameGenerator> groupNameGenerators)
        {
            _configSettingsDbRepository = configSettingsDbRepository;
            _dbContentChildrenGroupsLoader = dbContentChildrenGroupsLoader;
            _groupNameGenerators = groupNameGenerators;
        }

        public ActionResult LoadConfigurations()
        {
            return new RestResult
            {
                Data = new
                {
                    items = _configSettingsDbRepository.LoadAll().ToList().Select(x => new ConfigurationViewModel
                    {
                        contentLink = x.ContainerContentLink.ToReferenceWithoutVersion().ID.ToString(),
                        containerTypeName = x.ContainerType.TypeToString(),
                        routingEnabled = x.RoutingEnabled,
                        groupLevelConfigurations = x.GroupLevelConfigurations.Select(g => g.Key)
                    }),
                    availableNameGenerators = _groupNameGenerators.Where(x => x is IDbAvailableGroupNameGenerator)
                        .Select(x => x.Key)
                },
                SafeResponse = true
            };
        }

        [HttpPost]
        public ActionResult Save(IEnumerable<ConfigurationViewModel> configs)
        {
            if (configs == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var containerConfigurations = configs.Select(x => new ContainerConfiguration
            {
                ContainerContentLink = ContentReference.Parse(x.contentLink),
                ContainerType = Type.GetType(x.containerTypeName),
                RoutingEnabled = x.routingEnabled,
                GroupLevelConfigurations = x.groupLevelConfigurations
                    .Select(g => _groupNameGenerators.Single(n => n.Key == g))
            }).ToList();
            _configSettingsDbRepository.Save(containerConfigurations);

            _dbContentChildrenGroupsLoader.ClearCache();
            return new RestResult
            {
                Data = "ok",
                SafeResponse = true
            };
        }

        public class ConfigurationViewModel
        {
            public string contentLink { get; set; }
            public string containerTypeName { get; set; }
            public bool routingEnabled { get; set; }
            public IEnumerable<string> groupLevelConfigurations { get; set; }
        }
    }
}