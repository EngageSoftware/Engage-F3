<%@ Control language="C#" Inherits="Engage.Dnn.F3.ViewLinks" AutoEventWireup="false"  Codebehind="ViewLinks.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/labelControl.ascx" %>

<div id="engageF3" class="Normal">
<div style="clear:both;">
<asp:Label ID="lblInstructions" resourcekey="lblInstructions" runat="server" />
<br />
<br />
</div>

<dnn:label id="lblSearchString" resourcekey="lblSearchString" Runat="server"></dnn:label>
<asp:TextBox ID="txtSearchString" runat="server"></asp:TextBox>
<asp:Button ID="btnSearch" resourcekey="btnSearch" runat="server" Text="Text/HTML" OnClick="btnSearch_Click" />
<asp:Button ID="btnEngagePublish" resourcekey="btnEngagePublish" runat="server" Text="Engage: Publish" OnClick="btnEngagePublish_Click" />
<asp:DataGrid ID="dgResults" runat="server" AutoGenerateColumns="false" CssClass="DataGrid_Container">
<HeaderStyle VerticalAlign="Top" CssClass="DataGrid_Header" />
<ItemStyle CssClass="DataGrid_Item" />
<AlternatingItemStyle CssClass="DataGrid_AlternatingItem" />
<FooterStyle CssClass="DataGrid_Footer" />
<Columns>
    <asp:TemplateColumn>
    <HeaderTemplate>
        <asp:Label ID="lblPageNameHeader" resourcekey="lblPageNameHeader" runat="server"></asp:Label>
    </HeaderTemplate>
    <ItemTemplate>
        <asp:Label ID="lblPageName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"TabName") %>'></asp:Label>
    </ItemTemplate>
    </asp:TemplateColumn>
    
    <asp:TemplateColumn>
       <HeaderTemplate>
       <asp:Label ID="lblModuleTitleHeader" resourcekey="lblModuleTitleHeader" runat="server"></asp:Label>
       </HeaderTemplate>
       <ItemTemplate>
        <asp:Label ID="lblModuleText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ModuleTitle") %>'></asp:Label>
       </ItemTemplate>   
    </asp:TemplateColumn>
    
    <asp:TemplateColumn>
       <HeaderTemplate>
       <asp:Label ID="lblModuleTextHeader" resourcekey="lblModuleTextHeader" runat="server"></asp:Label>
       </HeaderTemplate>
       <ItemTemplate>
        <asp:Label ID="lblModuleText" runat="server" Text='<%# CleanupText(DataBinder.Eval(Container.DataItem,"DesktopHtml").ToString()) %>'></asp:Label>
       </ItemTemplate>   
    </asp:TemplateColumn>
   
      <asp:TemplateColumn>
       <HeaderTemplate>
        <asp:Label ID="lblEditHeader" resourcekey="lblEditHeader" runat="server"></asp:Label>
       </HeaderTemplate>
       <ItemTemplate>
        <asp:HyperLink ID="lnkEditLink" Target="_blank" NavigateUrl='<%# GetEditLink(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"ModuleId")), Convert.ToInt32(DataBinder.Eval(Container.DataItem,"TabId")))%>' resourcekey="lnkEditLink" runat="server"></asp:HyperLink>
       </ItemTemplate>   
    </asp:TemplateColumn>
    
</Columns>
</asp:DataGrid>

<asp:DataGrid ID="dgPublishResults" runat="server" AutoGenerateColumns="false" CssClass="DataGrid_Container">
<HeaderStyle VerticalAlign="Top" CssClass="DataGrid_Header" />
<ItemStyle CssClass="DataGrid_Item" />
<AlternatingItemStyle CssClass="DataGrid_AlternatingItem" />
<FooterStyle CssClass="DataGrid_Footer" />
<Columns>

<asp:TemplateColumn>
       <HeaderTemplate>
       <asp:Label ID="lblModuleTitleHeader" resourcekey="lblItemId" runat="server"></asp:Label>
       </HeaderTemplate>
       <ItemTemplate>
        <asp:Label ID="lblModuleText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ItemId") %>'></asp:Label>
       </ItemTemplate>   
    </asp:TemplateColumn>
    
    <asp:TemplateColumn>
       <HeaderTemplate>
       <asp:Label ID="lblModuleTitleHeader" resourcekey="lblArticleTitleHeader" runat="server"></asp:Label>
       </HeaderTemplate>
       <ItemTemplate>
        <asp:Label ID="lblModuleText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>'></asp:Label>
       </ItemTemplate>   
    </asp:TemplateColumn>
    
    <asp:TemplateColumn>
       <HeaderTemplate>
       <asp:Label ID="lblModuleTextHeader" resourcekey="lblModuleTextHeader" runat="server"></asp:Label>
       </HeaderTemplate>
       <ItemTemplate>
        <asp:Label ID="lblModuleText" runat="server" Text='<%# CleanupText(DataBinder.Eval(Container.DataItem,"ArticleText").ToString()) %>'></asp:Label>
       </ItemTemplate>   
    </asp:TemplateColumn>
   
      <asp:TemplateColumn>
       <HeaderTemplate>
        <asp:Label ID="lblEditHeader" resourcekey="lblEditPublishHeader" runat="server"></asp:Label>
       </HeaderTemplate>
       <ItemTemplate>
        <asp:HyperLink ID="lnkEditLink" Target="_blank" NavigateUrl='<%# GetPublishLink(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"ItemID")))%>' resourcekey="lnkEditLink" runat="server"></asp:HyperLink>
       </ItemTemplate>   
    </asp:TemplateColumn>
    
</Columns>
</asp:DataGrid>
<br /><br />

<asp:panel ID="pnlReplacement" runat="server" Visible="false">
    <dnn:label ID="plHtmlReplace" runat="server" />
    <asp:TextBox ID="txtReplacementText" runat="server"></asp:TextBox>
    <asp:Button ID="btnReplace" resourcekey="btnReplace" runat="server" OnClick="btnReplace_Click" />
    <br />
    <asp:Label ID="lblReplacementResults" CssClass="error" runat="server" />
</asp:panel>
</div>