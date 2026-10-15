<%@ Page Title="Đăng Nhập" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangNhap.aspx.cs" Inherits="ShoeStore.DangNhap" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/dangnhap.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="auth-page-container">
        <div class="auth-card-page">
            <a href="TrangChu.aspx" class="auth-back-link"><i class="fa-solid fa-arrow-left"></i> Trang chủ</a>
            <div class="auth-header">
                <div class="auth-badge-icon"><i class="fa-solid fa-shoe-prints"></i></div>
                <h2 class="auth-title">Đăng Nhập</h2>
                <p class="auth-subtitle">Chào mừng bạn quay lại với SneakerX!</p>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Tên đăng nhập / Email *</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input-pill" Placeholder="Nhập tên đăng nhập hoặc email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqUsername" runat="server" ControlToValidate="txtUsername"
                    ErrorMessage="Vui lòng nhập tên đăng nhập hoặc email." CssClass="field-error" Display="Dynamic" />
                <asp:CustomValidator ID="cvUsername" runat="server" ControlToValidate="txtUsername"
                    ClientValidationFunction="validateUsernameLive"
                    OnServerValidate="cvUsername_ServerValidate" CssClass="field-error" Display="Dynamic" />
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Mật khẩu *</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input-pill" TextMode="Password" Placeholder="Nhập mật khẩu"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Vui lòng nhập mật khẩu." CssClass="field-error" Display="Dynamic" />
            </div>

            <div class="chk-remember-flex">
                <asp:CheckBox ID="chkRemember" runat="server" />
                <label for="<%= chkRemember.ClientID %>">Ghi nhớ mật khẩu</label>
            </div>

            <asp:CustomValidator ID="cvLoginSummary" runat="server" CssClass="field-error" Display="Dynamic"
                OnServerValidate="cvLoginSummary_ServerValidate" />

            <asp:Button ID="btnLogin" runat="server" Text="Đăng Nhập" CssClass="btn-pink-submit" OnClick="btnLogin_Click" />

            <div class="auth-footer-link">
                Chưa có tài khoản? <a href="DangKy.aspx">Đăng ký ngay</a>
            </div>
        </div>
    </div>
</asp:Content>