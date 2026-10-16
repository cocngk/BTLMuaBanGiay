<%@ Page Title="Đăng Nhập" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangNhap.aspx.cs" Inherits="ShoeStore.DangNhap" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/dangnhap.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="auth-page-container">
        <div class="auth-card-page">
            <a href="TrangChu.aspx" class="auth-back-link"><i class="fa-solid fa-arrow-left"></i> Trang chủ</a>
            <asp:Panel ID="pnlLoginForm" runat="server" DefaultButton="btnLogin">
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
            </asp:Panel>
        </div>
    </div>

    <script type="text/javascript">
        // Live-check định dạng username bằng JS thường (không qua framework);
        // server check lại làm chuẩn khi submit.
        (function () {
            var u = document.getElementById('<%= txtUsername.ClientID %>');
            var cv = document.getElementById('<%= cvUsername.ClientID %>');
            var btn = document.getElementById('<%= btnLogin.ClientID %>');
            function check() {
                if (!u || !cv) return true;
                var v = (u.value || '').trim();
                var msg = '';
                if (v !== '') {
                    if (v.indexOf('@') >= 0) {
                        if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(v)) msg = 'Địa chỉ email không hợp lệ.';
                    } else if (v.length < 3) {
                        msg = 'Tên đăng nhập phải có ít nhất 3 ký tự.';
                    }
                }
                if (msg !== '') {
                    cv.textContent = msg;
                    cv.style.display = 'block';
                    cv.dataset.own = '1';
                    return false;
                }
                if (cv.dataset.own === '1') {
                    cv.style.display = 'none';
                    cv.dataset.own = '';
                }
                return true;
            }
            if (u) {
                u.addEventListener('input', check);
                u.addEventListener('blur', check);
            }
            if (btn) {
                btn.addEventListener('click', function (ev) {
                    if (!check() && ev.preventDefault) ev.preventDefault();
                });
            }
        })();
    </script>
</asp:Content>