using ContentChildrenGrouping;
using ContentChildrenGrouping.RegisterFromCode;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AlloySample.Business.Initialization
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentGroupingInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
           context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().RegisterByLetter(new ContentReference(113));
        }

        public void Uninitialize(InitializationEngine context)
        {
           
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddTransient(serviceLocator => new ContentChildrenGroupingOptions
            {
                RouterEnabled = false,
                DatabaseConfigurationsEnabled = true,
                StructureUpdateEnabled = true
            });
        }
    }
}

//TODO: [grouping] cleanup plugin - remove containers from current structure