using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    /// <summary>
    /// Danh sách sản phẩm cho quản trị viên: xem toàn bộ danh mục, tìm, sửa, xoá.
    /// Khác trang Sản phẩm của khách ở chỗ thấy cả tồn kho và thao tác ghi — và ở
    /// chỗ bắt buộc phải là admin, kiểm tra ở server chứ không chỉ ẩn link.
    /// </summary>
    public partial class QuanTriSanPham : ShopPage
    {
        private const int PageSize = 12;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            pnlAdmin.Visible = true;

            if (!IsPostBack)
            {
                CurrentPage = 1;
                BindList();
            }
        }

        /// <summary>
        /// Số trang giữ trong ViewState thay vì query string: các nút Trước/Sau là
        /// postback, nên không cần dựng URL và cũng không sợ mất từ khóa đang tìm.
        /// </summary>
        private int CurrentPage
        {
            get
            {
                object value = ViewState["Page"];
                return value == null ? 1 : (int)value;
            }
            set
            {
                ViewState["Page"] = value;
            }
        }

        private void BindList()
        {
            CatalogQuery query = new CatalogQuery
            {
                Keyword = txtSearch.Text.Trim(),
                Sort = "NameAsc"
            };

            List<ProductItem> all = Catalog.All();
            List<ProductItem> filtered = new List<ProductItem>(query.Apply(all));

            PagedResult<ProductItem> result = Paging.Paginate(filtered, CurrentPage, PageSize);
            CurrentPage = result.Page;

            rptProducts.DataSource = result.Items;
            rptProducts.DataBind();

            bool empty = result.Items.Count == 0;
            pnlTable.Visible = !empty;
            pnlEmpty.Visible = empty;
            pnlPager.Visible = result.TotalPages > 1;

            litCount.Text = filtered.Count == all.Count
                ? all.Count + " sản phẩm"
                : filtered.Count + " / " + all.Count + " sản phẩm";

            litEmpty.Text = string.IsNullOrEmpty(query.Keyword)
                ? "Danh mục đang trống. Bấm \"Thêm sản phẩm\" để bắt đầu."
                : "Không có sản phẩm nào khớp với \"" + query.Keyword + "\".";

            litPage.Text = result.Page.ToString();
            litTotalPages.Text = result.TotalPages.ToString();

            btnPrev.Enabled = result.Page > 1;
            btnNext.Enabled = result.Page < result.TotalPages;
            btnPrev.CssClass = result.Page > 1 ? "btn-row" : "btn-row is-disabled";
            btnNext.CssClass = result.Page < result.TotalPages ? "btn-row" : "btn-row is-disabled";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            CurrentPage = 1;
            BindList();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            CurrentPage = Math.Max(1, CurrentPage - 1);
            BindList();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            CurrentPage = CurrentPage + 1;
            BindList();
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Handler chạy cả khi Page_Load đã bị chặn (vòng đời trang vẫn đi tiếp),
            // nên phải kiểm tra quyền lại ở đây — nếu chỉ dựa vào cửa lúc tải trang
            // thì người không phải admin vẫn xoá được bằng một postback tự dựng.
            if (!RequireAdmin())
            {
                return;
            }

            if (e.CommandName != "Delete")
            {
                return;
            }

            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id))
            {
                return;
            }

            ProductItem product = Catalog.Find(id);
            if (product == null)
            {
                Toast.Info(this, "Sản phẩm này đã bị xoá trước đó.");
            }
            else if (Catalog.Delete(id))
            {
                Toast.Success(this, "Đã xoá sản phẩm " + product.Name + ".");
            }
            else
            {
                Toast.Info(this, "Không xoá được sản phẩm này. Vui lòng thử lại.");
            }

            BindList();
        }

        /// <summary>Nhãn tồn kho đổi màu khi sắp hết — 3 đôi trở xuống là mức cần nhập thêm.</summary>
        protected string StockClass(object value)
        {
            int stock;
            if (!int.TryParse(Convert.ToString(value), out stock))
            {
                return "stock-badge";
            }

            return stock <= 3 ? "stock-badge is-low" : "stock-badge";
        }
    }
}
