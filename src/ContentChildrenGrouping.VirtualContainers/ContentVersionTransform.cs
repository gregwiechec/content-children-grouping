using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Cms.Shell.UI.Rest.Models;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace ContentChildrenGrouping.VirtualContainers
{
    //[ServiceConfiguration(typeof(IModelTransform))]
    public class ExtendedContentVersionFilter: ContentVersionFilter
    {
        public ExtendedContentVersionFilter(
            MissingContentLanguageInformationResolver contentLanguageInformationResolver,
            IContentRepository contentRepository, LanguageResolver languageResolver,
            ServiceAccessor<SiteDefinition> currentSiteDefinition) : base(contentLanguageInformationResolver,
            contentRepository, languageResolver, currentSiteDefinition)
        {
        }

        public override void TransformInstance(IContent source, StructureStoreContentDataModel target,
            IModelTransformContext context)
        {
            if (source.IsVirtualContainer() && target.MissingLanguageBranch != null)
            {
                target.MissingLanguageBranch.Reason = LanguageSelectionSource.None;
            }
        }

        public override TransformOrder Order => TransformOrder.OutputFilter;
    }
}