using System.Collections.Generic;
using System.Web.SessionState;

namespace ShoeStore.Data
{
    /// <summary>
    /// Danh sách yêu thích trong session. Cùng kiểu seam như SessionCartStore và
    /// SessionOrderStore: khi thêm database chỉ thay lớp này.
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