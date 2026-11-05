<%@ Page Title="Đăng ký" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangKy.aspx.cs" Inherits="ShoeStore.DangKy" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Đăng ký tài khoản</h1>

    <div class="form">
        <div class="dong">
            <label for="<%= txtTenDangNhap.ClientID %>">Tên đăng nhập</label>
            <asp:TextBox ID="txtTenDangNhap" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtHoTen.ClientID %>">Họ tên</label>
            <asp:TextBox ID="txtHoTen" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtDienThoai.ClientID %>">Điện thoại</label>
            <asp:TextBox ID="txtDienThoai" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtMatKhau.ClientID %>">Mật khẩu</label>
            <asp:TextBox ID="txtMatKhau" runat="server" TextMode="Password" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtNhapLai.ClientID %>">Nhập lại mật khẩu</label>
            <asp:TextBox ID="txtNhapLai" runat="server" TextMode="Password" CssClass="o-nhap"></asp:TextBox>
        </div>

        <asp:Button ID="btnDangKy" runat="server" Text="Đăng ký" CssClass="nut" OnClick="btnDangKy_Click" />

        <asp:Literal ID="litThongBao" runat="server" />

        <p class="nhan" style="margin-top: 18px;">
            Đã có tài khoản? <a href="DangNhap.aspx">Đăng nhập</a>
        </p>
    </div>

</asp:Content>
