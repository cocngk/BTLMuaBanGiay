using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class SanPham : System.Web.UI.Page
    {
        public class ProductItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Brand { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string Sizes { get; set; }
            public string ImageUrl { get; set; }
            public bool IsHot { get; set; }
        }

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
            List<ProductItem> allProducts = GetMockProducts();

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
            if (Session["Username"] == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "RequireLoginToast",
                    "showToast('Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!', 'info'); openLoginModal();", true);
                return;
            }

            int productId = Convert.ToInt32(e.CommandArgument);
            List<ProductItem> allProducts = GetMockProducts();
            ProductItem selected = allProducts.FirstOrDefault(p => p.Id == productId);

            if (selected == null) return;

            string selectedSize = string.IsNullOrEmpty(hfSelectedSize.Value) ? "40" : hfSelectedSize.Value;

            if (e.CommandName == "AddToCart")
            {
                List<GioHang.CartItem> cart = Session["Cart"] as List<GioHang.CartItem> ?? new List<GioHang.CartItem>();
                GioHang.CartItem existing = cart.FirstOrDefault(c => c.Id == productId && c.Size == selectedSize);

                if (existing != null)
                {
                    existing.Quantity += 1;
                }
                else
                {
                    cart.Add(new GioHang.CartItem
                    {
                        Id = selected.Id,
                        Name = selected.Name,
                        Price = selected.Price,
                        Quantity = 1,
                        Size = selectedSize,
                        ImageUrl = selected.ImageUrl
                    });
                }

                Session["Cart"] = cart;

                string msg = "Đã thêm " + selected.Name.Replace("'", "\\'") + " vào giỏ hàng!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddToCartToast", "showToast('" + msg + "', 'success');", true);
            }
            else if (e.CommandName == "BuyNow")
            {
                List<GioHang.CartItem> singleCart = new List<GioHang.CartItem>
                {
                    new GioHang.CartItem
                    {
                        Id = selected.Id,
                        Name = selected.Name,
                        Price = selected.Price,
                        Quantity = 1,
                        Size = selectedSize,
                        ImageUrl = selected.ImageUrl
                    }
                };

                Session["Cart"] = singleCart;
                Response.Redirect("ThanhToan.aspx");
            }
        }

        private List<ProductItem> GetMockProducts()
        {
            return new List<ProductItem>
            {
                new ProductItem { Id = 1, Name = "Nike Air Force 1 '07", Brand = "Nike", Description = "Thiết kế kinh điển, đệm Air siêu êm.", Price = 2900000, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?q=80&w=400", IsHot = true },
                new ProductItem { Id = 2, Name = "Adidas Ultraboost Light", Brand = "Adidas", Description = "Trọng lượng siêu nhẹ, êm ái khi chạy bộ.", Price = 3800000, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400", IsHot = true },
                new ProductItem { Id = 3, Name = "Jordan 1 Retro High Chicago", Brand = "Jordan", Description = "Phối màu huyền thoại dành cho tín đồ Sneaker.", Price = 4500000, Sizes = "40,41,42,43", ImageUrl = "https://images.unsplash.com/photo-1556906781-9a412961c28c?q=80&w=400", IsHot = false },
                new ProductItem { Id = 4, Name = "Puma RS-X Reinvent Red", Brand = "Puma", Description = "Cá tính, phối màu năng động cá tính.", Price = 1800000, Sizes = "38,39,40", ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400", IsHot = false },
                new ProductItem { Id = 5, Name = "Converse Chuck 70 High", Brand = "Converse", Description = "Phong cách Retro Vintage tối giản cổ điển.", Price = 1950000, Sizes = "38,39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?q=80&w=400", IsHot = true },
                new ProductItem { Id = 6, Name = "Vans Old Skool Black", Brand = "Vans", Description = "Mẫu giày Skateboard trường tồn với thời gian.", Price = 1850000, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400", IsHot = false },
                new ProductItem { Id = 7, Name = "Nike Dunk Low Retro", Brand = "Nike", Description = "Phong cách bóng rổ đường phố 80s quyến rũ.", Price = 3100000, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1512374382149-233c42b6a83b?q=80&w=400", IsHot = true },
                new ProductItem { Id = 8, Name = "Adidas Forum Low White", Brand = "Adidas", Description = "Quai dán cổ điển mang đậm chất cổ điển.", Price = 2500000, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1582588678413-dbf45f4823e9?q=80&w=400", IsHot = false },
                new ProductItem { Id = 9, Name = "Jordan 4 Retro Thunder", Brand = "Jordan", Description = "Phối màu vàng đen cực kỳ cuốn hút.", Price = 5200000, Sizes = "41,42,43", ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400", IsHot = true },
                new ProductItem { Id = 10, Name = "Puma Suede Classic", Brand = "Puma", Description = "Chất liệu da lộn êm ái sang trọng.", Price = 1750000, Sizes = "38,39,40", ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400", IsHot = false },
                new ProductItem { Id = 11, Name = "Converse Run Star Hike", Brand = "Converse", Description = "Đế răng cưa hầm hố tôn dáng ấn tượng.", Price = 2450000, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?q=80&w=400", IsHot = false },
                new ProductItem { Id = 12, Name = "Vans Sk8-Hi Reissue", Brand = "Vans", Description = "Giày cổ cao bảo vệ cổ chân thể thao năng động.", Price = 2100000, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400", IsHot = true }
            };
        }
    }
}