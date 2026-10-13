<%@ Page Title="Trang Chủ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrangChu.aspx.cs" Inherits="ShoeStore.TrangChu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .hero-section {
            position: relative;
            background: linear-gradient(rgba(0,0,0,0.3), rgba(0,0,0,0.3)), url('https://images.unsplash.com/photo-1552346154-21d32810aba3?q=80&w=1200') center/cover;
            height: 380px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            border-bottom-left-radius: 40px;
            border-bottom-right-radius: 40px;
            overflow: hidden;
            cursor: pointer;
        }

        .hero-action-box {
            display: flex;
            flex-direction: column;
            align-items: center;
            opacity: 0;
            transform: translateY(25px);
            transition: all 0.4s ease-in-out;
            pointer-events: none;
        }

        .hero-section:hover .hero-action-box {
            opacity: 1;
            transform: translateY(0);
            pointer-events: auto;
        }

        .btn-hero-discover {
            background: #e63946;
            color: white;
            padding: 12px 28px;
            border-radius: 25px;
            text-decoration: none;
            font-weight: 700;
            font-size: 14px;
            box-shadow: 0 6px 20px rgba(230, 57, 70, 0.5);
            margin-bottom: 20px;
            display: inline-block;
            transition: transform 0.2s;
            border: none;
            cursor: pointer;
        }

        .btn-hero-discover:hover {
            transform: scale(1.05);
            background: #c1121f;
        }

        .hero-features {
            background: rgba(255, 255, 255, 0.95);
            padding: 10px 25px;
            border-radius: 30px;
            display: flex;
            gap: 20px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.15);
            backdrop-filter: blur(5px);
        }

        .feature-item {
            font-size: 12px;
            font-weight: 600;
            color: #444;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .feature-item i { color: #e63946; }

        .hero-hint-text {
            position: absolute;
            bottom: 15px;
            color: rgba(255, 255, 255, 0.8);
            font-size: 12px;
            font-weight: 600;
            transition: opacity 0.3s;
        }

        .hero-section:hover .hero-hint-text {
            opacity: 0;
        }

        .section-container {
            max-width: 1200px;
            margin: 40px auto;
            padding: 0 20px;
        }

        .section-header-flex {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        .section-title-text {
            font-size: 26px;
            font-weight: 800;
            color: #e63946;
        }

        .section-subtitle-text {
            font-size: 13px;
            color: #888;
            margin-top: 4px;
        }

        .btn-view-all-pill {
            border: 1px solid #e63946;
            color: #e63946;
            padding: 6px 18px;
            border-radius: 20px;
            text-decoration: none;
            font-weight: 700;
            font-size: 13px;
            transition: all 0.3s;
        }

        .btn-view-all-pill:hover {
            background: #e63946;
            color: white;
        }

        .products-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
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

        .product-card-custom:hover {
            transform: translateY(-5px);
        }

        .badge-hot-pink {
            position: absolute;
            top: 20px;
            left: 20px;
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
            top: 20px;
            right: 20px;
            background: #ffffff;
            border: none;
            width: 32px;
            height: 32px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #e63946;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            cursor: pointer;
            z-index: 2;
        }

        .product-thumb {
            width: 100%;
            height: 200px;
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

        .product-title-bold {
            font-size: 16px;
            font-weight: 800;
            color: #222;
            margin-bottom: 4px;
        }

        .product-desc-muted {
            font-size: 12px;
            color: #888;
            margin-bottom: 15px;
        }

        .product-footer-flex {
            margin-top: auto;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .product-price-bold {
            font-size: 16px;
            font-weight: 800;
            color: #e63946;
        }

        .btn-add-plus-circle {
            background: #e63946;
            color: white;
            border: none;
            width: 36px;
            height: 36px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            text-decoration: none;
            transition: background 0.3s;
        }

        .btn-add-plus-circle:hover {
            background: #c1121f;
        }
    </style>

    <!-- BANNER HERO -->
    <div class="hero-section">
        <div class="hero-hint-text"><i class="fa-solid fa-hand-pointer"></i> Di chuột vào đây để khám phá</div>

        <div class="hero-action-box">
            <asp:LinkButton ID="btnHeroDiscover" runat="server" CssClass="btn-hero-discover" OnClick="btnHeroDiscover_Click">
                <i class="fa-solid fa-shoe-prints"></i> Khám phá ngay &rarr;
            </asp:LinkButton>
            <div class="hero-features">
                <div class="feature-item"><i class="fa-solid fa-check-circle"></i> Sản phẩm chính hãng</div>
                <div class="feature-item"><i class="fa-solid fa-heart"></i> Phục vụ bằng tình yêu</div>
                <div class="feature-item"><i class="fa-solid fa-gift"></i> Quà tặng hấp dẫn</div>
            </div>
        </div>
    </div>

    <!-- SẢN PHẨM NỔI BẬT -->
    <div class="section-container" id="san-pham">
        <div class="section-header-flex">
            <div>
                <h2 class="section-title-text">Sản phẩm nổi bật <i class="fa-solid fa-heart"></i></h2>
                <p class="section-subtitle-text">Những đôi giày thể thao tuyệt vời được yêu thích nhất tại SneakerX.</p>
            </div>
            <a href="SanPham.aspx" class="btn-view-all-pill">Xem tất cả &rarr;</a>
        </div>

        <div class="products-grid">
            <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
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

                            <div class="product-footer-flex">
                                <span class="product-price-bold"><%# string.Format("{0:N0}đ", Eval("Price")) %></span>
                                <asp:LinkButton ID="btnQuickAdd" runat="server" CommandName="QuickAdd" CommandArgument='<%# Eval("Id") %>' CssClass="btn-add-plus-circle" title="Thêm vào giỏ hàng">
                                    <i class="fa-solid fa-plus"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>