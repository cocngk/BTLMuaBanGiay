using System;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TinTuc : ShopPage
    {
        // 3 BÀI VIẾT / TRANG
        private const int PageSize = 3;

        public int CurrentNewsPage { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Trang là link thật (?page=N) nên mỗi lần mở đều bind lại toàn bộ —
            // sidebar không còn biến mất sau khi đổi trang như thời ViewState.
            BindStaticSections();
            LoadBlogs();
        }

        private void BindStaticSections()
        {
            var all = BlogStore.GetAll();

            BlogItem featured = all.First();
            imgHero.ImageUrl = ResolveUrl(featured.ImageUrl);
            imgHero.AlternateText = featured.Title;
            litHeroCategory.Text = featured.Category;
            litHeroTitle.Text = featured.Title;
            litHeroDate.Text = featured.Date;
            litHeroViews.Text = string.Format("{0:N0} lượt xem", featured.Views);
            litHeroDesc.Text = featured.Summary;

            rptPopular.DataSource = all.OrderByDescending(b => b.Views).Take(2).ToList();
            rptPopular.DataBind();

            rptCategories.DataSource = all.GroupBy(b => b.Category)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count).ToList();
            rptCategories.DataBind();
        }

        private void LoadBlogs()
        {
            int page = Paging.PageFromQuery(Request.QueryString["page"]);
            PagedResult<BlogItem> paged = Paging.Paginate(BlogStore.GetAll(), page, PageSize);
            CurrentNewsPage = paged.Page;

            rptBlogs.DataSource = paged.Items;
            rptBlogs.DataBind();

            rptNewsPages.DataSource = Enumerable.Range(1, paged.TotalPages)
                .Select(n => new
                {
                    Number = n,
                    Url = Paging.PageUrl("TinTuc.aspx", n),
                    Css = n == CurrentNewsPage ? "page-link-custom active" : "page-link-custom"
                }).ToList();
            rptNewsPages.DataBind();

            pnlNewsPagination.Visible = paged.TotalPages > 1;

            bool hasPrev = CurrentNewsPage > 1;
            btnNewsPrev.NavigateUrl = Paging.PageUrl("TinTuc.aspx", hasPrev ? CurrentNewsPage - 1 : CurrentNewsPage);
            btnNewsPrev.CssClass = hasPrev ? "page-link-custom" : "page-link-custom disabled";
            btnNewsPrev.Attributes["onclick"] = hasPrev ? "" : "return false;";

            bool hasNext = CurrentNewsPage < paged.TotalPages;
            btnNewsNext.NavigateUrl = Paging.PageUrl("TinTuc.aspx", hasNext ? CurrentNewsPage + 1 : CurrentNewsPage);
            btnNewsNext.CssClass = hasNext ? "page-link-custom" : "page-link-custom disabled";
            btnNewsNext.Attributes["onclick"] = hasNext ? "" : "return false;";
        }
    }
}
