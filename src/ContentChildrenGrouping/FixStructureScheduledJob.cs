using System.Collections.Generic;
using System.Linq;
using ContentChildrenGrouping.Core;
using ContentChildrenGrouping.Extensions;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Logging.Compatibility;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace ContentChildrenGrouping
{
    [ScheduledPlugIn(DefaultEnabled = false,
        DisplayName = "Fix content groups hierarchy",
        Description = "Job will move content to containers based on configuration",
        GUID = "D2963BAF-778D-4E3A-8C29-516BEF812260",
        IntervalLength = 12,
        IntervalType = ScheduledIntervalType.Hours,
        Restartable = true)]
    [ServiceConfiguration]
    public class FixStructureScheduledJob : ScheduledJobBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FixStructureScheduledJob));
        private readonly IContentRepository _contentRepository;
        private readonly IEnumerable<IContentChildrenGroupsLoader> _contentChildrenGroupsLoaders;
        private readonly IContentStructureModifier _contentStructureModifier;
        private bool _isStopped;

        public FixStructureScheduledJob() : this(ServiceLocator.Current.GetInstance<IContentRepository>(),
            ServiceLocator.Current.GetAllInstances<IContentChildrenGroupsLoader>(),
            ServiceLocator.Current.GetInstance<IContentStructureModifier>())
        {
        }

        public FixStructureScheduledJob(IContentRepository contentRepository,
            IEnumerable<IContentChildrenGroupsLoader> contentChildrenGroupsLoaders,
            IContentStructureModifier contentStructureModifier)
        {
            _contentRepository = contentRepository;
            _contentChildrenGroupsLoaders = contentChildrenGroupsLoaders;
            _contentStructureModifier = contentStructureModifier;
            IsStoppable = true;
        }

        public override void Stop()
        {
            base.Stop();
            this._isStopped = true;
        }

        public override string Execute()
        {
            _isStopped = false;

            var totalUpdated = 0;
            var totalParsed = 0;

            void Notify(ContainerConfiguration configuration)
            {
                OnStatusChanged(
                    $"Analyzed {totalParsed} contents and updated {totalUpdated}. Currently updating contents for: {configuration.ContainerContentLink}.");
            }

            void FixStructure(ContainerConfiguration configuration, IContent parentContent)
            {
                var children = _contentRepository.GetChildren<IContent>(parentContent.ContentLink).ToList();

                foreach (var content in children)
                {
                    totalParsed++;
                    if (!configuration.GetContainerType().IsInstanceOfType(content))
                    {
                        if (_isStopped)
                        {
                            return;
                        }

                        var currentContentParent = content.ParentLink;
                        var correctParentNames = configuration.GroupLevelConfigurations
                            .Select(x => x.GetName(content))
                            .ToList();

                        var structureIsValid = true;
                        foreach (var parentName in correctParentNames.AsEnumerable().Reverse())
                        {
                            var parent = _contentRepository.Get<IContent>(currentContentParent);
                            if (!parent.Name.CompareStrings(parentName))
                            {
                                structureIsValid = false;
                                break;
                            }

                            currentContentParent = parent.ContentLink;
                        }

                        if (!structureIsValid)
                        {
                            var currentContainerContentLink = configuration.ContainerContentLink;
                            foreach (var parentName in correctParentNames)
                            {
                                //TODO: groups create a hashmap for [config,groupConfig,name] = parentLink
                                var parent = _contentRepository.GetChildren<IContent>(currentContainerContentLink)
                                    .FirstOrDefault(x => x.Name.CompareStrings(parentName));
                                if (parent == null)
                                {
                                    currentContainerContentLink = _contentStructureModifier.CreateParent(configuration,
                                        parentName, currentContainerContentLink, content);
                                }
                                else
                                {
                                    currentContainerContentLink = parent.ContentLink;
                                }
                            }

                            _contentRepository.Move(content.ContentLink, currentContainerContentLink,
                                AccessLevel.NoAccess, AccessLevel.NoAccess);
                        }
                    }

                    if (totalParsed % 100 == 0)
                    {
                        Notify(configuration);
                    }

                    FixStructure(configuration, content);
                }
            }

            var containerConfigurations = _contentChildrenGroupsLoaders.GetAllContainersConfigurations().ToList();
            foreach (var configuration in containerConfigurations)
            {
                Notify(configuration);
                if (_isStopped)
                {
                    return "Job stopped";
                }

                var result = _contentRepository.TryGet<IContent>(configuration.ContainerContentLink, out var content);
                if (result)
                {
                    FixStructure(configuration, content);
                }
            }

            return $"Job completed successfully. {totalUpdated} contents updated";
        }
    }
}