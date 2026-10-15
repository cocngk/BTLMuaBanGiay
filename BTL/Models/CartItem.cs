using System;

namespace ShoeStore.Models
{
    /// <summary>
    /// One line in the shopping cart. Top-level (was nested in GioHang page,
    /// forcing every consumer through the GioHang.CartItem seam).
    /// </summary>
    [Serializable]
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
