<%@ Page Title="Đơn hàng của tôi" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DonHang.aspx.cs" Inherits="ShoeStore.DonHang" %>
<%@ Import Namespace="ShoeStore.Helpers" %>
<%@ Import Namespace="ShoeStore.Models" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="description" content="Xem lại đơn hàng đã đặt tại SneakerX hoặc tra cứu đơn bằng mã đơn." />
    <link rel="stylesheet" href="Content/donhang.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="orders-shell">

        <div class="orders-header">
            <h1 class="orders-title"><i class="fa-solid fa-receipt"></i> Đơn hàng của tôi</h1>
            <p class="orders-subtitle">
                Đơn đã đặt được lưu lại kể cả khi bạn đóng trình duyệt.
                Có mã đơn trong tay mà chưa đăng nhập? Nhập mã vào ô tra cứu bên dưới.
            </p>
        </div>

        <!-- ================= TRA CỨU THEO MÃ ĐƠN ================= -->
        <div class="orders-lookup">
            <label class="orders-lookup-label" for="<%= txtCode.ClientID %>">
                <i class="fa-solid fa-magnifying-glass"></i> Tra cứu theo mã đơn
            </label>
            <div class="orders-lookup-row">
                <asp:TextBox ID="txtCode" runat="server" CssClass="orders-input" placeholder="VD: SX-261022-4821" />
                <asp:Button ID="btnLookup" runat="server" CssClass="orders-btn" Text="Tra cứu"
                    OnClick="btnLookup_Click" CausesValidation="false" />
            </div>
        </div>

        <!-- ================= KẾT QUẢ TRA CỨU ================= -->
        <asp:Panel ID="pnlLookup" runat="server" Visible="false" CssClass="order-card order-card-lookup">
            <div class="order-card-head">
                <div>
                    <p class="order-card-eyebrow">Kết quả tra cứu</p>
                    <p class="order-code"><asp:Literal ID="litLookupCode" runat="server" Mode="Encode"></asp:Literal></p>
                </div>
                <span class="order-status"><asp:Literal ID="litLookupStatus" runat="server" Mode="Encode"></asp:Literal></span>
            </div>

            <p class="order-meta">
                Đặt lúc <asp:Literal ID="litLookupDate" runat="server" Mode="Encode"></asp:Literal>
            </p>

            <ul class="order-lines">
                <asp:Repeater ID="rptLookupLines" runat="server">
                    <ItemTemplate>
                        <li class="order-line">
                            <span class="order-line-name">
                                <%#: Eval("Name") %>
                                <span class="order-line-size">Size <%#: Eval("Size") %></span>
                            </span>
                            <span class="order-line-qty">x<%# Eval("Quantity") %></span>
                            <span class="order-line-price"><%# Money.Format(Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Quantity"))) %></span>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>

            <p class="order-total">
                Tổng cộng:
                <strong><asp:Literal ID="litLookupTotal" runat="server" Mode="Encode"></asp:Literal></strong>
            </p>
        </asp:Panel>

        <!-- ================= CHƯA ĐĂNG NHẬP ================= -->
        <asp:Panel ID="pnlLoginHint" runat="server" Visible="false" CssClass="orders-empty">
            <i class="fa-solid fa-user-lock orders-empty-icon"></i>
            <p class="orders-empty-title">Bạn chưa đăng nhập</p>
            <p class="orders-empty-text">
                <a href="DangNhap.aspx">Đăng nhập</a> để xem toàn bộ đơn đã đặt,
                hoặc tra cứu bằng mã đơn ở trên.
            </p>
        </asp:Panel>

        <!-- ================= CHƯA CÓ ĐƠN NÀO ================= -->
        <asp:Panel ID="pnlNoOrders" runat="server" Visible="false" CssClass="orders-empty">
            <i class="fa-solid fa-box-open orders-empty-icon"></i>
            <p class="orders-empty-title">Chưa có đơn hàng nào</p>
            <p class="orders-empty-text">
                Bạn chưa đặt đơn nào. <a href="SanPham.aspx">Xem sản phẩm</a> để bắt đầu.
            </p>
        </asp:Panel>

        <!-- ================= DANH SÁCH ĐƠN CỦA TÔI ================= -->
        <asp:Panel ID="pnlMyOrders" runat="server" Visible="false">
            <h2 class="orders-section-title">
                Đơn đã đặt (<asp:Literal ID="litOrderCount" runat="server"></asp:Literal>)
            </h2>

            <asp:Repeater ID="rptOrders" runat="server">
                <ItemTemplate>
                    <div class="order-card">
                        <div class="order-card-head">
                            <div>
                                <p class="order-card-eyebrow">Mã đơn</p>
                                <p class="order-code"><%#: Eval("Code") %></p>
                            </div>
                            <span class="order-status"><%#: Eval("Status") %></span>
                        </div>

                        <p class="order-meta">
                            Đặt lúc <%# Eval("CreatedAt", "{0:dd/MM/yyyy HH:mm}") %>
                            &middot; <%# ((Order)Container.DataItem).TotalQuantity() %> sản phẩm
                        </p>

                        <ul class="order-lines">
                            <asp:Repeater ID="rptLines" runat="server" DataSource='<%# Eval("Lines") %>'>
                                <ItemTemplate>
                                    <li class="order-line">
                                        <span class="order-line-name">
                                            <%#: Eval("Name") %>
                                            <span class="order-line-size">Size <%#: Eval("Size") %></span>
                                        </span>
                                        <span class="order-line-qty">x<%# Eval("Quantity") %></span>
                                        <span class="order-line-price"><%# Money.Format(Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Quantity"))) %></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>

                        <p class="order-total">
                            Tổng cộng:
                            <strong><%# Money.Format(((Order)Container.DataItem).Total()) %></strong>
                        </p>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

    </div>

</asp:Content>
