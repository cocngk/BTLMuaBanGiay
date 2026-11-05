<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrangChu.aspx.cs" Inherits="ShoeStore.TrangChu" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <section class="hero">
        <h1>Giày chính hãng, chọn là vừa</h1>
        <p>Nike, Adidas, Converse, Vans, New Balance - sẵn size, giao trong ngày.</p>
        <a class="nut" href="SanPham.aspx">Xem tất cả sản phẩm</a>
    </section>

    <h2>Sản phẩm nổi bật</h2>

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

    <asp:Literal ID="litThongBao" runat="server" />

</asp:Content>
