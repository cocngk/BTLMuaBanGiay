<%@ Page Title="Trang Chủ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrangChu.aspx.cs" Inherits="ShoeStore.TrangChu" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/trangchu.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

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