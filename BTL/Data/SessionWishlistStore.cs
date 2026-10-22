using System.Collections.Generic;
using System.Web.SessionState;

namespace ShoeStore.Data
{
    /// <summary>
    /// Danh sách yêu thích trong session. Cùng kiểu seam như SessionCartStore:
    /// khi thêm database chỉ thay lớp này.
    /// Yêu thích phải riêng từng người nên không đưa vào Application (dùng chung
    /// toàn site sẽ khiến khách A thấy tim của khách B).
    /// Trước đây nút tim trên trang chi tiết chỉ đổi màu icon, tải lại là mất.
    /// </summary>
    public sealed class SessionWishlistStore
    {
        private const string Key = "Wishlist";

        private readonly HttpSessionState session;

        public SessionWishlistStore(HttpSessionState session)
        {
            this.session = session;
        }

        private HashSet<int> All()
        {
            return session[Key] as HashSet<int> ?? new HashSet<int>();
        }

        public bool Contains(int productId)
        {
            return All().Contains(productId);
        }

        /// <summary>
        /// Xoá sạch — gọi khi đăng xuất. Session gắn với trình duyệt chứ không gắn
        /// với tài khoản, nên tim của người vừa thoát vẫn nằm đó cho người đăng
        /// nhập sau trên cùng máy.
        /// </summary>
        public void Clear()
        {
            session.Remove(Key);
        }

        /// <summary>Bật/tắt một mục. Trả về true nếu sau khi bật.</summary>
        public bool Toggle(int productId)
        {
            HashSet<int> items = All();
            if (!items.Remove(productId))
            {
                items.Add(productId);
            }
            session[Key] = items;
            return items.Contains(productId);
        }
    }
}