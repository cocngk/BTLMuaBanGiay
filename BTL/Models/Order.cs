using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore.Models
{
    /// <summary>
    /// Một dòng đơn hàng — chụp lại tại thời điểm đặt, không tham chiếu giỏ hàng.
    /// Cần chụp chứ không giữ tham chiếu, vì giỏ sẽ bị xoá ngay sau khi đặt.
    /// </summary>
    public class OrderLine
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal()
        {
            return Price * Quantity;
        }

        public static List<OrderLine> FromCart(IEnumerable<CartItem> lines)
        {
            return lines.Select(c => new OrderLine
            {
                ProductId = c.Id,
                Name = c.Name,
                Size = c.Size,
                Price = c.Price,
                Quantity = c.Quantity
            }).ToList();
        }
    }

    /// <summary>
    /// Đơn hàng đã đặt. Trước đây đặt xong là Clear() giỏ rồi thoát, khách không
    /// có mã đơn để tra và hệ thống không lưu gì.
    /// </summary>
    public class Order
    {
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Khóa tài khoản đã đặt (AuthHelper.CurrentUserKey). Cần để lọc
        /// "Đơn hàng của tôi" — tên hiển thị không đủ tin cậy vì có thể trùng.
        /// </summary>
        public string Username { get; set; }

        /// <summary>Trạng thái đơn. Đơn mới luôn ở mức đầu; khu quản trị đổi các mức sau.</summary>
        public string Status { get; set; } = PendingStatus;

        // Bốn mức trạng thái của một đơn. Gom ở đây để trang quản trị, trang khách
        // và luật huỷ đơn cùng đọc một chỗ — trước đây chỉ có một chuỗi viết trong
        // Order và một bản sao trong trang Đơn hàng.
        public const string PendingStatus = "Chờ xác nhận";
        public const string ShippingStatus = "Đang giao";
        public const string DeliveredStatus = "Đã giao";
        public const string CancelledStatus = "Đã huỷ";

        /// <summary>Thứ tự hiển thị trong dropdown, cũng là thứ tự một đơn đi qua.</summary>
        public static List<string> AllStatuses()
        {
            return new List<string>
            {
                PendingStatus,
                ShippingStatus,
                DeliveredStatus,
                CancelledStatus
            };
        }

        public static bool IsKnownStatus(string status)
        {
            return AllStatuses().Contains(status);
        }

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderLine> Lines { get; set; }

        public decimal Total()
        {
            return Lines.Sum(l => l.LineTotal());
        }

        public int TotalQuantity()
        {
            return Lines.Sum(l => l.Quantity);
        }

        /// <summary>Mã đơn dạng SX-261017-4821: ngày đặt + số ngẫu nhiên.</summary>
        /// <remarks>
        /// Dùng một Random dùng chung có khóa: <c>new Random()</c> lấy seed từ
        /// Environment.TickCount, nên hai đơn đặt trong cùng một tick sẽ ra cùng
        /// một mã — tra cứu đơn sẽ trả về đơn của người khác.
        /// </remarks>
        public static string NewCode()
        {
            lock (CodeRandom)
            {
                return "SX-" + DateTime.Now.ToString("yyMMdd") + "-" + CodeRandom.Next(1000, 10000);
            }
        }

        private static readonly Random CodeRandom = new Random();
    }
}