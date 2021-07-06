using EPiServer.Shell;

namespace ContentChildrenGrouping.ContainerModel
{
    /// <summary>
    /// Describes how the UI should appear for <see cref="GroupingContainerPage"/> content.
    /// </summary>
    [UIDescriptorRegistration]
    public class ContainerUIDescriptor : UIDescriptor<GroupingContainerPage>
    {
        public ContainerUIDescriptor()
            : base(ContentTypeCssClassNames.Folder)
        {
            DefaultView = CmsViewNames.AllPropertiesView;
            DisabledViews = new [] { CmsViewNames.OnPageEditView };
        }
    }
}