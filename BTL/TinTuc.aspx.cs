using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TinTuc : ShopPage
    {
        // ĐÃ ĐIỀU CHỈNH: 3 BÀI VIẾT / TRANG
        private const int PageSize = 3;

        public int CurrentNewsPage
        {
            get { return ViewState["CurrentNewsPage"] != null ? (int)ViewState["CurrentNewsPage"] : 1; }
            set { ViewState["CurrentNewsPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentNewsPage = 1;
                BindStaticSections();
                LoadBlogs();
            }
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
            PagedResult<BlogItem> paged = Paging.Paginate(BlogStore.GetAll(), CurrentNewsPage, PageSize);
            CurrentNewsPage = paged.Page;

            rptBlogs.DataSource = paged.Items;
            rptBlogs.DataBind();

            rptNewsPages.DataSource = Enumerable.Range(1, paged.TotalPages).ToList();
            rptNewsPages.DataBind();

            pnlNewsPagination.Visible = paged.TotalPages > 1;

            btnNewsPrev.CssClass = CurrentNewsPage == 1 ? "page-link-custom disabled" : "page-link-custom";
            btnNewsNext.CssClass = CurrentNewsPage == paged.TotalPages ? "page-link-custom disabled" : "page-link-custom";
        }

        protected void rptNewsPages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentNewsPage = Convert.ToInt32(e.CommandArgument);
                LoadBlogs();
            }
        }

        protected void btnNewsPrev_Click(object sender, EventArgs e)
        {
            if (CurrentNewsPage > 1)
            {
                CurrentNewsPage--;
                LoadBlogs();
            }
        }

        protected void btnNewsNext_Click(object sender, EventArgs e)
        {
            CurrentNewsPage++;
            LoadBlogs();
        }
    }
}
