using AlloySample.Business.ContentGrouping;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Core.RegisterFromCode;
using ContentChildrenGrouping.VirtualContainers;
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

            //            context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().RegisterByLetter(2222233);
            //            context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().RegisterVirtualContainerByLetter(23456789);

            /*
                        context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().Register(
                            new ContainerConfiguration
                            {
                                ContainerContentLink = new ContentReference(123456789),
                                GroupLevelConfigurations = new IGroupNameGenerator[]
                                {
                                    new ByNameGroupNameGenerator(0, 1),
                                    new ByCreateDateGroupNameGenerator("yyyy"),
                                }
                            });
            */
            /*context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().Register(
                new ContainerConfiguration
                {
                    ContainerContentLink = new ContentReference(2823),
                    GroupLevelConfigurations = new []
                    {
                        new RangeGenerator(),
                    }
                });*/
        }

        public void Uninitialize(InitializationEngine context)
        {
           
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddTransient(serviceLocator => new VirtualContainersOptions
            {
                Enabled = true,
                SearchCommandEnabled = true,
                CustomIconsEnabled = true,
                DatabaseConfigurationsEnabled = true
            });
        }
    }
}