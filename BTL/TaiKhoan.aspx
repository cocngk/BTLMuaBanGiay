<%@ Page Title="Tài khoản" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TaiKhoan.aspx.cs" Inherits="ShoeStore.TaiKhoan" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Tài khoản của tôi</h1>

    <asp:Literal ID="litThongBao" runat="server" />

    <div class="form" style="max-width: 560px;">
        <p class="nhan">
            Tên đăng nhập: <b><asp:Literal ID="litTenDangNhap" runat="server" /></b>
        </p>

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

        <asp:Button ID="btnLuu" runat="server" Text="Lưu thay đổi" CssClass="nut" OnClick="btnLuu_Click" />
    </div>

    <h2>Đổi mật khẩu</h2>

    <div class="form" style="max-width: 560px;">
        <div class="dong">
            <label for="<%= txtMatKhauCu.ClientID %>">Mật khẩu hiện tại</label>
            <asp:TextBox ID="txtMatKhauCu" runat="server" TextMode="Password" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtMatKhauMoi.ClientID %>">Mật khẩu mới</label>
            <asp:TextBox ID="txtMatKhauMoi" runat="server" TextMode="Password" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtNhapLai.ClientID %>">Nhập lại mật khẩu mới</label>
            <asp:TextBox ID="txtNhapLai" runat="server" TextMode="Password" CssClass="o-nhap"></asp:TextBox>
        </div>

        <asp:Button ID="btnDoiMatKhau" runat="server" Text="Đổi mật khẩu" CssClass="nut" OnClick="btnDoiMatKhau_Click" />
        <asp:Literal ID="litThongBaoMatKhau" runat="server" />
    </div>

    <h2>Đơn hàng của tôi</h2>

    <table class="bang">
        <tr>
            <th>Mã đơn</th>
            <th>Ngày đặt</th>
            <th>Tổng tiền</th>
        </tr>
        <asp:Repeater ID="rptDonHang" runat="server">
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Code") %></td>
                    <td><%# Eval("CreatedAt", "{0:dd/MM/yyyy HH:mm}") %></td>
                    <td><%# Eval("Total", "{0:N0} đ") %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

    <asp:Literal ID="litChuaCoDon" runat="server" />

</asp:Content>
