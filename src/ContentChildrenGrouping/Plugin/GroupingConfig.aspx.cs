using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using ContentChildrenGrouping.ContainerModel;
using ContentChildrenGrouping.Extensions;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Framework.Modules;
using EPiServer.Framework.Serialization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

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
            SystemMessageContainer.HelpFile = "home";
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
                var startPageUrl = ServiceLocator.Current.GetInstance<IContentLoader>()
                    .Get<PageData>(ContentReference.StartPage).EditablePreviewUrl(ServiceLocator.Current.GetInstance<UrlResolver>(), 
                        ServiceLocator.Current.GetInstance<TemplateResolver>());


                var config = new
                {
                    baseUrl = ControllerUrl,
                    availableNameGenerators = _groupNameGenerators.Service.Where(x => x is IDbAvailableGroupNameGenerator).Select(x => x.Key),
                    structureUpdateEnabled = _childrenGroupingOptions.Service.StructureUpdateEnabled,
                    databaseConfigurationsEnabled = _childrenGroupingOptions.Service.DatabaseConfigurationsEnabled,
                    startPageUrl = startPageUrl,
                    defaultContainerType = typeof(GroupingContainerPage).TypeToString()
                };

                var objectSerializer = _serializerFactory.Service.GetSerializer(KnownContentTypes.Json);
                return objectSerializer.Serialize(config);
            }
        }
    }
}