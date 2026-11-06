<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrangChu.aspx.cs" Inherits="ShoeStore.TrangChu" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/css/sanpham.css" />
    <link rel="stylesheet" href="Content/css/trangchu.css" />
</asp:Content>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">
    <section class="home-hero">
        <div class="home-hero-copy">
            <h1>Giày chính hãng, chọn là vừa</h1>
            <p class="home-hero-description">Nike, Adidas, Converse, Vans, New Balance - sẵn size, giao trong ngày.</p>
            <a class="home-hero-cta" href="SanPham.aspx">Xem tất cả sản phẩm <span aria-hidden="true">→</span></a>
        </div>
    </section>

    <section class="section-container" id="san-pham">
        <div class="section-header-flex">
            <div>
                <p class="home-section-kicker">LỰA CHỌN ĐƯỢC YÊU THÍCH</p>
                <h2 class="section-title-text">Sản phẩm nổi bật <span aria-hidden="true">♥</span></h2>
                <p class="section-subtitle-text">Những đôi giày thể thao tuyệt vời được yêu thích nhất tại SneakerX.</p>
            </div>
            <a href="SanPham.aspx" class="btn-view-all-pill">Xem tất cả <span aria-hidden="true">→</span></a>
        </div>

        <asp:Literal ID="litThongBao" runat="server" />
        <div class="products-grid home-products-grid">
            <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
                <ItemTemplate>
                    <article class="product-card-custom">
                        <a class="product-image-link" href='ChiTietSanPham.aspx?id=<%#: Eval("Id") %>' aria-label='Xem <%#: Eval("Name") %>'>
                            <img class="product-thumb" src='<%#: ResolveUrl(Convert.ToString(Eval("ImageUrl"))) %>' alt='<%#: Eval("Name") %>' loading="lazy" />
                        </a>
                        <%# Convert.ToBoolean(Eval("IsHot")) ? "<span class=\"badge-hot-pink\">Bán chạy</span>" : "" %>
                        <asp:LinkButton ID="btnHeart" runat="server" CssClass='<%# IsProductLiked(Convert.ToInt32(Eval("Id"))) ? "btn-heart-wishlist active" : "btn-heart-wishlist" %>' CommandName="Wishlist" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" Text='<%# IsProductLiked(Convert.ToInt32(Eval("Id"))) ? "♥" : "♡" %>' ToolTip="Yêu thích" />
                        <div class="product-details">
                            <span class="product-brand"><%#: Eval("Brand") %></span>
                            <a class="product-title-bold" href='ChiTietSanPham.aspx?id=<%#: Eval("Id") %>'><%#: Eval("Name") %></a>
                            <p class="product-desc-muted"><%#: Eval("Description") %></p>
                            <div class="home-product-footer">
                                <span class="product-price-bold"><%#: Eval("Price", "{0:N0} đ") %></span>
                                <span class='<%# Convert.ToInt32(Eval("Stock")) > 0 ? "stock-available" : "stock-out" %>'><%# Convert.ToInt32(Eval("Stock")) > 0 ? "Còn hàng" : "Hết hàng" %></span>
                            </div>
                            <asp:LinkButton ID="btnQuickAdd" runat="server" CssClass="home-quick-add" CommandName="QuickAdd" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" Enabled='<%# Convert.ToInt32(Eval("Stock")) > 0 %>'>
                                <span aria-hidden="true">＋</span> Thêm nhanh vào giỏ
                            </asp:LinkButton>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>
</asp:Content>
