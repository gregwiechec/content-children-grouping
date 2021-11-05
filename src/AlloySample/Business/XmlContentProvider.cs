using EPiServer;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.DataAccess.Internal;
using EPiServer.Framework.Cache;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using ArgumentException = System.ArgumentException;

namespace CodeSamples
{
    /// <summary>
    /// The Content provider that handles contents stored in a Xml file.
    /// In this version we only support PageData. 
    /// </summary>
    public class XmlContentProvider : ContentProvider, IContentVersionRepository
    {
        private static string _xmlKey = "Custom:XMLContents";

        private readonly IContentFactory _contentFactory;
        private readonly ServiceAccessor<IContentTypeRepository> _contentTypeRepository;
        private readonly ILanguageBranchRepository _languageBranchRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IPrincipalAccessor _principalAccessor;
        private readonly ISynchronizedObjectInstanceCache _cacheInstance;
        private readonly IStatusTransitionEvaluator _statusTransitionEvaluator;
        private readonly PropertyValueConverterFactory _propertyValueConverterFactory;
        private string _filePath;

        #region Ctors

        /// <summary>
        /// Initialize an instance of XmlContentProvider
        /// </summary>
        public XmlContentProvider()
            : this(
                ServiceLocator.Current.GetInstance<IContentFactory>(),
                ServiceLocator.Current.GetInstance<ServiceAccessor<IContentTypeRepository>>(),
                ServiceLocator.Current.GetInstance<ILanguageBranchRepository>(),
                ServiceLocator.Current.GetInstance<IContentRepository>(),
                ServiceLocator.Current.GetInstance<ISynchronizedObjectInstanceCache>(),
                ServiceLocator.Current.GetInstance<IPrincipalAccessor>(),
                ServiceLocator.Current.GetInstance<IStatusTransitionEvaluator>(),
                ServiceLocator.Current.GetInstance<PropertyValueConverterFactory>())
        { }

        /// <summary>
        /// Initialize an instance of XmlContentProvider
        /// </summary>
        public XmlContentProvider(
            IContentFactory contentFactory,
            ServiceAccessor<IContentTypeRepository> contentTypeRepository,
            ILanguageBranchRepository languageBranchRepository,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache cacheInstance,
            IPrincipalAccessor principalAccessor,
            IStatusTransitionEvaluator statusTransitionEvaluator,
            PropertyValueConverterFactory propertyValueConverterFactory)
        {
            _contentFactory = contentFactory;
            _contentTypeRepository = contentTypeRepository;
            _languageBranchRepository = languageBranchRepository;
            _contentRepository = contentRepository;
            _cacheInstance = cacheInstance;
            _principalAccessor = principalAccessor;
            _statusTransitionEvaluator = statusTransitionEvaluator;
            _propertyValueConverterFactory = propertyValueConverterFactory;
        }

        #endregion

        #region Initialization Method

        /// <summary>
        /// Initializes the provider from configuration settings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="configParams">The config params.</param>
        public override void Initialize(string key, NameValueCollection configParams)
        {
            //Let base classes do their initialization
            base.Initialize(key, configParams);

            //Make sure a path to the backing xml file is given in configruation of page provider
            if (configParams["filePath"] == null)
            {
                throw new ArgumentException("XmlContentProvider requires configuration attribute filePath that should be a application relative path");
            }
        }

        #endregion

        #region Override Methods

        public override ContentReference Save(IContent content, SaveAction action)
        {
            var contents = GetContentVersions(content.ContentLink);
            var transition = _statusTransitionEvaluator.Evaluate(content, action);

            int versionId = GetVersionId(content, contents, transition);

            if (transition.NextStatus == VersionStatus.Published)
            {
                // We do nothing if the page has no changes and is already published
                IVersionable versionable = content as IVersionable;
                IModifiedTrackable modifiedTrackable = content as IModifiedTrackable;
                if ((modifiedTrackable != null && modifiedTrackable.IsModified) ||
                    (versionable != null && versionable.Status != VersionStatus.Published))
                {
                    SetContentStatus(content, transition.NextStatus);
                    Publish(content, versionId, contents);
                    if (!action.HasFlag(SaveAction.SkipSetCommonDraft))
                    {
                        HandleCommonDraft(content);
                    }
                    XmlContents.Save(FilePath);
                }
            }
            else
            {
                SetContentStatus(content, transition.NextStatus);
                Save(content, versionId, contents);
                if (!action.HasFlag(SaveAction.SkipSetCommonDraft))
                {
                    HandleCommonDraft(content);
                }
                XmlContents.Save(FilePath);
            }

            return content.ContentLink;
        }

        public override void Delete(ContentReference contentLink, bool forceDelete)
        {
            //First we delete children for the content
            DeleteChildren(contentLink, forceDelete);

            //Then we delete the page itself (or "contents" since it migth exist in multiple languages)
            var contentNodes = GetContentVersions(contentLink);
            if (contentNodes == null)
            {
                throw new ContentNotFoundException(contentLink);
            }

            //Set the nodes in a list that we can enumerate over and delete (we cant remove using an IEnumerable)
            List<XElement> toBeDeleted = new List<XElement>();
            foreach (XElement contentNode in contentNodes)
            {
                toBeDeleted.Add(contentNode);
            }
            for (int i = 0; i < toBeDeleted.Count; i++)
            {
                toBeDeleted[i].Remove();
            }
            XmlContents.Save(FilePath);
        }

        public override void DeleteChildren(ContentReference contentLink, bool forceDelete)
        {
            IList<GetChildrenReferenceResult> childrenReferenceResult = GetChildrenReferences<IContent>(contentLink, null, -1, -1);
            if (childrenReferenceResult.Count == 0)
            {
                // Nothing to delete
                return;
            }

            foreach (var childReferenceResult in childrenReferenceResult)
            {
                Delete(childReferenceResult.ContentLink, forceDelete);
            }
            XmlContents.Save(FilePath);
        }

        public override void Move(ContentReference contentLink, ContentReference destinationLink)
        {
            //Find all page languages
            var contentNodes = GetContentVersions(contentLink);
            if (contentNodes == null)
            {
                throw new ContentNotFoundException(contentLink);
            }

            string parentId = (from parent in contentNodes.First().Attributes("parent")
                               select parent.Value).Single<string>();

            //When pages are moved to archive StopPublish is reseted
            var content = LoadContent(GetContentReference(contentNodes.First()), LanguageSelector.AutoDetect(true));
            var isMovedToArchive = IsMovedToArchive(content, destinationLink);

            bool recursively = true;
            //Change parent information for the pages
            foreach (XElement contentNode in contentNodes)
            {
                contentNode.Attribute("parent").Value = destinationLink == EntryPoint ? "0" : destinationLink.ID.ToString();
                XElement parentPropertyNode = contentNode.Elements().Where(p => String.Equals(p.Attribute("name").Value, "PageParentLink")).SingleOrDefault<XElement>();
                if (parentPropertyNode != null)
                {
                    //we don't store ProviderKey information in storage, GetProperty and InitializeData will take care of setting that at load. XElement propNode = pageNode.Elements().Where(p => String.Equals(p.Attribute("name").Value, "PageDeleted")).SingleOrDefault<XElement>();
                    parentPropertyNode.SetValue(destinationLink.ID.ToString());
                }

                if (isMovedToArchive)
                {
                    XElement stopPublishNode = contentNode.Elements().Where(p => String.Equals(p.Attribute("name").Value, MetaDataProperties.PageStopPublish)).SingleOrDefault<XElement>();
                    if (stopPublishNode != null)
                    {
                        stopPublishNode.Remove();
                    }
                }

                //mark page as deleted if moved to wastebasket
                if (destinationLink == WastebasketReference)
                {
                    SetDeleteFlag(contentNode, true, recursively);
                    recursively = false;
                }
                else
                {
                    //or remove deleted flag if page is restored
                    SetDeleteFlag(contentNode, false, recursively);
                    recursively = false;
                }
            }

            XmlContents.Save(FilePath);
        }

        public override void MoveToWastebasket(ContentReference contentLink, string deletedBy)
        {
            Move(contentLink, WastebasketReference);
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            //Get the Content node from the XMl file (and also a list of all languages the Content exist on)
            List<CultureInfo> contentLanguages = new List<CultureInfo>();
            XElement contentNode = GetContentNode(contentLink, languageSelector, out contentLanguages);
            if (contentNode == null)
            {
                return null;
            }
            ContentReference parentLink = GetParentLink(contentNode);
            ContentType contentType = GetContentType(contentNode);
            ContentReference writableContentLink = GetContentLink(contentLink, contentNode);
            IContent content = CreateContent(contentType, parentLink, writableContentLink);
            PopulateProperties(contentNode, content);
            HandleSecurity(content, contentNode, parentLink);
            HandleLanguage(content, contentNode, contentLanguages);
            HandleLinkUrl(content, contentType.ID);
            HandleReadOnly(content);
            return content;
        }

        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageID, out bool languageSpecific)
        {
            //
            //children to entrypoint is stored with parent=0 in xml file (to avoid hard coupling to entrypoint)
            //
            int contentId = contentLink != EntryPoint ? contentLink.ID : 0;
            languageSpecific = false;
            var children = base.LoadChildrenReferencesAndTypes(contentLink, languageID, out languageSpecific);
            var query = from content in XmlContents.Descendants("content")
                        where String.Equals(content.Attribute("parent").Value, contentId.ToString())
                        select content;



            IEnumerator<XElement> enumerator = query.GetEnumerator();
            while (enumerator.MoveNext())
            {
                XElement content = enumerator.Current;
                int childId = Int32.Parse(content.Attribute("id").Value);
                var typeProperty = content.Elements("property").Attributes("name").Where(e => e.Value == "PageTypeName").FirstOrDefault();
                Type modelType = null;
                if (typeProperty != null && !String.IsNullOrEmpty(typeProperty.Parent.Value))
                {
                    modelType = _contentTypeRepository().Load(typeProperty.Parent.Value).ModelType;
                }
                //We make sure we only return unique ContentReferences
                ContentReference contentRef = ConstructLocalContentReference(childId);
                if (!children.Any(chr => chr.ContentLink == contentRef))
                {
                    GetChildrenReferenceResult childrenReferenceResult = new GetChildrenReferenceResult() { ContentLink = contentRef, ModelType = modelType };
                    //we dont want to add a reference for all languages
                    children.Add(childrenReferenceResult);
                }
            }
            return children;

        }

        #region LoadVersion

        /// <inheritdoc />
        public override IContentVersionRepository VersionRepository
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Delete a single version of a Content
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        public void Delete(ContentReference contentLink)
        {
            if (contentLink.WorkID == 0)
            {
                throw new NullReferenceException("Version ID of Content to delete is required");
            }

            XElement content = GetContentNode(contentLink);
            if (content != null)
            {
                content.Remove();
                XmlContents.Save(FilePath);
            }
        }

        /// <summary>
        /// Lists the published versions for a content
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns>
        /// All published versions for the content
        /// </returns>
        public IEnumerable<ContentVersion> ListPublished(ContentReference contentLink)
        {
            List<ContentVersion> versions = new List<ContentVersion>();
            foreach (var pv in List(contentLink))
            {
                if (pv.Status == VersionStatus.Published)
                    versions.Add(pv);

            }
            return versions;
        }

        /// <summary>
        /// Lists all versions
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns>
        /// All matching versions
        /// </returns>
        public IEnumerable<ContentVersion> List(ContentReference contentLink)
        {
            List<ContentVersion> collection = new List<ContentVersion>();
            ContentVersion contentVersion;
            var contents = from content in XmlContents.Elements("contents").Elements<XElement>("content")
                           where int.Parse(content.Attribute("id").Value) == contentLink.ID
                           select content;
            foreach (XElement content in contents)
            {
                contentVersion = new ContentVersion(
                                                ConstructLocalContentReference(contentLink.ID, int.Parse(content.Attribute("versionid").Value)),
                                                GetPropertyValue(MetaDataProperties.PageName, content),
                                                (VersionStatus)int.Parse(GetPropertyValue(MetaDataProperties.PageWorkStatus, content)),
                                                GetDate(GetPropertyValue(MetaDataProperties.PageChanged, content) == null ? GetPropertyValue(MetaDataProperties.PageCreated, content) :
                                                GetPropertyValue(MetaDataProperties.PageChanged, content)),
                                                "",
                                                "",
                                                0,
                                                GetPropertyValue(MetaDataProperties.PageLanguageBranch, content),
                                                String.Equals(GetPropertyValue(MetaDataProperties.PageLanguageBranch, content),
                                                GetPropertyValue(MetaDataProperties.PageMasterLanguageBranch, content)), content.Attribute("isCommonDraft").Value == true.ToString());
                collection.Add(contentVersion);
            }
            return collection;
        }

        /// <summary>
        /// Lists all versions for a page current language selection if the languageBranch is null otherwise
        /// lists all versions for a page for a specific language
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <param name="languageBranch">The language branch</param>
        /// <returns>
        /// All matching versions
        /// </returns>
        public IEnumerable<ContentVersion> List(ContentReference contentLink, string languageBranch)
        {
            bool loadMaster = String.IsNullOrEmpty(languageBranch);
            List<ContentVersion> versions = new List<ContentVersion>();
            foreach (ContentVersion pv in List(contentLink))
            {
                if ((loadMaster && pv.IsMasterLanguageBranch) ||
                    String.Equals(pv.LanguageBranch, languageBranch, StringComparison.OrdinalIgnoreCase))
                {
                    versions.Add(pv);
                }
            }
            return versions;
        }

        /// <inheritdoc/>
        public IEnumerable<ContentVersion> List(VersionFilter filter, int startIndex, int maxRows, out int totalCount)
        {
            totalCount = 0;
            if (!ContentReference.IsNullOrEmpty(filter.ContentLink))
            {
                var versions = filter.Languages != null && filter.Languages.Any() ?
                    filter.Languages.SelectMany(c => List(filter.ContentLink, c.Name)) :
                    List(filter.ContentLink);

                totalCount = versions.Count();
                return versions;
            }

            return Enumerable.Empty<ContentVersion>();
        }

        /// <summary>
        ///  Lists all content references that are set to delayed publish.
        /// </summary> 
        /// <returns>
        /// All matching content Versions
        /// </returns>
        public new IEnumerable<ContentReference> ListDelayedPublish()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the published version a page with current language selection if the languageBranch is null otherwise
        /// loads the published version a page for a specific language
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <param name="languageBranch">The language branch.</param>
        /// <returns>
        /// A <see cref="EPiServer.DataAbstraction.PageVersion"/>
        /// </returns>
        public ContentVersion LoadPublished(ContentReference contentLink, string languageBranch)
        {
            IEnumerable<XElement> contents = GetContentVersions(contentLink);
            List<XElement> languageContents = new List<XElement>();
            if (contents != null)
            {
                // ... if there is any filter it by the language...
                foreach (XElement content in contents)
                {
                    var lContent = (from languagePage in content.Elements("property").Attributes("name")
                                    where String.Equals(languagePage.Value, MetaDataProperties.PageLanguageBranch)
                                    && String.Equals(languagePage.Parent.Value, languageBranch)
                                    select languagePage.Parent.Parent).SingleOrDefault<XElement>();
                    if (lContent != null)
                        languageContents.Add(lContent);
                }
                // ... pick the one that is published...
                XElement theContent = null;
                foreach (XElement content in languageContents)
                {
                    theContent = (from pageWorkStatus in content.Elements("property").Attributes("name")
                                  where String.Equals(pageWorkStatus.Value, "PageWorkStatus") && String.Equals(pageWorkStatus.Parent.Value, "4")
                                  select pageWorkStatus.Parent.Parent).SingleOrDefault<XElement>();

                    if (theContent != null)
                        break;
                }
                // ...and finally instantiate your PageVersion
                if (theContent != null)
                {
                    ContentVersion cv = new ContentVersion(contentLink,
                                               GetPropertyValue("PageName", theContent),
                                              (VersionStatus)int.Parse(GetPropertyValue("PageWorkStatus", theContent)),
                                              GetDate(GetPropertyValue("PageChanged", theContent) == null ? GetPropertyValue("PageCreated", theContent) : GetPropertyValue("PageChanged", theContent)),
                                              "",
                                              "",
                                              0, null, true, theContent.Attribute("isCommonDraft").Value == true.ToString());
                    return cv;
                }
            }
            return null;
        }

        /// <summary>
        /// Loads the published version
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns>
        /// A <see cref="EPiServer.DataAbstraction.PageVersion"/>
        /// </returns>
        public ContentVersion LoadPublished(ContentReference contentLink)
        {
            return LoadPublished(contentLink, null);
        }
        /// <summary>
        /// Loads the version.
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns>
        /// A <see cref="EPiServer.DataAbstraction.PageVersion"/>
        /// </returns>
        public ContentVersion Load(ContentReference contentLink)
        {
            XElement content = GetContentNode(contentLink);
            if (content != null)
            {
                ContentVersion pv = new ContentVersion(contentLink,
                                                   GetPropertyValue("PageName", content),
                                                  (VersionStatus)int.Parse(GetPropertyValue("PageWorkStatus", content)),
                                                  GetDate(GetPropertyValue("PageChanged", content) == null ? GetPropertyValue("PageCreated", content) : GetPropertyValue("PageChanged", content)),
                                                  "",
                                                  "",
                                                  0, null, true, content.Attribute("isCommonDraft").Value == true.ToString());
                pv.LanguageBranch = GetPropertyValue(MetaDataProperties.PageLanguageBranch, content);
                return pv;
            }
            return null;
        }

        /// <summary>
        /// Loads the common draft.
        /// </summary>
        /// <param name="contentLink">The content link to load common draft for.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// CommonDraft if it exist, otherwise Published, If no published exist the latest saved version is returned
        /// </returns>
        public ContentVersion LoadCommonDraft(ContentReference contentLink, string language)
        {
            var commonDraft = List(contentLink).Where(cv => cv.IsCommonDraft == true && cv.LanguageBranch == language).FirstOrDefault();
            if (commonDraft == null)
            {
                commonDraft = LoadPublished(contentLink);
                if (commonDraft == null)
                {
                    commonDraft = List(contentLink).OrderByDescending(cv => cv.Saved).FirstOrDefault();
                    commonDraft.IsCommonDraft = true;
                }
            }
            return commonDraft;
        }

        /// <summary>
        /// Sets the specified version as common draft.
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        public void SetCommonDraft(ContentReference contentLink)
        {
            XElement contentNode = GetContentNode(contentLink);
            if (contentNode != null)
            {
                var isCommonAttribute = contentNode.Attribute("isCommonDraft");
                isCommonAttribute.Value = true.ToString();
            }
            XmlContents.Save(FilePath);
        }

        private void HandleCommonDraft(IContent content)
        {
            ILocalizable langData = content as ILocalizable;
            var res = XmlContents.Elements("contents").Elements<XElement>("content").Where(xe => int.Parse(xe.Attribute("id").Value) == content.ContentLink.ID);
            foreach (var c in res)
            {
                if (langData != null)
                {
                    if (langData.Language != null)
                    {
                        var lang = GetPropertyValue(MetaDataProperties.PageLanguageBranch, c);
                        if (lang == langData.Language.Name)
                        {
                            c.Attribute("isCommonDraft").Value = false.ToString();
                        }
                    }
                }
                else
                {
                    c.Attribute("isCommonDraft").Value = false.ToString();
                }
            }
            SetCommonDraft(content.ContentLink);
        }
        #endregion

        #region Resolve Content

        /// <inheritdoc />
        protected override ContentResolveResult ResolveContent(ContentReference contentLink)
        {
            if (contentLink == null)
            {
                return null;
            }
            //There might be several pageNodes in Xml with same id (different languages of same page). Take first since
            //guid is the same.
            var contents = GetContentVersions(contentLink);
            if (!(contents?.FirstOrDefault()?.HasElements ?? false))
            {
                return null;
            }
            var content = contents.FirstOrDefault<XElement>();
            var contentGuid = GetGuid(content);
            ContentResolveResult contentResolveResult = new ContentResolveResult();
            contentResolveResult.ContentLink = contentLink;
            contentResolveResult.UniqueID = contentGuid;
            contentResolveResult.ContentUri = ConstructContentUri(GetContentTypeID(content), contentLink, contentGuid);
            return contentResolveResult;
        }

        /// <inheritdoc />
        protected override ContentResolveResult ResolveContent(Guid contentGuid)
        {

            if (contentGuid == Guid.Empty)
            {
                return null;
            }
            //There might be several pageNodes in Xml with same guid (different languages of same page). Take first since
            //pagelink is the same.
            var content = (from pGuid in XmlContents.Descendants("content").Descendants<XElement>("property").Attributes("name")
                           where (pGuid.Value == "PageGUID" && pGuid.Parent.Value == contentGuid.ToString())
                           select pGuid.Parent.Parent).FirstOrDefault<XElement>();
            if (content == null || !content.HasElements)
            {
                return null;
            }

            var contentLink = GetContentReference(content);
            ContentResolveResult contentResolveResult = new ContentResolveResult();
            contentResolveResult.ContentLink = contentLink;
            contentResolveResult.UniqueID = contentGuid;
            contentResolveResult.ContentUri = ConstructContentUri(GetContentTypeID(content), contentLink, contentGuid);
            return contentResolveResult;
        }

        #endregion

        #endregion

        #region Construct ContentReference
        protected virtual ContentReference ConstructContentReference(int id)
        {
            return id != EntryPoint.ID ? ConstructLocalContentReference(id) : new ContentReference(EntryPoint.ID);
        }
        protected virtual ContentReference ConstructContentReference(int id, int pageVersion)
        {
            return id != EntryPoint.ID ? ConstructLocalContentReference(id, pageVersion) : new ContentReference(EntryPoint.ID);
        }
        protected virtual ContentReference ConstructLocalContentReference(int id)
        {
            return new ContentReference(id, ProviderKey);
        }
        protected virtual ContentReference ConstructLocalContentReference(int id, int pageVersion)
        {
            return new ContentReference(id, pageVersion, ProviderKey);
        }

        #endregion

        #region Privates

        private void SetProperty(PropertyData property, string value)
        {
            if (property == null || String.Equals(property.Name, "PageLink") || String.Equals(property.Name, "PageParentLink"))
            {
                return;
            }

            if (property.Type == PropertyDataType.ContentReference)
            {
                property.Value = ContentReference.Parse(value);
            }


            var converter = _propertyValueConverterFactory.Get(property.GetType());

            var dataRecord = ParseDataRecord(property.Type, value);
            converter.SetValue(property, dataRecord, new PropertyValueConverterContext());
        }

        private void HandleSecurity(IContent content, XElement contentNode, ContentReference parentLink)
        {
            var securableContent = content as IContentSecurable;
            if (securableContent != null)
            {
                var acl = GetContentAcl(content, contentNode, parentLink);

                var contentSecurityDescriptor = securableContent.GetContentSecurityDescriptor();
                if (contentSecurityDescriptor != null)
                {
                    contentSecurityDescriptor.IsInherited = acl.IsInherited;
                    contentSecurityDescriptor.ContentLink = acl.ContentLink;
                    if (!contentSecurityDescriptor.IsInherited)
                    {
                        contentSecurityDescriptor.Clear();
                        contentSecurityDescriptor.ContentLink = content.ContentLink;
                        foreach (var entry in acl)
                        {
                            contentSecurityDescriptor.AddEntry(entry.Value);
                        }
                    }
                }
            }
        }

        private void HandleReadOnly(IContent content)
        {
            IReadOnly readOnlyContent = content as IReadOnly;
            if (readOnlyContent != null)
            {
                readOnlyContent.MakeReadOnly();
            }
        }

        private void HandleLinkUrl(IContent content, int contentTypeId)
        {
            PageData page = content as PageData;
            if (page != null && page.LinkType == PageShortcutType.Shortcut)
            {
                var propertyPageReference = (PropertyContentReference)page.Property[MetaDataProperties.PageShortcutLink];
                if (propertyPageReference.GuidValue != Guid.Empty)
                {
                    page.Property[MetaDataProperties.PageLinkURL].LoadData(PermanentLinkUtility.GetPermanentLinkVirtualPath(propertyPageReference.GuidValue, ".aspx"));
                }
            }
            else if (page != null)
            {
                page.Property[MetaDataProperties.PageLinkURL].Value = ConstructContentUri(contentTypeId, content.ContentLink, content.ContentGuid);
            }
        }

        private void HandleLinkUrl(IContent content)
        {
            PageData page = content as PageData;
            if (page != null)
            {
                var propertyPageReference = (PropertyContentReference)page.Property[MetaDataProperties.PageLinkURL];
                if (propertyPageReference.GuidValue != Guid.Empty)
                {
                    page.Property[MetaDataProperties.PageLinkURL].LoadData(PermanentLinkUtility.GetPermanentLinkVirtualPath(propertyPageReference.GuidValue, ".aspx"));
                }
            }
        }

        private void HandleLanguage(IContent content, XElement contentNode, List<CultureInfo> existingLanguages)
        {

            ILocalizable localizableContent = content as ILocalizable;
            if (localizableContent != null)
            {
                localizableContent.ExistingLanguages = existingLanguages;
                localizableContent.Language = CultureInfo.GetCultureInfo(GetPropertyValue(MetaDataProperties.PageLanguageBranch, contentNode));
                localizableContent.MasterLanguage = CultureInfo.GetCultureInfo(GetPropertyValue(MetaDataProperties.PageMasterLanguageBranch, contentNode));
            }

        }

        private XElement GetContentNode(ContentReference contentLink, ILanguageSelector selector, out List<CultureInfo> existingLanguages)
        {

            XElement selectedNode = null;

            // Helper lambdas to avoid null reference errors when processing xml attribute values
            // (using a lambda instead of methods still enables the expression tree to be expanded as if the
            // expressions were in-line)
            Func<XElement, string, string> getAttributeValue =
                (pageNode, attributeName) =>
                {
                    var attr = pageNode.Attribute(attributeName);
                    return (attr == null) ? null : attr.Value;
                };

            Func<XElement, string, string> getPropertyElementValue =
                (pageNode, propertyName) =>
                {
                    var attr = pageNode.Elements("property")
                        .Attributes("name")
                        .FirstOrDefault(a => a.Value == propertyName);
                    return (attr == null) || (attr.Parent == null) ? null : attr.Parent.Value;
                };

            // Select nodes matching the page ID and project some of their properties into an anonymous
            // to simplify processing
            var projectedPageNodes =
                XmlContents.Descendants("content").
                    Where(n => Convert.ToInt32(getAttributeValue(n, "id")) == contentLink.ID).
                    Select(
                        n =>
                        new
                        {
                            ContentNode = n,
                            VersionId = Convert.ToInt32(getAttributeValue(n, "versionid")),
                            IsPublishedVersion = Convert.ToInt32(
                                getPropertyElementValue(n, MetaDataProperties.PageWorkStatus) ?? "4").Equals(
                                    (int)VersionStatus.Published),
                            Language = (getPropertyElementValue(n, MetaDataProperties.PageLanguageBranch) ?? "").ToLower(),
                            MasterLanguage = (getPropertyElementValue(n, MetaDataProperties.PageMasterLanguageBranch) ?? "").ToLower()
                        }).
                    ToList();

            // Find all occurring languages
            existingLanguages = projectedPageNodes.Select(a => CultureInfo.GetCultureInfo(a.Language)).Distinct().ToList();

            // Find page node to return
            if (contentLink.WorkID > 0)
            {
                // Specific version requested, no need to match language, just match version
                selectedNode =
                    projectedPageNodes.
                        Where(a => a.VersionId == contentLink.WorkID).
                        Select(a => a.ContentNode).
                        SingleOrDefault();
            }
            else
            {
                // No specific version requested, find best language and version match

                // Find page master language (should be same on all nodes)
                string masterLanguage = projectedPageNodes.Select(p => p.MasterLanguage).FirstOrDefault();

                // Order pages by language and then by publish status and finally version, making the first
                // occurrence for a language either the published or newest unpublished version
                var orderedContents =
                    projectedPageNodes.
                        OrderByDescending(a => a.Language).
                        ThenByDescending(a => a.IsPublishedVersion).
                        ThenByDescending(a => a.VersionId).
                        ToList();

                if (contentLink.GetPublishedOrLatest)
                {
                    LanguageBranch langBr = null;
                    if (selector.Language != null && !selector.Language.Equals(CultureInfo.InvariantCulture))
                    {
                        langBr = _languageBranchRepository.Load(selector.Language);
                    }
                    String selectedLang = langBr != null ? langBr.Culture.Name : masterLanguage;
                    selectedNode =
                        orderedContents.
                       Where(a => String.Equals(a.Language, selectedLang, StringComparison.OrdinalIgnoreCase)).
                       Select(a => a.ContentNode).
                       FirstOrDefault();
                }
                else
                {
                    if (selector.Language != null && !selector.Language.Equals(CultureInfo.InvariantCulture))
                    {
                        //load language given by context.SelectedLanguage
                        selectedNode =
                            orderedContents.
                                Where(a => String.Equals(a.Language, selector.Language.Name, StringComparison.OrdinalIgnoreCase)).
                                Select(a => a.ContentNode).
                                FirstOrDefault();
                    }
                }

                //If the specific language does not exits then load master language version content 
                if (selectedNode == null)
                {
                    //load page on masterlanguage
                    selectedNode =
                        orderedContents.
                            Where(a => String.Equals(a.Language, masterLanguage, StringComparison.OrdinalIgnoreCase)).
                            Select(a => a.ContentNode).
                            FirstOrDefault();
                }
            }

            return selectedNode;
        }

        private XElement GetContentNode(ContentReference contentLink)
        {
            List<CultureInfo> existingLanguages;
            return GetContentNode(contentLink, LanguageSelector.AutoDetect(), out existingLanguages);
        }

        private ContentAccessControlList GetContentAcl(IContent content, XElement contentNode, ContentReference parentLink)
        {
            string securitySetting = (from securityConfig in contentNode.Attributes("security")
                                      select securityConfig.Value).SingleOrDefault<string>();

            if (!String.IsNullOrEmpty(securitySetting))
            {
                return ParseAcl(securitySetting);
            }
            else
            {
                ContentAccessControlList contentAccessControlList = new ContentAccessControlList(content.ContentLink);
                contentAccessControlList.IsInherited = true;
                return contentAccessControlList;
            }
        }

        private ContentAccessControlList ParseAcl(string configurationString)
        {
            var acl = new ContentAccessControlList();

            if (!String.IsNullOrEmpty(configurationString))
            {
                string[] entries = configurationString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    string[] accessEntryStrings = entry.Split(':');

                    string username = accessEntryStrings[0];
                    AccessLevel accessLevel = (AccessLevel)Enum.Parse(typeof(AccessLevel), accessEntryStrings[1]);

                    acl.Add(new AccessControlEntry(username, accessLevel, Roles.RoleExists(username) ? SecurityEntityType.Role : SecurityEntityType.User));
                }
            }

            return acl;
        }

        private XDocument XmlContents
        {
            get
            {
                string cacheKey = _xmlKey + ProviderKey;
                var contents = _cacheInstance.Get(cacheKey) as XDocument;
                if (contents == null)
                {
                    contents = XDocument.Load(FilePath);
#pragma warning disable 618
                    var cacheEvictionPolicy = new CacheEvictionPolicy(TimeSpan.Zero, CacheTimeoutType.Undefined, null, null, new string[] { FilePath });
#pragma warning restore 618
                    _cacheInstance.Insert(cacheKey, contents, cacheEvictionPolicy);
                }
                return contents;
            }
        }

        private string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    _filePath = Parameters.Get("filePath");
                    if (!Path.IsPathRooted(_filePath))
                    {
                        if (HttpContext.Current != null)
                        {
                            _filePath = HttpContext.Current.Server.MapPath(_filePath);
                        }
                        else
                        {
                            _filePath = Path.Combine(Environment.CurrentDirectory, _filePath);
                        }
                    }
                    ChangeReadonlyFlagOnFile(_filePath);
                }
                return _filePath;
            }
        }

        private static void ChangeReadonlyFlagOnFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                using (var file = File.CreateText(filePath))
                {
                    file.WriteLine(@"<?xml version=""1.0""?>");
                    file.WriteLine(@"<contents />");
                }
            }
            File.SetAttributes(filePath, FileAttributes.Normal);
        }

        private bool IsPublishedVersion(XElement contentNode)
        {
            return (from properties in contentNode.Elements("property")
                    where String.Equals(properties.Attribute("name").Value, "PageWorkStatus")
                    select properties).Any(prop => prop.Value == ((int)VersionStatus.Published).ToString());
        }

        private int GetVersionId(IContent content, IEnumerable<XElement> contentVersNodes, StatusTransition transition)
        {
            var latestVersionID = GetLatestVersionID(contentVersNodes);

            if (transition.CreateNewVersion)
            {
                return latestVersionID + 1;
            }
            else if (content.ContentLink.WorkID == 0)
            {
                return latestVersionID;
            }

            return content.ContentLink.WorkID;
        }

        private static int GetLatestVersionID(IEnumerable<XElement> contentVersNodes)
        {
            int versionId = 0;
            foreach (XElement pageNode in contentVersNodes)
            {
                if (int.Parse(pageNode.Attribute("versionid").Value) > versionId)
                    versionId = int.Parse(pageNode.Attribute("versionid").Value);
            }
            return versionId;
        }

        private void SetContentStatus(IContent content, VersionStatus status)
        {
            IVersionable versionable = content as IVersionable;
            if (versionable != null)
            {
                versionable.Status = status;
                if (status == VersionStatus.Published)
                {
                    versionable.IsPendingPublish = false;
                }
            }
        }

        private void Save(IContent content, int versionId, IEnumerable<XElement> pageVersNodes)
        {
            if (!(content is PageData))
            {
                throw new ArgumentException("This version of XMLContentProvider providers only PageData.");
            }

            int contentId = content.ContentLink.ID;

            if (content is IChangeTrackable)
            {
                (content as IChangeTrackable).Saved = DateTime.Now;
            }

            var contentVersNode = (from aPage in pageVersNodes
                                   where int.Parse(aPage.Attribute("versionid").Value) == versionId
                                   select aPage).SingleOrDefault<XElement>();

            if (contentVersNode == null)
            {
                //
                //If it is a new page we assign a guid based identifier for it
                //
                Guid contentGuid = content.ContentGuid != Guid.Empty ? content.ContentGuid : Guid.NewGuid();
                contentId = contentId > 0 ? contentId : NextId;

                //If parent is EntryPoint we store it with id 0 to avoid having hard coupling to EntryPoint id in backing xml file.
                int parentId = content.ParentLink == EntryPoint ? 0 : content.ParentLink.ID;
                contentVersNode = new XElement("content",
                        new XAttribute("id", contentId.ToString()),
                        new XAttribute("parent", parentId.ToString()),
                        new XAttribute("pagetypeid", content.ContentTypeID.ToString()),
                        new XAttribute("versionid", versionId.ToString()),
                        new XAttribute("isCommonDraft", false.ToString()),
                        new XElement("property",
                            new XAttribute("name", "PageGUID"), contentGuid.ToString()));

                if (pageVersNodes != null && pageVersNodes.Attributes("security").FirstOrDefault() != null)
                {
                    contentVersNode.SetAttributeValue("security", pageVersNodes.Attributes("security").FirstOrDefault().Value);
                }

                content.ContentLink = ConstructLocalContentReference(contentId, versionId);
                SetPropertyValuesOnXML(content, contentVersNode);
                XmlContents.Element("contents").Add(contentVersNode);
            }
            else
            {
                SetPropertyValuesOnXML(content, contentVersNode);
            }
        }

        private void SetPropertyValuesOnXML(IContent content, XElement contentVersNode)
        {
            //
            //Clear all old values first (since they might have had a value before but now had been cleared)
            //
            foreach (XElement oldProp in contentVersNode.Elements().Where(element => element.Attribute("name").Value != "PageGUID"))
            {
                oldProp.SetValue(string.Empty);
            }

            var propertyContext = new PropertyValueConverterContext { ContentGuid = content.ContentGuid, ContentLink = content.ContentLink, Language = (content as ILocale)?.Language };

            foreach (var property in content.Property)
            {
                //Get property node (if it exist else create it) and set value for it
                XElement propNode = contentVersNode.Elements().Where(p => String.Equals(p.Attribute("name").Value, property.Name)).SingleOrDefault<XElement>();

                if (propNode == null)
                {
                    propNode = new XElement("property", new XAttribute("name", property.Name));
                    contentVersNode.Add(propNode);
                }

                if (string.Equals(property.Name, MetaDataProperties.PageChanged))
                {
                    propNode.SetValue(GetDate(DateTime.Now));
                    continue;
                }

                // HACK: Skip using Permanent GUIDs
                if (property.Type == PropertyDataType.ContentReference || property.Type == PropertyDataType.PageReference)
                {
                    propNode.SetValue((property.Value as ContentReference)?.ToReferenceWithoutVersion().ToString() ?? string.Empty);
                    continue;
                }

                var converter = _propertyValueConverterFactory.Get(property.GetType());
                var dataRecord = converter.GetValue(property, propertyContext);

                propNode.SetValue(GetDataRecordValue(dataRecord));
            }
        }

        private string GetDataRecordValue(PropertyDataRecord r)
        {
            if (r.String != null)
                return r.String;
            if (r.LongString != null)
                return r.LongString;
            if (r.CategoryString != null)
                return r.CategoryString;
            if (r.Boolean.HasValue)
                return r.Boolean.Value.ToString();
            if (r.Guid.HasValue)
                return r.Guid.Value.ToString();
            if (r.Integer.HasValue)
                return r.Integer.Value.ToString();
            if (r.Double.HasValue)
                return r.Double.Value.ToString();
            if (r.Date.HasValue)
                return r.Date.Value.ToString();
            if (r.PageType.HasValue)
                return r.PageType.Value.ToString();

            return string.Empty;
        }

        private PropertyDataRecord ParseDataRecord(PropertyDataType dataType, string value)
        {
            var r = new PropertyDataRecord();
            if (!string.IsNullOrEmpty(value))
            {
                switch (dataType)
                {
                    case PropertyDataType.Boolean:
                        r.Boolean = bool.Parse(value);
                        break;
                    case PropertyDataType.Number:
                        r.Integer = int.Parse(value);
                        break;
                    case PropertyDataType.PageType:
                        r.PageType = int.Parse(value);
                        break;
                    case PropertyDataType.FloatNumber:
                        r.Double = double.Parse(value);
                        break;
                    case PropertyDataType.Date:
                        r.Date = DateTime.Parse(value);
                        break;
                    case PropertyDataType.String:
                    case PropertyDataType.PageReference:
                    case PropertyDataType.ContentReference:
                        r.String = value;
                        break;
                    case PropertyDataType.LongString:
                    case PropertyDataType.Json:
                    case PropertyDataType.LinkCollection:
                        r.LongString = value;
                        break;
                    case PropertyDataType.Category:
                        r.CategoryString = value;
                        break;
                    case PropertyDataType.Block:
                        throw new NotSupportedException("Block properties are not supported by the XmlContentProvider.s");
                }
            }
            return r;
        }

        private String GetDate(DateTime value)
        {
            return value.ToString(DateTimeFormatInfo.InvariantInfo);
        }

        private DateTime GetDate(String value)
        {
            return DateTime.Parse(value, System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        private Dictionary<string, string> GetPropertyValues(IContent content)
        {

            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (PropertyData prop in content.Property)
            {
                if (prop.IsDynamicProperty)
                {
                    continue;
                }
                if (prop.IsNull)
                {
                    continue;
                }
                if (prop.IsMetaData && prop.Name != MetaDataProperties.PageName)
                {
                    continue;
                }
                if (prop.Name == MetaDataProperties.PageLink || prop.Name == MetaDataProperties.PageParentLink)
                {
                    var pageReferenceProperty = prop.Value as ContentReference;
                    if (pageReferenceProperty != null)
                    {
                        var externalFormat = new ContentReference(pageReferenceProperty.ID, pageReferenceProperty.WorkID);
                        properties.Add(prop.Name, externalFormat.ToString());
                        continue;
                    }
                }
#pragma warning disable CS0618 // Type or member is obsolete
                properties.Add(prop.Name, prop.ToRawProperty().Value);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            return properties;
        }

        /// <summary>
        /// Gets the next available id from the xml file
        /// </summary>
        /// <value>The next id.</value>
        private int NextId
        {
            get
            {
                Int32 nextId = 1;
                var pages = from p in XmlContents.Descendants("content") select p;
                foreach (XElement page in pages)
                {
                    Int32 currentId = Int32.Parse(page.Attribute("id").Value);
                    if (currentId >= nextId)
                    {
                        nextId = currentId + 1;
                    }
                }
                return nextId;
            }
        }

        /// <summary>
        /// Invoked in case Save with SaveAction.Publish is called
        /// </summary>
        /// <param name="content">Page to publish</param>
        /// <param name="versionId">Version to publish</param>
        /// <param name="contentVersNodes">Collection of XElements that rappresents different versions of the same page</param>
        private void Publish(IContent content, int versionId, IEnumerable<XElement> contentVersNodes)
        {
            //set Saved date to now
            var trackableContent = content as IChangeTrackable;
            if (trackableContent != null)
            {
                trackableContent.Saved = DateTime.Now;
            }

            SetStartPublish(content);

            if (ContentReference.IsNullOrEmpty(content.ContentLink))
            {
                //
                // it is a new page so we need to save it 
                //
                Save(content, versionId, contentVersNodes);
                return;
            }

            ILocalizable localizableContent = content as ILocalizable;
            IEnumerable<XElement> languageContents = Enumerable.Empty<XElement>();
            if (localizableContent != null)
            {
                //Get all versions of the same language
                languageContents = from langElem in contentVersNodes.Elements("property").Attributes("name")
                                   where String.Equals(langElem.Value, MetaDataProperties.PageLanguageBranch)
                                   let p = langElem.Parent
                                   where String.Equals(p.Value, localizableContent.Language != null ? localizableContent.Language.Name : "")//????
                                   select langElem.Parent.Parent;
            }

            foreach (XElement contentNode in languageContents)
            {
                //
                // Unpublish other version that is currently published(5 = VersionStatus.PreviouslyPublished, 4 = VersionStatus.Published)
                //
                XElement propNode = contentNode.Elements().Where(p => String.Equals(p.Attribute("name").Value, "PageWorkStatus")).SingleOrDefault<XElement>();

                if (propNode != null && (propNode.Value == "4"))
                {
                    propNode.SetValue("5");
                }
            }

            var contentVersNode = (from aPage in contentVersNodes
                                   where int.Parse(aPage.Attribute("versionid").Value) == versionId
                                   select aPage).SingleOrDefault<XElement>();
            if (contentVersNode == null)
            {
                // this is actually a new version that we are publishing
                Save(content, versionId, contentVersNodes);
            }
            else
            {
                SetPropertyValuesOnXML(content, contentVersNode);
            }
        }

        private static void SetStartPublish(IContent content)
        {
            var versionable = content as IVersionable;
            if (versionable != null && !versionable.StartPublish.HasValue)
            {
                versionable.StartPublish = DateTime.Now;
            }
        }

        private string GetPropertyValue(string valueName, XElement page)
        {
            var a = (from pageProp in page.Elements("property")
                     where String.Equals(pageProp.Attribute("name").Value, valueName)
                     select pageProp.Value).SingleOrDefault<string>();
            return a;
        }

        private void SetDeleteFlag(XElement contentNode, bool delete, bool recursively)
        {
            // Get property node (if it exist else create it) and set value for it
            var propDeleted = contentNode.Elements().SingleOrDefault(p => String.Equals(p.Attribute("name").Value, "PageDeleted"));
            if (propDeleted == null)
            {
                if (delete)
                {
                    EnsureUniquenessAttribute(contentNode, "PageDeleted", "true");
                    EnsureUniquenessAttribute(contentNode, "PageDeletedBy", _principalAccessor.CurrentName());
                    EnsureUniquenessAttribute(contentNode, "PageDeletedDate", GetDate(DateTime.Now));
                }
            }
            else
            {
                var propDeletedBy = contentNode.Elements().SingleOrDefault(p => String.Equals(p.Attribute("name").Value, "PageDeletedBy"));
                var propDeletedDate = contentNode.Elements().SingleOrDefault(p => String.Equals(p.Attribute("name").Value, "PageDeletedDate"));

                if (delete)
                {
                    propDeleted.SetValue("true");
                    propDeletedBy.SetValue(_principalAccessor.CurrentName());
                    propDeletedDate.SetValue(GetDate(DateTime.Now));
                }
                else
                {
                    propDeleted.Remove();
                    // Also remove related nodes: PageDeletedBy, PageDeletedDate
                    propDeletedBy.Remove();
                    propDeletedDate.Remove();
                }
            }

            if (recursively)
            {
                var pageId = (from pId in contentNode.Attributes("id")
                              select pId.Value).SingleOrDefault<string>();

                var children = from page in XmlContents.Descendants("content")
                               where page.Attribute("parent").Value == pageId
                               select page;

                var handledIds = new List<string>();
                foreach (var child in children)
                {
                    var id = (from pId in child.Attributes("id") select pId.Value).SingleOrDefault<string>();
                    if (!handledIds.Contains(id))
                    {
                        //It is enough if we call recursively for one node (we can have several languages/versisons)
                        handledIds.Add(id);
                        SetDeleteFlag(child, delete, true);
                    }
                    else
                    {
                        SetDeleteFlag(child, delete, false);
                    }
                }
            }
        }

        private void EnsureUniquenessAttribute(XElement node, String attribuetName, String value)
        {
            var theElement = node.Elements().SingleOrDefault(p => String.Equals(p.Attribute("name").Value, attribuetName));
            if (theElement == null)
            {
                node.Add(new XElement("property", new XAttribute("name", attribuetName), value));
            }
            else
            {
                theElement.Value = value;
            }
        }

        private IEnumerable<XElement> GetContentVersions(ContentReference contentLink)
        {
            var contentNodes = from contentVersions in XmlContents.Descendants("content")
                               where contentVersions.Attribute("id").Value == contentLink.ID.ToString()
                               select contentVersions;
            return contentNodes;
        }

        private ContentReference GetParentLink(XElement contentNode)
        {
            int parentId = int.Parse(contentNode.Attribute("parent").Value);
            //Children to entryPoint is stored with parentId = 0 in backing xml file to avoid hard coupling to EntryPoint id. Meaning
            //we can change entryPoint for provider without having to change in xml file.
            ContentReference parentLink = parentId != 0 ? ConstructLocalContentReference(parentId) : EntryPoint;
            return parentLink;
        }

        private ContentType GetContentType(XElement contentNode)
        {
            Int32 contentTypeId = GetContentTypeID(contentNode);
            ContentType contentType = _contentTypeRepository().Load(contentTypeId) as ContentType;
            if (contentType == null)
            {
                throw new EPiServerException(string.Format("Failed To Load Content Type with the ID = {0}\r\n Hints:Review the Content Type  value in the XML Content file {1}", contentTypeId, FilePath));
            }
            string pageTypeName = GetPropertyValue("PageTypeName", contentNode);
            if (!String.IsNullOrEmpty(pageTypeName) && !String.Equals(contentType.Name, pageTypeName, StringComparison.OrdinalIgnoreCase))
            {
                ContentType contentTypeByName = _contentTypeRepository().Load(pageTypeName) as ContentType;
                if (contentTypeByName.ID != contentTypeId)
                {
                    throw new ArgumentException(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "Missmatch ContentType between Saved content type {0} and Loaded content type {1}", contentType.Name, contentTypeByName.Name));
                }
            }
            return contentType;
        }

        private IContent CreateContent(ContentType contentType, ContentReference parentLink, ContentReference writableContentLink)
        {
            IContent content = _contentFactory.CreateContent(contentType);
            content.ContentLink = writableContentLink;
            content.ParentLink = parentLink;
            return content;
        }

        private void PopulateProperties(XElement contentNode, IContent content)
        {
            //Now get all properties from Content  node and populate Content object.
            if (contentNode.HasElements)
            {
                foreach (XElement property in contentNode.Elements())
                {
                    String propName = property.Attribute("name").Value;
                    SetProperty(content.Property[propName], property.Value);
                }
            }
        }

        private ContentReference GetContentLink(ContentReference contentLink, XElement contentNode)
        {
            ContentReference writableContentLink = contentLink.CreateWritableClone() as ContentReference;
            var foundContentId = Int32.Parse(contentNode.Attribute("versionid").Value);

            if (contentLink.GetPublishedOrLatest && !IsPublishedVersion(contentNode))
            {
                writableContentLink.WorkID = foundContentId;
            }
            return writableContentLink;
        }

        private static bool IsMovedToArchive(IContent content, ContentReference destinationLink)
        {
            bool isMovedToArchive = false;
            //Review: What about Icontent 
            PageData page = content as PageData;
            if (page != null)
            {
                isMovedToArchive = page.PendingArchive && page.ArchiveLink == destinationLink;
            }
            return isMovedToArchive;
        }

        private ContentReference GetContentReference(XElement content)
        {
            var contentId = (from contentID in content.Attributes("id")
                             select contentID.Value).SingleOrDefault<string>();

            return ConstructLocalContentReference(int.Parse(contentId));
        }

        private Guid GetGuid(XElement content)
        {
            string guidString = (from pageGuid in content.Elements("property").Attributes("name")
                                 where pageGuid.Value == "PageGUID"
                                 select pageGuid.Parent.Value).SingleOrDefault<string>();
            if (String.IsNullOrEmpty(guidString))
            {
                throw new EPiServerException("Page must have GUID");
            }

            return new Guid(guidString);
        }

        private int GetContentTypeID(XElement page)
        {
            int contentTypeId = 0;
            if (!int.TryParse(page.Attribute("pagetypeid").Value, out contentTypeId))
            {
                throw new EPiServerException("ID value of pagetype must be numeric");
            }
            return contentTypeId;
        }

        private static bool IsNewVersionRequire(IContent content)
        {
            if (ContentReference.IsNullOrEmpty(content.ContentLink))
            {
                return true;
            }
            if (content.ContentLink.WorkID == 0)
            {
                return true;
            }

            IVersionable versionStatus = content as IVersionable;
            if (versionStatus != null)
            {
                switch (versionStatus.Status)
                {
                    case VersionStatus.NotCreated:
                    case VersionStatus.PreviouslyPublished:
                    case VersionStatus.CheckedIn:
                    case VersionStatus.Published:
                        return true;
                    case VersionStatus.CheckedOut:
                    case VersionStatus.DelayedPublish:
                    case VersionStatus.Rejected:
                        return false;
                }
            }

            return false;
        }

        #endregion

    }
}