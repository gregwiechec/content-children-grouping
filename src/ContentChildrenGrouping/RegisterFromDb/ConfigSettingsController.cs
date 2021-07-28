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
                        ContainerContentLink = x.ContainerContentLink.ToReferenceWithoutVersion().ID.ToString(),
                        ContainerType = x.ContainerType.FullName,
                        RoutingEnabled = x.RoutingEnabled,
                        GroupLevelConfigurations = string.Join(",", x.GroupLevelConfigurations.Select(g=>g.Key))
                    }),
                    availableNameGenerators = _groupNameGenerators.Where(x=>x is IDbAvailableGroupNameGenerator).Select(x=>x.Key)
                }
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
                Data = "ok"
            };
        }

        public class ConfigurationViewModel
        {
            public string ContainerContentLink { get; set; }
            public string ContainerType { get; set; }
            public bool RoutingEnabled { get; set; }
            public string GroupLevelConfigurations { get; set; }
        }
    }
}

//http://localhost:50061/EPiServer/content-children-grouping/ConfigSettings/GetSomething