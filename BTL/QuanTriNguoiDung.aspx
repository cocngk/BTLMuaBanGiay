<%@ Page Title="Quản trị người dùng" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuanTriNguoiDung.aspx.cs" Inherits="ShoeStore.QuanTriNguoiDung" %>
<%@ Register Src="~/Controls/TabQuanTri.ascx" TagPrefix="uc" TagName="TabQuanTri" %>
<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server"><link rel="stylesheet" href="Content/css/quantri.css" /></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="MainContent" runat="server">
<section class="admin-page"><h1>Quản trị người dùng</h1><uc:TabQuanTri ID="ucTabs" runat="server" ActiveTab="users" />
<div class="admin-toolbar"><asp:TextBox ID="txtSearch" runat="server" CssClass="o-nhap" placeholder="Tên, tài khoản, email hoặc điện thoại" /><asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="nut" OnClick="btnSearch_Click" /></div>
<asp:Literal ID="litMessage" runat="server" /><div class="admin-table-wrap"><asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" DataKeyNames="Id" CssClass="admin-table" OnRowCommand="gvUsers_RowCommand" EmptyDataText="Chưa có người dùng nào.">
<Columns><asp:BoundField DataField="FullName" HeaderText="Họ tên" /><asp:BoundField DataField="Username" HeaderText="Tài khoản" /><asp:BoundField DataField="Email" HeaderText="Email" /><asp:BoundField DataField="Phone" HeaderText="Điện thoại" /><asp:CheckBoxField DataField="IsAdmin" HeaderText="Quản trị viên" /><asp:TemplateField HeaderText="Thao tác"><ItemTemplate><asp:LinkButton runat="server" Text='<%# Convert.ToBoolean(Eval("IsAdmin")) ? "Thu quyền" : "Cấp quyền" %>' CommandName="ToggleAdmin" CommandArgument='<%# Eval("Id") %>' CssClass="nut-link" /></ItemTemplate></asp:TemplateField></Columns>
</asp:GridView></div></section></asp:Content>
