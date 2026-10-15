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
                if (Request.QueryString["search"] != null)
                {
                    txtStoreSearch.Text = Request.QueryString["search"];
                }

                CurrentPage = 1;
                FilterAndBindProducts();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            FilterAndBindProducts();
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            cblBrands.ClearSelection();
            rblPriceRange.SelectedValue = "All";
            hfSelectedSize.Value = "";
            txtStoreSearch.Text = "";
            ddlSort.SelectedValue = "Default";

            CurrentPage = 1;
            FilterAndBindProducts();
        }

        private void FilterAndBindProducts()
        {
            List<ProductItem> allProducts = ProductStore.GetAll();

            string keyword = txtStoreSearch.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                allProducts = allProducts.Where(p => p.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                     p.Brand.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                     p.Description.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            List<string> selectedBrands = cblBrands.Items.Cast<ListItem>()
                                                 .Where(i => i.Selected)
                                                 .Select(i => i.Value)
                                                 .ToList();
            if (selectedBrands.Count > 0)
            {
                allProducts = allProducts.Where(p => selectedBrands.Contains(p.Brand, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            string priceRange = rblPriceRange.SelectedValue;
            if (priceRange == "Under2M")
            {
                allProducts = allProducts.Where(p => p.Price < 2000000).ToList();
            }
            else if (priceRange == "2Mto4M")
            {
                allProducts = allProducts.Where(p => p.Price >= 2000000 && p.Price <= 4000000).ToList();
            }
            else if (priceRange == "Above4M")
            {
                allProducts = allProducts.Where(p => p.Price > 4000000).ToList();
            }

            string selectedSize = hfSelectedSize.Value;
            if (!string.IsNullOrEmpty(selectedSize))
            {
                allProducts = allProducts.Where(p => p.Sizes.Contains(selectedSize)).ToList();
            }

            string sortType = ddlSort.SelectedValue;
            if (sortType == "PriceAsc")
            {
                allProducts = allProducts.OrderBy(p => p.Price).ToList();
            }
            else if (sortType == "PriceDesc")
            {
                allProducts = allProducts.OrderByDescending(p => p.Price).ToList();
            }
            else if (sortType == "NameAsc")
            {
                allProducts = allProducts.OrderBy(p => p.Name).ToList();
            }

            int totalItems = allProducts.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            if (totalPages < 1) totalPages = 1;
            if (CurrentPage > totalPages) CurrentPage = totalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            var pagedProducts = allProducts.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            if (pagedProducts.Count > 0)
            {
                pnlNoResults.Visible = false;
                rptAllProducts.Visible = true;
                pnlPagination.Visible = totalPages > 1;

                rptAllProducts.DataSource = pagedProducts;
                rptAllProducts.DataBind();

                List<int> pages = Enumerable.Range(1, totalPages).ToList();
                rptPages.DataSource = pages;
                rptPages.DataBind();

                btnPrevPage.CssClass = CurrentPage == 1 ? "page-link-custom disabled" : "page-link-custom";
                btnNextPage.CssClass = CurrentPage == totalPages ? "page-link-custom disabled" : "page-link-custom";
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
                CurrentPage = Convert.ToInt32(e.CommandArgument);
                FilterAndBindProducts();
            }
        }

        protected void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                FilterAndBindProducts();
            }
        }

        protected void btnNextPage_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            FilterAndBindProducts();
        }

        protected void rptAllProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!CartHelper.IsLoggedIn())
            {
                CartHelper.RequireLoginToast(this, "RequireLoginToast",
                    "Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!");
                return;
            }

            int productId = Convert.ToInt32(e.CommandArgument);
            ProductItem selected = ProductStore.GetAll().FirstOrDefault(p => p.Id == productId);

            if (selected == null) return;

            string selectedSize = string.IsNullOrEmpty(hfSelectedSize.Value) ? "40" : hfSelectedSize.Value;

            if (e.CommandName == "AddToCart")
            {
                CartHelper.AddOrMerge(selected.Id, selected.Name, selected.Price, selectedSize, 1, selected.ImageUrl, selected.Stock);
                CartHelper.SuccessToast(this, "AddToCartToast", "Đã thêm " + selected.Name + " vào giỏ hàng!");
            }
            else if (e.CommandName == "BuyNow")
            {
                // Gộp vào giỏ hiện có (không xóa giỏ cũ) rồi sang thanh toán
                CartHelper.AddOrMerge(selected.Id, selected.Name, selected.Price, selectedSize, 1, selected.ImageUrl, selected.Stock);
                Response.Redirect("ThanhToan.aspx");
            }
        }

    }
}