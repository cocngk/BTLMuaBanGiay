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
        private const int PageSize = 6;
        public int CurrentPage { get { return ViewState["CurrentPage"] == null ? 1 : (int)ViewState["CurrentPage"]; } set { ViewState["CurrentPage"] = value; } }
        private CatalogFilterControls Filters { get { return new CatalogFilterControls(txtStoreSearch, cblBrands, rblPriceRange, hfSelectedSize, ddlSort); } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cblBrands.Items.Clear();
            foreach (string brand in Catalog.Brands()) cblBrands.Items.Add(new ListItem(brand, brand));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CatalogQuery query = CatalogQuery.FromQuery(Request.QueryString);
                Filters.Write(query);
                CurrentPage = query.Page;
            }
            BindProducts();
        }

        protected void btnFilter_Click(object sender, EventArgs e) { Redirect(Filters.Read(1).ToUrl()); }

        private void BindProducts()
        {
            CatalogQuery query = IsPostBack ? Filters.Read(CurrentPage) : CatalogQuery.FromQuery(Request.QueryString);
            PagedResult<ProductItem> page = Catalog.Search(query, PageSize);
            CurrentPage = page.Page;
            bool empty = page.Items.Count == 0;
            rptAllProducts.Visible = !empty;
            pnlNoResults.Visible = empty;
            pnlPagination.Visible = !empty && page.TotalPages > 1;
            if (empty) return;
            rptAllProducts.DataSource = page.Items;
            rptAllProducts.DataBind();
            rptPages.DataSource = Enumerable.Range(1, page.TotalPages).Select(n => new { Number = n, Url = query.ToUrl("SanPham.aspx", n), Css = n == CurrentPage ? "page-link-custom active" : "page-link-custom" }).ToList();
            rptPages.DataBind();
            bool previous = CurrentPage > 1;
            lnkPrevPage.NavigateUrl = query.ToUrl("SanPham.aspx", previous ? CurrentPage - 1 : CurrentPage);
            lnkPrevPage.CssClass = previous ? "page-link-custom" : "page-link-custom disabled";
            lnkPrevPage.Attributes["onclick"] = previous ? "" : "return false;";
            bool next = CurrentPage < page.TotalPages;
            lnkNextPage.NavigateUrl = query.ToUrl("SanPham.aspx", next ? CurrentPage + 1 : CurrentPage);
            lnkNextPage.CssClass = next ? "page-link-custom" : "page-link-custom disabled";
            lnkNextPage.Attributes["onclick"] = next ? "" : "return false;";
        }

        protected void rptAllProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!")) return;
            ProductItem product = Catalog.Find(Convert.ToInt32(e.CommandArgument));
            if (product == null) return;
            string size = string.IsNullOrEmpty(hfSelectedSize.Value) ? product.FirstSize() : hfSelectedSize.Value;
            CartStore.AddProduct(product, size, 1);
            if (e.CommandName == "AddToCart") Toast.Success(this, "Đã thêm " + product.Name + " vào giỏ hàng!");
            else if (e.CommandName == "BuyNow") Redirect("ThanhToan.aspx");
        }
    }
}
