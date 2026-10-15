using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

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
                List<ProductItem> list = ProductStore.GetAll();
                ProductItem product = list.FirstOrDefault(p => p.Id == id);

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
                    List<ProductItem> related = list
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

        private string ResolveSize(ProductItem product)
        {
            List<string> sizes = (product.Sizes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            return !string.IsNullOrEmpty(hfSelectedSize.Value) && sizes.Contains(hfSelectedSize.Value)
                ? hfSelectedSize.Value
                : sizes.FirstOrDefault() ?? "40";
        }

        private int ResolveQty(ProductItem product)
        {
            int qty = int.TryParse(txtQuantity.Text, out int q) ? (q > 0 ? q : 1) : 1;
            return Math.Min(qty, Math.Max(product.Stock, 1));
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (!CartHelper.IsLoggedIn())
            {
                CartHelper.RequireLoginToast(this, "RequireLoginDetailToast",
                    "Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!");
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            ProductItem product = ProductStore.GetAll().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                string size = ResolveSize(product);
                CartHelper.AddOrMerge(product.Id, product.Name, product.Price, size, ResolveQty(product), product.ImageUrl, product.Stock);
                CartHelper.SuccessToast(this, "DetailToastSuccess",
                    "Đã thêm " + product.Name + " (Size " + size + ") vào giỏ hàng!");
            }
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (!CartHelper.IsLoggedIn())
            {
                CartHelper.RequireLoginToast(this, "RequireLoginBuyNowToast",
                    "Vui lòng đăng nhập để tiến hành thanh toán!");
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            ProductItem product = ProductStore.GetAll().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                // Gộp vào giỏ hiện có (không xóa giỏ cũ) rồi sang thanh toán
                string size = ResolveSize(product);
                CartHelper.AddOrMerge(product.Id, product.Name, product.Price, size, ResolveQty(product), product.ImageUrl, product.Stock);
                Response.Redirect("ThanhToan.aspx");
            }
        }

    }
}
