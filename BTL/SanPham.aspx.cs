using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class SanPham : System.Web.UI.Page
    {

        private const int PageSize = 6;

        public int CurrentPage
        {
            get { return ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1; }
            set { ViewState["CurrentPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CatalogQuery q = CatalogQuery.FromRequest(Request.QueryString);
                q.ApplyToControls(txtStoreSearch, cblBrands, rblPriceRange, hfSelectedSize, ddlSort);
                CurrentPage = q.Page;
            }

            FilterAndBindProducts();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            CatalogQuery q = CatalogQuery.FromControls(txtStoreSearch, cblBrands, rblPriceRange, hfSelectedSize, ddlSort, 1);
            Response.Redirect(q.ToUrl());
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("SanPham.aspx");
        }

        private void FilterAndBindProducts()
        {
            CatalogQuery q = IsPostBack
                ? CatalogQuery.FromControls(txtStoreSearch, cblBrands, rblPriceRange, hfSelectedSize, ddlSort, CurrentPage)
                : CatalogQuery.FromRequest(Request.QueryString);

            PagedResult<ProductItem> paged = Paging.Paginate(q.Apply(ProductStore.GetAll()), q.Page, PageSize);
            CurrentPage = paged.Page;

            if (paged.Items.Count > 0)
            {
                pnlNoResults.Visible = false;
                rptAllProducts.Visible = true;
                pnlPagination.Visible = paged.TotalPages > 1;

                rptAllProducts.DataSource = paged.Items;
                rptAllProducts.DataBind();

                List<int> pages = Enumerable.Range(1, paged.TotalPages).ToList();
                rptPages.DataSource = pages;
                rptPages.DataBind();

                btnPrevPage.CssClass = CurrentPage == 1 ? "page-link-custom disabled" : "page-link-custom";
                btnNextPage.CssClass = CurrentPage == paged.TotalPages ? "page-link-custom disabled" : "page-link-custom";
            }
            else
            {
                rptAllProducts.Visible = false;
                pnlNoResults.Visible = true;
                pnlPagination.Visible = false;
            }
        }

        protected void rptPages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CatalogQuery q = CatalogQuery.FromRequest(Request.QueryString);
                int page;
                q.Page = (int.TryParse(Convert.ToString(e.CommandArgument), out page) && page > 0) ? page : 1;
                Response.Redirect(q.ToUrl());
            }
        }

        protected void btnPrevPage_Click(object sender, EventArgs e)
        {
            CatalogQuery q = CatalogQuery.FromRequest(Request.QueryString);
            q.Page = Math.Max(q.Page - 1, 1);
            Response.Redirect(q.ToUrl());
        }

        protected void btnNextPage_Click(object sender, EventArgs e)
        {
            CatalogQuery q = CatalogQuery.FromRequest(Request.QueryString);
            q.Page = q.Page + 1;
            Response.Redirect(q.ToUrl());
        }

        protected void rptAllProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this, "RequireLoginToast",
                    "Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!");
                return;
            }

            int productId = Convert.ToInt32(e.CommandArgument);
            ProductItem selected = ProductStore.GetAll().FirstOrDefault(p => p.Id == productId);

            if (selected == null) return;

            string selectedSize = string.IsNullOrEmpty(hfSelectedSize.Value) ? selected.FirstSize() : hfSelectedSize.Value;

            if (e.CommandName == "AddToCart")
            {
                new SessionCartStore(Session).AddOrMerge(selected.Id, selected.Name, selected.Price, selectedSize, 1, selected.ImageUrl, selected.Stock);
                Toast.Success(this, "AddToCartToast", "Đã thêm " + selected.Name + " vào giỏ hàng!");
            }
            else if (e.CommandName == "BuyNow")
            {
                // Gộp vào giỏ hiện có (không xóa giỏ cũ) rồi sang thanh toán
                new SessionCartStore(Session).AddOrMerge(selected.Id, selected.Name, selected.Price, selectedSize, 1, selected.ImageUrl, selected.Stock);
                Response.Redirect("ThanhToan.aspx");
            }
        }

    }
}