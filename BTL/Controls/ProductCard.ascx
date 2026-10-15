<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCard.ascx.cs" Inherits="ShoeStore.ProductCard" %>
<%-- Thẻ sản phẩm dùng chung (TrangChu / SanPham / Related). Nút bấm bubble Command về Repeater cha. --%>
<div class="product-card-custom">
    <asp:Panel ID="pnlHot" runat="server" CssClass="badge-hot-pink" Visible="false">
        Nổi bật
    </asp:Panel>

    <button type="button" class="btn-heart-wishlist"><i class="fa-solid fa-heart"></i></button>

    <a id="lnkImage" runat="server" href="#">
        <asp:Image ID="imgThumb" runat="server" CssClass="product-thumb" />
    </a>

    <div class="product-details">
        <a id="lnkName" runat="server" href="#" style="text-decoration:none;">
            <h3 class="product-title-bold"><asp:Literal ID="litCardName" runat="server" Mode="Encode"></asp:Literal></h3>
        </a>
        <p class="product-desc-muted"><asp:Literal ID="litCardDesc" runat="server" Mode="Encode"></asp:Literal></p>
        <div class="product-footer-flex">
            <span class="product-price-bold"><asp:Literal ID="litCardPrice" runat="server"></asp:Literal></span>
            <asp:LinkButton ID="btnQuickAdd" runat="server" CommandName="QuickAdd" CssClass="btn-add-plus-circle" Visible="false" title="Thêm vào giỏ hàng">
                <i class="fa-solid fa-plus"></i>
            </asp:LinkButton>
        </div>
        <asp:Panel ID="pnlFullActions" runat="server" CssClass="product-action-buttons" Visible="false">
            <asp:LinkButton ID="btnAddToCart" runat="server" CommandName="AddToCart" CssClass="btn-action-cart" title="Thêm vào giỏ">
                <i class="fa-solid fa-cart-plus"></i> Thêm giỏ
            </asp:LinkButton>
            <asp:LinkButton ID="btnBuyNow" runat="server" CommandName="BuyNow" CssClass="btn-action-buy" title="Thanh toán ngay">
                <i class="fa-solid fa-credit-card"></i> Mua ngay
            </asp:LinkButton>
        </asp:Panel>
    </div>
</div>
