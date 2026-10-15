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
        <asp:UpdatePanel ID="updCart" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
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
                                <tr class="cart-row" data-price='<%# Convert.ToDecimal(Eval("Price")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'>
                                    <td>
                                        <asp:HiddenField ID="hfKey" runat="server" Value='<%# Eval("Id") + "|" + Eval("Size") %>' />
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
                                        <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Eval("Quantity") %>' CssClass="qty-input js-qty" TextMode="Number" min="1"></asp:TextBox>
                                    </td>
                                    <td class="item-price line-total"><%# string.Format("{0:N0}đ", Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Quantity"))) %></td>
                                    <td>
                                        <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("Id") + "|" + Eval("Size") %>' CssClass="btn-delete">
                                            <i class="fa-solid fa-trash-can"></i>
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

                <div style="margin-top: 20px; display: flex; justify-content: space-between; align-items: center;">
                    <a href="SanPham.aspx" class="btn-continue" style="background:var(--muted);">&larr; Tiếp tục chọn hàng</a>
                    <span style="font-size:13px;color:var(--muted);">Số lượng thay đổi sẽ tự cập nhật tiền</span>
                    <asp:Button ID="btnUpdateCart" runat="server" OnClick="btnUpdateCart_Click" Style="display:none;" />
                </div>
            </div>

            <div class="checkout-card">
                <h3 class="summary-title">Tổng quan giỏ hàng</h3>

                <div class="summary-row">
                    <span>Tạm tính:</span>
                    <span id="subTotalValue"><asp:Literal ID="litSubTotal" runat="server">0đ</asp:Literal></span>
                </div>
                <div class="summary-row">
                    <span>Phí vận chuyển:</span>
                    <span style="color:#28a745;">Miễn phí</span>
                </div>
                <div class="summary-row total">
                    <span>Tổng tiền:</span>
                    <span id="totalValue"><asp:Literal ID="litTotal" runat="server">0đ</asp:Literal></span>
                </div>

                <!-- CHUYỂN THÀNH OnClick Server-Side ĐỂ KIỂM TRA ĐĂNG NHẬP -->
                <asp:Button ID="btnGoToCheckout" runat="server" Text="TIẾN HÀNH THANH TOÁN &rarr;" OnClick="btnGoToCheckout_Click" CssClass="btn-checkout" />
            </div>
        </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <script type="text/javascript">
        function bindCartAuto() {
            function fmt(n) {
                return n.toLocaleString('vi-VN') + 'đ';
            }
            function recalc() {
                var total = 0;
                document.querySelectorAll('.cart-row').forEach(function (row) {
                    var price = parseFloat(row.getAttribute('data-price')) || 0;
                    var input = row.querySelector('.js-qty');
                    var qty = parseInt(input ? input.value : '1', 10);
                    if (isNaN(qty) || qty < 1) qty = 1;
                    total += price * qty;
                    var cell = row.querySelector('.line-total');
                    if (cell) cell.textContent = fmt(price * qty);
                });
                var sub = document.getElementById('subTotalValue');
                var tot = document.getElementById('totalValue');
                if (sub) sub.textContent = fmt(total);
                if (tot) tot.textContent = fmt(total);
            }
            var timer = null;
            function scheduleSave() {
                var btnId = '<%= btnUpdateCart.ClientID %>';
                var btn = document.getElementById(btnId);
                if (!btn) return;
                clearTimeout(timer);
                timer = setTimeout(function () { btn.click(); }, 800);
            }
            document.querySelectorAll('.js-qty').forEach(function (input) {
                input.addEventListener('input', function () {
                    if (input.value !== '' && parseInt(input.value, 10) < 1) input.value = '1';
                    recalc();
                    scheduleSave();
                });
                input.addEventListener('change', function () {
                    if (input.value === '' || parseInt(input.value, 10) < 1) input.value = '1';
                    recalc();
                    scheduleSave();
                });
            });
            recalc();
        }
        bindCartAuto();
        // UpdatePanel render lại DOM sau mỗi async postback -> gắn lại listener
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () { bindCartAuto(); });
        }
    </script>
</asp:Content>