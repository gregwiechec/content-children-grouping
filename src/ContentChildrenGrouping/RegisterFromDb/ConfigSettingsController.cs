﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.Extensions;
using ContentChildrenGrouping.Extensions;
using ContentChildrenGrouping.RegisterFromDb;
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

        public ConfigSettingsController(IConfigSettingsDbRepository configSettingsDbRepository,
            DbContentChildrenGroupsLoader dbContentChildrenGroupsLoader,
            IEnumerable<IContentChildrenGroupsLoader> childrenGroupsLoaders,
            IEnumerable<IGroupNameGenerator> groupNameGenerators,
            ContentStructureCleaner contentStructureCleaner
        )
        {
            _configSettingsDbRepository = configSettingsDbRepository;
            _dbContentChildrenGroupsLoader = dbContentChildrenGroupsLoader;
            _childrenGroupsLoaders = childrenGroupsLoaders;
            _groupNameGenerators = groupNameGenerators;
            _contentStructureCleaner = contentStructureCleaner;
        }

        public ActionResult LoadConfigurations()
        {
            var configurationViewModels = new List<ConfigurationViewModel>();
            foreach (var contentChildrenGroupsLoader in _childrenGroupsLoaders)
            {
                var containerConfigurations = contentChildrenGroupsLoader.GetConfigurations();
                foreach (var containerConfiguration in containerConfigurations)
                {
                    var viewModel = new ConfigurationViewModel
                    {
                        contentLink = containerConfiguration.ContainerContentLink.ToReferenceWithoutVersion().ID
                            .ToString(),
                        containerTypeName = containerConfiguration.ContainerType.TypeToString(),
                        routingEnabled = containerConfiguration.RoutingEnabled,
                        groupLevelConfigurations = containerConfiguration.GroupLevelConfigurations.Select(g => g.Key),
                        fromCode = !(contentChildrenGroupsLoader is DbContentChildrenGroupsLoader)
                    };
                    configurationViewModels.Add(viewModel);
                }
            }

            return new RestResult
            {
                Data = new
                {
                    items = configurationViewModels
                },
                SafeResponse = true
            };
        }

        [HttpPost]
        public ActionResult Save(SaveConfigurationViewModel config)
        {
            if (config == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Configuration is empty");
            }

            if (!ContentReference.TryParse(config.contentLink, out var contentLink))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ContentLink has wrong format");
            }

            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ContentLink cannot be empty");
            }


            ContainerConfiguration ConvertViewModelToModel()
            {
                return new ContainerConfiguration
                {
                    ContainerContentLink = ContentReference.Parse(config.contentLink),
                    ContainerType = string.IsNullOrWhiteSpace(config.containerTypeName)
                        ? null
                        : Type.GetType(config.containerTypeName),
                    RoutingEnabled = config.routingEnabled,
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
            return new RestResult
            {
                Data = "ok",
                SafeResponse = true
            };
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Configuration for {contentLink} not found");
            }

            containerConfigurations.Remove(configuration);
            _configSettingsDbRepository.Save(containerConfigurations);
            _dbContentChildrenGroupsLoader.ClearCache();
            return LoadConfigurations();
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
        }

        public class SaveConfigurationViewModel : ConfigurationViewModel
        {
            public bool IsNew { get; set; }
        }
    }
}