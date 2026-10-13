<%@ Page Title="Thanh Toán" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ThanhToan.aspx.cs" Inherits="ShoeStore.ThanhToan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .checkout-container {
            max-width: 1100px;
            margin: 30px auto;
            padding: 0 20px;
        }

        .checkout-title {
            font-size: 26px;
            font-weight: 800;
            color: #e63946;
            margin-bottom: 25px;
        }

        .checkout-grid {
            display: grid;
            grid-template-columns: 1.2fr 1fr;
            gap: 30px;
        }

        @media (max-width: 850px) {
            .checkout-grid { grid-template-columns: 1fr; }
        }

        .card-box {
            background: #ffffff;
            border-radius: 20px;
            padding: 25px;
            border: 1px solid #fce8ec;
            box-shadow: 0 4px 15px rgba(0,0,0,0.03);
            margin-bottom: 20px;
        }

        .card-title {
            font-size: 16px;
            font-weight: 800;
            color: #111;
            margin-bottom: 18px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .card-title i { color: #e63946; }

        .form-group { margin-bottom: 15px; }

        .form-label {
            display: block;
            font-size: 13px;
            font-weight: 700;
            color: #444;
            margin-bottom: 5px;
        }

        .form-control-pill {
            width: 100%;
            padding: 11px 16px;
            border: 1px solid #f8d7da;
            border-radius: 12px;
            font-size: 13px;
            outline: none;
            background: #fafafa;
        }

        .form-control-pill:focus { border-color: #e63946; background: #fff; }

        .order-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 0;
            border-bottom: 1px dashed #fce8ec;
        }

        .item-info { display: flex; align-items: center; gap: 12px; }

        .item-img { width: 50px; height: 50px; border-radius: 10px; object-fit: cover; }

        .item-name { font-size: 13px; font-weight: 700; color: #222; }

        .item-qty { font-size: 12px; color: #777; }

        .item-price { font-size: 13px; font-weight: 700; color: #e63946; }

        .total-row {
            display: flex;
            justify-content: space-between;
            margin-top: 12px;
            font-size: 14px;
            color: #555;
        }

        .total-row.final {
            border-top: 2px solid #fce8ec;
            padding-top: 15px;
            margin-top: 15px;
            font-size: 18px;
            font-weight: 800;
            color: #e63946;
        }

        .btn-confirm-checkout {
            width: 100%;
            background: #e63946;
            color: white;
            border: none;
            padding: 14px;
            border-radius: 15px;
            font-weight: 700;
            font-size: 15px;
            cursor: pointer;
            margin-top: 20px;
            box-shadow: 0 4px 15px rgba(230, 57, 70, 0.3);
            transition: background 0.3s;
        }

        .btn-confirm-checkout:hover { background: #c1121f; }

        .payment-methods td { padding: 8px 0; font-size: 13px; }
        .payment-methods label { margin-left: 8px; cursor: pointer; }
    </style>

    <div class="checkout-container">
        <h1 class="checkout-title"><i class="fa-solid fa-credit-card"></i> Thanh Toán Đơn Hàng</h1>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
            <div class="card-box" style="text-align: center; padding: 50px 20px;">
                <i class="fa-solid fa-bag-shopping" style="font-size: 40px; color: #f8d7da; margin-bottom: 15px;"></i>
                <p style="font-size: 15px; font-weight: 700;">Chưa có sản phẩm nào để thanh toán!</p>
                <p style="font-size: 13px; color: #777; margin-bottom: 20px;">Vui lòng chọn sản phẩm vào giỏ hàng trước khi thực hiện thanh toán.</p>
                <a href="SanPham.aspx" style="background:#e63946; color:white; padding:10px 25px; border-radius:20px; text-decoration:none; font-weight:700;">Xem sản phẩm &rarr;</a>
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