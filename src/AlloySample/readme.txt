To setup virtual containers for page from code you need to use IContentChildrenGroupsRegistration
For example:

namespace AlloySample.Business.Initialization
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentGroupingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
			PageReference containerContentLink = ... // find parent page
            context.Locate.Advanced.GetInstance<IContentChildrenGroupsRegistration>().RegisterByLetter(containerContentLink);
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}

Virtual containers can be also configured using admin mode plugin.

More configuration options can be found in the documentation: https://github.com/gregwiechec/content-children-grouping