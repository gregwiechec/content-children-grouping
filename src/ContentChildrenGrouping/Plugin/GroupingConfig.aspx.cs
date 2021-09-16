using System;
using System.Web;
using EPiServer.Framework.Modules;
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
        protected Injected<ContentChildrenGroupingOptions> _options { get; set; }

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

        protected bool IsDatabaseConfigurationsEnabled => _options.Service.DatabaseConfigurationsEnabled;

        protected string GetPath(string url)
        {
            _moduleResolver.Accessor()
                .TryResolveClientPath(typeof(GroupingConfig).Assembly, url, out var resolvedPath);
            return resolvedPath;
        }

        protected string ControllerUrl
        {
            get
            {
                _moduleResolver.Accessor().TryResolvePath(typeof(GroupingConfig).Assembly, "ConfigSettings/",
                    out var resolvedPath);
                return resolvedPath;
            }
        }
    }
}