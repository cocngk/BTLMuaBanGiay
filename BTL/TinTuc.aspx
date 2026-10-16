<%@ Page Title="Tin Tức" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TinTuc.aspx.cs" Inherits="ShoeStore.TinTuc" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/tintuc.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="news-container">
        <!-- BÀI VIẾT NỔI BẬT HERO -->
        <div class="featured-news-hero">
            <asp:Image ID="imgHero" runat="server" CssClass="featured-news-img" />
            <div class="featured-news-content">
                <span class="badge-category"><asp:Literal ID="litHeroCategory" runat="server" Mode="Encode"></asp:Literal></span>
                <h2 class="featured-news-title"><asp:Literal ID="litHeroTitle" runat="server" Mode="Encode"></asp:Literal></h2>
                <div class="news-meta">
                    <span><i class="fa-regular fa-calendar"></i> <asp:Literal ID="litHeroDate" runat="server"></asp:Literal></span>
                    <span><i class="fa-regular fa-eye"></i> <asp:Literal ID="litHeroViews" runat="server"></asp:Literal></span>
                </div>
                <p class="featured-news-desc"><asp:Literal ID="litHeroDesc" runat="server" Mode="Encode"></asp:Literal></p>
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
                        <asp:Repeater ID="rptCategories" runat="server">
                            <ItemTemplate>
                                <li class="category-item"><span><%# Eval("Name") %></span> <span>(<%# Eval("Count") %>)</span></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <div class="sidebar-widget">
                    <h3 class="widget-title">Bài viết đọc nhiều</h3>
                    <asp:Repeater ID="rptPopular" runat="server">
                        <ItemTemplate>
                            <div class="popular-news-item">
                                <img src='<%# Eval("ImageUrl") %>' class="popular-news-img" alt='<%# Eval("Title") %>' />
                                <div>
                                    <div class="popular-news-title"><%# Eval("Title") %></div>
                                    <span style="font-size:11px; color:var(--muted);"><%# Eval("Date") %></span>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </aside>
        </div>
    </div>
</asp:Content>