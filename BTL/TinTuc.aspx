<%@ Page Title="Tin Tức" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TinTuc.aspx.cs" Inherits="ShoeStore.TinTuc" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/tintuc.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

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