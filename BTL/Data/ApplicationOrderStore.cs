using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Seam lưu đơn: OrderIntake chỉ biết interface này, nên đổi kho (Application,
    /// SQL, file) không phải sửa luồng đặt hàng.
    /// </summary>
    public interface IOrderStore
    {
        void Save(Order order);
        List<Order> All();
        List<Order> ByUser(string username);
        Order FindByCode(string code);
    }

    /// <summary>
    /// Đơn hàng ở cấp ứng dụng: đơn của mọi khách nằm cùng một chỗ, nên xem lại
    /// được sau khi đóng trình duyệt hoặc đăng nhập ở máy khác — khác với giỏ hàng,
    /// vốn phải riêng từng người nên vẫn nằm trong Session.
    /// </summary>
    public sealed class ApplicationOrderStore : IOrderStore
    {
        private static List<Order> Inner
        {
            get
            {
                return ApplicationStore.GetOrCreate(
                    ApplicationStore.OrdersKey,
                    () => new List<Order>());
            }
        }

        public void Save(Order order)
        {
            if (order == null)
            {
                return;
            }

            lock (ApplicationStore.Gate)
            {
                Inner.Insert(0, order); // đơn mới nhất lên đầu
            }
        }

        /// <summary>Bản sao — trả list thật thì nơi gọi ghi được mà không qua khóa.</summary>
        public List<Order> All()
        {
            lock (ApplicationStore.Gate)
            {
                return new List<Order>(Inner);
            }
        }

        public List<Order> ByUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new List<Order>();
            }

            return All()
                .Where(o => string.Equals(o.Username, username, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Order FindByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            string target = code.Trim();
            lock (ApplicationStore.Gate)
            {
                return Inner.FirstOrDefault(
                    o => string.Equals(o.Code, target, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
