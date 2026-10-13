using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class TrangChu : System.Web.UI.Page
    {
        public class ProductItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
            public bool IsHot { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadHomeProducts();
            }
        }

        private void LoadHomeProducts()
        {
            rptProducts.DataSource = GetMockProducts();
            rptProducts.DataBind();
        }

        // BẤM NÚT "KHÁM PHÁ NGAY" TRÊN HERO BANNER
        protected void btnHeroDiscover_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HomeHeroRequireLogin",
                    "showToast('Vui lòng đăng nhập hoặc đăng ký tài khoản để khám phá cửa hàng!', 'info'); openLoginModal();", true);
                return;
            }

            Response.Redirect("SanPham.aspx");
        }

        // BẤM NÚT DẤU CỘNG Ở CÁC SẢN PHẨM TRANG CHỦ
        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "QuickAdd")
            {
                // 1. KIỂM TRA ĐĂNG NHẬP
                if (Session["Username"] == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "HomeRequireLoginToast",
                        "showToast('Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!', 'info'); openLoginModal();", true);
                    return;
                }

                // 2. THÊM SẢN PHẨM VÀO GIỎ HÀNG
                int productId = Convert.ToInt32(e.CommandArgument);
                List<ProductItem> allProducts = GetMockProducts();
                ProductItem selected = allProducts.FirstOrDefault(p => p.Id == productId);

                if (selected != null)
                {
                    List<GioHang.CartItem> cart = Session["Cart"] as List<GioHang.CartItem> ?? new List<GioHang.CartItem>();
                    GioHang.CartItem existing = cart.FirstOrDefault(c => c.Id == productId);

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
                            Size = "40",
                            ImageUrl = selected.ImageUrl
                        });
                    }

                    Session["Cart"] = cart;

                    string msg = "Đã thêm " + selected.Name.Replace("'", "\\'") + " (Size 40) vào giỏ hàng!";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "HomeAddToCartSuccessToast",
                        "showToast('" + msg + "', 'success');", true);
                }
            }
        }

        private List<ProductItem> GetMockProducts()
        {
            return new List<ProductItem>
            {
                new ProductItem { Id = 1, Name = "Nike Air Force 1 '07", Description = "Êm ái, kiểu dáng thời trang đường phố.", Price = 2900000, ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?q=80&w=400", IsHot = true },
                new ProductItem { Id = 2, Name = "Adidas Ultraboost Light", Description = "Trọng lượng nhẹ, hoàn trả năng lượng.", Price = 3800000, ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400", IsHot = false },
                new ProductItem { Id = 3, Name = "Jordan 1 Retro High", Description = "Mẫu giày bóng rổ huyền thoại.", Price = 4500000, ImageUrl = "https://images.unsplash.com/photo-1556906781-9a412961c28c?q=80&w=400", IsHot = false },
                new ProductItem { Id = 4, Name = "Puma RS-X Sport", Description = "Xinh xắn, cá tính cho các dịp vui chơi.", Price = 1800000, ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400", IsHot = false }
            };
        }
    }
}