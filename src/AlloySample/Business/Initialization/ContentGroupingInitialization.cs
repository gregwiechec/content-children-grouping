using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.RegisterFromCode;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AlloySample.Business.Initialization
{
    /// <summary>
    /// Change configuration for Content Containers
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentGroupingInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
           context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().RegisterByLetter(113);
           context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().RegisterVirtualContainerByLetter(220);
        }

        public void Uninitialize(InitializationEngine context)
        {
           
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddTransient(serviceLocator => new ContentChildrenGroupingOptions
            {
                RouterEnabled = true,
                DatabaseConfigurationsEnabled = true,
                StructureUpdateEnabled = true,
                SearchCommandEnabled = true,
                CustomIconsEnabled = true
            });
            context.Services.AddTransient(serviceLocator => new VirtualContainersOptions
            {
                Enabled = true
            });
        }
    }
}