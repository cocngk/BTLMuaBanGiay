<%@ Register Src="~/Controls/TheSanPham.ascx" TagPrefix="uc" TagName="TheSanPham" %>
<%@ Page Title="Chi Tiết Sản Phẩm" Language="C#" MasterPageFile="~/KhungTrang.Master" AutoEventWireup="true" CodeBehind="ChiTietSanPham.aspx.cs" Inherits="ShoeStore.ChiTietSanPham" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/the-san-pham.css" />
    <link rel="stylesheet" href="Content/chitietsanpham.css" />

    <style>
        .review-container {
            margin-top: 35px;
            padding: 25px;
            background: var(--panel);
            border: 1px solid var(--border);
            border-radius: 16px;
        }

        .review-title {
            font-size: 24px;
            font-weight: 800;
            margin-bottom: 20px;
            color: var(--heading);
        }

        .review-summary {
            display: flex;
            align-items: center;
            gap: 20px;
            padding: 20px;
            background: var(--tint);
            border-radius: 12px;
            margin-bottom: 25px;
        }

        .review-average {
            font-size: 34px;
            font-weight: 800;
            color: var(--brand);
        }

        .review-average span {
            font-size: 15px;
        }

        .review-stars {
            color: #e5a000;
            font-size: 19px;
            letter-spacing: 2px;
        }

        .review-notice {
            padding: 16px;
            margin-bottom: 20px;
            background: var(--tint);
            border-radius: 8px;
            color: var(--body);
        }

        .review-input-group {
            margin-bottom: 15px;
        }

        .review-input-group label {
            display: block;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .review-select {
            width: 100%;
            max-width: 250px;
            padding: 10px;
            border: 1px solid var(--border-strong);
            border-radius: 8px;
            background: var(--panel);
        }

        .review-textarea {
            width: 100%;
            box-sizing: border-box;
            padding: 12px;
            border: 1px solid var(--border-strong);
            border-radius: 8px;
            resize: vertical;
            font-family: inherit;
        }

        .btn-submit-review {
            padding: 12px 22px;
            border: none;
            border-radius: 8px;
            background: var(--brand);
            color: white;
            font-weight: 700;
            cursor: pointer;
        }

        .btn-submit-review:hover {
            opacity: 0.85;
        }

        .review-message {
            display: block;
            margin-top: 12px;
            color: var(--brand);
            font-weight: 600;
        }

        .review-list {
            margin-top: 30px;
        }

        .review-list h3 {
            margin-bottom: 18px;
        }

        .review-item {
            padding: 18px 0;
            border-bottom: 1px solid var(--border);
        }

        .review-user {
            display: flex;
            justify-content: space-between;
            gap: 10px;
            margin-bottom: 8px;
        }

        .review-user span {
            color: var(--muted);
            font-size: 12px;
        }

        .review-comment {
            margin-top: 10px;
            line-height: 1.7;
            color: var(--body);
            white-space: pre-wrap;
            overflow-wrap: anywhere;
        }

        .review-empty {
            padding: 20px;
            text-align: center;
            color: var(--muted);
            background: var(--tint);
            border-radius: 10px;
        }

        @media (max-width: 600px) {
            .review-container {
                padding: 16px;
            }

            .review-summary {
                gap: 12px;
                padding: 14px;
            }

            .review-average {
                font-size: 28px;
            }

            .review-user {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="detail-container">

        <div class="breadcrumb-link">
            <a href="TrangChu.aspx">Trang chủ</a> /
            <a href="SanPham.aspx">Sản phẩm</a> /
            <span>
                <asp:Literal ID="litBreadcrumb" runat="server"
                    Mode="Encode" Text="Chi tiết sản phẩm">
                </asp:Literal>
            </span>
        </div>

        <asp:Panel ID="pnlNotFound" runat="server"
            Visible="false"
            Style="text-align:center; padding:50px;">

            <h2>Sản phẩm không tồn tại!</h2>
            <a href="SanPham.aspx" class="notfound-back-link">
                &larr; Quay lại danh sách sản phẩm
            </a>
        </asp:Panel>

        <asp:Panel ID="pnlProductDetail" runat="server">

            <div class="product-detail-grid">

                <div>
                    <div class="zoom-wrap" id="zoomWrap">
                        <asp:Image ID="imgProduct" runat="server"
                            CssClass="product-main-img" />
                    </div>

                    <asp:Panel ID="pnlColourways" runat="server"
                        Visible="false" CssClass="colourway-row">

                        <span class="colourway-label">Màu sắc:</span>

                        <asp:Repeater ID="rptColourways" runat="server">
                            <ItemTemplate>
                                <button type="button"
                                    class='<%# Eval("Active") %>'
                                    data-large='<%# ResolveUrl(Eval("Large").ToString()) %>'
                                    onclick="swapMainImage(this)">

                                    <img src='<%# ResolveUrl(Eval("Small").ToString()) %>'
                                        alt='<%# Eval("Alt") %>' />

                                    <span class="colourway-name">
                                        <%# Eval("Name") %>
                                    </span>
                                </button>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>

                    <p class="zoom-hint">
                        <i class="fa-solid fa-magnifying-glass-plus"></i>
                        Di chuột vào ảnh để phóng to, bấm một màu để đổi ảnh
                    </p>
                </div>

                <div class="product-info-box">

                    <span class="product-brand-tag">
                        <asp:Literal ID="litBrand" runat="server"
                            Mode="Encode">
                        </asp:Literal>
                    </span>

                    <h1 class="product-title-large">
                        <asp:Literal ID="litName" runat="server"
                            Mode="Encode">
                        </asp:Literal>
                    </h1>

                    <div class="detail-meta-row">
                        <span class="sku-text">
                            Mã SP:
                            <asp:Literal ID="litSku" runat="server"
                                Mode="Encode">
                            </asp:Literal>
                        </span>
                    </div>

                    <div class="price-row">
                        <span class="product-price-large">
                            <asp:Literal ID="litPrice" runat="server"
                                Mode="Encode">
                            </asp:Literal>
                        </span>

                        <asp:Panel ID="pnlDiscount" runat="server"
                            Visible="false" CssClass="discount-inline">

                            <span class="old-price">
                                <asp:Literal ID="litOldPrice" runat="server"
                                    Mode="Encode">
                                </asp:Literal>
                            </span>

                            <span class="discount-badge">
                                <asp:Literal ID="litDiscountPct" runat="server"
                                    Mode="Encode">
                                </asp:Literal>
                            </span>
                        </asp:Panel>
                    </div>

                    <div class="stock-row">
                        <asp:Literal ID="litStock" runat="server"></asp:Literal>
                    </div>

                    <div class="size-label-row">
                        <span class="section-label">Chọn Size giày:</span>
                        <a href="javascript:void(0)"
                            class="size-guide-link"
                            onclick="toggleSizeGuide()">

                            <i class="fa-solid fa-ruler"></i>
                            Bảng size
                        </a>
                    </div>

                    <asp:HiddenField ID="hfSelectedSize" runat="server" />
                    <asp:HiddenField ID="hfMaxQty" runat="server" Value="10" />

                    <div class="size-selector">
                        <asp:Repeater ID="rptSizes" runat="server">
                            <ItemTemplate>
                                <div class='<%# Container.ItemIndex == 0 ? "size-option-btn active" : "size-option-btn" %>'
                                    data-size='<%# Eval("Size") %>'
                                    onclick="selectDetailSize(this, '<%# Eval("Size") %>')">

                                    <%# Eval("Size") %>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div id="sizeGuideBox" class="size-guide-box"
                        style="display:none;">

                        <table class="size-guide-table">
                            <tr>
                                <th>EU</th>
                                <th>US</th>
                                <th>Chiều dài chân (cm)</th>
                            </tr>
                            <tr><td>38</td><td>6</td><td>24.0</td></tr>
                            <tr><td>39</td><td>6.5</td><td>24.5</td></tr>
                            <tr><td>40</td><td>7</td><td>25.0</td></tr>
                            <tr><td>41</td><td>8</td><td>26.0</td></tr>
                            <tr><td>42</td><td>8.5</td><td>26.5</td></tr>
                            <tr><td>43</td><td>9.5</td><td>27.5</td></tr>
                        </table>
                    </div>

                    <span class="section-label">Số lượng:</span>

                    <div class="qty-box-flex">

                        <button type="button"
                            class="qty-step-btn"
                            onclick="stepQty(-1)">−</button>

                        <asp:TextBox ID="txtQuantity" runat="server"
                            Text="1" TextMode="Number"
                            min="1" max="10"
                            CssClass="qty-input-detail js-qty-detail">
                        </asp:TextBox>

                        <button type="button"
                            class="qty-step-btn"
                            onclick="stepQty(1)">+</button>

                        <asp:Button ID="btnWishlist" runat="server"
                            CssClass="btn-wishlist-detail"
                            Text="&#9825;"
                            ToolTip="Thêm vào yêu thích"
                            OnClick="btnWishlist_Click"
                            CausesValidation="false" />
                    </div>

                    <div class="detail-action-buttons">

                        <asp:Button ID="btnAddToCart" runat="server"
                            Text="THÊM VÀO GIỎ HÀNG"
                            CssClass="btn-detail-cart"
                            OnClick="btnAddToCart_Click" />

                        <asp:Button ID="btnBuyNow" runat="server"
                            Text="THANH TOÁN NGAY"
                            CssClass="btn-detail-buy"
                            OnClick="btnBuyNow_Click" />
                    </div>

                    <div class="ship-box">
                        <div class="ship-item">
                            <i class="fa-solid fa-truck-fast"></i>
                            <span><b>Miễn phí ship</b> đơn từ 500K, giao 2–4 ngày</span>
                        </div>

                        <div class="ship-item">
                            <i class="fa-solid fa-rotate-left"></i>
                            <span>Đổi size miễn phí trong <b>7 ngày</b></span>
                        </div>

                        <div class="ship-item">
                            <i class="fa-solid fa-shield-halved"></i>
                            <span>Thanh toán khi nhận hàng (COD), bảo hành keo <b>6 tháng</b></span>
                        </div>
                    </div>
                </div>
            </div>

            <div class="product-desc-box">

                <h3 class="desc-title">Mô Tả Sản Phẩm Chi Tiết</h3>

                <p class="desc-content">
                    <asp:Literal ID="litDescription" runat="server"
                        Mode="Encode">
                    </asp:Literal>
                </p>

                <ul class="spec-list">
                    <li><b>Chất liệu:</b> Da tổng hợp cao cấp, lưới thoáng khí</li>
                    <li><b>Đế:</b> Cao su chống trơn trượt, độ bền cao</li>
                    <li><b>Phù hợp:</b> Đi học, đi chơi, chạy bộ nhẹ mỗi ngày</li>
                    <li><b>Bảo hành:</b> 6 tháng (bong keo, lỗi chỉ may)</li>
                </ul>
            </div>

            <!-- PHẦN ĐÁNH GIÁ SẢN PHẨM -->
            <div class="review-container">

                <h2 class="review-title">Đánh giá từ khách hàng</h2>

                <div class="review-summary">

                    <div class="review-average">
                        <asp:Label ID="litAverageRating" runat="server"
                            Text="0.0">
                        </asp:Label>
                        <span>/ 5</span>
                    </div>

                    <div>
                        <div class="review-stars">★★★★★</div>

                        <asp:Label ID="litReviewCount" runat="server"
                            Text="0 đánh giá">
                        </asp:Label>
                    </div>
                </div>

                <asp:Panel ID="pnlReviewLogin" runat="server"
                    Visible="false" CssClass="review-notice">

                    Bạn cần đăng nhập và có đơn hàng đã được giao
                    chứa sản phẩm này để gửi đánh giá.
                </asp:Panel>

                <asp:Panel ID="pnlReviewForm" runat="server"
                    Visible="false">

                    <h3>Viết đánh giá của bạn</h3>

                    <div class="review-input-group">
                        <label>Chất lượng sản phẩm:</label>

                        <asp:DropDownList ID="ddlRating" runat="server"
                            CssClass="review-select">

                            <asp:ListItem Value="5">5 sao - Rất tốt</asp:ListItem>
                            <asp:ListItem Value="4">4 sao - Tốt</asp:ListItem>
                            <asp:ListItem Value="3">3 sao - Bình thường</asp:ListItem>
                            <asp:ListItem Value="2">2 sao - Không tốt</asp:ListItem>
                            <asp:ListItem Value="1">1 sao - Rất kém</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="review-input-group">
                        <label>Nhận xét của bạn:</label>

                        <asp:TextBox ID="txtReview" runat="server"
                            TextMode="MultiLine"
                            Rows="4"
                            MaxLength="1000"
                            CssClass="review-textarea"
                            placeholder="Hãy chia sẻ trải nghiệm mua hàng của bạn...">
                        </asp:TextBox>
                    </div>

                    <asp:Button ID="btnSubmitReview" runat="server"
                        Text="GỬI ĐÁNH GIÁ"
                        CssClass="btn-submit-review"
                        OnClick="btnSubmitReview_Click"
                        CausesValidation="false" />

                    <asp:Label ID="litReviewMessage" runat="server"
                        CssClass="review-message"
                        EnableViewState="false">
                    </asp:Label>

                </asp:Panel>

                <div class="review-list">

                    <h3>Nhận xét của khách hàng</h3>

                    <asp:Panel ID="pnlNoReviews" runat="server"
                        Visible="false" CssClass="review-empty">

                        Sản phẩm chưa có đánh giá nào.
                    </asp:Panel>

                    <asp:Repeater ID="rptReviews" runat="server">
                        <ItemTemplate>

                            <div class="review-item">

                                <div class="review-user">
                                    <strong><%#: Eval("CustomerName") %></strong>

                                    <span>
                                        <%#: Eval("CreatedAt", "{0:dd/MM/yyyy HH:mm}") %>
                                    </span>
                                </div>

                                <div class="review-stars">
                                    <%#: Eval("Stars") %>
                                </div>

                                <p class="review-comment">
                                    <%#: Eval("Comment") %>
                                </p>

                            </div>

                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

            <!-- SẢN PHẨM LIÊN QUAN -->
            <div class="section-container">

                <div class="section-header-flex">

                    <div>
                        <h2 class="section-title-text">Sản phẩm liên quan</h2>
                        <p class="section-subtitle-text">
                            Bạn cũng có thể sẽ thích những đôi này.
                        </p>
                    </div>

                    <asp:HyperLink ID="lnkViewAllRelated" runat="server"
                        CssClass="btn-view-all-pill"
                        NavigateUrl="SanPham.aspx">

                        Xem tất cả &rarr;
                    </asp:HyperLink>
                </div>

                <div class="products-grid">

                    <asp:Repeater ID="rptRelated" runat="server">
                        <ItemTemplate>

                            <uc:TheSanPham runat="server"
                                ActionMode="None"
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

            btns.forEach(function (b) {
                b.classList.remove('active');
            });

            element.classList.add('active');
            hf.value = sizeVal;
        }

        function maxBuyable() {
            var max = parseInt(
                document.getElementById('<%= hfMaxQty.ClientID %>').value,
                10
            );

            return isNaN(max) || max < 1 ? 1 : max;
        }

        var capNotified = false;

        function normalizeQty(raw, quiet) {
            var max = maxBuyable();
            var val = parseInt(raw, 10);

            if (isNaN(val)) val = 1;
            if (val < 1) val = 1;

            if (val > max) {
                val = max;

                if (!quiet && !capNotified &&
                    typeof showToast === 'function') {

                    capNotified = true;

                    showToast(
                        'Tối đa ' + max +
                        ' đôi cho sản phẩm này (tồn kho còn lại).',
                        'info'
                    );
                }
            } else {
                capNotified = false;
            }

            return val;
        }

        function stepQty(delta) {
            var input = document.getElementById(
                '<%= txtQuantity.ClientID %>'
            );

            input.value = normalizeQty(input.value) + delta;
            input.value = normalizeQty(input.value, true);
        }

        function bindDetailQty() {
            var input = document.getElementById(
                '<%= txtQuantity.ClientID %>'
            );

            if (!input) return;

            input.addEventListener('change', function () {
                input.value = normalizeQty(input.value);
            });

            input.value = normalizeQty(input.value, true);
        }

        bindDetailQty();

        function toggleSizeGuide() {
            var box = document.getElementById('sizeGuideBox');

            box.style.display =
                (box.style.display === 'none') ? 'block' : 'none';
        }

        function swapMainImage(chip) {
            var main = document.getElementById(
                '<%= imgProduct.ClientID %>'
            );

            var large = chip.getAttribute('data-large');

            if (!main || !large) return;

            main.src = large;

            document.querySelectorAll('.colourway-chip')
                .forEach(function (c) {
                    c.classList.remove('active');
                });

            chip.classList.add('active');
        }

        (function () {
            var wrap = document.getElementById('zoomWrap');

            if (!wrap) return;

            var img = wrap.querySelector('img');

            wrap.addEventListener('mousemove', function (e) {
                var r = wrap.getBoundingClientRect();

                img.style.transformOrigin =
                    ((e.clientX - r.left) / r.width * 100) + '% ' +
                    ((e.clientY - r.top) / r.height * 100) + '%';
            });
        })();
    </script>

</asp:Content>