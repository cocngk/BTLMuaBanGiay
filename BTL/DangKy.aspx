<%@ Page Title="Đăng Ký Tài Khoản" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangKy.aspx.cs" Inherits="ShoeStore.DangKy" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/dangky.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="auth-page-container">
        <div class="auth-card-page">
            <div class="auth-header">
                <div class="auth-badge-icon"><i class="fa-solid fa-user-plus"></i></div>
                <h2 class="auth-title">Đăng Ký Tài Khoản</h2>
                <p class="auth-subtitle">Trở thành thành viên của SneakerX ngay hôm nay!</p>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Họ và tên *</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-input-pill" Placeholder="Nhập họ và tên"></asp:TextBox>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Tên đăng nhập *</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input-pill" Placeholder="Nhập tên đăng nhập"></asp:TextBox>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Địa chỉ Email *</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input-pill" TextMode="Email" Placeholder="example@domain.com"></asp:TextBox>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Số điện thoại *</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-input-pill" Placeholder="Nhập số điện thoại (10 số)"></asp:TextBox>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Mật khẩu *</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input-pill" TextMode="Password" Placeholder="Nhập mật khẩu"></asp:TextBox>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Xác nhận mật khẩu *</label>
                <asp:TextBox ID="txtConfirm" runat="server" CssClass="form-input-pill" TextMode="Password" Placeholder="Nhập lại mật khẩu"></asp:TextBox>
            </div>

            <asp:Button ID="btnRegister" runat="server" Text="Đăng Ký Tài Khoản" CssClass="btn-pink-submit" OnClick="btnRegister_Click" />

            <div class="auth-footer-link">
                Đã có tài khoản? <a href="DangNhap.aspx">Đăng nhập ngay</a>
            </div>
        </div>
    </div>
</asp:Content>