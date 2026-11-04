using System;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class SanPham : ShopPage
    {
        private const int KichThuocTrang = 6;
        public int TrangHienTai
        {
            get { return ViewState["CurrentPage"] == null ? 1 : (int)ViewState["CurrentPage"]; }
            set { ViewState["CurrentPage"] = value; }
        }

        private CatalogFilterControls BoLoc
        {
            get
            {
                return new CatalogFilterControls(
                    txtStoreSearch, cblBrands, rblPriceRange,
                    hfSelectedSize, ddlSort);
            }
        }

        // Xử lý sự kiện vòng đời của điều khiển.
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cblBrands.Items.Clear();
            foreach (string thuongHieu in Catalog.Brands())
                cblBrands.Items.Add(new ListItem(thuongHieu, thuongHieu));
        }

        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!IsPostBack)
            {
                CatalogQuery truyVan = CatalogQuery.FromQuery(Request.QueryString);
                BoLoc.Write(truyVan);
                TrangHienTai = truyVan.Page;
            }
            BindProducts();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnFilter_Click(object nguonSuKien, EventArgs e)
        {
            Redirect(BoLoc.Read(1).ToUrl());
        }

        // Thực hiện việc nạp sản phẩm.
        private void BindProducts()
        {
            CatalogQuery truyVan = IsPostBack
                ? BoLoc.Read(TrangHienTai)
                : CatalogQuery.FromQuery(Request.QueryString);
            PagedResult<ProductItem> trang = Catalog.Search(truyVan, KichThuocTrang);
            TrangHienTai = trang.Page;
            bool rong = trang.Items.Count == 0;
            rptAllProducts.Visible = !rong;
            pnlNoResults.Visible = rong;
            pnlPagination.Visible = !rong && trang.TotalPages > 1;
            if (rong) return;
            rptAllProducts.DataSource = trang.Items;
            rptAllProducts.DataBind();
            rptPages.DataSource = Enumerable.Range(1, trang.TotalPages)
                .Select(n => new
                {
                    Number = n,
                    Url = truyVan.ToUrl("SanPham.aspx", n),
                    Css = n == TrangHienTai
                        ? "page-link-custom active"
                        : "page-link-custom"
                })
                .ToList();
            rptPages.DataBind();
            bool previous = TrangHienTai > 1;
            lnkPrevPage.NavigateUrl = truyVan.ToUrl("SanPham.aspx", previous ? TrangHienTai - 1 : TrangHienTai);
            lnkPrevPage.CssClass = previous ? "page-link-custom" : "page-link-custom disabled";
            lnkPrevPage.Attributes["onclick"] = previous ? "" : "return false;";
            bool next = TrangHienTai < trang.TotalPages;
            lnkNextPage.NavigateUrl = truyVan.ToUrl("SanPham.aspx", next ? TrangHienTai + 1 : TrangHienTai);
            lnkNextPage.CssClass = next ? "page-link-custom" : "page-link-custom disabled";
            lnkNextPage.Attributes["onclick"] = next ? "" : "return false;";
        }

        // Xử lý lệnh trên một dòng dữ liệu.
        protected void rptAllProducts_ItemCommand(object nguon, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "AddToCart" && e.CommandName != "BuyNow") return;
            if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!")) return;

            int maSanPham;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out maSanPham)) return;
            ProductItem sanPham = Catalog.Find(maSanPham);
            if (sanPham == null) return;
            if (sanPham.Stock <= 0)
            {
                Toast.Info(this, "Sản phẩm này hiện đã hết hàng.");
                return;
            }

            string kichThuoc = sanPham.SizeList().Contains(hfSelectedSize.Value)
                ? hfSelectedSize.Value
                : sanPham.FirstSize();
            CartStore.AddProduct(sanPham, kichThuoc, 1);
            if (e.CommandName == "AddToCart")
                Toast.Success(this, "Đã thêm " + sanPham.Name + " vào giỏ hàng!");
            else
                Redirect("ThanhToan.aspx");
        }
    }
}
