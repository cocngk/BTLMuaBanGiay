<%@ Page Title="Chi Tiết Sản Phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChiTietSanPham.aspx.cs" Inherits="ShoeStore.ChiTietSanPham" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/chitietsanpham.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

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