<%@ Page Title="Đăng nhập" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangNhap.aspx.cs" Inherits="ShoeStore.DangNhap" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/css/dangnhap.css?v=field-validation-1" />
    <script src="Content/js/auth-validation.js?v=field-validation-2" defer></script>
</asp:Content>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">
    <section class="auth-page-container auth-card-page">
    <header class="auth-header">
        <span class="auth-badge-icon"><i class="fa-solid fa-user-lock" aria-hidden="true"></i></span>
        <h1 class="auth-title">Chào mừng trở lại</h1>
        <p class="auth-subtitle">Đăng nhập để tiếp tục mua sắm tại SneakerX</p>
    </header>

    <asp:Panel ID="pnlDangNhap" runat="server" CssClass="auth-form" DefaultButton="btnDangNhap">
        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtTenDangNhap.ClientID %>">Tên đăng nhập hoặc email</label>
            <asp:TextBox ID="txtTenDangNhap" runat="server" CssClass="form-input-pill" MaxLength="254" autocomplete="username" data-validation="login-identifier" aria-describedby="MainContent_lblTenDangNhapError"></asp:TextBox>
            <asp:Label ID="lblTenDangNhapError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtTenDangNhap" />
        </div>

        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtMatKhau.ClientID %>">Mật khẩu</label>
            <asp:TextBox ID="txtMatKhau" runat="server" TextMode="Password" CssClass="form-input-pill" MaxLength="128" autocomplete="current-password" data-validation="required" aria-describedby="MainContent_lblMatKhauError"></asp:TextBox>
            <asp:Label ID="lblMatKhauError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtMatKhau" />
        </div>

        <div class="auth-remember">
            <asp:CheckBox ID="chkNho" runat="server" Text=" Nhớ đăng nhập trong 7 ngày" />
        </div>

        <asp:Button ID="btnDangNhap" runat="server" Text="Đăng nhập" CssClass="btn-pink-submit" OnClick="btnDangNhap_Click" />

        <div class="auth-message"><asp:Literal ID="litThongBao" runat="server" /></div>

        <p class="auth-footer-link">
            Chưa có tài khoản? <a href="DangKy.aspx">Đăng ký ngay</a>
        </p>
    </asp:Panel>
    </section>

</asp:Content>
