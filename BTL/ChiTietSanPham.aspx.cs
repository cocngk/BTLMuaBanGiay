using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace ShoeStore
{
    public partial class ChiTietSanPham : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProductDetails();
            }
        }

        private void LoadProductDetails()
        {
            if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int id))
            {
                List<SanPham.ProductItem> list = GetMockProducts();
                SanPham.ProductItem product = list.FirstOrDefault(p => p.Id == id);

                if (product != null)
                {
                    pnlProductDetail.Visible = true;
                    pnlNotFound.Visible = false;

                    imgProduct.ImageUrl = product.ImageUrl;
                    litBrand.Text = product.Brand;
                    litName.Text = product.Name;
                    litPrice.Text = string.Format("{0:N0}đ", product.Price);
                    litDescription.Text = product.Description + " Được làm từ chất liệu cao cấp, độ bền hoàn hảo và thiết kế tối ưu nâng tầm phong cách thời trang năng động cho bạn.";
                }
                else
                {
                    pnlProductDetail.Visible = false;
                    pnlNotFound.Visible = true;
                }
            }
            else
            {
                pnlProductDetail.Visible = false;
                pnlNotFound.Visible = true;
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "RequireLoginDetailToast",
                    "showToast('Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!', 'info'); openLoginModal();", true);
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            SanPham.ProductItem product = GetMockProducts().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                int qty = int.TryParse(txtQuantity.Text, out int q) ? (q > 0 ? q : 1) : 1;
                string size = string.IsNullOrEmpty(hfSelectedSize.Value) ? "40" : hfSelectedSize.Value;

                List<GioHang.CartItem> cart = Session["Cart"] as List<GioHang.CartItem> ?? new List<GioHang.CartItem>();
                GioHang.CartItem existing = cart.FirstOrDefault(c => c.Id == id && c.Size == size);

                if (existing != null)
                {
                    existing.Quantity += qty;
                }
                else
                {
                    cart.Add(new GioHang.CartItem
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = qty,
                        Size = size,
                        ImageUrl = product.ImageUrl
                    });
                }

                Session["Cart"] = cart;

                string msg = "Đã thêm " + product.Name.Replace("'", "\\'") + " (Size " + size + ") vào giỏ hàng!";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DetailToastSuccess", "showToast('" + msg + "', 'success');", true);
            }
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "RequireLoginBuyNowToast",
                    "showToast('Vui lòng đăng nhập để tiến hành thanh toán!', 'info'); openLoginModal();", true);
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            SanPham.ProductItem product = GetMockProducts().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                int qty = int.TryParse(txtQuantity.Text, out int q) ? (q > 0 ? q : 1) : 1;
                string size = string.IsNullOrEmpty(hfSelectedSize.Value) ? "40" : hfSelectedSize.Value;

                List<GioHang.CartItem> singleCart = new List<GioHang.CartItem>
                {
                    new GioHang.CartItem
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = qty,
                        Size = size,
                        ImageUrl = product.ImageUrl
                    }
                };

                Session["Cart"] = singleCart;
                Response.Redirect("ThanhToan.aspx");
            }
        }

        private List<SanPham.ProductItem> GetMockProducts()
        {
            return new List<SanPham.ProductItem>
            {
                new SanPham.ProductItem { Id = 1, Name = "Nike Air Force 1 '07", Brand = "Nike", Description = "Thiết kế kinh điển, đệm Air siêu êm.", Price = 2900000, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?q=80&w=400" },
                new SanPham.ProductItem { Id = 2, Name = "Adidas Ultraboost Light", Brand = "Adidas", Description = "Trọng lượng siêu nhẹ, êm ái khi chạy bộ.", Price = 3800000, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400" },
                new SanPham.ProductItem { Id = 3, Name = "Jordan 1 Retro High Chicago", Brand = "Jordan", Description = "Phối màu huyền thoại dành cho tín đồ Sneaker.", Price = 4500000, Sizes = "40,41,42,43", ImageUrl = "https://images.unsplash.com/photo-1556906781-9a412961c28c?q=80&w=400" },
                new SanPham.ProductItem { Id = 4, Name = "Puma RS-X Reinvent Red", Brand = "Puma", Description = "Cá tính, phối màu năng động cá tính.", Price = 1800000, Sizes = "38,39,40", ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400" },
                new SanPham.ProductItem { Id = 5, Name = "Converse Chuck 70 High", Brand = "Converse", Description = "Phong cách Retro Vintage tối giản cổ điển.", Price = 1950000, Sizes = "38,39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?q=80&w=400" },
                new SanPham.ProductItem { Id = 6, Name = "Vans Old Skool Black", Brand = "Vans", Description = "Mẫu giày Skateboard trường tồn với thời gian.", Price = 1850000, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400" }
            };
        }
    }
}