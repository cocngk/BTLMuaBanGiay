using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class QuanTriSanPham : ShopPage
    {
        private const int KichThuocTrang = 12;
        private int TrangHienTai
        {
            get { return ViewState["Page"] == null ? 1 : (int)ViewState["Page"]; }
            set { ViewState["Page"] = value; }
        }

        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            pnlAdmin.Visible = true;
            if (!IsPostBack) { TrangHienTai = 1; BindList(); }
        }

        // Thực hiện việc nạp danh sách.
        private void BindList()
        {
            CatalogQuery truyVan = new CatalogQuery { Keyword = txtSearch.Text.Trim(), Sort = "NameAsc" };
            List<ProductItem> all = Catalog.All();
            List<ProductItem> filtered = truyVan.Apply(all).ToList();
            PagedResult<ProductItem> trang = Paging.Paginate(filtered, TrangHienTai, KichThuocTrang);
            TrangHienTai = trang.Page;
            rptProducts.DataSource = trang.Items;
            rptProducts.DataBind();
            bool rong = trang.Items.Count == 0;
            pnlTable.Visible = !rong;
            pnlEmpty.Visible = rong;
            pnlPager.Visible = trang.TotalPages > 1;
            litCount.Text = filtered.Count == all.Count
                ? all.Count + " sản phẩm"
                : filtered.Count + " / " + all.Count + " sản phẩm";
            litEmpty.Text = truyVan.Keyword.Length == 0 ? "Danh mục đang trống. Bấm \"Thêm sản phẩm\" để bắt đầu." : "Không có sản phẩm nào khớp với \"" + truyVan.Keyword + "\".";
            litPage.Text = trang.Page.ToString();
            litTotalPages.Text = trang.TotalPages.ToString();
            btnPrev.Enabled = trang.Page > 1;
            btnNext.Enabled = trang.Page < trang.TotalPages;
            btnPrev.CssClass = trang.Page > 1 ? "btn-row" : "btn-row is-disabled";
            btnNext.CssClass = trang.Page < trang.TotalPages ? "btn-row" : "btn-row is-disabled";
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnSearch_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            TrangHienTai = 1;
            BindList();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnPrev_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            TrangHienTai = Math.Max(1, TrangHienTai - 1);
            BindList();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnNext_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            TrangHienTai++;
            BindList();
        }

        // Xử lý lệnh trên một dòng dữ liệu.
        protected void rptProducts_ItemCommand(object nguon, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin() || e.CommandName != "Delete") return;
            int ma;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out ma)) return;
            ProductItem sanPham = Catalog.Find(ma);
            if (sanPham == null) Toast.Info(this, "Sản phẩm này đã bị xoá trước đó.");
            else if (Catalog.Delete(ma)) Toast.Success(this, "Đã xoá sản phẩm " + sanPham.Name + ".");
            else Toast.Info(this, "Không xoá được sản phẩm này. Vui lòng thử lại.");
            BindList();
        }

        // Thực hiện việc stock lớp.
        protected string StockClass(object giaTri)
        {
            int stock;
            return int.TryParse(Convert.ToString(giaTri), out stock)
                && stock <= 3
                ? "stock-badge is-low"
                : "stock-badge";
        }
    }
}
