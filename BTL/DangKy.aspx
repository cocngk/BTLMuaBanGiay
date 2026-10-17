<%@ Page Title="Đăng Ký Tài Khoản" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangKy.aspx.cs" Inherits="ShoeStore.DangKy" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/dangky.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="auth-page-container">
        <div class="auth-card-page">
            <a href="TrangChu.aspx" class="auth-back-link"><i class="fa-solid fa-arrow-left"></i> Trang chủ</a>
            <asp:Panel ID="pnlRegisterForm" runat="server" DefaultButton="btnRegister">
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

            <asp:Panel ID="pnlFormError" runat="server" Visible="false" CssClass="form-error-banner">
                <i class="fa-solid fa-circle-exclamation"></i>
                <asp:Literal ID="litFormError" runat="server" />
            </asp:Panel>

            <script type="text/javascript">
                // Live-check trùng tài khoản khi gõ (server vẫn chốt lúc submit)
                (function () {
                    var userInput = document.getElementById('<%= txtUsername.ClientID %>');
                    var emailInput = document.getElementById('<%= txtEmail.ClientID %>');
                    var timer = null;
                    function checkLive() {
                        var box = document.getElementById('<%= pnlFormError.ClientID %>');
                        if (!box) return;
                        var u = userInput ? userInput.value.trim() : '';
                        var m = emailInput ? emailInput.value.trim() : '';
                        if (u.length < 3 && m === '') { box.style.display = 'none'; return; }
                        if (typeof PageMethods === 'undefined') return;
                        PageMethods.CheckDuplicate(u, m, function (res) {
                            var msgs = [];
                            if (res && res.userTaken) msgs.push('Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.');
                            if (res && res.emailTaken) msgs.push('Email này đã được đăng ký!');
                            if (msgs.length > 0) {
                                var icon = box.querySelector('i');
                                box.textContent = msgs.join(' ');
                                if (icon) box.insertBefore(icon, box.firstChild);
                                box.style.display = 'flex';
                            } else { box.style.display = 'none'; }
                        });
                    }
                    function schedule() { clearTimeout(timer); timer = setTimeout(checkLive, 500); }
                    if (userInput) userInput.addEventListener('input', schedule);
                    if (emailInput) emailInput.addEventListener('input', schedule);
                })();
            </script>

            <asp:Button ID="btnRegister" runat="server" Text="Đăng Ký Tài Khoản" CssClass="btn-pink-submit" OnClick="btnRegister_Click" />

            <div class="auth-footer-link">
                Đã có tài khoản? <a href="DangNhap.aspx">Đăng nhập ngay</a>
            </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>