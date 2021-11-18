using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Shell;

namespace ContentChildrenGrouping.VirtualContainers
{
    /// <summary>
    /// Type of virtual Container
    /// </summary>
    [ContentType(
        GUID = "22F84DED-6005-4040-9C86-BA2A4377A19E",
        AvailableInEditMode = false)]
    public class VirtualContainerPage : PageData
    {
    }

    /// <summary>
    /// UI descriptor for VirtualContainerPage with custom Page type settings
    /// </summary>
    [UIDescriptorRegistration]
    public class VirtualContainerPageUIDescriptor : UIDescriptor<VirtualContainerPage>
    {
        public VirtualContainerPageUIDescriptor()
            : base(ContentTypeCssClassNames.Container)
        {
            DefaultView = CmsViewNames.AllPropertiesView;
            DisabledViews = new[] { CmsViewNames.OnPageEditView };
        }
    }

    /*
      internal class VirtualContainersContentProvider : ContentProvider
    {
        private readonly IContentRepository _contentRepository;
        private string _providerKey;
        private ContentReference _wasteBasketReference;

        public VirtualContainersContentProvider()
        {
            _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        }

        public VirtualContainersContentProvider(string providerKey)
        {
            this._providerKey = providerKey;
        }

        public VirtualContainersContentProvider(string providerKey, ContentReference wasteBasketReference)
        {
            this._providerKey = providerKey;
            this._wasteBasketReference = wasteBasketReference;
        }

        public override string ProviderKey
        {
            get { return "VirtualContainers"; }
        }

        public override NameValueCollection Parameters
        {
            get { return new NameValueCollection(); }
        }

        public override ContentReference Save(IContent content, SaveAction action)
        {
            throw new Exception("Cannot save item");
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var virtualContainerPage =
                _contentRepository.GetDefault<VirtualContainerPage>(new ContentReference(contentLink.ID));
            virtualContainerPage.Name = contentLink.WorkID == 0 ? "UNKNOWN" : ((char) contentLink.WorkID).ToString();
            virtualContainerPage.ContentLink = contentLink;
            return virtualContainerPage;
        }
    }
     */
}