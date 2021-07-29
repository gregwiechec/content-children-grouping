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
                Data = new {
                    items = _configSettingsDbRepository.LoadAll().ToList().Select(x=> new ConfigurationViewModel
                    {
                        containerContentLink = x.ContainerContentLink.ToReferenceWithoutVersion().ID.ToString(),
                        containerType = x.ContainerType.FullName,
                        routingEnabled = x.RoutingEnabled,
                        groupLevelConfigurations = string.Join(",", x.GroupLevelConfigurations.Select(g=>g.Key))
                    }),
                    availableNameGenerators = _groupNameGenerators.Where(x=>x is IDbAvailableGroupNameGenerator).Select(x=>x.Key)
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

            _dbContentChildrenGroupsLoader.ClearCache();
            return new RestResult
            {
                Data = "ok",
                SafeResponse = true
            };
        }

        public class ConfigurationViewModel
        {
            public string containerContentLink { get; set; }
            public string containerType { get; set; }
            public bool routingEnabled { get; set; }
            public string groupLevelConfigurations { get; set; }
        }
    }
}

//http://localhost:50061/EPiServer/content-children-grouping/ConfigSettings/GetSomething