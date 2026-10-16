<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchBox.ascx.cs" Inherits="ShoeStore.SearchBox" %>
<%-- Ô tìm kiếm header tự trị: tự giữ từ khóa, tự chuyển trang. --%>
<asp:Panel ID="pnlSearch" runat="server" CssClass="search-pill" DefaultButton="btnGo">
    <asp:TextBox ID="txtQuery" runat="server" Placeholder="Tìm giày..."></asp:TextBox>
    <asp:LinkButton ID="btnGo" runat="server" CssClass="btn-search-icon"
        OnClientClick="return headerSearchSubmit(this);" OnClick="btnGo_Click">
        <i class="fa-solid fa-magnifying-glass"></i>
    </asp:LinkButton>
</asp:Panel>
