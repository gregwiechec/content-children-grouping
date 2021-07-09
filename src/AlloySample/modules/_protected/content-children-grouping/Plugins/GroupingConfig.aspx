<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupingConfig.aspx.cs" Inherits="ContentChildrenGrouping.Plugin.GroupingConfig" %>
<%@ Import Namespace="EPiServer.Framework.Modules.Internal" %>

<asp:Content runat="server" ContentPlaceHolderID="MainRegion">
    
    <%= ModuleResourceResolver.Instance.ResolveClientPath("content-children-grouping", "grouping-config.js") %>
    
    
    <div class="epi-formArea">
        <div class="epi-size20">
            aaa
        </div>
    </div>
</asp:Content>