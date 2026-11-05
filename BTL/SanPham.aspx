<%@ Page Title="Sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SanPham.aspx.cs" Inherits="ShoeStore.SanPham" %>
<%@ Import Namespace="System.Web" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Sản phẩm</h1>

    <p>
        Lọc theo hãng:
        <a href="SanPham.aspx">Tất cả</a>
        <asp:Repeater ID="rptHang" runat="server">
            <SeparatorTemplate> &middot; </SeparatorTemplate>
            <ItemTemplate>
                <a href='<%# "SanPham.aspx?brand=" + HttpUtility.UrlEncode(Convert.ToString(Eval("Brand"))) %>'><%# Eval("Brand") %></a>
            </ItemTemplate>
        </asp:Repeater>
    </p>

    <p class="nhan"><asp:Literal ID="litKetQua" runat="server" /></p>

    <div class="luoi">
        <asp:Repeater ID="rptSanPham" runat="server">
            <ItemTemplate>
                <div class="the">
                    <a href='ChiTietSanPham.aspx?id=<%# Eval("Id") %>'>
                        <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                        <h3><%# Eval("Name") %></h3>
                    </a>
                    <p class="gia"><%# Eval("Price", "{0:N0} đ") %></p>
                    <p class="nhan"><%# Eval("Brand") %></p>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <p class="nhan"><asp:Literal ID="litRong" runat="server" /></p>

</asp:Content>
