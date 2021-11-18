using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.RegisterFromDb;
using ContentChildrenGrouping.VirtualContainers;
using ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator;
using ContentChildrenGrouping.VirtualContainers.Extensions;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Shell.Services.Rest;

namespace ContentChildrenGrouping.Containers.RegisterFromDb
{
    /// <summary>
    /// Controller responsible for managing all Admin Mode plugin REST actions
    /// </summary>
    public class ConfigSettingsController : Controller
    {
        private readonly IConfigSettingsDbRepository _configSettingsDbRepository;
        private readonly DbContentChildrenGroupsLoader _dbContentChildrenGroupsLoader;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _childrenGroupsLoaders;
        private readonly IEnumerable<IDbAvailableGroupNameGenerator> _groupNameGenerators;
        private readonly IContentLoader _contentLoader;
        private readonly IPrincipalAccessor _principalAccessor;

        public ConfigSettingsController(IConfigSettingsDbRepository configSettingsDbRepository,
            DbContentChildrenGroupsLoader dbContentChildrenGroupsLoader,
            IEnumerable<IContentChildrenGroupsLoader> childrenGroupsLoaders,
            IEnumerable<IDbAvailableGroupNameGenerator> groupNameGenerators,
            IContentLoader contentLoader,
            IPrincipalAccessor principalAccessor)
        {
            _configSettingsDbRepository = configSettingsDbRepository;
            _dbContentChildrenGroupsLoader = dbContentChildrenGroupsLoader;
            _childrenGroupsLoaders = childrenGroupsLoaders;
            _groupNameGenerators = groupNameGenerators;
            _contentLoader = contentLoader;
            _principalAccessor = principalAccessor;
        }

        [HttpGet]
        public ActionResult LoadConfigurations()
        {
            return new RestResult
            {
                Data = new
                {
                    items = LoadItems()
                },
                SafeResponse = true
            };
        }

        private IEnumerable<ConfigurationViewModel> LoadItems()
        {
            var configurationViewModels = new List<ConfigurationViewModel>();
            foreach (var contentChildrenGroupsLoader in _childrenGroupsLoaders)
            {
                var containerConfigurations = contentChildrenGroupsLoader.GetConfigurations();
                foreach (var containerConfiguration in containerConfigurations)
                {
                    var viewModel = ConvertModelToViewModel(containerConfiguration, !(contentChildrenGroupsLoader is DbContentChildrenGroupsLoader));
                    configurationViewModels.Add(viewModel);
                }
            }

            configurationViewModels = configurationViewModels.OrderBy(x => x.contentLink).ToList();

            return configurationViewModels;
        }

        private ConfigurationViewModel ConvertModelToViewModel(ContainerConfiguration containerConfiguration, bool fromCode)
        {
            var contentExists = _contentLoader.TryGet<IContent>(containerConfiguration.ContainerContentLink, out var _);

            var dbConfiguration = containerConfiguration as DbContainerConfiguration;
            return new ConfigurationViewModel
            {
                contentLink = containerConfiguration.ContainerContentLink.ToReferenceWithoutVersion().ID
                    .ToString(),
                groupLevelConfigurations = containerConfiguration.GroupLevelConfigurations.Select(g =>
                {
                    var dbGenerator = g as IDbAvailableGroupNameGenerator;
                    var generatorSettingsViewModel = new GeneratorSettingsViewModel
                    {
                        name = dbGenerator == null ? g.GetType().Name : dbGenerator.Key,
                        settings = dbGenerator == null ? new Dictionary<string, string>() : dbGenerator.Settings
                    };
                    return generatorSettingsViewModel;
                }),
                fromCode = fromCode,
                contentExists = contentExists,
                changedBy = dbConfiguration?.ChangedBy ?? "",
                changedOn = dbConfiguration?.ChangedOn?.ToString("yyyy-MM-dd HH:mm") ?? ""
            };
        }

        [HttpGet]
        public ActionResult Get(ContentReference contentLink)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ContentLink cannot be empty");
            }

            var containerConfigurations = _childrenGroupsLoaders.GetAllConfigurations();
            var configuration = containerConfigurations.FirstOrDefault(x => x.ContainerContentLink == contentLink);
            if (configuration == null)
            {
                return GetError($"Configuration not found for [{contentLink}]");
            }

            var isDbConfig = _dbContentChildrenGroupsLoader.GetConfigurations()
                .Any(x => x.ContainerContentLink == contentLink);

            var viewModel = ConvertModelToViewModel(configuration, !isDbConfig);

            return new RestResult
            {
                Data = viewModel,
                SafeResponse = true
            };
        }

        [HttpPost]
        public ActionResult Save(SaveConfigurationViewModel config)
        {
            if (config == null)
            {
                return GetError("Configuration is empty");
            }

            if (!ContentReference.TryParse(config.contentLink, out var contentLink))
            {
                return GetError("ContentLink has wrong format");
            }

            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                return GetError("ContentLink cannot be empty");
            }

            if (!_contentLoader.TryGet<IContent>(contentLink, out var _))
            {
                return GetError($"Content not found for {contentLink} content link");
            }

            DbContainerConfiguration ConvertViewModelToModel()
            {
                return new DbContainerConfiguration
                {
                    ContainerContentLink = contentLink,
                    GroupLevelConfigurations = config.groupLevelConfigurations
                        .Select(g => _groupNameGenerators.Single(n => n.Key.CompareStrings(g.name)).CreateGenerator(g.settings)),
                    ChangedBy = _principalAccessor?.Principal?.Identity?.Name,
                    ChangedOn = DateTime.Now
                };
            }

            if (config.IsNew)
            {
                var containerConfigurations = _childrenGroupsLoaders.GetAllConfigurations().ToList();
                if (containerConfigurations.Any(x => x.ContainerContentLink == contentLink))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                        $"Cannot register two configurations with same ContentLink: [{contentLink}]");
                }

                var newConfiguration = ConvertViewModelToModel();
                var dbContainerConfigurations = _configSettingsDbRepository.LoadAll().ToList();
                dbContainerConfigurations.Add(newConfiguration);
                _configSettingsDbRepository.Save(dbContainerConfigurations);
            }
            else
            {
                var containerConfigurations = _configSettingsDbRepository.LoadAll().ToList();
                var configuration = containerConfigurations.FirstOrDefault(x => x.ContainerContentLink == contentLink);
                if (configuration == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                        $"Database configuration not found for ContentLink: [{contentLink}]");
                }

                var updatedConfig = ConvertViewModelToModel();
                var index = containerConfigurations.IndexOf(configuration);
                containerConfigurations[index] = updatedConfig;
                _configSettingsDbRepository.Save(containerConfigurations);
            }

            _dbContentChildrenGroupsLoader.ClearCache();
            return Get(contentLink);
        }

        [HttpDelete]
        public ActionResult Delete(ContentReference contentLink)
        {
            var containerConfigurations = _configSettingsDbRepository.LoadAll().ToList();
            var configuration = containerConfigurations.FirstOrDefault(x => x.ContainerContentLink == contentLink);
            if (configuration == null)
            {
                return GetError($"Configuration for {contentLink} not found");
            }

            containerConfigurations.Remove(configuration);
            _configSettingsDbRepository.Save(containerConfigurations);
            _dbContentChildrenGroupsLoader.ClearCache();
            return new RestResult
            {
                Data = LoadItems(),
                SafeResponse = true
            };
        }

        private ActionResult GetError(string message)
        {
            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Configuration for {contentLink} not found");
            //return new RestStatusCodeResult(HttpStatusCode.BadRequest, message);
            return new RestResult
            {
                Data = new
                {
                    Error = message,
                    Status = HttpStatusCode.BadRequest
                },
                SafeResponse = true
            };
        }

        public class ClearContainersViewModel
        {
            public ContentReference contentLink { get; set; }
        }

        public class ConfigurationViewModel
        {
            public string contentLink { get; set; }
            public bool fromCode { get; set; }
            public IEnumerable<GeneratorSettingsViewModel> groupLevelConfigurations { get; set; }
            public bool contentExists { get; set; }
            public string changedBy { get; set; }
            public string changedOn { get; set; }
        }

        public class SaveConfigurationViewModel : ConfigurationViewModel
        {
            public bool IsNew { get; set; }
        }
    }

    public class GeneratorSettingsViewModel
    {
        public string name { get; set; }
        public Dictionary<string, string> settings { get; set; }
    }
}