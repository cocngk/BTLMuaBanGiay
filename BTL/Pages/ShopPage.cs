using System;
using ShoeStore.Data;
using ShoeStore.Helpers;

namespace ShoeStore
{
    /// <summary>
    /// Base page cho mọi trang trong shop. Gom hai thứ từng lặp ở khắp site:
    /// guard đăng nhập và cách truy cập giỏ hàng. Tiền tệ thì dùng thẳng
    /// Helpers.Money.Format — không bọc lại, tránh trùng tên.
    /// Sửa quy tắc ở đây thì áp cho tất cả trang, không phải đi sửa từng bản sao.
    /// </summary>
    public class ShopPage : System.Web.UI.Page
    {
        /// <summary>
        /// Seam duy nhất để đọc/ghi giỏ hàng — không ai đụng Session["Cart"] trực tiếp.
        /// Đặt tên CartStore chứ không phải Cart để không che mất lớp Models.Cart.
        /// </summary>
        protected SessionCartStore CartStore
        {
            get { return new SessionCartStore(Session); }
        }

        /// <summary>Seam cho danh sách yêu thích — cùng kiểu với CartStore.</summary>
        protected SessionWishlistStore Wishlist
        {
            get { return new SessionWishlistStore(Session); }
        }

        /// <summary>
        /// Chuyển trang theo kiểu sạch: endResponse = false rồi CompleteRequest,
        /// để không ném ThreadAbortException giữa chừng handler.
        /// </summary>
        protected void Redirect(string url)
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Chặn trang khi khách chưa đăng nhập. Trả false kèm toast + mở form đăng nhập
        /// để phía gọi return ngay, thay vì tự lặp lại if (!IsLoggedIn) ở từng trang.
        /// </summary>
        protected bool RequireLogin(string message)
        {
            if (AuthHelper.IsLoggedIn())
            {
                return true;
            }

            Toast.RequireLogin(this, message);
            return false;
        }
    }
}