﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContentChildrenGrouping.Containers;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.ContainerNameGenerator;
using ContentChildrenGrouping.Extensions;
using ContentChildrenGrouping.PhysicalContainers;
using ContentChildrenGrouping.PhysicalContainers.ContainerModel;
using ContentChildrenGrouping.VirtualContainers;
using EPiServer.Framework.Modules;
using EPiServer.Framework.Serialization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Web;

namespace ContentChildrenGrouping.Plugin
{
    [GuiPlugIn(DisplayName = "Configure content groups",
        Description = "Configure content groups",
        Area = EPiServer.PlugIn.PlugInArea.AdminMenu,
        UrlFromModuleFolder = "Plugins/GroupingConfig.aspx")]
    public partial class GroupingConfig : EPiServer.Shell.WebForms.WebFormsBase
    {
        protected Injected<IModuleResourceResolver> _moduleResolver { get; set; }
        protected Injected<IObjectSerializerFactory> _serializerFactory { get; set; }
        private readonly Injected<ContentChildrenGroupingOptions> _childrenGroupingOptions;
        private readonly Injected<VirtualContainersOptions> _virtualContainerOptions;
        private readonly Injected<IEnumerable<IGroupNameGenerator>> _groupNameGenerators;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                Locate.Advanced.GetInstance<IAccessDeniedHandler>()
                    .AccessDenied(new HttpContextWrapper(HttpContext.Current));
            }

            SystemMessageContainer.Heading = "Configure content groups";
            SystemMessageContainer.HelpFile = "home";
        }

        protected string GetPath(string url)
        {
            _moduleResolver.Service.TryResolveClientPath(typeof(GroupingConfig).Assembly, url, out var resolvedPath);
            return resolvedPath;
        }

        protected string Configuration
        {
            get
            {
                var contentUrl = Paths.ToResource("CMS", "Home#context=epi.cms.contentdata:///{contentLink}");

                _moduleResolver.Service.TryResolvePath(typeof(GroupingConfig).Assembly, "ConfigSettings/",
                    out var controllerUrl);

                var defaultGroupingOptions = new ContentChildrenGroupingOptions();
                var defaultVirtualOptions = new VirtualContainersOptions();

                var groupingOptions = _childrenGroupingOptions.Service;
                var config = new
                {
                    baseUrl = controllerUrl,
                    availableNameGenerators = _groupNameGenerators.Service.OfType<IDbAvailableGroupNameGenerator>().Select(x => x.Key),
                    options = new
                    {
                        PhysicalContainersEnabled = groupingOptions.Enabled,
                        groupingOptions.StructureUpdateEnabled,
                        groupingOptions.CustomIconsEnabled,
                        groupingOptions.SearchCommandEnabled,
                        groupingOptions.DatabaseConfigurationsEnabled,
                        groupingOptions.RouterEnabled,
                        VirtualContainersEnabled = _virtualContainerOptions.Service.Enabled
                    },
                    defaultOptions = new
                    {
                        PhysicalContainersEnabled = defaultGroupingOptions.Enabled,
                        defaultGroupingOptions.Enabled,
                        defaultGroupingOptions.StructureUpdateEnabled,
                        defaultGroupingOptions.CustomIconsEnabled,
                        defaultGroupingOptions.SearchCommandEnabled,
                        defaultGroupingOptions.DatabaseConfigurationsEnabled,
                        defaultGroupingOptions.RouterEnabled,
                        VirtualContainersEnabled = defaultVirtualOptions.Enabled
                    },
                    contentUrl,
                    defaultContainerType = typeof(GroupingContainerPage).TypeToString()
                };

                var objectSerializer = _serializerFactory.Service.GetSerializer(KnownContentTypes.Json);
                return objectSerializer.Serialize(config);
            }
        }
    }
}