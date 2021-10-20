<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArticlesGenerator.aspx.cs" Inherits="AlloySample.Plugins.ArticlesGenerator" %>
<%@ Register TagPrefix="EPiServerUI" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI, Version=11.25.0.0, Culture=neutral, PublicKeyToken=8fe83dea738b45b7" %>

<asp:Content runat="server" ContentPlaceHolderID="MainRegion">
    <div class="epi-formArea">
        <div class="epi-size20">
            <div>
                <asp:Label runat="server" AssociatedControlID="ArticlesContainer" Translate="<%$ Resources: EPiServer, admin.siteinformationedit.startpage%>" />
                <EPiServer:InputPageReference Style="display: inline;" ID="ArticlesContainer" AutoPostBack="false" runat="server"/>
            </div>            
            <div>
                <asp:Label runat="server" AssociatedControlID="ArticleType" Text="Article type" />
                <asp:RadioButtonList id="ArticleType" runat="server">
                    <asp:ListItem Selected="True">Animal</asp:ListItem>
                    <asp:ListItem>Recipe</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div>
                <asp:Label AssociatedControlID="NumberOfArticles" runat="server" >Number of articles</asp:Label>
                <asp:TextBox ID="NumberOfArticles" runat="server" MaxLength="4" Width="100" Text="100" />
                <asp:RangeValidator ID="MaxNumberOfArticlesValidator" runat="server" ControlToValidate="NumberOfArticles" Type="Integer" MinimumValue="1" MaximumValue="10000" Display="Dynamic" Text="*" ErrorMessage="Number of articles should be between 1 and 10000" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="NumberOfArticles" Display="Dynamic" Text="*" ErrorMessage="Number of articles is required" />
            </div>
            <div class="epi-indent">
                <EPiServerUI:ToolButton ID="GenerateButton" OnClick="GenerateButton_Click" runat="server" SkinID="Report" Text="Generate" />
            </div>
            <div>
                <asp:Label ID="lblResult" runat="server" Visible="false">Content generated</asp:Label>
            </div>
        </div>
    </div>
</asp:Content>