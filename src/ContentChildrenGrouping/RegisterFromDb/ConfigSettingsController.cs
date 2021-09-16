﻿using System;
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
        private readonly IEnumerable<IContentChildrenGroupsLoader> _childrenGroupsLoaders;
        private readonly IEnumerable<IGroupNameGenerator> _groupNameGenerators;
        private readonly ContentChildrenGroupingOptions _childrenGroupingOptions;

        public ConfigSettingsController(IConfigSettingsDbRepository configSettingsDbRepository,
            DbContentChildrenGroupsLoader dbContentChildrenGroupsLoader,
            IEnumerable<IContentChildrenGroupsLoader> childrenGroupsLoaders,
            IEnumerable<IGroupNameGenerator> groupNameGenerators,
            ContentChildrenGroupingOptions childrenGroupingOptions)
        {
            _configSettingsDbRepository = configSettingsDbRepository;
            _dbContentChildrenGroupsLoader = dbContentChildrenGroupsLoader;
            _childrenGroupsLoaders = childrenGroupsLoaders;
            _groupNameGenerators = groupNameGenerators;
            _childrenGroupingOptions = childrenGroupingOptions;
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
                        contentLink = containerConfiguration.ContainerContentLink.ToReferenceWithoutVersion().ID.ToString(),
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
        public ActionResult Save(IEnumerable<ConfigurationViewModel> configs)
        {
            if (configs == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var containerConfigurations = configs.Where(x => !x.fromCode).Select(x => new ContainerConfiguration
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

        //TODO: save and delete single configuration

        [HttpPost]
        public ActionResult ClearContainers(string contentLink)
        {
            return new RestResult
            {
                Data = "TEST " + contentLink,
                SafeResponse = true
            };
        }

        public class ConfigurationViewModel
        {
            public string contentLink { get; set; }
            public string containerTypeName { get; set; }
            public bool routingEnabled { get; set; }
            public bool fromCode { get; set; }
            public IEnumerable<string> groupLevelConfigurations { get; set; }
        }
    }
}