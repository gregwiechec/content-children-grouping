using System;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.Models;
using EPiServer.Cms.Shell.UI.Rest.Models.Internal;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Notification;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace ContentChildrenGrouping.VirtualContainers
{
    public class CustomContentDataModelBaseTransform : ContentDataModelBaseTransform
    {
        public CustomContentDataModelBaseTransform(IInUseNotificationRepository inUseNotificationRepository,
            IContentTypeRepository contentTypeRepository, IContentRepository contentRepository,
            ContentCapabilitiesResolver capabilitiesResolver, IPermanentLinkMapper permanentLinkMapper,
            ILanguageBranchRepository languageBranchRepository, IContentProviderManager contentProviderManager,
            UIDescriptorRegistry uiDescriptorRegistry, TemplateResolver templateResolver, UrlResolver urlResolver,
            ISiteDefinitionRepository siteDefinitionRepository, LanguageResolver languageResolver,
            ServiceAccessor<SiteDefinition> currentSiteDefinition) : base(inUseNotificationRepository,
            contentTypeRepository, contentRepository, capabilitiesResolver, permanentLinkMapper,
            languageBranchRepository, contentProviderManager, uiDescriptorRegistry, templateResolver, urlResolver,
            siteDefinitionRepository, languageResolver, currentSiteDefinition)
        {
        }

        public CustomContentDataModelBaseTransform() : base(
            ServiceLocator.Current.GetInstance<IInUseNotificationRepository>(),
            ServiceLocator.Current.GetInstance<IContentTypeRepository>(),
            ServiceLocator.Current.GetInstance<IContentRepository>(),
            ServiceLocator.Current.GetInstance<ContentCapabilitiesResolver>(),
            ServiceLocator.Current.GetInstance<IPermanentLinkMapper>(),
            ServiceLocator.Current.GetInstance<ILanguageBranchRepository>(),
            ServiceLocator.Current.GetInstance<IContentProviderManager>(),
            ServiceLocator.Current.GetInstance<UIDescriptorRegistry>(),
            ServiceLocator.Current.GetInstance<TemplateResolver>(), ServiceLocator.Current.GetInstance<UrlResolver>(),
            ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>(),
            ServiceLocator.Current.GetInstance<LanguageResolver>(),
            () => ServiceLocator.Current.GetInstance<SiteDefinition>())
        {
        }

        public override void TransformInstance(IContent source, ContentDataStoreModelBase target, IModelTransformContext context)
        {
            if (source.ContentLink.ProviderName?.StartsWith("VirtualContainers") == true)
            {
                target.Name = source.Name;
                target.ContentLink = source.ContentLink;
                target.ParentLink = source.ParentLink;
                target.ContentGuid = source.ContentGuid;
                target.InUseNotifications = Enumerable.Empty<InUseNotificationViewModel>();

                target.IsDeleted = false;
                target.Uri = source.GetUri();

                target.HasTemplate = false;
                target.TypeIdentifier = GetTypeIdentifier(source);
                return;
            }

            base.TransformInstance(source, target, context);
        }

        private string GetTypeIdentifier(IContent content)
        {
            return content == null ? String.Empty : content.GetOriginalType().FullName.ToLowerInvariant();
        }
    }
}