<%@ Page Title="Tin Tức" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TinTuc.aspx.cs" Inherits="ShoeStore.TinTuc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .news-container {
            max-width: 1200px;
            margin: 30px auto;
            padding: 0 20px;
        }

        .featured-news-hero {
            display: grid;
            grid-template-columns: 1.2fr 1fr;
            gap: 25px;
            background: #ffffff;
            border-radius: 25px;
            overflow: hidden;
            border: 1px solid #fce8ec;
            box-shadow: 0 5px 20px rgba(0,0,0,0.03);
            margin-bottom: 40px;
        }

        .featured-news-img {
            width: 100%;
            height: 320px;
            object-fit: cover;
        }

        .featured-news-content {
            padding: 30px 30px 30px 0;
            display: flex;
            flex-direction: column;
            justify-content: center;
        }

        .badge-category {
            background: #fde8ed;
            color: #e63946;
            font-size: 12px;
            font-weight: 700;
            padding: 4px 12px;
            border-radius: 15px;
            width: fit-content;
            margin-bottom: 12px;
        }

        .featured-news-title {
            font-size: 22px;
            font-weight: 800;
            color: #111;
            margin-bottom: 10px;
            line-height: 1.4;
        }

        .news-meta {
            font-size: 12px;
            color: #888;
            margin-bottom: 12px;
            display: flex;
            gap: 15px;
        }

        .featured-news-desc {
            font-size: 13px;
            color: #666;
            line-height: 1.6;
            margin-bottom: 20px;
        }

        .btn-read-more {
            color: #e63946;
            font-weight: 700;
            font-size: 13px;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .news-layout {
            display: grid;
            grid-template-columns: 1fr 300px;
            gap: 30px;
        }

        .blogs-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
            gap: 20px;
        }

        .blog-card {
            background: #ffffff;
            border-radius: 20px;
            overflow: hidden;
            border: 1px solid #fce8ec;
            display: flex;
            flex-direction: column;
        }

        .blog-thumb {
            width: 100%;
            height: 170px;
            object-fit: cover;
        }

        .blog-body {
            padding: 15px;
            display: flex;
            flex-direction: column;
            flex-grow: 1;
        }

        .blog-title {
            font-size: 15px;
            font-weight: 800;
            color: #222;
            margin-bottom: 8px;
            line-height: 1.4;
        }

        .blog-desc {
            font-size: 12px;
            color: #777;
            line-height: 1.5;
            margin-bottom: 15px;
        }

        .sidebar-widget {
            background: #ffffff;
            border-radius: 20px;
            padding: 20px;
            border: 1px solid #fce8ec;
            margin-bottom: 25px;
        }

        .widget-title {
            font-size: 16px;
            font-weight: 800;
            color: #111;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #fde8ed;
        }

        .category-list { list-style: none; }

        .category-item {
            display: flex;
            justify-content: space-between;
            padding: 8px 0;
            border-bottom: 1px dashed #eee;
            font-size: 13px;
            color: #555;
            cursor: pointer;
        }

        .category-item:hover { color: #e63946; }

        .popular-news-item {
            display: flex;
            gap: 12px;
            margin-bottom: 15px;
        }

        .popular-news-img {
            width: 60px;
            height: 60px;
            border-radius: 12px;
            object-fit: cover;
        }

        .popular-news-title {
            font-size: 13px;
            font-weight: 700;
            color: #222;
            line-height: 1.3;
        }

        /* --- STYLES PHÂN TRANG TIN TỨC --- */
        .pagination-container {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 8px;
            margin-top: 30px;
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
    </style>

    <div class="news-container">
        <!-- BÀI VIẾT NỔI BẬT HERO -->
        <div class="featured-news-hero">
            <img src="https://images.unsplash.com/photo-1512374382149-233c42b6a83b?q=80&w=800" class="featured-news-img" alt="Featured" />
            <div class="featured-news-content">
                <span class="badge-category">Mẹo chọn đồ</span>
                <h2 class="featured-news-title">Cách chọn đúng size giày Sneaker chuẩn 100% khi đặt hàng Online</h2>
                <div class="news-meta">
                    <span><i class="fa-regular fa-calendar"></i> 12/10/2026</span>
                    <span><i class="fa-regular fa-eye"></i> 1,250 lượt xem</span>
                </div>
                <p class="featured-news-desc">Không còn lo lắng việc đặt giày về bị chật hay rộng nữa. Dưới đây là quy chuẩn bảng đo chiều dài chân chuẩn từng milimet dành riêng cho bạn...</p>
                <a href="#" class="btn-read-more">Đọc chi tiết bài viết &rarr;</a>
            </div>
        </div>

        <div class="news-layout">
            <div>
                <!-- LƯỚI DANH SÁCH BÀI VIẾT -->
                <div class="blogs-grid">
                    <asp:Repeater ID="rptBlogs" runat="server">
                        <ItemTemplate>
                            <div class="blog-card">
                                <img src='<%# Eval("ImageUrl") %>' class="blog-thumb" />
                                <div class="blog-body">
                                    <div class="news-meta">
                                        <span><i class="fa-regular fa-calendar"></i> <%# Eval("Date") %></span>
                                    </div>
                                    <h3 class="blog-title"><%# Eval("Title") %></h3>
                                    <p class="blog-desc"><%# Eval("Summary") %></p>
                                    <a href="#" class="btn-read-more" style="margin-top:auto;">Xem thêm &rarr;</a>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <!-- THANH PHÂN TRANG TIN TỨC -->
                <div class="pagination-container" id="pnlNewsPagination" runat="server">
                    <asp:LinkButton ID="btnNewsPrev" runat="server" CssClass="page-link-custom" OnClick="btnNewsPrev_Click">&laquo; Trước</asp:LinkButton>
                    <asp:Repeater ID="rptNewsPages" runat="server" OnItemCommand="rptNewsPages_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnPage" runat="server" CommandName="Page" CommandArgument='<%# Container.DataItem %>' CssClass='<%# Convert.ToInt32(Container.DataItem) == CurrentNewsPage ? "page-link-custom active" : "page-link-custom" %>'>
                                <%# Container.DataItem %>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:LinkButton ID="btnNewsNext" runat="server" CssClass="page-link-custom" OnClick="btnNewsNext_Click">Sau &raquo;</asp:LinkButton>
                </div>
            </div>

            <!-- SIDEBAR BÊN PHẢI -->
            <aside>
                <div class="sidebar-widget">
                    <h3 class="widget-title">Chuyên mục tin tức</h3>
                    <ul class="category-list">
                        <li class="category-item"><span>Mẹo chọn Size giày</span> <span>(12)</span></li>
                        <li class="category-item"><span>Hướng dẫn vệ sinh</span> <span>(8)</span></li>
                        <li class="category-item"><span>Xu hướng Sneaker 2026</span> <span>(15)</span></li>
                        <li class="category-item"><span>Phối đồ thời trang</span> <span>(20)</span></li>
                    </ul>
                </div>

                <div class="sidebar-widget">
                    <h3 class="widget-title">Bài viết đọc nhiều</h3>
                    <div class="popular-news-item">
                        <img src="https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=200" class="popular-news-img" />
                        <div>
                            <div class="popular-news-title">5 Bước làm sạch giày trắng siêu tốc</div>
                            <span style="font-size:11px; color:#999;">10/10/2026</span>
                        </div>
                    </div>
                    <div class="popular-news-item">
                        <img src="https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?q=80&w=200" class="popular-news-img" />
                        <div>
                            <div class="popular-news-title">Top 10 đôi Sneaker không bao giờ lỗi mốt</div>
                            <span style="font-size:11px; color:#999;">08/10/2026</span>
                        </div>
                    </div>
                </div>
            </aside>
        </div>
    </div>
</asp:Content>