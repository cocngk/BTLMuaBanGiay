<%@ Page Title="Đặt hàng" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ThanhToan.aspx.cs" Inherits="ShoeStore.ThanhToan" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Đặt hàng</h1>

    <asp:Panel ID="pnlXong" runat="server" Visible="false">
        <div class="form" style="max-width: 620px;">
            <span class="thong-bao tot">Đặt hàng thành công!</span>
            <p>Mã đơn của bạn: <b><asp:Literal ID="litMaDon" runat="server" /></b></p>
            <p>Tổng tiền: <b><asp:Literal ID="litTongXong" runat="server" /></b></p>
            <p class="nhan">Chúng tôi sẽ liên hệ xác nhận đơn trong thời gian sớm nhất.</p>
            <p>
                <a class="nut phu" href="SanPham.aspx">Tiếp tục mua</a>
                <a class="nut" href="TaiKhoan.aspx">Xem đơn hàng</a>
            </p>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlTrong" runat="server" Visible="false">
        <div class="trong">
            <p>Giỏ hàng đang trống, chưa thể đặt hàng.</p>
            <a class="nut" href="SanPham.aspx">Chọn giày ngay</a>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlDatHang" runat="server">

        <h2>Đơn hàng của bạn</h2>

        <table class="bang">
            <tr>
                <th>Sản phẩm</th>
                <th>Size</th>
                <th>Giá</th>
                <th>Số lượng</th>
                <th>Thành tiền</th>
            </tr>
            <asp:Repeater ID="rptTomTat" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("Ten") %></td>
                        <td><%# Eval("Size") %></td>
                        <td><%# Eval("Gia", "{0:N0} đ") %></td>
                        <td><%# Eval("SoLuong") %></td>
                        <td><%# Eval("ThanhTien", "{0:N0} đ") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <p class="tong">
            <span>Tổng cộng</span>
            <span><asp:Literal ID="litTong" runat="server" /></span>
        </p>

        <h2>Thông tin nhận hàng</h2>

        <div class="form" style="max-width: 620px;">
            <div class="dong">
                <label for="<%= txtHoTen.ClientID %>">Họ tên người nhận</label>
                <asp:TextBox ID="txtHoTen" runat="server" CssClass="o-nhap"></asp:TextBox>
            </div>

            <div class="dong">
                <label for="<%= txtDienThoai.ClientID %>">Điện thoại</label>
                <asp:TextBox ID="txtDienThoai" runat="server" CssClass="o-nhap"></asp:TextBox>
            </div>

            <div class="dong">
                <label for="<%= txtDiaChi.ClientID %>">Địa chỉ nhận hàng</label>
                <asp:TextBox ID="txtDiaChi" runat="server" CssClass="o-nhap"></asp:TextBox>
            </div>

            <div class="dong">
                <label>Hình thức thanh toán</label>
                <asp:RadioButtonList ID="rblThanhToan" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="COD" Selected="True">Thanh toán khi nhận hàng</asp:ListItem>
                    <asp:ListItem Value="ChuyenKhoan">Chuyển khoản</asp:ListItem>
                </asp:RadioButtonList>
            </div>

            <asp:Button ID="btnDatHang" runat="server" Text="Xác nhận đặt hàng" CssClass="nut" OnClick="btnDatHang_Click" />
            <asp:Literal ID="litThongBao" runat="server" />
        </div>

    </asp:Panel>

</asp:Content>
