using System.Collections.Generic;
using System.Web.SessionState;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Lưu đơn hàng trong session. Cùng kiểu seam như SessionCartStore: module Order
    /// không bao giờ chạm Session trực tiếp. Khi thêm database thì chỉ thay lớp này.
    /// </summary>
    public sealed class SessionOrderStore
    {
        private const string Key = "Orders";

        private readonly HttpSessionState session;

        public SessionOrderStore(HttpSessionState session)
        {
            this.session = session;
        }

        private List<Order> All()
        {
            return session[Key] as List<Order> ?? new List<Order>();
        }

        /// <summary>Mới nhất lên đầu để tra cứu nhanh.</summary>
        public void Save(Order order)
        {
            if (order == null) return;

            List<Order> orders = All();
            orders.Insert(0, order);
            session[Key] = orders;
        }

        public List<Order> Recent()
        {
            return All();
        }

        public Order Latest()
        {
            List<Order> orders = All();
            return orders.Count > 0 ? orders[0] : null;
        }
    }
}