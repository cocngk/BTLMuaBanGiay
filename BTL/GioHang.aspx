<%@ Page Title="Giỏ Hàng" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GioHang.aspx.cs" Inherits="ShoeStore.GioHang" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/giohang.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

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
                    <a href="SanPham.aspx" class="btn-continue" style="background:var(--muted);">&larr; Tiếp tục chọn hàng</a>
                    <asp:Button ID="btnUpdateCart" runat="server" Text="Cập nhật giỏ hàng" OnClick="btnUpdateCart_Click" Style="background:var(--heading); color:white; border:none; padding:10px 20px; border-radius:20px; cursor:pointer;" />
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