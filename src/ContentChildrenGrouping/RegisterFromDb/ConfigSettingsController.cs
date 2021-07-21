using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EPiServer.Shell.Services.Rest;

namespace ContentChildrenGrouping.RegisterFromDb
{
    public class ConfigSettingsController : Controller
    {
        private readonly IConfigSettingsDbRepository _configSettingsDbRepository;
        private readonly DbContentChildrenGroupsLoader _dbContentChildrenGroupsLoader;

        public ConfigSettingsController(IConfigSettingsDbRepository configSettingsDbRepository,
            DbContentChildrenGroupsLoader dbContentChildrenGroupsLoader)
        {
            _configSettingsDbRepository = configSettingsDbRepository;
            _dbContentChildrenGroupsLoader = dbContentChildrenGroupsLoader;
        }

        public ActionResult LoadConfigurations()
        {
            return new RestResult
            {
                Data = _configSettingsDbRepository.LoadAll().ToList()
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