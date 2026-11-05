<%@ Page Title="Giỏ hàng" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GioHang.aspx.cs" Inherits="ShoeStore.GioHang" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Giỏ hàng</h1>

    <asp:Literal ID="litThongBao" runat="server" />

    <asp:Panel ID="pnlGio" runat="server">

        <div class="dong-gio" style="font-size: 13px; font-weight: 600; color: #6b6b70;">
            <div></div>
            <div>Sản phẩm</div>
            <div>Size</div>
            <div>Giá</div>
            <div>Số lượng</div>
            <div>Thành tiền</div>
            <div></div>
        </div>

        <asp:Repeater ID="rptGio" runat="server" OnItemCommand="rptGio_ItemCommand">
            <ItemTemplate>
                <div class="dong-gio">
                    <img src='<%# Eval("Anh") %>' alt='<%# Eval("Ten") %>' />
                    <div><%# Eval("Ten") %></div>
                    <div><%# Eval("Size") %></div>
                    <div><%# Eval("Gia", "{0:N0} đ") %></div>
                    <div>
                        <asp:TextBox ID="txtSoLuong" runat="server" Text='<%# Eval("SoLuong") %>' CssClass="o-so"></asp:TextBox>
                        <asp:LinkButton ID="btnLuu" runat="server" Text="Lưu" CommandName="Sua"
                            CommandArgument='<%# Convert.ToString(Eval("MaSanPham")) + "|" + Convert.ToString(Eval("Size")) %>'
                            CssClass="nut-link"></asp:LinkButton>
                    </div>
                    <div><%# Eval("ThanhTien", "{0:N0} đ") %></div>
                    <div>
                        <asp:LinkButton ID="btnXoa" runat="server" Text="Xóa" CommandName="Xoa"
                            CommandArgument='<%# Convert.ToString(Eval("MaSanPham")) + "|" + Convert.ToString(Eval("Size")) %>'
                            CssClass="nut-link" OnClientClick="return confirm('Xóa sản phẩm này khỏi giỏ?');"></asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="tong">
            <span>Tổng cộng</span>
            <span><asp:Literal ID="litTong" runat="server" /></span>
        </div>

        <p style="margin-top: 20px;">
            <a class="nut phu" href="SanPham.aspx">Tiếp tục mua</a>
            <asp:Button ID="btnDatHang" runat="server" Text="Đặt hàng" CssClass="nut" OnClick="btnDatHang_Click" />
        </p>

    </asp:Panel>

    <asp:Panel ID="pnlTrong" runat="server" Visible="false">
        <div class="trong">
            <p>Giỏ hàng đang trống.</p>
            <a class="nut" href="SanPham.aspx">Chọn giày ngay</a>
        </div>
    </asp:Panel>

</asp:Content>
