using System;
using System.Web.UI;
using ShoeStore.Data;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public enum CardActionMode { None, QuickAdd, Full }

    public partial class ProductCard : System.Web.UI.UserControl
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsHot { get; set; }
        public CardActionMode ActionMode { get; set; }
        private WishlistStore Wishlist { get { return new WishlistStore(Session); } }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            string url = ResolveUrl("~/ChiTietSanPham.aspx") + "?id=" + ProductId;
            lnkImage.HRef = url;
            lnkName.HRef = url;
            imgThumb.ImageUrl = ResolveUrl(ImageUrl);
            imgThumb.AlternateText = ProductName ?? "";
            litCardName.Text = ProductName ?? "";
            litCardDesc.Text = Description ?? "";
            litCardPrice.Text = Money.Format(Price);
            pnlHot.Visible = IsHot;
            btnQuickAdd.Visible = ActionMode == CardActionMode.QuickAdd;
            btnQuickAdd.CommandArgument = ProductId.ToString();
            pnlFullActions.Visible = ActionMode == CardActionMode.Full;
            btnAddToCart.CommandArgument = ProductId.ToString();
            btnBuyNow.CommandArgument = ProductId.ToString();
            btnHeart.CommandArgument = ProductId.ToString();
            RenderHeart(Wishlist.Contains(ProductId));
        }

        protected void btnHeart_Click(object sender, EventArgs e)
        {
            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this, "Vui lòng đăng nhập để lưu sản phẩm vào danh sách yêu thích!");
                return;
            }
            int id;
            if (!int.TryParse(btnHeart.CommandArgument, out id) || id <= 0) return;
            bool liked = Wishlist.Toggle(id);
            RenderHeart(liked);
            Toast.Success(this, liked ? "Đã thêm sản phẩm vào danh sách yêu thích." : "Đã bỏ sản phẩm khỏi danh sách yêu thích.");
        }

        private void RenderHeart(bool liked)
        {
            btnHeart.Text = liked ? "♥" : "♡";
            btnHeart.CssClass = liked ? "btn-heart-wishlist active" : "btn-heart-wishlist";
            btnHeart.ToolTip = AuthHelper.IsLoggedIn() ? (liked ? "Bỏ khỏi yêu thích" : "Thêm vào yêu thích") : "Đăng nhập để lưu vào yêu thích";
        }
    }
}
