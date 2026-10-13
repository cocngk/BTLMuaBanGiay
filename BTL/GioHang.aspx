<%@ Page Title="Giỏ Hàng" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GioHang.aspx.cs" Inherits="ShoeStore.GioHang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .cart-page-title {
            font-size: 28px;
            font-weight: 800;
            color: #111;
            margin-bottom: 8px;
        }

        .cart-empty-box {
            background: #ffffff;
            border-radius: 15px;
            padding: 50px 20px;
            text-align: center;
            box-shadow: 0 4px 15px rgba(0,0,0,0.05);
            margin-top: 20px;
        }

        .cart-empty-icon {
            font-size: 50px;
            color: #ced4da;
            margin-bottom: 15px;
        }

        .cart-empty-text {
            font-size: 16px;
            color: #6c757d;
            margin-bottom: 20px;
        }

        .btn-continue {
            display: inline-block;
            background-color: #e63946;
            color: #ffffff;
            padding: 10px 25px;
            border-radius: 20px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.3s;
        }

        .btn-continue:hover {
            background-color: #c1121f;
        }

        .cart-layout {
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 30px;
            margin-top: 20px;
        }

        @media (max-width: 900px) {
            .cart-layout {
                grid-template-columns: 1fr;
            }
        }

        .cart-table-card, .checkout-card {
            background: #ffffff;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.05);
        }

        .cart-table {
            width: 100%;
            border-collapse: collapse;
        }

        .cart-table th {
            text-align: left;
            padding-bottom: 15px;
            border-bottom: 2px solid #f1f3f5;
            color: #495057;
            font-size: 14px;
        }

        .cart-table td {
            padding: 15px 0;
            border-bottom: 1px solid #f1f3f5;
            vertical-align: middle;
        }

        .item-info {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .item-img {
            width: 70px;
            height: 70px;
            object-fit: cover;
            border-radius: 10px;
            background: #f8f9fa;
        }

        .item-name {
            font-weight: 700;
            font-size: 15px;
            color: #212529;
        }

        .item-size {
            font-size: 12px;
            color: #6c757d;
        }

        .item-price {
            font-weight: 700;
            color: #e63946;
        }

        .qty-input {
            width: 60px;
            padding: 6px;
            text-align: center;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            font-weight: bold;
        }

        .btn-delete {
            background: none;
            border: none;
            color: #adb5bd;
            cursor: pointer;
            font-size: 16px;
        }

        .btn-delete:hover {
            color: #e63946;
        }

        .summary-title {
            font-size: 18px;
            font-weight: 800;
            margin-bottom: 20px;
            color: #111;
        }

        .summary-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 12px;
            font-size: 14px;
            color: #495057;
        }

        .summary-row.total {
            border-top: 2px dashed #f1f3f5;
            padding-top: 15px;
            margin-top: 15px;
            font-size: 18px;
            font-weight: 800;
            color: #e63946;
        }

        .btn-checkout {
            width: 100%;
            background: #e63946;
            color: white;
            border: none;
            padding: 14px;
            border-radius: 10px;
            font-weight: 700;
            font-size: 15px;
            cursor: pointer;
            margin-top: 15px;
            transition: 0.3s;
            text-align: center;
            display: block;
        }

        .btn-checkout:hover {
            background: #c1121f;
        }
    </style>

    <h1 class="cart-page-title">Giỏ hàng của bạn</h1>

    <asp:Panel ID="pnlEmptyCart" runat="server">
        <div class="cart-empty-box">
            <div class="cart-empty-icon"><i class="fa-solid fa-bag-shopping"></i></div>
            <p class="cart-empty-text">Giỏ hàng của bạn đang trống.</p>
            <a href="SanPham.aspx" class="btn-continue">Tiếp tục chọn hàng</a>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlCartHasItems" runat="server">
        <div class="cart-layout">
            <div class="cart-table-card">
                <table class="cart-table">
                    <thead>
                        <tr>
                            <th>Sản phẩm</th>
                            <th>Đơn giá</th>
                            <th>Số lượng</th>
                            <th>Thành tiền</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptCart" runat="server" OnItemCommand="rptCart_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="item-info">
                                            <img src='<%# Eval("ImageUrl") %>' class="item-img" />
                                            <div>
                                                <div class="item-name"><%# Eval("Name") %></div>
                                                <div class="item-size">Size: <%# Eval("Size") %></div>
                                            </div>
                                        </div>
                                    </td>
                                    <td class="item-price"><%# string.Format("{0:N0}đ", Eval("Price")) %></td>
                                    <td>
                                        <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Eval("Quantity") %>' CssClass="qty-input" TextMode="Number" min="1" oninput="if(this.value<1)this.value=1;"></asp:TextBox>
                                    </td>
                                    <td class="item-price"><%# string.Format("{0:N0}đ", Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Quantity"))) %></td>
                                    <td>
                                        <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("Id") %>' CssClass="btn-delete">
                                            <i class="fa-solid fa-trash-can"></i>
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

                <div style="margin-top: 20px; display: flex; justify-content: space-between;">
                    <a href="SanPham.aspx" class="btn-continue" style="background:#6c757d;">&larr; Tiếp tục chọn hàng</a>
                    <asp:Button ID="btnUpdateCart" runat="server" Text="Cập nhật giỏ hàng" OnClick="btnUpdateCart_Click" Style="background:#111; color:white; border:none; padding:10px 20px; border-radius:20px; cursor:pointer;" />
                </div>
            </div>

            <div class="checkout-card">
                <h3 class="summary-title">Tổng quan giỏ hàng</h3>

                <div class="summary-row">
                    <span>Tạm tính:</span>
                    <span><asp:Literal ID="litSubTotal" runat="server">0đ</asp:Literal></span>
                </div>
                <div class="summary-row">
                    <span>Phí vận chuyển:</span>
                    <span style="color:#28a745;">Miễn phí</span>
                </div>
                <div class="summary-row total">
                    <span>Tổng tiền:</span>
                    <span><asp:Literal ID="litTotal" runat="server">0đ</asp:Literal></span>
                </div>

                <!-- CHUYỂN THÀNH OnClick Server-Side ĐỂ KIỂM TRA ĐĂNG NHẬP -->
                <asp:Button ID="btnGoToCheckout" runat="server" Text="TIẾN HÀNH THANH TOÁN &rarr;" OnClick="btnGoToCheckout_Click" CssClass="btn-checkout" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>