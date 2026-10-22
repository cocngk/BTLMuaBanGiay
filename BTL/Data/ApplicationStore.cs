using System;
using System.Collections.Generic;
using System.Web;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Cổng duy nhất chạm HttpApplicationState — cùng kiểu seam với SessionCartStore.
    /// Trạng thái cấp ứng dụng dùng chung cho mọi phiên và sống qua các request,
    /// nên mọi thao tác đọc-sửa-ghi đều đi qua một khóa duy nhất.
    /// </summary>
    internal static class ApplicationStore
    {
        /// <summary>Tên khóa đặt một chỗ — không rải chuỗi "Products"/"Orders" khắp nơi.</summary>
        public const string ProductsKey = "Products";
        public const string UsersKey = "UserList";
        public const string OrdersKey = "Orders";

        /// <summary>Khóa chung cho mọi read-modify-write trên Application.</summary>
        public static readonly object Gate = new object();

        /// <summary>
        /// Kho tạm khi không có HttpContext (chạy ngoài IIS, unit test). Không phải
        /// nguồn dữ liệu thật — chỉ để code không ném NullReferenceException.
        /// </summary>
        private static readonly Dictionary<string, object> Fallback = new Dictionary<string, object>();

        /// <summary>
        /// Lấy giá trị theo khóa, chưa có thì tạo bằng factory rồi ghi lại.
        /// Hai request đồng thời không thể tạo hai kho khác nhau.
        /// </summary>
        public static T GetOrCreate<T>(string key, Func<T> factory) where T : class
        {
            lock (Gate)
            {
                HttpApplicationState state = Current;
                if (state == null)
                {
                    object cached;
                    if (!Fallback.TryGetValue(key, out cached) || cached == null)
                    {
                        cached = factory();
                        Fallback[key] = cached;
                    }
                    return (T)cached;
                }

                T value = state[key] as T;
                if (value == null)
                {
                    value = factory();
                    state[key] = value;
                }
                return value;
            }
        }

        /// <summary>
        /// Nạp sẵn trạng thái lúc khởi động (Global.asax). Idempotent: gọi lại
        /// không ghi đè dữ liệu đang có.
        /// </summary>
        public static void Initialize(HttpApplicationState state)
        {
            if (state == null)
            {
                return;
            }

            lock (Gate)
            {
                if (!(state[ProductsKey] is List<ProductItem>))
                {
                    state[ProductsKey] = ProductSeed.Items();
                }

                if (!(state[UsersKey] is MemoryUserStore))
                {
                    state[UsersKey] = new MemoryUserStore();
                }

                if (!(state[OrdersKey] is List<Order>))
                {
                    state[OrdersKey] = new List<Order>();
                }
            }
        }

        /// <summary>Null khi chạy ngoài pipeline ASP.NET — phía gọi tự rơi về Fallback.</summary>
        private static HttpApplicationState Current
        {
            get
            {
                HttpContext context = HttpContext.Current;
                return context != null ? context.Application : null;
            }
        }
    }
}
