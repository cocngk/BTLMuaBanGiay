<%@ Page Title="Đăng ký" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangKy.aspx.cs" Inherits="ShoeStore.DangKy" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/css/dangky.css?v=field-validation-1" />
    <script src="Content/js/auth-validation.js?v=field-validation-2" defer></script>
</asp:Content>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">
    <section class="auth-page-container auth-register-container auth-card-page">
    <header class="auth-header">
        <span class="auth-badge-icon"><i class="fa-solid fa-user-plus" aria-hidden="true"></i></span>
        <h1 class="auth-title">Tạo tài khoản</h1>
        <p class="auth-subtitle">Đăng ký thành viên SneakerX — chỉ mất một phút</p>
    </header>

    <asp:Panel ID="pnlDangKy" runat="server" CssClass="auth-form" DefaultButton="btnDangKy">
        <div class="auth-register-grid">
        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtTenDangNhap.ClientID %>">Tên đăng nhập</label>
            <asp:TextBox ID="txtTenDangNhap" runat="server" CssClass="form-input-pill" MaxLength="50" autocomplete="username" data-validation="username" aria-describedby="MainContent_lblTenDangNhapError"></asp:TextBox>
            <asp:Label ID="lblTenDangNhapError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtTenDangNhap" />
        </div>

        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtHoTen.ClientID %>">Họ tên</label>
            <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-input-pill" MaxLength="100" autocomplete="name" data-validation="required" aria-describedby="MainContent_lblHoTenError"></asp:TextBox>
            <asp:Label ID="lblHoTenError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtHoTen" />
        </div>

        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtEmail.ClientID %>">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input-pill" TextMode="Email" MaxLength="254" autocomplete="email" data-validation="email" aria-describedby="MainContent_lblEmailError"></asp:TextBox>
            <asp:Label ID="lblEmailError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtEmail" />
        </div>

        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtDienThoai.ClientID %>">Điện thoại</label>
            <asp:TextBox ID="txtDienThoai" runat="server" CssClass="form-input-pill" MaxLength="11" autocomplete="tel" inputmode="numeric" data-validation="phone" aria-describedby="MainContent_lblDienThoaiError"></asp:TextBox>
            <asp:Label ID="lblDienThoaiError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtDienThoai" />
        </div>

        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtMatKhau.ClientID %>">Mật khẩu</label>
            <asp:TextBox ID="txtMatKhau" runat="server" TextMode="Password" CssClass="form-input-pill" MaxLength="128" autocomplete="new-password" data-validation="password" aria-describedby="MainContent_lblMatKhauError"></asp:TextBox>
            <asp:Label ID="lblMatKhauError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtMatKhau" />
        </div>

        <div class="form-group-item">
            <label class="form-label-custom" for="<%= txtNhapLai.ClientID %>">Nhập lại mật khẩu</label>
            <asp:TextBox ID="txtNhapLai" runat="server" TextMode="Password" CssClass="form-input-pill" MaxLength="128" autocomplete="new-password" data-validation="confirm-password" data-match="MainContent_txtMatKhau" aria-describedby="MainContent_lblNhapLaiError"></asp:TextBox>
            <asp:Label ID="lblNhapLaiError" runat="server" CssClass="field-error input-validation-message" AssociatedControlID="txtNhapLai" />
        </div>
        </div>

        <asp:Button ID="btnDangKy" runat="server" Text="Tạo tài khoản" CssClass="btn-pink-submit" OnClick="btnDangKy_Click" />

        <div class="auth-message"><asp:Literal ID="litThongBao" runat="server" /></div>

        <p class="auth-footer-link">
            Đã có tài khoản? <a href="DangNhap.aspx">Đăng nhập</a>
        </p>
    </asp:Panel>
    </section>

</asp:Content>
