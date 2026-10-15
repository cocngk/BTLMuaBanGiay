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
                ApplyQueryToControls();
                int page;
                if (int.TryParse(Request.QueryString["page"], out page) && page > 0)
                {
                    CurrentPage = page;
                }
                else
                {
                    CurrentPage = 1;
                }
            }

            FilterAndBindProducts();
        }

        private class FilterParams
        {
            public string Keyword = "";
            public List<string> Brands = new List<string>();
            public string PriceRange = "All";
            public string Size = "";
            public string Sort = "Default";
            public int Page = 1;
        }

        private FilterParams GetFiltersFromControls()
        {
            return new FilterParams
            {
                Keyword = txtStoreSearch.Text.Trim(),
                Brands = cblBrands.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList(),
                PriceRange = rblPriceRange.SelectedValue,
                Size = hfSelectedSize.Value,
                Sort = ddlSort.SelectedValue,
                Page = CurrentPage
            };
        }

        private FilterParams GetFiltersFromQuery()
        {
            FilterParams f = new FilterParams();
            f.Keyword = Request.QueryString["search"] ?? "";
            string brands = Request.QueryString["brand"] ?? "";
            f.Brands = brands.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(b => b.Trim()).ToList();
            f.PriceRange = string.IsNullOrEmpty(Request.QueryString["price"]) ? "All" : Request.QueryString["price"];
            f.Size = Request.QueryString["size"] ?? "";
            f.Sort = string.IsNullOrEmpty(Request.QueryString["sort"]) ? "Default" : Request.QueryString["sort"];
            int page;
            f.Page = (int.TryParse(Request.QueryString["page"], out page) && page > 0) ? page : 1;
            return f;
        }

        private void ApplyQueryToControls()
        {
            FilterParams f = GetFiltersFromQuery();
            txtStoreSearch.Text = f.Keyword;
            foreach (ListItem item in cblBrands.Items)
            {
                item.Selected = f.Brands.Contains(item.Value, StringComparer.OrdinalIgnoreCase);
            }
            if (rblPriceRange.Items.FindByValue(f.PriceRange) != null)
            {
                rblPriceRange.SelectedValue = f.PriceRange;
            }
            hfSelectedSize.Value = f.Size;
            if (ddlSort.Items.FindByValue(f.Sort) != null)
            {
                ddlSort.SelectedValue = f.Sort;
            }
        }

        private string BuildFilterUrl(FilterParams f)
        {
            List<string> parts = new List<string>();
            if (!string.IsNullOrEmpty(f.Keyword)) parts.Add("search=" + Server.UrlEncode(f.Keyword));
            if (f.Brands.Count > 0) parts.Add("brand=" + Server.UrlEncode(string.Join(",", f.Brands)));
            if (!string.IsNullOrEmpty(f.PriceRange) && f.PriceRange != "All") parts.Add("price=" + Server.UrlEncode(f.PriceRange));
            if (!string.IsNullOrEmpty(f.Size)) parts.Add("size=" + Server.UrlEncode(f.Size));
            if (!string.IsNullOrEmpty(f.Sort) && f.Sort != "Default") parts.Add("sort=" + Server.UrlEncode(f.Sort));
            if (f.Page > 1) parts.Add("page=" + f.Page);
            return "SanPham.aspx" + (parts.Count > 0 ? "?" + string.Join("&", parts) : "");
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            FilterParams f = GetFiltersFromControls();
            f.Page = 1;
            Response.Redirect(BuildFilterUrl(f));
        }

        protected void btnResetFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("SanPham.aspx");
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
                FilterParams f = GetFiltersFromQuery();
                int page;
                f.Page = (int.TryParse(Convert.ToString(e.CommandArgument), out page) && page > 0) ? page : 1;
                Response.Redirect(BuildFilterUrl(f));
            }
        }

        protected void btnPrevPage_Click(object sender, EventArgs e)
        {
            FilterParams f = GetFiltersFromQuery();
            f.Page = Math.Max(f.Page - 1, 1);
            Response.Redirect(BuildFilterUrl(f));
        }

        protected void btnNextPage_Click(object sender, EventArgs e)
        {
            FilterParams f = GetFiltersFromQuery();
            f.Page = f.Page + 1;
            Response.Redirect(BuildFilterUrl(f));
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