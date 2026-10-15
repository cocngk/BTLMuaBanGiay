<%@ Register Src="~/Controls/ProductCard.ascx" TagPrefix="uc" TagName="ProductCard" %>
<%@ Page Title="Trang Chủ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrangChu.aspx.cs" Inherits="ShoeStore.TrangChu" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/product-card.css" />
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
                    <uc:ProductCard runat="server" ActionMode="QuickAdd"
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
</asp:Content>