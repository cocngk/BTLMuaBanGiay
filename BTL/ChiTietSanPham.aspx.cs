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
    public partial class ChiTietSanPham : ShopPage
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

                    imgProduct.ImageUrl = ResolveUrl(product.LargeImageUrl());
                    BindGallery(product);
                    imgProduct.AlternateText = product.Name;
                    litBrand.Text = product.Brand;
                    lnkViewAllRelated.NavigateUrl = "SanPham.aspx?brand=" + Server.UrlEncode(product.Brand);
                    lnkViewAllRelated.Text = "Xem thêm " + product.Brand + " &rarr;";
                    litName.Text = product.Name;
                    litBreadcrumb.Text = product.Name;
                    litSku.Text = "SNK-" + product.Id.ToString("D4");
                    litPrice.Text = Money.Format(product.Price);
                    litDescription.Text = product.Description + " Được làm từ chất liệu cao cấp, độ bền hoàn hảo và thiết kế tối ưu nâng tầm phong cách thời trang năng động cho bạn.";

                    ApplySeo(product);

                    // Giá gốc / giảm giá
                    if (product.OldPrice.HasValue && product.OldPrice.Value > product.Price)
                    {
                        pnlDiscount.Visible = true;
                        litOldPrice.Text = Money.Format(product.OldPrice.Value);
                        int pct = (int)Math.Round((1 - product.Price / product.OldPrice.Value) * 100);
                        litDiscountPct.Text = "-" + pct + "%";
                    }

                    // Tồn kho
                    hfMaxQty.Value = MaxBuyable(product).ToString();
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
                    List<string> sizes = product.SizeList();
                    if (sizes.Count == 0) sizes.Add(product.FirstSize());
                    hfSelectedSize.Value = sizes[0];
                    rptSizes.DataSource = sizes.Select(s => new { Size = s }).ToList();
                    rptSizes.DataBind();

                    // Sản phẩm liên quan: luật nằm trong ProductStore, trang chỉ lấy ra.
                    rptRelated.DataSource = ProductStore.RelatedTo(product, 4);
                    rptRelated.DataBind();

                    RenderWishlist(product.Id);
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

        private string ResolveSize(ProductItem product)
        {
            List<string> sizes = product.SizeList();
            return !string.IsNullOrEmpty(hfSelectedSize.Value) && sizes.Contains(hfSelectedSize.Value)
                ? hfSelectedSize.Value
                : product.FirstSize();
        }

        /// <summary>
        /// Dựng dải thumbnail từ dữ liệu. Chỉ hiện khi sản phẩm thật sự có nhiều
        /// góc nhìn — không dựng ảnh giả chỉ để lấp chỗ.
        /// </summary>
        private void BindGallery(ProductItem product)
        {
            if (product.Gallery == null || product.Gallery.Count < 2)
            {
                pnlGallery.Visible = false;
                return;
            }

            pnlGallery.Visible = true;
            rptThumbs.DataSource = product.Gallery
                .Select((g, i) => new { Small = g.Small, Large = g.Large, Alt = product.Name + " — góc " + (i + 1) })
                .ToList();
            rptThumbs.DataBind();
        }

        /// <summary>Title và meta description lấy theo sản phẩm đang xem.</summary>
        private void ApplySeo(ProductItem product)
        {
            this.Title = product.Name + " - " + product.Brand + " | " + ShopInfo.BrandName;

            HtmlMeta meta = new HtmlMeta();
            meta.Name = "description";
            meta.Content = product.Name + " chính hãng " + product.Brand + " - " + product.Description;
            this.Header.Controls.Add(meta);
        }

        /// <summary>Vẽ nút yêu thích theo trạng thái đã lưu, không phải đổi màu cho vui.</summary>
        private void RenderWishlist(int productId)
        {
            bool liked = Wishlist.Contains(productId);
            btnWishlist.Text = liked ? "\u2665" : "\u2661";
            btnWishlist.CssClass = liked ? "btn-wishlist-detail active" : "btn-wishlist-detail";
            btnWishlist.ToolTip = liked ? "Bỏ khỏi yêu thích" : "Thêm vào yêu thích";
        }

        protected void btnWishlist_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            Wishlist.Toggle(id);
            RenderWishlist(id);
        }

        /// <summary>
        /// Số đơn vị còn mua được của sản phẩm này: tồn kho trừ phần đã có trong giỏ
        /// (cộng mọi size). Dùng chung một luật với trang giỏ hàng qua Cart.RemainingToBuy
        /// để nút +/- ở đây không cho thêm quá hàng tồn.
        /// </summary>
        private int MaxBuyable(ProductItem product)
        {
            Cart cart = CartStore.Load();
            int remaining = cart.RemainingToBuy(product.Id, product.Stock, null);
            return remaining > 0 ? remaining : 1;
        }

        private int ResolveQty(ProductItem product)
        {
            int qty = int.TryParse(txtQuantity.Text, out int q) ? (q > 0 ? q : 1) : 1;
            return Cart.ClampQty(qty, MaxBuyable(product));
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!"))
            {
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            ProductItem product = ProductStore.GetAll().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                string size = ResolveSize(product);
                CartStore.AddOrMerge(product.Id, product.Name, product.Price, size, ResolveQty(product), product.ImageUrl, product.Stock);
                Toast.Success(this, "DetailToastSuccess",
                    "Đã thêm " + product.Name + " (Size " + size + ") vào giỏ hàng!");
            }
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiến hành thanh toán!"))
            {
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            ProductItem product = ProductStore.GetAll().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                // Gộp vào giỏ hiện có (không xóa giỏ cũ) rồi sang thanh toán
                string size = ResolveSize(product);
                CartStore.AddOrMerge(product.Id, product.Name, product.Price, size, ResolveQty(product), product.ImageUrl, product.Stock);
                Redirect("ThanhToan.aspx");
            }
        }

    }
}
