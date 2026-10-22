using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore.Models
{
    /// <summary>
    /// Deep Cart domain module: owns lines (keyed by Id+Size), totals and
    /// stock clamping. Pure logic — no HttpContext, no Session — so this is
    /// the unit-test surface. Persistence lives behind Data.CartStore.
    /// </summary>
    public class Cart
    {
        private readonly List<CartItem> lines;

        public Cart() : this(null)
        {
        }

        public Cart(IEnumerable<CartItem> existing)
        {
            lines = existing != null ? new List<CartItem>(existing) : new List<CartItem>();
        }

        public IReadOnlyList<CartItem> Lines => lines;

        public bool IsEmpty => lines.Count == 0;

        /// <summary>
        /// Chuẩn hoá số lượng về [1, maxAllowed]. maxAllowed là số còn mua được,
        /// không phải tồn kho thô — xem RemainingToBuy.
        /// </summary>
        public static int ClampQty(int qty, int maxAllowed)
        {
            int max = maxAllowed > 0 ? maxAllowed : 1;
            if (qty < 1) qty = 1;
            if (qty > max) qty = max;
            return qty;
        }

        /// <summary>
        /// Tổng số đơn vị của sản phẩm đang có trong giỏ, cộng mọi size.
        /// Stock là của cả sản phẩm nên phải tính tổng: clamp từng dòng riêng lẻ
        /// sẽ cho size 40 × 12 + size 41 × 12 = 24 đôi dù kho chỉ còn 12.
        /// </summary>
        public int ProductQuantity(int id)
        {
            return lines.Where(c => c.Id == id).Sum(c => c.Quantity);
        }

        /// <summary>
        /// Số đơn vị còn mua được = tồn kho − phần đã có trong giỏ.
        /// excludeSize là size của dòng đang chỉnh, để không tự trừ chính nó.
        /// </summary>
        public int RemainingToBuy(int id, int stock, string excludeSize)
        {
            int inCart = ProductQuantity(id);
            if (excludeSize != null)
            {
                inCart -= lines.Where(c => c.Id == id && c.Size == excludeSize).Sum(c => c.Quantity);
            }
            return stock - inCart;
        }

        public void AddOrMerge(int id, string name, decimal price, string size, int qty, string imageUrl, int stock)
        {
            CartItem existing = lines.FirstOrDefault(c => c.Id == id && c.Size == size);
            if (existing != null)
            {
                existing.Quantity = ClampQty(existing.Quantity + qty, RemainingToBuy(id, stock, size));
            }
            else
            {
                lines.Add(new CartItem
                {
                    Id = id,
                    Name = name,
                    Price = price,
                    Quantity = ClampQty(qty, RemainingToBuy(id, stock, null)),
                    Size = size,
                    ImageUrl = imageUrl
                });
            }
        }

        /// <summary>Removes exactly one size line. Returns false if absent.</summary>
        public bool Remove(int id, string size)
        {
            return lines.RemoveAll(c => c.Id == id && c.Size == size) > 0;
        }

        public void SetQuantity(int id, string size, int qty, int stock)
        {
            CartItem existing = lines.FirstOrDefault(c => c.Id == id && c.Size == size);
            if (existing != null)
            {
                existing.Quantity = ClampQty(qty, RemainingToBuy(id, stock, size));
            }
        }

        public decimal SubTotal()
        {
            return lines.Sum(item => item.Price * item.Quantity);
        }

        public int TotalQuantity()
        {
            return lines.Sum(item => item.Quantity);
        }

        public void Clear()
        {
            lines.Clear();
        }
    }
}
