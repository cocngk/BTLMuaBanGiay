using System;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TinTuc : ShopPage
    {
        private const int KichThuocTrang = 3;
        public int TrangTinHienTai { get; private set; }

        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (IsPostBack) return;
            BindStaticSections();
            LoadBlogs();
        }

        // Thực hiện việc nạp static sections.
        private void BindStaticSections()
        {
            System.Collections.Generic.List<BlogItem> tatCa = BlogStore.GetAll();
            if (tatCa.Count == 0) return;

            BlogItem featured = tatCa[0];
            imgHero.ImageUrl = ResolveUrl(featured.ImageUrl);
            imgHero.AlternateText = featured.Title;
            litHeroCategory.Text = featured.Category;
            litHeroTitle.Text = featured.Title;
            litHeroDate.Text = featured.Date;
            litHeroViews.Text = string.Format("{0:N0} lượt xem", featured.Views);
            litHeroDesc.Text = featured.Summary;
            rptPopular.DataSource = tatCa
                .OrderByDescending(x => x.Views)
                .Take(2)
                .ToList();
            rptPopular.DataBind();
            rptCategories.DataSource = tatCa
                .GroupBy(x => x.Category)
                .Select(nhom => new { Name = nhom.Key, Count = nhom.Count() })
                .OrderByDescending(nhom => nhom.Count)
                .ToList();
            rptCategories.DataBind();
        }

        // Thực hiện việc tải blogs.
        private void LoadBlogs()
        {
            PagedResult<BlogItem> trang = Paging.Paginate(
                BlogStore.GetAll(),
                Paging.PageFromQuery(Request.QueryString["page"]),
                KichThuocTrang);
            TrangTinHienTai = trang.Page;
            rptBlogs.DataSource = trang.Items;
            rptBlogs.DataBind();
            rptNewsPages.DataSource = Enumerable.Range(1, trang.TotalPages)
                .Select(n => new
                {
                    Number = n,
                    Url = Paging.PageUrl("TinTuc.aspx", n),
                    Css = n == TrangTinHienTai
                        ? "page-link-custom active"
                        : "page-link-custom"
                })
                .ToList();
            rptNewsPages.DataBind();
            pnlNewsPagination.Visible = trang.TotalPages > 1;
            bool previous = TrangTinHienTai > 1;
            btnNewsPrev.NavigateUrl = Paging.PageUrl("TinTuc.aspx", previous ? TrangTinHienTai - 1 : TrangTinHienTai);
            btnNewsPrev.CssClass = previous ? "page-link-custom" : "page-link-custom disabled";
            btnNewsPrev.Attributes["onclick"] = previous ? "" : "return false;";
            bool next = TrangTinHienTai < trang.TotalPages;
            btnNewsNext.NavigateUrl = Paging.PageUrl("TinTuc.aspx", next ? TrangTinHienTai + 1 : TrangTinHienTai);
            btnNewsNext.CssClass = next ? "page-link-custom" : "page-link-custom disabled";
            btnNewsNext.Attributes["onclick"] = next ? "" : "return false;";
        }
    }
}
