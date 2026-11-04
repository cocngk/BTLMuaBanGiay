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
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!IsPostBack) LoadProduct();
        }

        // Thực hiện việc tải sản phẩm.
        private void LoadProduct()
        {
            int ma;
            ProductItem sanPham = int.TryParse(Request.QueryString["id"], out ma) ? Catalog.Find(ma) : null;
            if (sanPham == null) { pnlProductDetail.Visible = false; pnlNotFound.Visible = true; return; }
            pnlProductDetail.Visible = true;
            pnlNotFound.Visible = false;
            imgProduct.ImageUrl = ResolveUrl(sanPham.LargeImageUrl());
            imgProduct.AlternateText = sanPham.Name;
            BindColourways(sanPham);
            litBrand.Text = sanPham.Brand;
            lnkViewAllRelated.NavigateUrl = "SanPham.aspx?brand=" + Server.UrlEncode(sanPham.Brand);
            lnkViewAllRelated.Text = "Xem thêm " + sanPham.Brand + " &rarr;";
            litName.Text = litBreadcrumb.Text = sanPham.Name;
            litSku.Text = "SNK-" + sanPham.Id.ToString("D4");
            litPrice.Text = Money.Format(sanPham.Price);
            litDescription.Text = sanPham.Description + " Được làm từ chất liệu cao cấp, độ bền hoàn hảo và thiết kế tối ưu nâng tầm phong cách thời trang năng động cho bạn.";
            ApplySeo(sanPham);
            if (sanPham.OldPrice.HasValue && sanPham.OldPrice.Value > sanPham.Price)
            {
                pnlDiscount.Visible = true;
                litOldPrice.Text = Money.Format(sanPham.OldPrice.Value);
                litDiscountPct.Text = "-" + (int)Math.Round((1 - sanPham.Price / sanPham.OldPrice.Value) * 100) + "%";
            }
            int max = MaxBuyable(sanPham);
            hfMaxQty.Value = max.ToString();
            if (sanPham.Stock <= 0)
            {
                litStock.Text = "<span class=\"stock-pill out\">Hết hàng</span>";
                btnAddToCart.Enabled = btnBuyNow.Enabled = txtQuantity.Enabled = false;
            }
            else if (sanPham.Stock <= 5) litStock.Text = "<span class=\"stock-pill low\">Sắp hết hàng — chỉ còn " + max + " đôi</span>";
            else litStock.Text = "<span class=\"stock-pill in\">Còn hàng • sẵn sàng giao ngay</span>";
            List<string> kichThuoc = sanPham.SizeList();
            if (kichThuoc.Count == 0) kichThuoc.Add(sanPham.FirstSize());
            hfSelectedSize.Value = kichThuoc[0];
            rptSizes.DataSource = kichThuoc.Select(x => new { Size = x }).ToList();
            rptSizes.DataBind();
            rptRelated.DataSource = Catalog.RelatedTo(sanPham.Id, 4);
            rptRelated.DataBind();
            RenderWishlist(sanPham.Id);
            LoadReviews(sanPham.Id);
        }

        // Thực hiện việc nạp màu sắc.
        private void BindColourways(ProductItem sanPham)
        {
            if (sanPham.Colourways == null || sanPham.Colourways.Count < 2) { pnlColourways.Visible = false; return; }
            pnlColourways.Visible = true;
            rptColourways.DataSource = sanPham.Colourways.Select((x, chiSo) => new { Name = x.Name, Small = x.Small, Large = x.Large, Active = chiSo == 0 ? " colourway-chip active" : " colourway-chip", Alt = sanPham.Name + " — màu " + x.Name }).ToList();
            rptColourways.DataBind();
        }

        // Thực hiện việc áp dụng seo.
        private void ApplySeo(ProductItem sanPham)
        {
            Title = sanPham.Name + " - " + sanPham.Brand + " | " + ShopInfo.BrandName;
            HtmlMeta meta = new HtmlMeta { Name = "description", Content = sanPham.Name + " chính hãng " + sanPham.Brand + " - " + sanPham.Description };
            Header.Controls.Add(meta);
        }

        // Thực hiện việc hiển thị yêu thích.
        private void RenderWishlist(int ma)
        {
            bool daThich = DanhSachYeuThich.Contains(ma);
            btnWishlist.Text = daThich ? "♥" : "♡";
            btnWishlist.CssClass = daThich ? "btn-wishlist-detail active" : "btn-wishlist-detail";
            btnWishlist.ToolTip = !AuthHelper.IsLoggedIn() ? "Đăng nhập để lưu vào yêu thích" : (daThich ? "Bỏ khỏi yêu thích" : "Thêm vào yêu thích");
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnWishlist_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để lưu sản phẩm vào danh sách yêu thích!")) return;
            int ma;
            if (int.TryParse(Request.QueryString["id"], out ma)) { DanhSachYeuThich.Toggle(ma); RenderWishlist(ma); }
        }

        // Thực hiện việc max buyable.
        private int MaxBuyable(ProductItem sanPham) { return CartService.MaxBuyable(CartStore.Load(), sanPham.Id); }

        // Thực hiện việc selected size.
        private string SelectedSize(ProductItem sanPham)
        {
            return sanPham.SizeList().Contains(hfSelectedSize.Value) ? hfSelectedSize.Value : sanPham.FirstSize();
        }

        // Thực hiện việc số lượng.
        private int Quantity(ProductItem sanPham)
        {
            int giaTri;
            return Cart.ClampQty(int.TryParse(txtQuantity.Text, out giaTri) ? giaTri : 1, MaxBuyable(sanPham));
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnAddToCart_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!")) return;
            ProductItem sanPham = ProductFromQuery();
            if (sanPham == null) return;
            if (sanPham.Stock <= 0)
            {
                Toast.Info(this, "Sản phẩm này hiện đã hết hàng.");
                return;
            }

            string kichThuoc = SelectedSize(sanPham);
            CartStore.AddProduct(sanPham, kichThuoc, Quantity(sanPham));
            Toast.Success(this, "Đã thêm " + sanPham.Name + " (Size " + kichThuoc + ") vào giỏ hàng!");
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnBuyNow_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiến hành thanh toán!")) return;
            ProductItem sanPham = ProductFromQuery();
            if (sanPham == null) return;
            if (sanPham.Stock <= 0)
            {
                Toast.Info(this, "Sản phẩm này hiện đã hết hàng.");
                return;
            }

            CartStore.AddProduct(sanPham, SelectedSize(sanPham), Quantity(sanPham));
            Redirect("ThanhToan.aspx");
        }

        // Thực hiện việc sản phẩm chuyển đổi từ query.
        private ProductItem ProductFromQuery()
        {
            int ma;
            return int.TryParse(Request.QueryString["id"], out ma) ? Catalog.Find(ma) : null;
        }

        // Thực hiện việc has purchased.
        private bool HasPurchased(int ma)
        {
            if (!AuthHelper.IsLoggedIn()) return false;
            return new ApplicationOrderStore().ByUser(AuthHelper.CurrentUserKey()).Any(o => o.Status == Order.DeliveredStatus && (o.Lines ?? new List<OrderLine>()).Any(x => x.ProductId == ma));
        }

        // Thực hiện việc tải đánh giá.
        private void LoadReviews(int ma)
        {
            List<ProductReview> reviews = ProductReviewStore.GetByProduct(ma);
            litAverageRating.Text = ProductReviewStore.GetAverage(ma).ToString("0.0");
            litReviewCount.Text = reviews.Count + " đánh giá";
            rptReviews.DataSource = reviews;
            rptReviews.DataBind();
            pnlNoReviews.Visible = reviews.Count == 0;
            bool canReview = HasPurchased(ma);
            pnlReviewForm.Visible = canReview;
            pnlReviewLogin.Visible = !canReview;
            if (canReview)
            {
                ProductReview danhGia = ProductReviewStore.Find(ma, AuthHelper.CurrentUserKey());
                if (danhGia != null) { ddlRating.SelectedValue = danhGia.Rating.ToString(); txtReview.Text = danhGia.Comment; }
            }
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnSubmitReview_Click(object nguonSuKien, EventArgs e)
        {
            int ma, rating;
            if (!AuthHelper.IsLoggedIn()) { litReviewMessage.Text = "Bạn cần đăng nhập trước khi đánh giá."; return; }
            if (!int.TryParse(Request.QueryString["id"], out ma) || !HasPurchased(ma)) { litReviewMessage.Text = "Bạn cần mua và nhận sản phẩm trước khi đánh giá."; return; }
            if (!int.TryParse(ddlRating.SelectedValue, out rating) || rating < 1 || rating > 5) { litReviewMessage.Text = "Vui lòng chọn số sao từ 1 đến 5."; return; }
            string comment = txtReview.Text.Trim();
            if (comment.Length < 5) { litReviewMessage.Text = "Nhận xét cần có ít nhất 5 ký tự."; return; }
            if (comment.Length > 1000) { litReviewMessage.Text = "Nhận xét không được vượt quá 1000 ký tự."; return; }
            UserAccount taiKhoan = AuthHelper.CurrentAccount();
            if (taiKhoan == null) { litReviewMessage.Text = "Không tìm thấy thông tin tài khoản."; return; }
            ProductReviewStore.Save(new ProductReview { ProductId = ma, UserKey = AuthHelper.CurrentUserKey(), CustomerName = taiKhoan.DisplayName, Rating = rating, Comment = comment, CreatedAt = DateTime.Now });
            LoadReviews(ma);
            litReviewMessage.Text = "Cảm ơn bạn! Đánh giá đã được gửi thành công.";
        }
    }
}
