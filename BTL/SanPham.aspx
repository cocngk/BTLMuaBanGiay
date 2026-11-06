<%@ Page Title="Sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SanPham.aspx.cs" Inherits="ShoeStore.SanPham" %>
<%@ Import Namespace="System.Web" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/css/sanpham.css?v=mobile-audit-1" />
</asp:Content>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">
    <section class="shop-page-layout">
        <aside class="filter-sidebar" aria-label="Bộ lọc sản phẩm">
            <details class="filter-disclosure" open>
            <summary class="filter-heading"><span>☰</span> Bộ lọc</summary>
            <div class="filter-group">
                <h2 class="filter-title">Thương hiệu</h2>
                <asp:CheckBoxList ID="cblBrands" runat="server" CssClass="custom-filter-list" />
            </div>
            <div class="filter-group">
                <h2 class="filter-title">Khoảng giá</h2>
                <asp:RadioButtonList ID="rblPriceRange" runat="server" CssClass="custom-filter-list">
                    <asp:ListItem Value="All" Selected="True">Tất cả mức giá</asp:ListItem>
                    <asp:ListItem Value="Under2M">Dưới 2.000.000 đ</asp:ListItem>
                    <asp:ListItem Value="2Mto4M">2.000.000 – 4.000.000 đ</asp:ListItem>
                    <asp:ListItem Value="Above4M">Trên 4.000.000 đ</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div class="filter-group">
                <h2 class="filter-title">Kích cỡ (Size)</h2>
                <asp:HiddenField ID="hfSelectedSize" runat="server" />
                <div class="size-grid">
                    <button type="button" class="btn-size-chip" data-size="38">38</button>
                    <button type="button" class="btn-size-chip" data-size="39">39</button>
                    <button type="button" class="btn-size-chip" data-size="40">40</button>
                    <button type="button" class="btn-size-chip" data-size="41">41</button>
                    <button type="button" class="btn-size-chip" data-size="42">42</button>
                    <button type="button" class="btn-size-chip" data-size="43">43</button>
                </div>
            </div>
            <asp:Button ID="btnApplyFilter" runat="server" Text="Áp dụng bộ lọc" CssClass="btn-apply-filter" OnClick="btnApplyFilter_Click" />
            <a href="SanPham.aspx" class="btn-reset-filter">Xóa bộ lọc</a>
            </details>
        </aside>

        <div class="shop-results">
            <div class="shop-intro">
                <div><p class="shop-eyebrow">SNEAKERX · CATALOG</p><h1>Sản phẩm</h1></div>
                <span class="result-count"><asp:Literal ID="litKetQua" runat="server" /></span>
            </div>
            <div class="shop-toolbar">
                <asp:Panel ID="pnlStoreSearch" runat="server" DefaultButton="btnStoreSearch" CssClass="search-inner">
                    <asp:TextBox ID="txtStoreSearch" runat="server" placeholder="Tìm kiếm trong cửa hàng..." aria-label="Tìm kiếm trong cửa hàng" />
                    <asp:Button ID="btnStoreSearch" runat="server" Text="⌕" CssClass="btn-inner-search" OnClick="btnApplyFilter_Click" ToolTip="Tìm kiếm" />
                </asp:Panel>
                <label class="sort-label" for="<%= ddlSort.ClientID %>">Sắp xếp
                    <asp:DropDownList ID="ddlSort" runat="server" CssClass="sort-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
                        <asp:ListItem Value="Default">Mặc định</asp:ListItem>
                        <asp:ListItem Value="PriceAsc">Giá: thấp đến cao</asp:ListItem>
                        <asp:ListItem Value="PriceDesc">Giá: cao đến thấp</asp:ListItem>
                        <asp:ListItem Value="NameAsc">Tên: A – Z</asp:ListItem>
                    </asp:DropDownList>
                </label>
            </div>

            <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="no-results">
                <div class="empty-icon">⌕</div><h2>Không tìm thấy sản phẩm phù hợp</h2>
                <p>Hãy thử điều chỉnh bộ lọc hoặc từ khóa tìm kiếm.</p>
                <a class="btn-reset-filter" href="SanPham.aspx">Xem tất cả sản phẩm</a>
            </asp:Panel>

            <div class="products-grid">
                <asp:Literal ID="litActionMessage" runat="server" />
                <asp:Repeater ID="rptSanPham" runat="server" OnItemCommand="rptSanPham_ItemCommand">
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
                                <div class="product-meta"><span>Size <%#: Eval("Sizes") %></span><span class='<%# Convert.ToInt32(Eval("Stock")) > 0 ? "stock-available" : "stock-out" %>'><%# Convert.ToInt32(Eval("Stock")) > 0 ? "Còn hàng" : "Hết hàng" %></span></div>
                                <div class="product-footer-flex">
                                    <span class="product-price-bold"><%#: Eval("Price", "{0:N0} đ") %></span>
                                </div>
                                <div class="product-action-buttons">
                                    <asp:LinkButton ID="btnAddToCart" runat="server" CssClass="btn-action-cart" CommandName="AddToCart" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Thêm giỏ</asp:LinkButton>
                                    <asp:LinkButton ID="btnBuyNow" runat="server" CssClass="btn-action-buy" CommandName="BuyNow" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Mua ngay</asp:LinkButton>
                                </div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlPagination" runat="server" CssClass="pagination-container">
                <asp:HyperLink ID="lnkPrevPage" runat="server" CssClass="page-link-custom">‹ Trước</asp:HyperLink>
                <asp:Repeater ID="rptPages" runat="server">
                    <ItemTemplate><a href='<%#: Eval("Url") %>' class='<%#: Eval("Css") %>'><%#: Eval("Number") %></a></ItemTemplate>
                </asp:Repeater>
                <asp:HyperLink ID="lnkNextPage" runat="server" CssClass="page-link-custom">Sau ›</asp:HyperLink>
            </asp:Panel>
        </div>
    </section>
    <script>
        (function () {
            var filterDisclosure = document.querySelector('.filter-disclosure');
            if (filterDisclosure && window.matchMedia('(max-width: 680px)').matches) {
                filterDisclosure.open = false;
            }
            var field = document.getElementById('<%= hfSelectedSize.ClientID %>');
            var chips = document.querySelectorAll('.btn-size-chip');
            chips.forEach(function (chip) {
                if (chip.getAttribute('data-size') === field.value) chip.classList.add('active');
                chip.addEventListener('click', function () {
                    var selected = chip.getAttribute('data-size');
                    field.value = field.value === selected ? '' : selected;
                    chips.forEach(function (item) {
                        item.classList.toggle('active', item.getAttribute('data-size') === field.value);
                    });
                });
            });
        }());
    </script>
</asp:Content>
