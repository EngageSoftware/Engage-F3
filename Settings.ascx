<%@ Control Language="C#" AutoEventWireup="false" Inherits="Engage.Dnn.F3.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:CheckBox ID="chkEnablePublish" runat="server" resourcekey="chkEnablePublish" /><br />

<dnn:Label id="lblLowerTab" runat="server" resourcekey="lblLowerTab" /> <asp:TextBox ID="txtLowerTab" runat="server"></asp:TextBox><br />
<dnn:Label id="lblUpperTab" runat="server" resourcekey="lblUpperTab" /> <asp:TextBox ID="txtUpperTab" runat="server"></asp:TextBox>
