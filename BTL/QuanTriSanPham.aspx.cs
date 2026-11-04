using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class QuanTriSanPham : ShopPage
    {
        private const int PageSize = 12;
        private int CurrentPage { get { return ViewState["Page"] == null ? 1 : (int)ViewState["Page"]; } set { ViewState["Page"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin()) return;
            pnlAdmin.Visible = true;
            if (!IsPostBack) { CurrentPage = 1; BindList(); }
        }

        private void BindList()
        {
            CatalogQuery query = new CatalogQuery { Keyword = txtSearch.Text.Trim(), Sort = "NameAsc" };
            List<ProductItem> all = Catalog.All();
            List<ProductItem> filtered = new List<ProductItem>(query.Apply(all));
            PagedResult<ProductItem> page = Paging.Paginate(filtered, CurrentPage, PageSize);
            CurrentPage = page.Page;
            rptProducts.DataSource = page.Items;
            rptProducts.DataBind();
            bool empty = page.Items.Count == 0;
            pnlTable.Visible = !empty;
            pnlEmpty.Visible = empty;
            pnlPager.Visible = page.TotalPages > 1;
            litCount.Text = filtered.Count == all.Count ? all.Count + " sản phẩm" : filtered.Count + " / " + all.Count + " sản phẩm";
            litEmpty.Text = query.Keyword.Length == 0 ? "Danh mục đang trống. Bấm \"Thêm sản phẩm\" để bắt đầu." : "Không có sản phẩm nào khớp với \"" + query.Keyword + "\".";
            litPage.Text = page.Page.ToString();
            litTotalPages.Text = page.TotalPages.ToString();
            btnPrev.Enabled = page.Page > 1;
            btnNext.Enabled = page.Page < page.TotalPages;
            btnPrev.CssClass = page.Page > 1 ? "btn-row" : "btn-row is-disabled";
            btnNext.CssClass = page.Page < page.TotalPages ? "btn-row" : "btn-row is-disabled";
        }

        protected void btnSearch_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage = 1; BindList(); } }
        protected void btnPrev_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage = Math.Max(1, CurrentPage - 1); BindList(); } }
        protected void btnNext_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage++; BindList(); } }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin() || e.CommandName != "Delete") return;
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            ProductItem product = Catalog.Find(id);
            if (product == null) Toast.Info(this, "Sản phẩm này đã bị xoá trước đó.");
            else if (Catalog.Delete(id)) Toast.Success(this, "Đã xoá sản phẩm " + product.Name + ".");
            else Toast.Info(this, "Không xoá được sản phẩm này. Vui lòng thử lại.");
            BindList();
        }

        protected string StockClass(object value)
        {
            int stock;
            return int.TryParse(Convert.ToString(value), out stock) && stock <= 3 ? "stock-badge is-low" : "stock-badge";
        }
    }
}
