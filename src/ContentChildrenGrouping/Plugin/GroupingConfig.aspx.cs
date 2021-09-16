using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Framework.Modules;
using EPiServer.Framework.Serialization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
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
        }

        protected string GetPath(string url)
        {
            _moduleResolver.Service.TryResolveClientPath(typeof(GroupingConfig).Assembly, url, out var resolvedPath);
            return resolvedPath;
        }

        private string ControllerUrl
        {
            get
            {
                _moduleResolver.Service.TryResolvePath(typeof(GroupingConfig).Assembly, "ConfigSettings/",
                    out var resolvedPath);
                return resolvedPath;
            }
        }

        protected string Configuration
        {
            get
            {
                var config = new
                {
                    baseUrl = ControllerUrl,
                    availableNameGenerators = _groupNameGenerators.Service.Where(x => x is IDbAvailableGroupNameGenerator).Select(x => x.Key),
                    structureUpdateEnabled = _childrenGroupingOptions.Service.StructureUpdateEnabled,
                    databaseConfigurationsEnabled = _childrenGroupingOptions.Service.DatabaseConfigurationsEnabled
                };

                var objectSerializer = _serializerFactory.Service.GetSerializer(KnownContentTypes.Json);
                return objectSerializer.Serialize(config);
            }
        }
    }
}