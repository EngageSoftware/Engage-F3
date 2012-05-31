<%@ Control Language="C#" AutoEventWireup="false" Inherits="Engage.Dnn.F3.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<ul class="settings">
    <li>
        <asp:CheckBox ID="EnablePublishCheckBox" runat="server" resourcekey="chkEnablePublish" />
    </li>
    <li>
        <dnn:Label runat="server" resourcekey="lblLowerTab" /> <asp:TextBox ID="LowerTabIdTextBox" runat="server"/>
    </li>
    <li>
        <dnn:Label runat="server" resourcekey="lblUpperTab" /> <asp:TextBox ID="UpperTabIdTextBox" runat="server"/>
    </li>
</ul>