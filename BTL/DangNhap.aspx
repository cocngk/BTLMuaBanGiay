<%@ Page Title="Đăng nhập" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangNhap.aspx.cs" Inherits="ShoeStore.DangNhap" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Đăng nhập</h1>

    <div class="form">
        <div class="dong">
            <label for="<%= txtTenDangNhap.ClientID %>">Tên đăng nhập</label>
            <asp:TextBox ID="txtTenDangNhap" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtMatKhau.ClientID %>">Mật khẩu</label>
            <asp:TextBox ID="txtMatKhau" runat="server" TextMode="Password" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <asp:CheckBox ID="chkNho" runat="server" Text=" Nhớ đăng nhập trong 7 ngày" />
        </div>

        <asp:Button ID="btnDangNhap" runat="server" Text="Đăng nhập" CssClass="nut" OnClick="btnDangNhap_Click" />

        <asp:Literal ID="litThongBao" runat="server" />

        <p class="nhan" style="margin-top: 18px;">
            Chưa có tài khoản? <a href="DangKy.aspx">Đăng ký</a>
        </p>

        <p class="nhan">
            Tài khoản mẫu: <b>admin / admin123</b> (quản trị) và <b>khach / khach123</b> (khách).
        </p>
    </div>

</asp:Content>
