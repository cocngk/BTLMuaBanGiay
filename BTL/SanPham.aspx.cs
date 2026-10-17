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
    public partial class SanPham : ShopPage
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

        /// <summary>
        /// Lọc vẫn phải postback: giá trị nằm trong control nên server cần đọc
        /// chúng mới dựng được URL. Phân trang thì không — nó chỉ đổi số trang
        /// nên render thành link, bấm là đi thẳng không tốn vòng postback.
        /// </summary>
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            CatalogQuery q = CatalogQuery.FromControls(txtStoreSearch, cblBrands, rblPriceRange, hfSelectedSize, ddlSort, 1);
            Redirect(q.ToUrl());
        }

        private void FilterAndBindProducts()
        {
            CatalogQuery q = IsPostBack
                ? CatalogQuery.FromControls(txtStoreSearch, cblBrands, rblPriceRange, hfSelectedSize, ddlSort, CurrentPage)
                : CatalogQuery.FromRequest(Request.QueryString);

            PagedResult<ProductItem> paged = Paging.Paginate(q.Apply(ProductStore.GetAll()), q.Page, PageSize);
            CurrentPage = paged.Page;

            if (paged.Items.Count == 0)
            {
                rptAllProducts.Visible = false;
                pnlNoResults.Visible = true;
                pnlPagination.Visible = false;
                return;
            }

            pnlNoResults.Visible = false;
            rptAllProducts.Visible = true;
            pnlPagination.Visible = paged.TotalPages > 1;

            rptAllProducts.DataSource = paged.Items;
            rptAllProducts.DataBind();

            BindPageLinks(q, paged.TotalPages);
        }

        private void BindPageLinks(CatalogQuery q, int totalPages)
        {
            rptPages.DataSource = Enumerable.Range(1, totalPages)
                .Select(n => new
                {
                    Number = n,
                    Url = q.ToUrl("SanPham.aspx", n),
                    Css = n == CurrentPage ? "page-link-custom active" : "page-link-custom"
                }).ToList();
            rptPages.DataBind();

            bool hasPrev = CurrentPage > 1;
            lnkPrevPage.NavigateUrl = q.ToUrl("SanPham.aspx", CurrentPage - 1);
            lnkPrevPage.CssClass = hasPrev ? "page-link-custom" : "page-link-custom disabled";
            lnkPrevPage.Attributes["onclick"] = hasPrev ? "" : "return false;";

            bool hasNext = CurrentPage < totalPages;
            lnkNextPage.NavigateUrl = q.ToUrl("SanPham.aspx", CurrentPage + 1);
            lnkNextPage.CssClass = hasNext ? "page-link-custom" : "page-link-custom disabled";
            lnkNextPage.Attributes["onclick"] = hasNext ? "" : "return false;";
        }

        protected void rptAllProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!"))
            {
                return;
            }

            int productId = Convert.ToInt32(e.CommandArgument);
            ProductItem selected = ProductStore.GetAll().FirstOrDefault(p => p.Id == productId);

            if (selected == null) return;

            string selectedSize = string.IsNullOrEmpty(hfSelectedSize.Value) ? selected.FirstSize() : hfSelectedSize.Value;

            if (e.CommandName == "AddToCart")
            {
                CartStore.AddOrMerge(selected.Id, selected.Name, selected.Price, selectedSize, 1, selected.ImageUrl, selected.Stock);
                Toast.Success(this, "AddToCartToast", "Đã thêm " + selected.Name + " vào giỏ hàng!");
            }
            else if (e.CommandName == "BuyNow")
            {
                // Gộp vào giỏ hiện có (không xóa giỏ cũ) rồi sang thanh toán
                CartStore.AddOrMerge(selected.Id, selected.Name, selected.Price, selectedSize, 1, selected.ImageUrl, selected.Stock);
                Redirect("ThanhToan.aspx");
            }
        }
    }
}