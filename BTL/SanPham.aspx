<%@ Page Title="Sản Phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SanPham.aspx.cs" Inherits="ShoeStore.SanPham" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .shop-page-layout {
            display: grid;
            grid-template-columns: 260px 1fr;
            gap: 30px;
            max-width: 1200px;
            margin: 30px auto;
            padding: 0 20px;
        }

        .filter-sidebar {
            background: #ffffff;
            border-radius: 20px;
            padding: 25px;
            border: 1px solid #fce8ec;
            height: fit-content;
            box-shadow: 0 4px 15px rgba(0,0,0,0.02);
        }

        .filter-group {
            margin-bottom: 22px;
            padding-bottom: 18px;
            border-bottom: 1px solid #f8d7da;
        }

        .filter-group:last-child {
            border-bottom: none;
            margin-bottom: 0;
            padding-bottom: 0;
        }

        .filter-title {
            font-size: 15px;
            font-weight: 800;
            color: #111;
            margin-bottom: 12px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .filter-title i { color: #e63946; }

        .custom-filter-list td {
            padding: 5px 0;
            font-size: 13px;
            color: #555;
        }

        .custom-filter-list label {
            margin-left: 8px;
            cursor: pointer;
        }

        .size-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 8px;
        }

        .btn-size-chip {
            border: 1px solid #f8d7da;
            background: #fff;
            padding: 8px;
            border-radius: 10px;
            font-size: 13px;
            font-weight: 700;
            text-align: center;
            cursor: pointer;
            color: #444;
            transition: all 0.2s;
            user-select: none;
        }

        .btn-size-chip:hover {
            border-color: #e63946;
            color: #e63946;
        }

        .btn-size-chip.active {
            background: #e63946 !important;
            color: #ffffff !important;
            border-color: #e63946 !important;
            box-shadow: 0 3px 8px rgba(230, 57, 70, 0.3);
        }

        .btn-apply-filter {
            width: 100%;
            background: #e63946;
            color: white;
            border: none;
            padding: 11px;
            border-radius: 15px;
            font-weight: 700;
            cursor: pointer;
            margin-top: 10px;
            transition: background 0.3s;
        }

        .btn-apply-filter:hover { background: #c1121f; }

        .btn-reset-filter {
            width: 100%;
            background: #fdf2f4;
            color: #e63946;
            border: 1px solid #f8d7da;
            padding: 8px;
            border-radius: 15px;
            font-weight: 700;
            cursor: pointer;
            margin-top: 8px;
            font-size: 12px;
        }

        .shop-toolbar {
            background: #ffffff;
            border-radius: 20px;
            padding: 15px 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border: 1px solid #fce8ec;
            margin-bottom: 25px;
        }

        .search-inner {
            display: flex;
            align-items: center;
            background: #fafafa;
            border: 1px solid #f8d7da;
            border-radius: 20px;
            padding: 4px 10px 4px 15px;
            width: 280px;
        }

        .search-inner input {
            border: none;
            background: transparent;
            outline: none;
            font-size: 13px;
            width: 100%;
        }

        .btn-inner-search {
            background: #e63946;
            color: white;
            border: none;
            width: 30px; height: 30px;
            border-radius: 50%;
            cursor: pointer;
            display: flex; align-items: center; justify-content: center;
        }

        .sort-select {
            padding: 8px 15px;
            border-radius: 15px;
            border: 1px solid #f8d7da;
            outline: none;
            font-size: 13px;
            color: #444;
            background: #fff;
            cursor: pointer;
        }

        .products-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
            gap: 20px;
        }

        .product-card-custom {
            background: #ffffff;
            border-radius: 20px;
            padding: 12px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.03);
            position: relative;
            display: flex;
            flex-direction: column;
            border: 1px solid #fce8ec;
            transition: transform 0.3s;
        }

        .product-card-custom:hover { transform: translateY(-5px); }

        .badge-hot-pink {
            position: absolute;
            top: 20px; left: 20px;
            background: #fde8ed;
            color: #e63946;
            font-size: 11px;
            font-weight: 700;
            padding: 4px 12px;
            border-radius: 15px;
            z-index: 2;
        }

        .btn-heart-wishlist {
            position: absolute;
            top: 20px; right: 20px;
            background: #ffffff;
            border: none;
            width: 32px; height: 32px;
            border-radius: 50%;
            display: flex; align-items: center; justify-content: center;
            color: #e63946;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            cursor: pointer;
            z-index: 2;
        }

        .product-thumb {
            width: 100%; height: 190px;
            object-fit: cover;
            border-radius: 15px;
            background: #f9f9f9;
        }

        .product-details {
            padding: 12px 5px 5px;
            display: flex;
            flex-direction: column;
            flex-grow: 1;
        }

        .product-title-bold { font-size: 15px; font-weight: 800; color: #222; margin-bottom: 4px; }
        .product-desc-muted { font-size: 12px; color: #888; margin-bottom: 12px; }

        .product-price-bold {
            font-size: 16px;
            font-weight: 800;
            color: #e63946;
            margin-bottom: 10px;
        }

        .product-action-buttons {
            display: flex;
            gap: 8px;
            margin-top: auto;
        }

        .btn-action-cart {
            flex: 1;
            background: #fde8ed;
            color: #e63946;
            border: 1px solid #f8c2cc;
            padding: 8px 10px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 700;
            cursor: pointer;
            text-decoration: none;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 5px;
            transition: all 0.2s;
        }

        .btn-action-cart:hover { background: #f8c2cc; }

        .btn-action-buy {
            flex: 1;
            background: #e63946;
            color: white;
            border: 1px solid #e63946;
            padding: 8px 10px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 700;
            cursor: pointer;
            text-decoration: none;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 5px;
            transition: all 0.2s;
            box-shadow: 0 3px 8px rgba(230, 57, 70, 0.25);
        }

        .btn-action-buy:hover { background: #c1121f; }

        .pagination-container {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 8px;
            margin-top: 35px;
            margin-bottom: 15px;
        }

        .page-link-custom {
            padding: 8px 16px;
            border-radius: 12px;
            background: #ffffff;
            border: 1px solid #f8d7da;
            color: #555;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
            transition: all 0.2s;
            cursor: pointer;
        }

        .page-link-custom:hover, .page-link-custom.active {
            background: #e63946;
            color: white;
            border-color: #e63946;
            box-shadow: 0 3px 10px rgba(230, 57, 70, 0.25);
        }

        .page-link-custom.disabled {
            background: #f8f9fa;
            color: #ccc;
            border-color: #eee;
            cursor: not-allowed;
            pointer-events: none;
        }

        .no-results {
            grid-column: 1 / -1;
            text-align: center;
            padding: 60px 20px;
            background: #fff;
            border-radius: 20px;
            color: #777;
        }
    </style>

    <div class="shop-page-layout">
        <!-- SIDEBAR BỘ LỌC -->
        <aside class="filter-sidebar">
            <div class="filter-group">
                <div class="filter-title"><i class="fa-solid fa-filter"></i> Thương hiệu</div>
                <asp:CheckBoxList ID="cblBrands" runat="server" CssClass="custom-filter-list">
                    <asp:ListItem Value="Nike">Nike</asp:ListItem>
                    <asp:ListItem Value="Adidas">Adidas</asp:ListItem>
                    <asp:ListItem Value="Jordan">Jordan</asp:ListItem>
                    <asp:ListItem Value="Puma">Puma</asp:ListItem>
                    <asp:ListItem Value="Converse">Converse</asp:ListItem>
                    <asp:ListItem Value="Vans">Vans</asp:ListItem>
                </asp:CheckBoxList>
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

            <asp:Button ID="btnApplyFilter" runat="server" Text="Áp Dụng Bộ Lọc" CssClass="btn-apply-filter" OnClick="btnFilter_Click" />
            <asp:Button ID="btnResetFilter" runat="server" Text="Xóa bộ lọc" CssClass="btn-reset-filter" OnClick="btnResetFilter_Click" />
        </aside>

        <!-- KHU VỰC HIỂN THỊ SẢN PHẨM -->
        <div>
            <div class="shop-toolbar">
                <div class="search-inner">
                    <asp:TextBox ID="txtStoreSearch" runat="server" Placeholder="Tìm kiếm trong cửa hàng..."></asp:TextBox>
                    <asp:LinkButton ID="btnStoreSearch" runat="server" CssClass="btn-inner-search" OnClick="btnFilter_Click">
                        <i class="fa-solid fa-magnifying-glass"></i>
                    </asp:LinkButton>
                </div>
                <div>
                    <span style="font-size:13px; color:#777; margin-right:10px;">Sắp xếp:</span>
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
                        <div class="product-card-custom">
                            <asp:Panel ID="pnlHot" runat="server" CssClass="badge-hot-pink" Visible='<%# Convert.ToBoolean(Eval("IsHot")) %>'>
                                Nổi bật
                            </asp:Panel>

                            <button type="button" class="btn-heart-wishlist"><i class="fa-solid fa-heart"></i></button>

                            <a href='ChiTietSanPham.aspx?id=<%# Eval("Id") %>'>
                                <img src='<%# Eval("ImageUrl") %>' class="product-thumb" alt="Product" />
                            </a>

                            <div class="product-details">
                                <a href='ChiTietSanPham.aspx?id=<%# Eval("Id") %>' style="text-decoration:none;">
                                    <h3 class="product-title-bold"><%# Eval("Name") %></h3>
                                </a>
                                <p class="product-desc-muted"><%# Eval("Description") %></p>
                                <span class="product-price-bold"><%# string.Format("{0:N0}đ", Eval("Price")) %></span>

                                <div class="product-action-buttons">
                                    <asp:LinkButton ID="btnAddToCart" runat="server" CommandName="AddToCart" CommandArgument='<%# Eval("Id") %>' CssClass="btn-action-cart" title="Thêm vào giỏ">
                                        <i class="fa-solid fa-cart-plus"></i> Thêm giỏ
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnBuyNow" runat="server" CommandName="BuyNow" CommandArgument='<%# Eval("Id") %>' CssClass="btn-action-buy" title="Thanh toán ngay">
                                        <i class="fa-solid fa-credit-card"></i> Mua ngay
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="no-results">
                    <i class="fa-solid fa-box-open" style="font-size: 40px; color: #f8d7da; margin-bottom: 10px;"></i>
                    <p style="font-size: 15px; font-weight: 700;">Không tìm thấy sản phẩm phù hợp!</p>
                    <p style="font-size: 13px;">Vui lòng thử điều chỉnh lại bộ lọc hoặc từ khóa tìm kiếm.</p>
                </asp:Panel>
            </div>

            <!-- THANH PHÂN TRANG -->
            <div class="pagination-container" id="pnlPagination" runat="server">
                <asp:LinkButton ID="btnPrevPage" runat="server" CssClass="page-link-custom" OnClick="btnPrevPage_Click">&laquo; Trước</asp:LinkButton>
                <asp:Repeater ID="rptPages" runat="server" OnItemCommand="rptPages_ItemCommand">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnPage" runat="server" CommandName="Page" CommandArgument='<%# Container.DataItem %>' CssClass='<%# Convert.ToInt32(Container.DataItem) == CurrentPage ? "page-link-custom active" : "page-link-custom" %>'>
                            <%# Container.DataItem %>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:LinkButton ID="btnNextPage" runat="server" CssClass="page-link-custom" OnClick="btnNextPage_Click">Sau &raquo;</asp:LinkButton>
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