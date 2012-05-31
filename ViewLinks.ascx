<%@ Control Language="C#" Inherits="Engage.Dnn.F3.ViewLinks" AutoEventWireup="false" CodeBehind="ViewLinks.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/labelControl.ascx" %>
<div class="Normal engageF3">
    <div style="clear: both;">
        <p>
            <asp:Label resourcekey="lblInstructions" runat="server" />
        </p>
    </div>
    <dnn:label ResourceKey="lblSearchString" runat="server"/>
    <asp:TextBox ID="SearchTextBox" runat="server"/>
    <asp:Button ID="SearchTextHtmlButton" resourcekey="btnSearch" runat="server" Text="Text/HTML" />
    <asp:Button ID="SearchPublishButton" resourcekey="btnEngagePublish" runat="server" Text="Engage: Publish" />
    <asp:DataGrid ID="ResultsGrid" runat="server" AutoGenerateColumns="false" CssClass="DataGrid_Container" GridLines="None">
        <HeaderStyle VerticalAlign="Top" CssClass="DataGrid_Header" />
        <ItemStyle CssClass="DataGrid_Item" />
        <AlternatingItemStyle CssClass="DataGrid_AlternatingItem" />
        <FooterStyle CssClass="DataGrid_Footer" />
        <Columns>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblPageNameHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("TabName") %>'/>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblModuleTitleHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("ModuleTitle") %>'/>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblModuleTextHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# CleanupText(Eval("Content").ToString()) %>'/>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblContentStateIdHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("StateId") %>'/>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblEditHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink Target="_blank" NavigateUrl='<%# GetTextHtmlModuleEditLink((int)Eval("ModuleId"), (int)Eval("TabId"))%>' resourcekey="lnkEditLink" runat="server"/>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
    <asp:DataGrid ID="PublishResultsGrid" runat="server" AutoGenerateColumns="false" CssClass="DataGrid_Container" GridLines="None">
        <HeaderStyle VerticalAlign="Top" CssClass="DataGrid_Header" />
        <ItemStyle CssClass="DataGrid_Item" />
        <AlternatingItemStyle CssClass="DataGrid_AlternatingItem" />
        <FooterStyle CssClass="DataGrid_Footer" />
        <Columns>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblItemId" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("ItemId") %>'/>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblArticleTitleHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("Name") %>'/>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblModuleTextHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# CleanupText(Eval("ArticleText").ToString()) %>'/>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Label resourcekey="lblEditPublishHeader" runat="server"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink Target="_blank" NavigateUrl='<%# GetPublishViewLink((int)Eval("ItemID"))%>' resourcekey="lnkViewLink" runat="server"/>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
    <asp:Panel ID="ReplacementPanel" runat="server" Visible="false">
        <div>
            <dnn:label ResourceKey="HtmlReplaceLabel" runat="server" />
            <asp:TextBox ID="ReplacementTextBox" runat="server" />
            <asp:Button ID="ReplaceTextHtmlButton" resourcekey="btnReplace" runat="server" />
            <asp:Button ID="ReplacePublishButton" resourcekey="btnReplaceEngagePublish" runat="server" />
        </div>
        <div>
            <asp:Label ID="ReplacementResultsLabel" CssClass="error" runat="server" />
        </div>
    </asp:Panel>
</div>
