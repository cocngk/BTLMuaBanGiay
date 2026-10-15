using System;
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

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            // URL tuyệt đối từ gốc app: không bao giờ rớt vào /Controls/...
            string detailUrl = ResolveUrl("~/ChiTietSanPham.aspx") + "?id=" + ProductId;
            lnkImage.HRef = detailUrl;
            lnkName.HRef = detailUrl;

            imgThumb.ImageUrl = ImageUrl;
            imgThumb.AlternateText = ProductName ?? "";
            litCardName.Text = ProductName ?? "";
            litCardDesc.Text = Description ?? "";
            litCardPrice.Text = string.Format("{0:N0}đ", Price);
            pnlHot.Visible = IsHot;

            btnQuickAdd.Visible = (ActionMode == CardActionMode.QuickAdd);
            btnQuickAdd.CommandArgument = ProductId.ToString();

            pnlFullActions.Visible = (ActionMode == CardActionMode.Full);
            btnAddToCart.CommandArgument = ProductId.ToString();
            btnBuyNow.CommandArgument = ProductId.ToString();
        }
    }
}
