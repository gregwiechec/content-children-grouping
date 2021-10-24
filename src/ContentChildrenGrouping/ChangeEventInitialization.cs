using ContentChildrenGrouping.Core;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping.Containers
{
    /// <summary>
    /// Registers custom save event for updating content structure
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ChangeEventInitialization : IInitializableModule
    {
        private ILogger _log = LogManager.GetLogger(typeof(ChangeEventInitialization));

        public void Initialize(InitializationEngine context)
        {
            var structureUpdateEnabled = ServiceLocator.Current.GetInstance<ContentChildrenGroupingOptions>()
                .StructureUpdateEnabled;
            if (!structureUpdateEnabled)
            {
                return;
            }

            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.SavingContent += ContentEvents_SavingContent;
            _log.Information("Structure SavingContent registered");
        }

        public void Uninitialize(InitializationEngine context)
        {
            var structureUpdateEnabled = ServiceLocator.Current.GetInstance<ContentChildrenGroupingOptions>()
                .StructureUpdateEnabled;
            if (!structureUpdateEnabled)
            {
                return;
            }

            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.SavingContent -= ContentEvents_SavingContent;
            _log.Information("Structure SavingContent unregistered");
        }

        private static void ContentEvents_SavingContent(object sender, EPiServer.ContentEventArgs e)
        {
            ServiceLocator.Current.GetInstance<IContentStructureModifier>().UpdateContentParent(e.Content);
        }
    }
}