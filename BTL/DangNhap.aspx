<%@ Page Title="Đăng Nhập" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangNhap.aspx.cs" Inherits="ShoeStore.DangNhap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .auth-page-container {
            max-width: 440px;
            margin: 60px auto;
            padding: 0 20px;
        }

        .auth-card-page {
            background: #ffffff;
            border-radius: 25px;
            box-shadow: 0 10px 30px rgba(225, 112, 133, 0.12);
            padding: 40px 32px;
            border: 1px solid #fce8ec;
        }

        .auth-header { text-align: center; margin-bottom: 25px; }

        .auth-badge-icon {
            background: #fde8ed;
            color: #e63946;
            width: 52px; height: 52px;
            border-radius: 16px;
            display: flex; align-items: center; justify-content: center;
            margin: 0 auto 12px;
            font-size: 24px;
        }

        .auth-title { font-size: 24px; font-weight: 800; color: #e63946; margin-bottom: 4px; }
        .auth-subtitle { font-size: 13px; color: #888; }

        .form-group-item { margin-bottom: 16px; text-align: left; }
        .form-label-custom { display: block; font-size: 13px; font-weight: 700; color: #444; margin-bottom: 6px; }

        .form-input-pill {
            width: 100%;
            padding: 12px 16px;
            border: 1px solid #f8d7da;
            border-radius: 15px;
            font-size: 13px;
            outline: none;
            background: #fafafa;
        }

        .form-input-pill:focus { border-color: #e63946; background: #fff; }

        .chk-remember-flex { display: flex; align-items: center; gap: 8px; font-size: 13px; color: #555; margin-bottom: 20px; }

        .btn-pink-submit {
            width: 100%;
            background: #e63946;
            color: #ffffff;
            border: none;
            padding: 13px;
            border-radius: 15px;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 4px 12px rgba(230, 57, 70, 0.25);
            transition: background 0.3s;
        }

        .btn-pink-submit:hover { background: #c1121f; }

        .auth-footer-link { text-align: center; margin-top: 22px; font-size: 13px; color: #666; }
        .auth-footer-link a { color: #e63946; font-weight: 700; text-decoration: none; }
    </style>

    <div class="auth-page-container">
        <div class="auth-card-page">
            <div class="auth-header">
                <div class="auth-badge-icon"><i class="fa-solid fa-shoe-prints"></i></div>
                <h2 class="auth-title">Đăng Nhập</h2>
                <p class="auth-subtitle">Chào mừng bạn quay lại với SneakerX!</p>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Tên đăng nhập / Email *</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input-pill" Placeholder="Nhập tên đăng nhập hoặc email"></asp:TextBox>
            </div>

            <div class="form-group-item">
                <label class="form-label-custom">Mật khẩu *</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input-pill" TextMode="Password" Placeholder="Nhập mật khẩu"></asp:TextBox>
            </div>

            <div class="chk-remember-flex">
                <asp:CheckBox ID="chkRemember" runat="server" />
                <label for="<%= chkRemember.ClientID %>">Ghi nhớ mật khẩu</label>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Đăng Nhập" CssClass="btn-pink-submit" OnClick="btnLogin_Click" />

            <div class="auth-footer-link">
                Chưa có tài khoản? <a href="DangKy.aspx">Đăng ký ngay</a>
            </div>
        </div>
    </div>
</asp:Content>