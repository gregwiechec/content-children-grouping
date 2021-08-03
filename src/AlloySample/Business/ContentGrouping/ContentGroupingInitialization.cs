using ContentChildrenGrouping.RegisterFromCode;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace AlloySample.Business.ContentGrouping
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentGroupingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
           context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().RegisterByLetter(new ContentReference(113));
        }

        public void Uninitialize(InitializationEngine context)
        {
           
        }
    }
}

//TODO: [grouping] cleanup plugin - remove containers from current structure