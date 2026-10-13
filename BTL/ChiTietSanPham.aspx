<%@ Page Title="Chi Tiết Sản Phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChiTietSanPham.aspx.cs" Inherits="ShoeStore.ChiTietSanPham" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .detail-container {
            max-width: 1100px;
            margin: 30px auto;
            padding: 0 20px;
        }

        .breadcrumb-link {
            font-size: 13px;
            color: #777;
            margin-bottom: 20px;
        }

        .breadcrumb-link a {
            color: #e63946;
            text-decoration: none;
            font-weight: 700;
        }

        .product-detail-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 40px;
            background: #ffffff;
            border-radius: 25px;
            padding: 30px;
            border: 1px solid #fce8ec;
            box-shadow: 0 5px 20px rgba(0,0,0,0.03);
        }

        @media (max-width: 850px) {
            .product-detail-grid { grid-template-columns: 1fr; }
        }

        .product-main-img {
            width: 100%;
            height: 400px;
            object-fit: cover;
            border-radius: 20px;
            background: #fafafa;
        }

        .product-info-box {
            display: flex;
            flex-direction: column;
        }

        .product-brand-tag {
            background: #fde8ed;
            color: #e63946;
            font-size: 12px;
            font-weight: 700;
            padding: 4px 12px;
            border-radius: 15px;
            width: fit-content;
            margin-bottom: 10px;
        }

        .product-title-large {
            font-size: 26px;
            font-weight: 800;
            color: #111;
            margin-bottom: 10px;
        }

        .product-price-large {
            font-size: 24px;
            font-weight: 800;
            color: #e63946;
            margin-bottom: 20px;
        }

        .section-label {
            font-size: 14px;
            font-weight: 700;
            color: #333;
            margin-bottom: 10px;
            display: block;
        }

        /* SIZE SELECTION STYLES */
        .size-selector {
            display: flex;
            gap: 10px;
            margin-bottom: 20px;
        }

        .size-option-btn {
            border: 1px solid #f8d7da;
            background: #fff;
            padding: 8px 18px;
            border-radius: 12px;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            color: #444;
            transition: all 0.2s;
            user-select: none;
        }

        .size-option-btn:hover {
            border-color: #e63946;
            color: #e63946;
        }

        .size-option-btn.active {
            background: #e63946 !important;
            color: #ffffff !important;
            border-color: #e63946 !important;
            box-shadow: 0 3px 8px rgba(230, 57, 70, 0.3);
        }

        .qty-box-flex {
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 25px;
        }

        .qty-input-detail {
            width: 70px;
            padding: 8px;
            border-radius: 12px;
            border: 1px solid #f8d7da;
            text-align: center;
            font-weight: bold;
            outline: none;
        }

        .detail-action-buttons {
            display: flex;
            gap: 15px;
            margin-top: auto;
        }

        .btn-detail-cart {
            flex: 1;
            background: #fde8ed;
            color: #e63946;
            border: 1.5px solid #f8c2cc;
            padding: 12px;
            border-radius: 15px;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            transition: all 0.3s;
        }

        .btn-detail-cart:hover { background: #f8c2cc; }

        .btn-detail-buy {
            flex: 1;
            background: #e63946;
            color: white;
            border: 1.5px solid #e63946;
            padding: 12px;
            border-radius: 15px;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            box-shadow: 0 4px 12px rgba(230, 57, 70, 0.3);
            transition: all 0.3s;
        }

        .btn-detail-buy:hover { background: #c1121f; }

        .product-desc-box {
            margin-top: 30px;
            background: #ffffff;
            border-radius: 20px;
            padding: 25px;
            border: 1px solid #fce8ec;
        }

        .desc-title {
            font-size: 18px;
            font-weight: 800;
            color: #111;
            margin-bottom: 12px;
            border-bottom: 2px solid #fde8ed;
            padding-bottom: 8px;
        }

        .desc-content {
            font-size: 14px;
            color: #555;
            line-height: 1.7;
        }
    </style>

    <div class="detail-container">
        <div class="breadcrumb-link">
            <a href="TrangChu.aspx">Trang chủ</a> / <a href="SanPham.aspx">Sản phẩm</a> / <span>Chi tiết sản phẩm</span>
        </div>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false" Style="text-align:center; padding:50px;">
            <h2>Sản phẩm không tồn tại!</h2>
            <a href="SanPham.aspx" style="color:#e63946; font-weight:bold;">&larr; Quay lại danh sách sản phẩm</a>
        </asp:Panel>

        <asp:Panel ID="pnlProductDetail" runat="server">
            <div class="product-detail-grid">
                <div>
                    <asp:Image ID="imgProduct" runat="server" CssClass="product-main-img" />
                </div>

                <div class="product-info-box">
                    <span class="product-brand-tag"><asp:Literal ID="litBrand" runat="server"></asp:Literal></span>
                    <h1 class="product-title-large"><asp:Literal ID="litName" runat="server"></asp:Literal></h1>
                    <div class="product-price-large"><asp:Literal ID="litPrice" runat="server"></asp:Literal></div>

                    <!-- KHU VỰC CHỌN SIZE ĐÃ FIX HOÀN TOÀN -->
                    <span class="section-label">Chọn Size giày:</span>
                    <asp:HiddenField ID="hfSelectedSize" runat="server" Value="40" />
                    <div class="size-selector">
                        <div class="size-option-btn" data-size="38" onclick="selectDetailSize(this, '38')">38</div>
                        <div class="size-option-btn" data-size="39" onclick="selectDetailSize(this, '39')">39</div>
                        <div class="size-option-btn active" data-size="40" onclick="selectDetailSize(this, '40')">40</div>
                        <div class="size-option-btn" data-size="41" onclick="selectDetailSize(this, '41')">41</div>
                        <div class="size-option-btn" data-size="42" onclick="selectDetailSize(this, '42')">42</div>
                    </div>

                    <!-- CHỌN SỐ LƯỢNG -->
                    <span class="section-label">Số lượng:</span>
                    <div class="qty-box-flex">
                        <asp:TextBox ID="txtQuantity" runat="server" Text="1" TextMode="Number" min="1" CssClass="qty-input-detail" oninput="if(this.value<1)this.value=1;"></asp:TextBox>
                    </div>

                    <div class="detail-action-buttons">
                        <asp:Button ID="btnAddToCart" runat="server" Text="THÊM VÀO GIỎ HÀNG" CssClass="btn-detail-cart" OnClick="btnAddToCart_Click" />
                        <asp:Button ID="btnBuyNow" runat="server" Text="THANH TOÁN NGAY" CssClass="btn-detail-buy" OnClick="btnBuyNow_Click" />
                    </div>
                </div>
            </div>

            <div class="product-desc-box">
                <h3 class="desc-title">Mô Tả Sản Phẩm Chi Tiết</h3>
                <p class="desc-content">
                    <asp:Literal ID="litDescription" runat="server"></asp:Literal>
                </p>
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

        // Tự giữ size 40 làm mặc định khi mở trang
        document.addEventListener("DOMContentLoaded", function () {
            var currentSize = document.getElementById('<%= hfSelectedSize.ClientID %>').value || '40';
            var targetBtn = document.querySelector('.size-option-btn[data-size="' + currentSize + '"]');
            if (targetBtn) {
                var btns = document.querySelectorAll('.size-option-btn');
                btns.forEach(function (b) { b.classList.remove('active'); });
                targetBtn.classList.add('active');
            }
        });
    </script>
</asp:Content>