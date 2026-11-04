using System;
using System.Web.UI;
using ShoeStore.Data;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public enum CardActionMode { None, QuickAdd, Full }

    public partial class TheSanPham : System.Web.UI.UserControl
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsHot { get; set; }
        public CardActionMode ActionMode { get; set; }
        private WishlistStore DanhSachYeuThich { get { return new WishlistStore(Session); } }

        // Xử lý sự kiện vòng đời của điều khiển.
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            string tenSanPham = ProductName ?? "";
            string duongDan = ResolveUrl("~/ChiTietSanPham.aspx") + "?id=" + ProductId;
            lnkImage.HRef = duongDan;
            lnkName.HRef = duongDan;
            imgThumb.ImageUrl = ResolveUrl(ImageUrl);
            imgThumb.AlternateText = tenSanPham;
            litCardName.Text = tenSanPham;
            litCardDesc.Text = Description ?? "";
            litCardPrice.Text = Money.Format(Price);
            pnlHot.Visible = IsHot;
            btnQuickAdd.Visible = ActionMode == CardActionMode.QuickAdd;
            btnQuickAdd.CommandArgument = ProductId.ToString();
            pnlFullActions.Visible = ActionMode == CardActionMode.Full;
            btnAddToCart.CommandArgument = ProductId.ToString();
            btnBuyNow.CommandArgument = ProductId.ToString();
            btnHeart.CommandArgument = ProductId.ToString();
            RenderHeart(DanhSachYeuThich.Contains(ProductId));
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnHeart_Click(object nguonSuKien, EventArgs e)
        {
            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this, "Vui lòng đăng nhập để lưu sản phẩm vào danh sách yêu thích!");
                return;
            }
            int ma;
            if (!int.TryParse(btnHeart.CommandArgument, out ma) || ma <= 0) return;
            bool daThich = DanhSachYeuThich.Toggle(ma);
            RenderHeart(daThich);
            Toast.Success(this, daThich ? "Đã thêm sản phẩm vào danh sách yêu thích." : "Đã bỏ sản phẩm khỏi danh sách yêu thích.");
        }

        // Thực hiện việc hiển thị heart.
        private void RenderHeart(bool daThich)
        {
            btnHeart.Text = daThich ? "♥" : "♡";
            btnHeart.CssClass = daThich ? "btn-heart-wishlist active" : "btn-heart-wishlist";
            btnHeart.ToolTip = AuthHelper.IsLoggedIn() ? (daThich ? "Bỏ khỏi yêu thích" : "Thêm vào yêu thích") : "Đăng nhập để lưu vào yêu thích";
        }
    }
}
