using System;
using System.Collections.Generic;
using System.Linq;
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
            int id;

            if (Request.QueryString["id"] != null &&
                int.TryParse(Request.QueryString["id"], out id))
            {
                ProductItem product = Catalog.Find(id);

                if (product != null)
                {
                    pnlProductDetail.Visible = true;
                    pnlNotFound.Visible = false;

                    imgProduct.ImageUrl = ResolveUrl(product.LargeImageUrl());
                    BindColourways(product);
                    imgProduct.AlternateText = product.Name;

                    litBrand.Text = product.Brand;
                    lnkViewAllRelated.NavigateUrl =
                        "SanPham.aspx?brand=" + Server.UrlEncode(product.Brand);
                    lnkViewAllRelated.Text =
                        "Xem thêm " + product.Brand + " &rarr;";

                    litName.Text = product.Name;
                    litBreadcrumb.Text = product.Name;
                    litSku.Text = "SNK-" + product.Id.ToString("D4");
                    litPrice.Text = Money.Format(product.Price);

                    litDescription.Text = product.Description +
                        " Được làm từ chất liệu cao cấp, độ bền hoàn hảo và thiết kế tối ưu nâng tầm phong cách thời trang năng động cho bạn.";

                    ApplySeo(product);

                    if (product.OldPrice.HasValue &&
                        product.OldPrice.Value > product.Price)
                    {
                        pnlDiscount.Visible = true;
                        litOldPrice.Text = Money.Format(product.OldPrice.Value);

                        int pct = (int)Math.Round(
                            (1 - product.Price / product.OldPrice.Value) * 100);

                        litDiscountPct.Text = "-" + pct + "%";
                    }

                    int canBuy = MaxBuyable(product);
                    hfMaxQty.Value = canBuy.ToString();

                    if (product.Stock <= 0)
                    {
                        litStock.Text =
                            "<span class=\"stock-pill out\">Hết hàng</span>";

                        btnAddToCart.Enabled = false;
                        btnBuyNow.Enabled = false;
                        txtQuantity.Enabled = false;
                    }
                    else if (product.Stock <= 5)
                    {
                        litStock.Text =
                            "<span class=\"stock-pill low\">Sắp hết hàng — chỉ còn " +
                            canBuy + " đôi</span>";
                    }
                    else
                    {
                        litStock.Text =
                            "<span class=\"stock-pill in\">Còn hàng • sẵn sàng giao ngay</span>";
                    }

                    List<string> sizes = product.SizeList();

                    if (sizes.Count == 0)
                        sizes.Add(product.FirstSize());

                    hfSelectedSize.Value = sizes[0];

                    rptSizes.DataSource =
                        sizes.Select(s => new { Size = s }).ToList();

                    rptSizes.DataBind();

                    rptRelated.DataSource = Catalog.RelatedTo(product.Id, 4);
                    rptRelated.DataBind();

                    RenderWishlist(product.Id);

                    // Tải đánh giá của sản phẩm
                    LoadReviews(product.Id);
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

            return !string.IsNullOrEmpty(hfSelectedSize.Value) &&
                   sizes.Contains(hfSelectedSize.Value)
                ? hfSelectedSize.Value
                : product.FirstSize();
        }

        private void BindColourways(ProductItem product)
        {
            if (product.Colourways == null ||
                product.Colourways.Count < 2)
            {
                pnlColourways.Visible = false;
                return;
            }

            pnlColourways.Visible = true;

            rptColourways.DataSource = product.Colourways
                .Select((c, i) => new
                {
                    Name = c.Name,
                    Small = c.Small,
                    Large = c.Large,
                    Active = i == 0
                        ? " colourway-chip active"
                        : " colourway-chip",
                    Alt = product.Name + " — màu " + c.Name
                })
                .ToList();

            rptColourways.DataBind();
        }

        private void ApplySeo(ProductItem product)
        {
            this.Title = product.Name + " - " +
                product.Brand + " | " + ShopInfo.BrandName;

            HtmlMeta meta = new HtmlMeta();
            meta.Name = "description";
            meta.Content = product.Name + " chính hãng " +
                product.Brand + " - " + product.Description;

            this.Header.Controls.Add(meta);
        }

        private void RenderWishlist(int productId)
        {
            bool liked = Wishlist.Contains(productId);

            btnWishlist.Text = liked ? "\u2665" : "\u2661";

            btnWishlist.CssClass = liked
                ? "btn-wishlist-detail active"
                : "btn-wishlist-detail";

            btnWishlist.ToolTip = liked
                ? "Bỏ khỏi yêu thích"
                : "Thêm vào yêu thích";

            if (!AuthHelper.IsLoggedIn())
            {
                btnWishlist.ToolTip =
                    "Đăng nhập để lưu vào yêu thích";
            }
        }

        protected void btnWishlist_Click(object sender, EventArgs e)
        {
            if (!RequireLogin(
                "Vui lòng đăng nhập để lưu sản phẩm vào danh sách yêu thích!"))
            {
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);

            Wishlist.Toggle(id);
            RenderWishlist(id);
        }

        private int MaxBuyable(ProductItem product)
        {
            return CartService.MaxBuyable(
                CartStore.Load(),
                product.Id);
        }

        private int ResolveQty(ProductItem product)
        {
            int qty = int.TryParse(txtQuantity.Text, out int q)
                ? (q > 0 ? q : 1)
                : 1;

            return Cart.ClampQty(qty, MaxBuyable(product));
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (!RequireLogin(
                "Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!"))
            {
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            ProductItem product = Catalog.Find(id);

            if (product != null)
            {
                string size = ResolveSize(product);

                CartStore.AddProduct(
                    product,
                    size,
                    ResolveQty(product));

                Toast.Success(this,
                    "Đã thêm " + product.Name +
                    " (Size " + size + ") vào giỏ hàng!");
            }
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            if (!RequireLogin(
                "Vui lòng đăng nhập để tiến hành thanh toán!"))
            {
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            ProductItem product = Catalog.Find(id);

            if (product != null)
            {
                string size = ResolveSize(product);

                CartStore.AddProduct(
                    product,
                    size,
                    ResolveQty(product));

                Redirect("ThanhToan.aspx");
            }
        }

        // Kiểm tra khách hàng đã mua và nhận sản phẩm hay chưa
        private bool HasPurchasedProduct(int productId)
        {
            if (!AuthHelper.IsLoggedIn())
                return false;

            string userKey = AuthHelper.CurrentUserKey();

            List<Order> orders =
                new ApplicationOrderStore().ByUser(userKey);

            return orders.Any(order =>
                order.Status == Order.DeliveredStatus &&
                order.Lines != null &&
                order.Lines.Any(line =>
                    line.ProductId == productId));
        }

        // Hiển thị đánh giá của sản phẩm
        private void LoadReviews(int productId)
        {
            List<ProductReview> reviews =
                ProductReviewStore.GetByProduct(productId);

            litAverageRating.Text =
                ProductReviewStore.GetAverage(productId).ToString("0.0");

            litReviewCount.Text = reviews.Count + " đánh giá";

            rptReviews.DataSource = reviews;
            rptReviews.DataBind();

            pnlNoReviews.Visible = reviews.Count == 0;

            bool canReview = HasPurchasedProduct(productId);

            pnlReviewForm.Visible = canReview;
            pnlReviewLogin.Visible = !canReview;

            if (canReview)
            {
                ProductReview oldReview = ProductReviewStore.Find(
                    productId,
                    AuthHelper.CurrentUserKey());

                if (oldReview != null)
                {
                    ddlRating.SelectedValue =
                        oldReview.Rating.ToString();

                    txtReview.Text = oldReview.Comment;
                }
            }
        }

        // Gửi đánh giá
        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (!AuthHelper.IsLoggedIn())
            {
                litReviewMessage.Text =
                    "Bạn cần đăng nhập trước khi đánh giá.";

                return;
            }

            int productId;

            if (!int.TryParse(
                Request.QueryString["id"],
                out productId))
            {
                return;
            }

            // Kiểm tra lại đơn hàng ở phía máy chủ
            if (!HasPurchasedProduct(productId))
            {
                litReviewMessage.Text =
                    "Bạn cần mua và nhận sản phẩm trước khi đánh giá.";

                return;
            }

            int rating;

            if (!int.TryParse(ddlRating.SelectedValue, out rating) ||
                rating < 1 || rating > 5)
            {
                litReviewMessage.Text =
                    "Vui lòng chọn số sao từ 1 đến 5.";

                return;
            }

            string comment = txtReview.Text.Trim();

            if (comment.Length < 5)
            {
                litReviewMessage.Text =
                    "Nhận xét cần có ít nhất 5 ký tự.";

                return;
            }

            if (comment.Length > 1000)
            {
                litReviewMessage.Text =
                    "Nhận xét không được vượt quá 1000 ký tự.";

                return;
            }

            UserAccount account = AuthHelper.CurrentAccount();

            if (account == null)
            {
                litReviewMessage.Text =
                    "Không tìm thấy thông tin tài khoản.";

                return;
            }

            ProductReview review = new ProductReview
            {
                ProductId = productId,
                UserKey = AuthHelper.CurrentUserKey(),
                CustomerName = account.DisplayName,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            ProductReviewStore.Save(review);

            // Tải lại danh sách rồi mới hiện thông báo thành công
            LoadReviews(productId);

            litReviewMessage.Text =
                "Cảm ơn bạn! Đánh giá đã được gửi thành công.";
        }
    }
}