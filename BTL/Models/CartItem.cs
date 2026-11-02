using System;

namespace ShoeStore.Models
{
    /// <summary>Một dòng trong giỏ hàng: sản phẩm, size và số lượng.</summary>
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
