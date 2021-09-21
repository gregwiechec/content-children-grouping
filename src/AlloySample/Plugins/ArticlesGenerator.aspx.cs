using System;
using System.Web;
using AlloySample.Models.Pages;
using EPiServer;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace AlloySample.Plugins
{
    [GuiPlugIn(DisplayName = "Generate articles",
        Description = "Generate articles",
        Area = EPiServer.PlugIn.PlugInArea.AdminMenu,
        Url = "~/Plugins/ArticlesGenerator.aspx")]
    public partial class ArticlesGenerator : EPiServer.Shell.WebForms.WebFormsBase
    {
        protected Injected<IContentRepository> _contentRepository;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                Locate.Advanced.GetInstance<IAccessDeniedHandler>()
                    .AccessDenied(new HttpContextWrapper(HttpContext.Current));
            }

            SystemMessageContainer.Heading = "Generate articles";
        }

        protected void GenerateButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            var numberOfArticles = int.Parse(this.NumberOfArticles.Text);
            var containerContentLink = ArticlesContainer.PageLink;

            var now = DateTime.Now;

            for (var i = 0; i < numberOfArticles; i++)
            {
                var index = i / ListOfAnimals.Animals.Length;

                var articlePage = _contentRepository.Service.GetDefault<ArticlePage>(containerContentLink);
                articlePage.Name = ListOfAnimals.Animals[i % ListOfAnimals.Animals.Length] +
                                   (index == 0 ? "" : index.ToString());
                articlePage.Created = now.AddMonths(-i % 20);
                _contentRepository.Service.Save(articlePage, SaveAction.Publish, AccessLevel.NoAccess);
            }

            lblResult.Visible = true;
        }
    }
}