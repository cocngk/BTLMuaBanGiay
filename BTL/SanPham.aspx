<%@ Register Src="~/Controls/ProductCard.ascx" TagPrefix="uc" TagName="ProductCard" %>
<%@ Page Title="Sản Phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SanPham.aspx.cs" Inherits="ShoeStore.SanPham" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/product-card.css" />
    <link rel="stylesheet" href="Content/sanpham.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="shop-page-layout">
        <!-- SIDEBAR BỘ LỌC -->
        <aside class="filter-sidebar">
            <div class="filter-group">
                <div class="filter-title"><i class="fa-solid fa-filter"></i> Thương hiệu</div>
                <asp:CheckBoxList ID="cblBrands" runat="server" CssClass="custom-filter-list" />
            </div>

            <div class="filter-group">
                <div class="filter-title"><i class="fa-solid fa-tag"></i> Khoảng giá</div>
                <asp:RadioButtonList ID="rblPriceRange" runat="server" CssClass="custom-filter-list">
                    <asp:ListItem Value="All" Selected="True">Tất cả mức giá</asp:ListItem>
                    <asp:ListItem Value="Under2M">Dưới 2.000.000đ</asp:ListItem>
                    <asp:ListItem Value="2Mto4M">2.000.000đ - 4.000.000đ</asp:ListItem>
                    <asp:ListItem Value="Above4M">Trên 4.000.000đ</asp:ListItem>
                </asp:RadioButtonList>
            </div>

            <!-- CỤM CHỌN SIZE ĐÃ SỬA LỖI -->
            <div class="filter-group">
                <div class="filter-title"><i class="fa-solid fa-shoe-prints"></i> Kích cỡ (Size)</div>
                <asp:HiddenField ID="hfSelectedSize" runat="server" Value="" />
                <div class="size-grid">
                    <div class="btn-size-chip" data-size="38" onclick="clickSelectSize(this, '38')">38</div>
                    <div class="btn-size-chip" data-size="39" onclick="clickSelectSize(this, '39')">39</div>
                    <div class="btn-size-chip" data-size="40" onclick="clickSelectSize(this, '40')">40</div>
                    <div class="btn-size-chip" data-size="41" onclick="clickSelectSize(this, '41')">41</div>
                    <div class="btn-size-chip" data-size="42" onclick="clickSelectSize(this, '42')">42</div>
                    <div class="btn-size-chip" data-size="43" onclick="clickSelectSize(this, '43')">43</div>
                </div>
            </div>

            <asp:Button ID="btnApplyFilter" runat="server" Text="Áp dụng bộ lọc" CssClass="btn-apply-filter" OnClick="btnFilter_Click" />
            <a href="SanPham.aspx" class="btn-reset-filter">Xóa bộ lọc</a>
        </aside>

        <!-- KHU VỰC HIỂN THỊ SẢN PHẨM -->
        <div>
            <div class="shop-toolbar">
                <div class="search-inner">
                    <asp:Panel ID="pnlStoreSearch" runat="server" DefaultButton="btnStoreSearch" Style="display:contents;">
                    <asp:TextBox ID="txtStoreSearch" runat="server" Placeholder="Tìm kiếm trong cửa hàng..."></asp:TextBox>
                    <asp:LinkButton ID="btnStoreSearch" runat="server" CssClass="btn-inner-search" OnClientClick="return headerSearchSubmit(this);" OnClick="btnFilter_Click">
                        <i class="fa-solid fa-magnifying-glass"></i>
                    </asp:LinkButton>
                    </asp:Panel>
                </div>
                <div>
                    <span style="font-size:13px; color:var(--muted); margin-right:10px;">Sắp xếp:</span>
                    <asp:DropDownList ID="ddlSort" runat="server" CssClass="sort-select" AutoPostBack="true" OnSelectedIndexChanged="btnFilter_Click">
                        <asp:ListItem Value="Default">Mặc định</asp:ListItem>
                        <asp:ListItem Value="PriceAsc">Giá: Thấp đến Cao</asp:ListItem>
                        <asp:ListItem Value="PriceDesc">Giá: Cao đến Thấp</asp:ListItem>
                        <asp:ListItem Value="NameAsc">Tên: A - Z</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="products-grid">
                <asp:Repeater ID="rptAllProducts" runat="server" OnItemCommand="rptAllProducts_ItemCommand">
                    <ItemTemplate>
                        <uc:ProductCard runat="server" ActionMode="Full"
                            ProductId='<%# Eval("Id") %>'
                            ProductName='<%# Eval("Name") %>'
                            Description='<%# Eval("Description") %>'
                            Price='<%# Eval("Price") %>'
                            ImageUrl='<%# Eval("ImageUrl") %>'
                            IsHot='<%# Eval("IsHot") %>' />
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="no-results">
                    <i class="fa-solid fa-box-open" style="font-size: 40px; color: var(--border-strong); margin-bottom: 10px;"></i>
                    <p style="font-size: 15px; font-weight: 700;">Không tìm thấy sản phẩm phù hợp!</p>
                    <p style="font-size: 13px;">Vui lòng thử điều chỉnh lại bộ lọc hoặc từ khóa tìm kiếm.</p>
                </asp:Panel>
            </div>

            <!-- THANH PHÂN TRANG -->
            <div class="pagination-container" id="pnlPagination" runat="server">
                <asp:HyperLink ID="lnkPrevPage" runat="server" CssClass="page-link-custom">&laquo; Trước</asp:HyperLink>
                <asp:Repeater ID="rptPages" runat="server">
                    <ItemTemplate>
                        <a href='<%# Eval("Url") %>' class='<%# Eval("Css") %>'><%# Eval("Number") %></a>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HyperLink ID="lnkNextPage" runat="server" CssClass="page-link-custom">Sau &raquo;</asp:HyperLink>
            </div>
        </div>
    </div>

    <!-- SCRIPT FIX LỖI TỰ ĐỘNG LƯU TRẠNG THÁI SIZE SANG DỮ LIỆU C# -->
    <script>
        function clickSelectSize(element, sizeValue) {
            var hf = document.getElementById('<%= hfSelectedSize.ClientID %>');
            var chips = document.querySelectorAll('.btn-size-chip');

            if (element.classList.contains('active')) {
                element.classList.remove('active');
                hf.value = "";
            } else {
                chips.forEach(function (c) { c.classList.remove('active'); });
                element.classList.add('active');
                hf.value = sizeValue;
            }
        }

        // Giữ trạng thái hiển thị màu đỏ của ô Size sau khi Reload/PostBack trang
        document.addEventListener("DOMContentLoaded", function () {
            var currentSize = document.getElementById('<%= hfSelectedSize.ClientID %>').value;
            if (currentSize) {
                var targetChip = document.querySelector('.btn-size-chip[data-size="' + currentSize + '"]');
                if (targetChip) {
                    targetChip.classList.add('active');
                }
            }
        });
    </script>
</asp:Content>