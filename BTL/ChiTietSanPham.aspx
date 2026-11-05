<%@ Page Title="Chi tiết sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChiTietSanPham.aspx.cs" Inherits="ShoeStore.ChiTietSanPham" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Literal ID="litThongBao" runat="server" />

    <asp:Panel ID="pnlSanPham" runat="server" CssClass="chi-tiet">

        <div>
            <asp:Image ID="imgAnh" runat="server" AlternateText="Ảnh sản phẩm" />
        </div>

        <div>
            <h1><asp:Literal ID="litTen" runat="server" /></h1>
            <p class="nhan">Hãng: <asp:Literal ID="litHang" runat="server" /></p>
            <p class="gia" style="font-size: 22px;"><asp:Literal ID="litGia" runat="server" /></p>
            <p class="mo-ta"><asp:Literal ID="litMoTa" runat="server" /></p>
            <p class="nhan">Còn lại: <asp:Literal ID="litTonKho" runat="server" /> đôi</p>

            <div class="dong">
                <label for="<%= ddlSize.ClientID %>">Chọn size</label>
                <asp:DropDownList ID="ddlSize" runat="server" CssClass="o-chon" style="max-width: 160px;"></asp:DropDownList>
            </div>

            <div class="dong">
                <label for="<%= txtSoLuong.ClientID %>">Số lượng</label>
                <asp:TextBox ID="txtSoLuong" runat="server" Text="1" CssClass="o-so"></asp:TextBox>
            </div>

            <asp:Button ID="btnThem" runat="server" Text="Thêm vào giỏ hàng" CssClass="nut" OnClick="btnThem_Click" />

            <asp:Literal ID="litKetQua" runat="server" />

            <p style="margin-top: 16px;">
                <a href="SanPham.aspx">&larr; Tiếp tục xem sản phẩm</a>
            </p>
        </div>

    </asp:Panel>

</asp:Content>
