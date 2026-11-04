using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!IsPostBack) LoadProduct();
        }

        private void LoadProduct()
        {
            int id;
            ProductItem product = int.TryParse(Request.QueryString["id"], out id) ? Catalog.Find(id) : null;
            if (product == null) { pnlProductDetail.Visible = false; pnlNotFound.Visible = true; return; }
            pnlProductDetail.Visible = true;
            pnlNotFound.Visible = false;
            imgProduct.ImageUrl = ResolveUrl(product.LargeImageUrl());
            imgProduct.AlternateText = product.Name;
            BindColourways(product);
            litBrand.Text = product.Brand;
            lnkViewAllRelated.NavigateUrl = "SanPham.aspx?brand=" + Server.UrlEncode(product.Brand);
            lnkViewAllRelated.Text = "Xem thêm " + product.Brand + " &rarr;";
            litName.Text = litBreadcrumb.Text = product.Name;
            litSku.Text = "SNK-" + product.Id.ToString("D4");
            litPrice.Text = Money.Format(product.Price);
            litDescription.Text = product.Description + " Được làm từ chất liệu cao cấp, độ bền hoàn hảo và thiết kế tối ưu nâng tầm phong cách thời trang năng động cho bạn.";
            ApplySeo(product);
            if (product.OldPrice.HasValue && product.OldPrice.Value > product.Price)
            {
                pnlDiscount.Visible = true;
                litOldPrice.Text = Money.Format(product.OldPrice.Value);
                litDiscountPct.Text = "-" + (int)Math.Round((1 - product.Price / product.OldPrice.Value) * 100) + "%";
            }
            int max = MaxBuyable(product);
            hfMaxQty.Value = max.ToString();
            if (product.Stock <= 0)
            {
                litStock.Text = "<span class=\"stock-pill out\">Hết hàng</span>";
                btnAddToCart.Enabled = btnBuyNow.Enabled = txtQuantity.Enabled = false;
            }
            else if (product.Stock <= 5) litStock.Text = "<span class=\"stock-pill low\">Sắp hết hàng — chỉ còn " + max + " đôi</span>";
            else litStock.Text = "<span class=\"stock-pill in\">Còn hàng • sẵn sàng giao ngay</span>";
            List<string> sizes = product.SizeList();
            if (sizes.Count == 0) sizes.Add(product.FirstSize());
            hfSelectedSize.Value = sizes[0];
            rptSizes.DataSource = sizes.Select(x => new { Size = x }).ToList();
            rptSizes.DataBind();
            rptRelated.DataSource = Catalog.RelatedTo(product.Id, 4);
            rptRelated.DataBind();
            RenderWishlist(product.Id);
            LoadReviews(product.Id);
        }

        private void BindColourways(ProductItem product)
        {
            if (product.Colourways == null || product.Colourways.Count < 2) { pnlColourways.Visible = false; return; }
            pnlColourways.Visible = true;
            rptColourways.DataSource = product.Colourways.Select((x, i) => new { Name = x.Name, Small = x.Small, Large = x.Large, Active = i == 0 ? " colourway-chip active" : " colourway-chip", Alt = product.Name + " — màu " + x.Name }).ToList();
            rptColourways.DataBind();
        }

        private void ApplySeo(ProductItem product)
        {
            Title = product.Name + " - " + product.Brand + " | " + ShopInfo.BrandName;
            HtmlMeta meta = new HtmlMeta { Name = "description", Content = product.Name + " chính hãng " + product.Brand + " - " + product.Description };
            Header.Controls.Add(meta);
        }

        private void RenderWishlist(int id)
        {
            bool liked = Wishlist.Contains(id);
            btnWishlist.Text = liked ? "♥" : "♡";
            btnWishlist.CssClass = liked ? "btn-wishlist-detail active" : "btn-wishlist-detail";
            btnWishlist.ToolTip = !AuthHelper.IsLoggedIn() ? "Đăng nhập để lưu vào yêu thích" : (liked ? "Bỏ khỏi yêu thích" : "Thêm vào yêu thích");
        }

        protected void btnWishlist_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để lưu sản phẩm vào danh sách yêu thích!")) return;
            int id;
            if (int.TryParse(Request.QueryString["id"], out id)) { Wishlist.Toggle(id); RenderWishlist(id); }
        }

        private int MaxBuyable(ProductItem product) { return CartService.MaxBuyable(CartStore.Load(), product.Id); }

        private string SelectedSize(ProductItem product)
        {
            return product.SizeList().Contains(hfSelectedSize.Value) ? hfSelectedSize.Value : product.FirstSize();
        }

        private int Quantity(ProductItem product)
        {
            int value;
            return Cart.ClampQty(int.TryParse(txtQuantity.Text, out value) ? value : 1, MaxBuyable(product));
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!")) return;
            ProductItem product = ProductFromQuery();
            if (product == null) return;
            string size = SelectedSize(product);
            CartStore.AddProduct(product, size, Quantity(product));
            Toast.Success(this, "Đã thêm " + product.Name + " (Size " + size + ") vào giỏ hàng!");
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiến hành thanh toán!")) return;
            ProductItem product = ProductFromQuery();
            if (product == null) return;
            CartStore.AddProduct(product, SelectedSize(product), Quantity(product));
            Redirect("ThanhToan.aspx");
        }

        private ProductItem ProductFromQuery()
        {
            int id;
            return int.TryParse(Request.QueryString["id"], out id) ? Catalog.Find(id) : null;
        }

        private bool HasPurchased(int id)
        {
            if (!AuthHelper.IsLoggedIn()) return false;
            return new ApplicationOrderStore().ByUser(AuthHelper.CurrentUserKey()).Any(o => o.Status == Order.DeliveredStatus && (o.Lines ?? new List<OrderLine>()).Any(x => x.ProductId == id));
        }

        private void LoadReviews(int id)
        {
            List<ProductReview> reviews = ProductReviewStore.GetByProduct(id);
            litAverageRating.Text = ProductReviewStore.GetAverage(id).ToString("0.0");
            litReviewCount.Text = reviews.Count + " đánh giá";
            rptReviews.DataSource = reviews;
            rptReviews.DataBind();
            pnlNoReviews.Visible = reviews.Count == 0;
            bool canReview = HasPurchased(id);
            pnlReviewForm.Visible = canReview;
            pnlReviewLogin.Visible = !canReview;
            if (canReview)
            {
                ProductReview review = ProductReviewStore.Find(id, AuthHelper.CurrentUserKey());
                if (review != null) { ddlRating.SelectedValue = review.Rating.ToString(); txtReview.Text = review.Comment; }
            }
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            int id, rating;
            if (!AuthHelper.IsLoggedIn()) { litReviewMessage.Text = "Bạn cần đăng nhập trước khi đánh giá."; return; }
            if (!int.TryParse(Request.QueryString["id"], out id) || !HasPurchased(id)) { litReviewMessage.Text = "Bạn cần mua và nhận sản phẩm trước khi đánh giá."; return; }
            if (!int.TryParse(ddlRating.SelectedValue, out rating) || rating < 1 || rating > 5) { litReviewMessage.Text = "Vui lòng chọn số sao từ 1 đến 5."; return; }
            string comment = txtReview.Text.Trim();
            if (comment.Length < 5) { litReviewMessage.Text = "Nhận xét cần có ít nhất 5 ký tự."; return; }
            if (comment.Length > 1000) { litReviewMessage.Text = "Nhận xét không được vượt quá 1000 ký tự."; return; }
            UserAccount account = AuthHelper.CurrentAccount();
            if (account == null) { litReviewMessage.Text = "Không tìm thấy thông tin tài khoản."; return; }
            ProductReviewStore.Save(new ProductReview { ProductId = id, UserKey = AuthHelper.CurrentUserKey(), CustomerName = account.DisplayName, Rating = rating, Comment = comment, CreatedAt = DateTime.Now });
            LoadReviews(id);
            litReviewMessage.Text = "Cảm ơn bạn! Đánh giá đã được gửi thành công.";
        }
    }
}
