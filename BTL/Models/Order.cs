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
        public static string NewCode()
        {
            Random random = new Random();
            return "SX-" + DateTime.Now.ToString("yyMMdd") + "-" + random.Next(1000, 9999);
        }
    }
}