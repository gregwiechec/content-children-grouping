using System.Web.Routing;
using ContentChildrenGrouping.Core;
using EPiServer;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace ContentChildrenGrouping.PartialRouter
{
    /// <summary>
    /// Register CustomPartialRouter
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class PartialRouterInitialization : IInitializableModule
    {
        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
            var options = context.Locate.Advanced.GetInstance<ContentChildrenGroupingOptions>();

            if (options.RouterEnabled)
            {
                var partialRouter = new CustomPartialRouter(context.Locate.ContentLoader(),
                    context.Locate.Advanced.GetAllInstances<IContentChildrenGroupsLoader>());
                RouteTable.Routes.RegisterPartialRouter(partialRouter);
            }
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
        }
    }
}