using System;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TinTuc : ShopPage
    {
        private const int PageSize = 3;
        public int CurrentNewsPage { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
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
            rptPopular.DataSource = all.OrderByDescending(x => x.Views).Take(2).ToList();
            rptPopular.DataBind();
            rptCategories.DataSource = all.GroupBy(x => x.Category).Select(x => new { Name = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).ToList();
            rptCategories.DataBind();
        }

        private void LoadBlogs()
        {
            PagedResult<BlogItem> page = Paging.Paginate(BlogStore.GetAll(), Paging.PageFromQuery(Request.QueryString["page"]), PageSize);
            CurrentNewsPage = page.Page;
            rptBlogs.DataSource = page.Items;
            rptBlogs.DataBind();
            rptNewsPages.DataSource = Enumerable.Range(1, page.TotalPages).Select(n => new { Number = n, Url = Paging.PageUrl("TinTuc.aspx", n), Css = n == CurrentNewsPage ? "page-link-custom active" : "page-link-custom" }).ToList();
            rptNewsPages.DataBind();
            pnlNewsPagination.Visible = page.TotalPages > 1;
            bool previous = CurrentNewsPage > 1;
            btnNewsPrev.NavigateUrl = Paging.PageUrl("TinTuc.aspx", previous ? CurrentNewsPage - 1 : CurrentNewsPage);
            btnNewsPrev.CssClass = previous ? "page-link-custom" : "page-link-custom disabled";
            btnNewsPrev.Attributes["onclick"] = previous ? "" : "return false;";
            bool next = CurrentNewsPage < page.TotalPages;
            btnNewsNext.NavigateUrl = Paging.PageUrl("TinTuc.aspx", next ? CurrentNewsPage + 1 : CurrentNewsPage);
            btnNewsNext.CssClass = next ? "page-link-custom" : "page-link-custom disabled";
            btnNewsNext.Attributes["onclick"] = next ? "" : "return false;";
        }
    }
}
