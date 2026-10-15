using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore.Models
{
    /// <summary>
    /// Deep Cart domain module: owns lines (keyed by Id+Size), totals and
    /// stock clamping. Pure logic — no HttpContext, no Session — so this is
    /// the unit-test surface. Persistence lives behind SessionCartStore.
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

        public static int ClampQty(int qty, int stock)
        {
            int max = stock > 0 ? stock : 1;
            if (qty < 1) qty = 1;
            if (qty > max) qty = max;
            return qty;
        }

        public void AddOrMerge(int id, string name, decimal price, string size, int qty, string imageUrl, int stock)
        {
            qty = ClampQty(qty, stock);
            CartItem existing = lines.FirstOrDefault(c => c.Id == id && c.Size == size);
            if (existing != null)
            {
                existing.Quantity = ClampQty(existing.Quantity + qty, stock);
            }
            else
            {
                lines.Add(new CartItem
                {
                    Id = id,
                    Name = name,
                    Price = price,
                    Quantity = qty,
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
                existing.Quantity = ClampQty(qty, stock);
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
