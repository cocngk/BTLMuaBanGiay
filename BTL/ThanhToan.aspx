<%@ Page Title="Thanh Toán" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ThanhToan.aspx.cs" Inherits="ShoeStore.ThanhToan" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/thanhtoan.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="checkout-container">
        <h1 class="checkout-title"><i class="fa-solid fa-credit-card"></i> Thanh Toán Đơn Hàng</h1>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
            <div class="card-box" style="text-align: center; padding: 50px 20px;">
                <i class="fa-solid fa-bag-shopping" style="font-size: 40px; color: var(--border-strong); margin-bottom: 15px;"></i>
                <p style="font-size: 15px; font-weight: 700;">Chưa có sản phẩm nào để thanh toán!</p>
                <p style="font-size: 13px; color: var(--muted); margin-bottom: 20px;">Vui lòng chọn sản phẩm vào giỏ hàng trước khi thực hiện thanh toán.</p>
                <a href="SanPham.aspx" style="background:var(--brand); color:white; padding:10px 25px; border-radius:20px; text-decoration:none; font-weight:700;">Xem sản phẩm &rarr;</a>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCheckout" runat="server">
            <div class="checkout-grid">
                <div>
                    <div class="card-box">
                        <h3 class="card-title"><i class="fa-solid fa-location-dot"></i> Thông tin giao hàng</h3>
                        
                        <div class="form-group">
                            <label class="form-label">Họ và tên người nhận</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control-pill" Placeholder="Nhập họ và tên"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Số điện thoại liên hệ</label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control-pill" Placeholder="Nhập số điện thoại"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Địa chỉ nhận hàng chi tiết</label>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control-pill" TextMode="MultiLine" Rows="2" Placeholder="Số nhà, tên đường, phường/xã, quận/huyện..."></asp:TextBox>
                        </div>
                    </div>

                    <div class="card-box">
                        <h3 class="card-title"><i class="fa-solid fa-wallet"></i> Phương thức thanh toán</h3>
                        <asp:RadioButtonList ID="rblPaymentMethod" runat="server" CssClass="payment-methods">
                            <asp:ListItem Value="COD" Selected="True">Thanh toán khi nhận hàng (COD)</asp:ListItem>
                            <asp:ListItem Value="QR">Chuyển khoản Ngân hàng / Mã QR VietQR</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>

                <div>
                    <div class="card-box">
                        <h3 class="card-title"><i class="fa-solid fa-receipt"></i> Đơn hàng của bạn</h3>

                        <asp:Repeater ID="rptSummary" runat="server">
                            <ItemTemplate>
                                <div class="order-item">
                                    <div class="item-info">
                                        <img src='<%# Eval("ImageUrl") %>' class="item-img" />
                                        <div>
                                            <div class="item-name"><%# Eval("Name") %></div>
                                            <div class="item-qty">Size: <%# Eval("Size") %> | SL: <%# Eval("Quantity") %></div>
                                        </div>
                                    </div>
                                    <div class="item-price"><%# string.Format("{0:N0}đ", Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Quantity"))) %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <div class="total-row">
                            <span>Tạm tính:</span>
                            <span><asp:Literal ID="litSubTotal" runat="server">0đ</asp:Literal></span>
                        </div>
                        <div class="total-row">
                            <span>Phí vận chuyển:</span>
                            <span style="color:#28a745; font-weight:700;">Miễn phí</span>
                        </div>
                        <div class="total-row final">
                            <span>TỔNG CỘNG:</span>
                            <span><asp:Literal ID="litTotal" runat="server">0đ</asp:Literal></span>
                        </div>

                        <asp:Button ID="btnConfirmCheckout" runat="server" Text="HOÀN TẤT ĐẶT HÀNG" CssClass="btn-confirm-checkout" OnClick="btnConfirmCheckout_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>