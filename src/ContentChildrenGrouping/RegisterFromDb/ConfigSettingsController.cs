using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.Extensions;
using ContentChildrenGrouping.Extensions;
using ContentChildrenGrouping.RegisterFromDb;
using EPiServer;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;

namespace ContentChildrenGrouping.Containers.RegisterFromDb
{
    /// <summary>
    /// Controller reposnsible for managing all Admin Mode plugin REST actions
    /// </summary>
    public class ConfigSettingsController : Controller
    {
        private readonly IConfigSettingsDbRepository _configSettingsDbRepository;
        private readonly DbContentChildrenGroupsLoader _dbContentChildrenGroupsLoader;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _childrenGroupsLoaders;
        private readonly IEnumerable<IGroupNameGenerator> _groupNameGenerators;
        private readonly ContentStructureCleaner _contentStructureCleaner;
        private readonly IContentLoader _contentLoader;

        public ConfigSettingsController(IConfigSettingsDbRepository configSettingsDbRepository,
            DbContentChildrenGroupsLoader dbContentChildrenGroupsLoader,
            IEnumerable<IContentChildrenGroupsLoader> childrenGroupsLoaders,
            IEnumerable<IGroupNameGenerator> groupNameGenerators,
            ContentStructureCleaner contentStructureCleaner, IContentLoader contentLoader)
        {
            _configSettingsDbRepository = configSettingsDbRepository;
            _dbContentChildrenGroupsLoader = dbContentChildrenGroupsLoader;
            _childrenGroupsLoaders = childrenGroupsLoaders;
            _groupNameGenerators = groupNameGenerators;
            _contentStructureCleaner = contentStructureCleaner;
            _contentLoader = contentLoader;
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
                    var contentExists = _contentLoader.TryGet<IContent>(containerConfiguration.ContainerContentLink, out var _);

                    var viewModel = new ConfigurationViewModel
                    {
                        contentLink = containerConfiguration.ContainerContentLink.ToReferenceWithoutVersion().ID
                            .ToString(),
                        containerTypeName = containerConfiguration.ContainerType.TypeToString(),
                        routingEnabled = containerConfiguration.RoutingEnabled,
                        groupLevelConfigurations = containerConfiguration.GroupLevelConfigurations.Select(g => g.Key),
                        isVirtualContainer = containerConfiguration.IsVirtualContainer,
                        fromCode = !(contentChildrenGroupsLoader is DbContentChildrenGroupsLoader),
                        contentExists = contentExists
                    };
                    configurationViewModels.Add(viewModel);
                }
            }

            configurationViewModels = configurationViewModels.OrderBy(x => x.contentLink).ToList();

            return configurationViewModels;
        }

        [HttpGet]
        public ActionResult Get(ContentReference contentLink)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ContentLink cannot be empty");
            }


            var containerConfigurations = _configSettingsDbRepository.LoadAll().ToList();
            var configuration = containerConfigurations.FirstOrDefault(x => x.ContainerContentLink == contentLink);
            if (configuration == null)
            {
                return GetError($"Configuration not found for [{contentLink}]");
            }

            var isDbConfig = _dbContentChildrenGroupsLoader.GetConfigurations()
                .Any(x => x.ContainerContentLink == contentLink);

            var viewModel = new ConfigurationViewModel
            {
                contentLink = configuration.ContainerContentLink.ToReferenceWithoutVersion().ID
                    .ToString(),
                containerTypeName = configuration.ContainerType.TypeToString(),
                routingEnabled = configuration.RoutingEnabled,
                groupLevelConfigurations = configuration.GroupLevelConfigurations.Select(g => g.Key),
                isVirtualContainer = configuration.IsVirtualContainer,
                fromCode = !isDbConfig
            };

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


            Type containerType;
            try
            {
                containerType = config.isVirtualContainer || string.IsNullOrWhiteSpace(config.containerTypeName)
                    ? null
                    : Type.GetType(config.containerTypeName, true);
            }
            catch (Exception e)
            {
                return GetError("Cannot parse type: " + e.Message);
            }

            ContainerConfiguration ConvertViewModelToModel()
            {
                return new ContainerConfiguration
                {
                    ContainerContentLink = contentLink,
                    ContainerType = containerType,
                    RoutingEnabled = config.isVirtualContainer ? false :  config.routingEnabled,
                    IsVirtualContainer = config.isVirtualContainer,
                    GroupLevelConfigurations = config.groupLevelConfigurations
                        .Select(g => _groupNameGenerators.Single(n => n.Key == g))
                };
            }

            if (config.IsNew)
            {
                var containerConfigurations = _childrenGroupsLoaders.GetAllContainersConfigurations().ToList();
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

        [HttpPost]
        public ActionResult ClearContainers(ClearContainersViewModel clearContainersViewModel)
        {
            _contentStructureCleaner.ClearContainers(clearContainersViewModel.contentLink, out var message);
            return new RestResult
            {
                Data = message,
                SafeResponse = true
            };
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
            public string containerTypeName { get; set; }
            public bool routingEnabled { get; set; }
            public bool fromCode { get; set; }
            public bool isVirtualContainer { get; set; }
            public IEnumerable<string> groupLevelConfigurations { get; set; }
            public bool contentExists { get; set; }
        }

        public class SaveConfigurationViewModel : ConfigurationViewModel
        {
            public bool IsNew { get; set; }
        }
    }
}