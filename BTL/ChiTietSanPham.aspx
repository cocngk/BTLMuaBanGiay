<%@ Register Src="~/Controls/ProductCard.ascx" TagPrefix="uc" TagName="ProductCard" %>
<%@ Page Title="Chi Tiết Sản Phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChiTietSanPham.aspx.cs" Inherits="ShoeStore.ChiTietSanPham" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/product-card.css" />
    <link rel="stylesheet" href="Content/chitietsanpham.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <div class="detail-container">
        <div class="breadcrumb-link">
            <a href="TrangChu.aspx">Trang chủ</a> / <a href="SanPham.aspx">Sản phẩm</a> / <span><asp:Literal ID="litBreadcrumb" runat="server" Mode="Encode" Text="Chi tiết sản phẩm"></asp:Literal></span>
        </div>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false" Style="text-align:center; padding:50px;">
            <h2>Sản phẩm không tồn tại!</h2>
            <a href="SanPham.aspx" class="notfound-back-link">&larr; Quay lại danh sách sản phẩm</a>
        </asp:Panel>

        <asp:Panel ID="pnlProductDetail" runat="server">
            <div class="product-detail-grid">
                <div>
                    <div class="zoom-wrap" id="zoomWrap">
                        <asp:Image ID="imgProduct" runat="server" CssClass="product-main-img" />
                    </div>
                    <asp:Panel ID="pnlSecondary" runat="server" Visible="false" CssClass="secondary-media">
                        <asp:Image ID="imgSecondary" runat="server" CssClass="product-secondary-img" />
                    </asp:Panel>
                    <p class="zoom-hint"><i class="fa-solid fa-magnifying-glass-plus"></i> Di chuột vào ảnh để phóng to</p>
                </div>

                <div class="product-info-box">
                    <span class="product-brand-tag"><asp:Literal ID="litBrand" runat="server" Mode="Encode"></asp:Literal></span>
                    <h1 class="product-title-large"><asp:Literal ID="litName" runat="server" Mode="Encode"></asp:Literal></h1>
                    <div class="detail-meta-row">
                        <span class="sku-text">Mã SP: <asp:Literal ID="litSku" runat="server" Mode="Encode"></asp:Literal></span>
                    </div>
                    <div class="price-row">
                        <span class="product-price-large"><asp:Literal ID="litPrice" runat="server" Mode="Encode"></asp:Literal></span>
                        <asp:Panel ID="pnlDiscount" runat="server" Visible="false" CssClass="discount-inline">
                            <span class="old-price"><asp:Literal ID="litOldPrice" runat="server" Mode="Encode"></asp:Literal></span>
                            <span class="discount-badge"><asp:Literal ID="litDiscountPct" runat="server" Mode="Encode"></asp:Literal></span>
                        </asp:Panel>
                    </div>
                    <div class="stock-row"><asp:Literal ID="litStock" runat="server"></asp:Literal></div>

                    <!-- CHỌN SIZE THEO DATA THẬT CỦA SẢN PHẨM -->
                    <div class="size-label-row">
                        <span class="section-label">Chọn Size giày:</span>
                        <a href="javascript:void(0)" class="size-guide-link" onclick="toggleSizeGuide()"><i class="fa-solid fa-ruler"></i> Bảng size</a>
                    </div>
                    <asp:HiddenField ID="hfSelectedSize" runat="server" />
                    <asp:HiddenField ID="hfMaxQty" runat="server" Value="10" />
                    <div class="size-selector">
                        <asp:Repeater ID="rptSizes" runat="server">
                            <ItemTemplate>
                                <div class='<%# Container.ItemIndex == 0 ? "size-option-btn active" : "size-option-btn" %>'
                                     data-size='<%# Eval("Size") %>'
                                     onclick="selectDetailSize(this, '<%# Eval("Size") %>')"><%# Eval("Size") %></div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div id="sizeGuideBox" class="size-guide-box" style="display:none;">
                        <table class="size-guide-table">
                            <tr><th>EU</th><th>US</th><th>Chiều dài chân (cm)</th></tr>
                            <tr><td>38</td><td>6</td><td>24.0</td></tr>
                            <tr><td>39</td><td>6.5</td><td>24.5</td></tr>
                            <tr><td>40</td><td>7</td><td>25.0</td></tr>
                            <tr><td>41</td><td>8</td><td>26.0</td></tr>
                            <tr><td>42</td><td>8.5</td><td>26.5</td></tr>
                            <tr><td>43</td><td>9.5</td><td>27.5</td></tr>
                        </table>
                    </div>

                    <!-- CHỌN SỐ LƯỢNG -->
                    <span class="section-label">Số lượng:</span>
                    <div class="qty-box-flex">
                        <button type="button" class="qty-step-btn" onclick="stepQty(-1)">−</button>
                        <asp:TextBox ID="txtQuantity" runat="server" Text="1" TextMode="Number" min="1" max='<%= hfMaxQty.Value %>' CssClass="qty-input-detail js-qty-detail"></asp:TextBox>
                        <button type="button" class="qty-step-btn" onclick="stepQty(1)">+</button>
                        <asp:Button ID="btnWishlist" runat="server" CssClass="btn-wishlist-detail" Text="&#9825;" ToolTip="Thêm vào yêu thích" OnClick="btnWishlist_Click" CausesValidation="false" />
                    </div>

                    <div class="detail-action-buttons">
                        <asp:Button ID="btnAddToCart" runat="server" Text="THÊM VÀO GIỎ HÀNG" CssClass="btn-detail-cart" OnClick="btnAddToCart_Click" />
                        <asp:Button ID="btnBuyNow" runat="server" Text="THANH TOÁN NGAY" CssClass="btn-detail-buy" OnClick="btnBuyNow_Click" />
                    </div>

                    <div class="ship-box">
                        <div class="ship-item"><i class="fa-solid fa-truck-fast"></i><span><b>Miễn phí ship</b> đơn từ 500K, giao 2–4 ngày</span></div>
                        <div class="ship-item"><i class="fa-solid fa-rotate-left"></i><span>Đổi size miễn phí trong <b>7 ngày</b></span></div>
                        <div class="ship-item"><i class="fa-solid fa-shield-halved"></i><span>Thanh toán khi nhận hàng (COD), bảo hành keo <b>6 tháng</b></span></div>
                    </div>
                </div>
            </div>

            <div class="product-desc-box">
                <h3 class="desc-title">Mô Tả Sản Phẩm Chi Tiết</h3>
                <p class="desc-content">
                    <asp:Literal ID="litDescription" runat="server" Mode="Encode"></asp:Literal>
                </p>
                <ul class="spec-list">
                    <li><b>Chất liệu:</b> Da tổng hợp cao cấp, lưới thoáng khí</li>
                    <li><b>Đế:</b> Cao su chống trơn trượt, độ bền cao</li>
                    <li><b>Phù hợp:</b> Đi học, đi chơi, chạy bộ nhẹ mỗi ngày</li>
                    <li><b>Bảo hành:</b> 6 tháng (bong keo, lỗi chỉ may)</li>
                </ul>
            </div>

            <div class="section-container">
                <div class="section-header-flex">
                    <div>
                        <h2 class="section-title-text">Sản phẩm liên quan</h2>
                        <p class="section-subtitle-text">Bạn cũng có thể sẽ thích những đôi này.</p>
                    </div>
                    <asp:HyperLink ID="lnkViewAllRelated" runat="server" CssClass="btn-view-all-pill" NavigateUrl="SanPham.aspx">Xem tất cả &rarr;</asp:HyperLink>
                </div>
                <div class="products-grid">
                    <asp:Repeater ID="rptRelated" runat="server">
                        <ItemTemplate>
                            <uc:ProductCard runat="server" ActionMode="None"
                                ProductId='<%# Eval("Id") %>'
                                ProductName='<%# Eval("Name") %>'
                                Description='<%# Eval("Description") %>'
                                Price='<%# Eval("Price") %>'
                                ImageUrl='<%# Eval("ImageUrl") %>'
                                IsHot='<%# Eval("IsHot") %>' />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script>
        function selectDetailSize(element, sizeVal) {
            var hf = document.getElementById('<%= hfSelectedSize.ClientID %>');
            var btns = document.querySelectorAll('.size-option-btn');
            btns.forEach(function (b) { b.classList.remove('active'); });
            element.classList.add('active');
            hf.value = sizeVal;
        }

        function maxBuyable() {
            var max = parseInt(document.getElementById('<%= hfMaxQty.ClientID %>').value, 10);
            return isNaN(max) || max < 1 ? 1 : max;
        }
        // Nguồn sự thật vẫn là server (ResolveQty giữ luật Cart.ClampQty); ở đây chỉ
        // chặn sớm để người dùng không gõ ra con số vô lý rồi mới bị cắt ngầm.
        var capNotified = false;
        function normalizeQty(raw, quiet) {
            var max = maxBuyable();
            var val = parseInt(raw, 10);
            if (isNaN(val)) val = 1;
            if (val < 1) val = 1;
            if (val > max) {
                val = max;
                if (!quiet && !capNotified && typeof showToast === 'function') {
                    capNotified = true;
                    showToast('Tối đa ' + max + ' đôi cho sản phẩm này (tồn kho còn lại).', 'info');
                }
            } else {
                capNotified = false;
            }
            return val;
        }
        function stepQty(delta) {
            var input = document.getElementById('<%= txtQuantity.ClientID %>');
            input.value = normalizeQty(input.value) + delta;
            input.value = normalizeQty(input.value, true);
        }
        function bindDetailQty() {
            var input = document.getElementById('<%= txtQuantity.ClientID %>');
            if (!input) return;
            input.addEventListener('change', function () {
                input.value = normalizeQty(input.value);
            });
            input.value = normalizeQty(input.value, true);
        }
        bindDetailQty();

        function toggleSizeGuide() {
            var box = document.getElementById('sizeGuideBox');
            box.style.display = (box.style.display === 'none') ? 'block' : 'none';
        }

        // Zoom theo vị trí chuột
        (function () {
            var wrap = document.getElementById('zoomWrap');
            if (!wrap) return;
            var img = wrap.querySelector('img');
            wrap.addEventListener('mousemove', function (e) {
                var r = wrap.getBoundingClientRect();
                img.style.transformOrigin = ((e.clientX - r.left) / r.width * 100) + '% ' + ((e.clientY - r.top) / r.height * 100) + '%';
            });
        })();
    </script>
</asp:Content>
