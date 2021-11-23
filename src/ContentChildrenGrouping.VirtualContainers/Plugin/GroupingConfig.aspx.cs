using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContentChildrenGrouping.VirtualContainers;
using ContentChildrenGrouping.VirtualContainers.ContainerNameGenerator;
using EPiServer.Framework.Modules;
using EPiServer.Framework.Serialization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Web;

namespace ContentChildrenGrouping.Plugin
{
    [GuiPlugIn(DisplayName = "Configure virtual containers",
        Description = "Manage virtual containers configurations",
        Area = EPiServer.PlugIn.PlugInArea.AdminMenu,
        UrlFromModuleFolder = "Plugins/GroupingConfig.aspx")]
    public partial class GroupingConfig : EPiServer.Shell.WebForms.WebFormsBase
    {
        protected Injected<IModuleResourceResolver> _moduleResolver { get; set; }
        protected Injected<IObjectSerializerFactory> _serializerFactory { get; set; }
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

            SystemMessageContainer.Heading = "Configure virtual containers";
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

                var defaultVirtualOptions = new VirtualContainersOptions();

                var config = new
                {
                    baseUrl = controllerUrl,
                    availableNameGenerators = _groupNameGenerators.Service.OfType<IDbAvailableGroupNameGenerator>().Select(x => x.Key),
                    options = new
                    {
                        _virtualContainerOptions.Service.CustomIconsEnabled,
                        _virtualContainerOptions.Service.SearchCommandEnabled,
                        _virtualContainerOptions.Service.DatabaseConfigurationsEnabled,
                        VirtualContainersEnabled = _virtualContainerOptions.Service.Enabled
                    },
                    defaultOptions = new
                    {
                        defaultVirtualOptions.CustomIconsEnabled,
                        defaultVirtualOptions.SearchCommandEnabled,
                        defaultVirtualOptions.DatabaseConfigurationsEnabled,
                        VirtualContainersEnabled = defaultVirtualOptions.Enabled
                    },
                    contentUrl
                };

                var objectSerializer = _serializerFactory.Service.GetSerializer(KnownContentTypes.Json);
                return objectSerializer.Serialize(config);
            }
        }
    }
}