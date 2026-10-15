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

            <div class="form-row-2col">
                <div class="form-group-item">
                    <label class="form-label-custom">Họ và tên *</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-input-pill" Placeholder="Nhập họ và tên"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqFullName" runat="server" ControlToValidate="txtFullName"
                        ErrorMessage="Vui lòng nhập họ và tên." CssClass="field-error" Display="Dynamic" />
                </div>

                <div class="form-group-item">
                    <label class="form-label-custom">Tên đăng nhập *</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input-pill" Placeholder="Nhập tên đăng nhập"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqUsername" runat="server" ControlToValidate="txtUsername"
                        ErrorMessage="Vui lòng nhập tên đăng nhập." CssClass="field-error" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revUsername" runat="server" ControlToValidate="txtUsername"
                        ValidationExpression="^.{3,}$" ErrorMessage="Tên đăng nhập phải có ít nhất 3 ký tự."
                        CssClass="field-error" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row-2col">
                <div class="form-group-item">
                    <label class="form-label-custom">Địa chỉ Email *</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input-pill" TextMode="Email" Placeholder="example@domain.com"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail"
                        ErrorMessage="Vui lòng nhập địa chỉ email." CssClass="field-error" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ErrorMessage="Địa chỉ email không hợp lệ."
                        CssClass="field-error" Display="Dynamic" />
                </div>

                <div class="form-group-item">
                    <label class="form-label-custom">Số điện thoại *</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-input-pill" Placeholder="Nhập số điện thoại (10 số)"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqPhone" runat="server" ControlToValidate="txtPhone"
                        ErrorMessage="Vui lòng nhập số điện thoại." CssClass="field-error" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revPhone" runat="server" ControlToValidate="txtPhone"
                        ValidationExpression="^[0-9]{9,11}$" ErrorMessage="Số điện thoại phải có từ 9 đến 11 chữ số."
                        CssClass="field-error" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row-2col">
                <div class="form-group-item">
                    <label class="form-label-custom">Mật khẩu *</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input-pill" TextMode="Password" Placeholder="Nhập mật khẩu"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="txtPassword"
                        ErrorMessage="Vui lòng nhập mật khẩu." CssClass="field-error" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword"
                        ValidationExpression="^.{6,}$" ErrorMessage="Mật khẩu phải có ít nhất 6 ký tự."
                        CssClass="field-error" Display="Dynamic" />
                </div>

                <div class="form-group-item">
                    <label class="form-label-custom">Xác nhận mật khẩu *</label>
                    <asp:TextBox ID="txtConfirm" runat="server" CssClass="form-input-pill" TextMode="Password" Placeholder="Nhập lại mật khẩu"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqConfirm" runat="server" ControlToValidate="txtConfirm"
                        ErrorMessage="Vui lòng xác nhận mật khẩu." CssClass="field-error" Display="Dynamic" />
                    <asp:CompareValidator ID="cmpConfirm" runat="server" ControlToValidate="txtConfirm"
                        ControlToCompare="txtPassword" ErrorMessage="Mật khẩu xác nhận không trùng khớp."
                        CssClass="field-error" Display="Dynamic" />
                </div>
            </div>

            <asp:CustomValidator ID="cvServerCheck" runat="server" CssClass="field-error" Display="Dynamic"
                OnServerValidate="cvServerCheck_ServerValidate" />

            <asp:Button ID="btnRegister" runat="server" Text="Đăng Ký Tài Khoản" CssClass="btn-pink-submit" OnClick="btnRegister_Click" />

            <div class="auth-footer-link">
                Đã có tài khoản? <a href="DangNhap.aspx">Đăng nhập ngay</a>
            </div>
        </div>
    </div>
</asp:Content>