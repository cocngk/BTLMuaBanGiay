using System;
using ShoeStore.Data;
using ShoeStore.Helpers;
using System.Web.UI;

namespace ShoeStore
{
    public enum CardActionMode
    {
        None,
        QuickAdd,
        Full
    }

    /// <summary>
    /// Thẻ sản phẩm dùng chung. Gán props (hỗ trợ Eval trong Repeater),
    /// control tự render ở OnDataBinding. Nút bấm bubble Command về Repeater cha.
    /// Riêng nút tim tự xử lý postback tại đây: nó phải chạy ở cả những trang
    /// không có handler ItemCommand (khối "Sản phẩm liên quan").
    /// </summary>
    public partial class ProductCard : System.Web.UI.UserControl
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsHot { get; set; }
        public CardActionMode ActionMode { get; set; }

        /// <summary>Seam duy nhất tới danh sách yêu thích — không đụng Session[...] trực tiếp.</summary>
        private WishlistStore Wishlist
        {
            get { return new WishlistStore(Session); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            // URL tuyệt đối từ gốc app: không bao giờ rớt vào /Controls/...
            string detailUrl = ResolveUrl("~/ChiTietSanPham.aspx") + "?id=" + ProductId;
            lnkImage.HRef = detailUrl;
            lnkName.HRef = detailUrl;

            imgThumb.ImageUrl = ResolveUrl(ImageUrl);
            imgThumb.AlternateText = ProductName ?? "";
            litCardName.Text = ProductName ?? "";
            litCardDesc.Text = Description ?? "";
            litCardPrice.Text = Money.Format(Price);
            pnlHot.Visible = IsHot;

            btnQuickAdd.Visible = (ActionMode == CardActionMode.QuickAdd);
            btnQuickAdd.CommandArgument = ProductId.ToString();

            pnlFullActions.Visible = (ActionMode == CardActionMode.Full);
            btnAddToCart.CommandArgument = ProductId.ToString();
            btnBuyNow.CommandArgument = ProductId.ToString();

            // CommandArgument nằm trong ViewState nên đọc lại được ở postback —
            // thuộc tính ProductId thì không, vì trang chỉ bind khi !IsPostBack.
            btnHeart.CommandArgument = ProductId.ToString();
            RenderHeart(ProductId > 0 && Wishlist.Contains(ProductId));
        }

        protected void btnHeart_Click(object sender, EventArgs e)
        {
            int productId;
            if (!int.TryParse(btnHeart.CommandArgument, out productId) || productId <= 0)
            {
                return;
            }

            bool liked = Wishlist.Toggle(productId);
            RenderHeart(liked);

            Toast.Success(this, liked
                ? "Đã thêm sản phẩm vào danh sách yêu thích."
                : "Đã bỏ sản phẩm khỏi danh sách yêu thích.");
        }

        /// <summary>Trạng thái tim phải đi theo dữ liệu, không chỉ đổi màu cho vui.</summary>
        private void RenderHeart(bool liked)
        {
            btnHeart.Text = liked ? "\u2665" : "\u2661";
            btnHeart.CssClass = liked ? "btn-heart-wishlist active" : "btn-heart-wishlist";
            btnHeart.ToolTip = liked ? "Bỏ khỏi yêu thích" : "Thêm vào yêu thích";
        }
    }
}
