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
        /// Seam duy nhất để đọc/ghi giỏ hàng — không ai đụng Session["Cart"] hay
        /// Application["Carts"] trực tiếp. Store tự chọn chỗ lưu: Application theo
        /// tài khoản khi đã đăng nhập, Session khi còn là khách.
        /// Đặt tên CartStore chứ không phải Cart để không che mất lớp Models.Cart.
        /// </summary>
        protected CartStore CartStore
        {
            get { return new CartStore(Session); }
        }

        /// <summary>Seam cho danh sách yêu thích — cùng kiểu với CartStore.</summary>
        protected WishlistStore Wishlist
        {
            get { return new WishlistStore(Session); }
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

        /// <summary>
        /// Chặn khu quản trị ở server. Ẩn link trên header chỉ là giao diện: gõ thẳng
        /// URL vẫn vào được, nên mỗi trang quản trị phải hỏi lại câu này ở Page_Load
        /// và ở mọi handler postback.
        /// Trang gọi phải để phần nội dung trong một Panel Visible=false rồi bật lên
        /// khi qua cửa — nếu chỉ return thì khung trang (tiêu đề, bảng rỗng) vẫn hiện.
        /// </summary>
        protected bool RequireAdmin()
        {
            if (AuthHelper.IsAdmin())
            {
                return true;
            }

            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this, "Vui lòng đăng nhập bằng tài khoản quản trị để vào khu này.");
                return false;
            }

            Toast.ShowAndRedirect(this,
                "Tài khoản của bạn không có quyền vào khu quản trị.",
                "info",
                "TrangChu.aspx");
            return false;
        }
    }
}