using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

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

                    string mainUrl = product.ImageUrl.Replace("w=400", "w=1200");
                    imgProduct.ImageUrl = mainUrl;
                    imgProduct.AlternateText = product.Name;
                    litBrand.Text = product.Brand;
                    litName.Text = product.Name;
                    litBreadcrumb.Text = product.Name;
                    litSku.Text = "SNK-" + product.Id.ToString("D4");
                    litPrice.Text = string.Format("{0:N0}đ", product.Price);
                    litDescription.Text = product.Description + " Được làm từ chất liệu cao cấp, độ bền hoàn hảo và thiết kế tối ưu nâng tầm phong cách thời trang năng động cho bạn.";

                    // SEO: title + meta description theo sản phẩm
                    this.Title = product.Name + " - " + product.Brand + " | SneakerX";
                    HtmlMeta meta = new HtmlMeta();
                    meta.Name = "description";
                    meta.Content = product.Name + " chính hãng " + product.Brand + " - " + product.Description;
                    this.Header.Controls.Add(meta);

                    // Rating giả lập ổn định theo Id
                    double score = 4.0 + (id % 3) * 0.5;
                    litAvgScore.Text = score.ToString("0.0");
                    litRatingStars.Text = RenderStars(score);
                    litReviewCount.Text = (id * 37 + 12).ToString();

                    // Giá gốc / giảm giá
                    if (product.OldPrice.HasValue && product.OldPrice.Value > product.Price)
                    {
                        pnlDiscount.Visible = true;
                        litOldPrice.Text = string.Format("{0:N0}đ", product.OldPrice.Value);
                        int pct = (int)Math.Round((1 - product.Price / product.OldPrice.Value) * 100);
                        litDiscountPct.Text = "-" + pct + "%";
                    }

                    // Tồn kho
                    hfMaxQty.Value = Math.Max(product.Stock, 1).ToString();
                    if (product.Stock <= 0)
                    {
                        litStock.Text = "<span class=\"stock-pill out\">Hết hàng</span>";
                        btnAddToCart.Enabled = false;
                        btnBuyNow.Enabled = false;
                        txtQuantity.Enabled = false;
                    }
                    else if (product.Stock <= 5)
                    {
                        litStock.Text = "<span class=\"stock-pill low\">Sắp hết hàng — chỉ còn " + product.Stock + " đôi</span>";
                    }
                    else
                    {
                        litStock.Text = "<span class=\"stock-pill in\">Còn hàng • sẵn sàng giao ngay</span>";
                    }

                    // Size theo data thật, mặc định size đầu tiên
                    List<string> sizes = (product.Sizes ?? "")
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()).ToList();
                    if (sizes.Count == 0) sizes.Add("40");
                    hfSelectedSize.Value = sizes[0];
                    rptSizes.DataSource = sizes.Select(s => new { Size = s }).ToList();
                    rptSizes.DataBind();

                    // Gallery: 3 góc crop khác nhau từ cùng 1 ảnh gốc
                    string base64 = product.ImageUrl.Replace("w=400", "w=200");
                    string[] crops = { "entropy", "top", "bottom" };
                    rptThumbs.DataSource = crops.Select((c, i) => new
                    {
                        Index = i,
                        Url = base64 + "&fit=crop&crop=" + c,
                        Alt = product.Name + " - góc " + (i + 1)
                    }).ToList();
                    rptThumbs.DataBind();

                    // Sản phẩm liên quan: cùng brand trước, rồi lấp đầy
                    List<SanPham.ProductItem> related = list
                        .Where(p => p.Id != id)
                        .OrderByDescending(p => p.Brand == product.Brand)
                        .ThenBy(p => p.Id)
                        .Take(4).ToList();
                    rptRelated.DataSource = related;
                    rptRelated.DataBind();
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

        private string RenderStars(double score)
        {
            int full = (int)Math.Floor(score);
            bool half = (score - full) >= 0.5;
            string s = new string('★', full);
            if (half) s += "☆";
            return s.PadRight(5, '☆');
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
                qty = Math.Min(qty, Math.Max(product.Stock, 1));
                List<string> sizes = (product.Sizes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                string size = !string.IsNullOrEmpty(hfSelectedSize.Value) && sizes.Contains(hfSelectedSize.Value)
                    ? hfSelectedSize.Value
                    : sizes.FirstOrDefault() ?? "40";

                List<GioHang.CartItem> cart = Session["Cart"] as List<GioHang.CartItem> ?? new List<GioHang.CartItem>();
                GioHang.CartItem existing = cart.FirstOrDefault(c => c.Id == id && c.Size == size);

                if (existing != null)
                {
                    existing.Quantity = Math.Min(existing.Quantity + qty, Math.Max(product.Stock, 1));
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
                // Gộp vào giỏ hiện có (không xóa giỏ cũ) rồi sang thanh toán
                int qty = int.TryParse(txtQuantity.Text, out int q) ? (q > 0 ? q : 1) : 1;
                qty = Math.Min(qty, Math.Max(product.Stock, 1));
                List<string> sizes = (product.Sizes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                string size = !string.IsNullOrEmpty(hfSelectedSize.Value) && sizes.Contains(hfSelectedSize.Value)
                    ? hfSelectedSize.Value
                    : sizes.FirstOrDefault() ?? "40";

                List<GioHang.CartItem> cart = Session["Cart"] as List<GioHang.CartItem> ?? new List<GioHang.CartItem>();
                GioHang.CartItem existing = cart.FirstOrDefault(c => c.Id == id && c.Size == size);
                if (existing != null)
                {
                    existing.Quantity = Math.Min(existing.Quantity + qty, Math.Max(product.Stock, 1));
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
                Response.Redirect("ThanhToan.aspx");
            }
        }

        private List<SanPham.ProductItem> GetMockProducts()
        {
            return new List<SanPham.ProductItem>
            {
                new SanPham.ProductItem { Id = 1, Name = "Nike Air Force 1 '07", Brand = "Nike", Description = "Thiết kế kinh điển, đệm Air siêu êm.", Price = 2900000, OldPrice = 3500000, Stock = 12, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?q=80&w=400" },
                new SanPham.ProductItem { Id = 2, Name = "Adidas Ultraboost Light", Brand = "Adidas", Description = "Trọng lượng siêu nhẹ, êm ái khi chạy bộ.", Price = 3800000, Stock = 8, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400" },
                new SanPham.ProductItem { Id = 3, Name = "Jordan 1 Retro High Chicago", Brand = "Jordan", Description = "Phối màu huyền thoại dành cho tín đồ Sneaker.", Price = 4500000, Stock = 3, Sizes = "40,41,42,43", ImageUrl = "https://images.unsplash.com/photo-1556906781-9a412961c28c?q=80&w=400" },
                new SanPham.ProductItem { Id = 4, Name = "Puma RS-X Reinvent Red", Brand = "Puma", Description = "Cá tính, phối màu năng động cá tính.", Price = 1800000, OldPrice = 2200000, Stock = 15, Sizes = "38,39,40", ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400" },
                new SanPham.ProductItem { Id = 5, Name = "Converse Chuck 70 High", Brand = "Converse", Description = "Phong cách Retro Vintage tối giản cổ điển.", Price = 1950000, Stock = 0, Sizes = "38,39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?q=80&w=400" },
                new SanPham.ProductItem { Id = 6, Name = "Vans Old Skool Black", Brand = "Vans", Description = "Mẫu giày Skateboard trường tồn với thời gian.", Price = 1850000, Stock = 20, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400" }
            };
        }
    }
}
