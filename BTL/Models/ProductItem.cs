namespace ShoeStore.Models
{
    /// <summary>
    /// Single product model shared by all pages (was triplicated in
    /// TrangChu / SanPham / ChiTietSanPham code-behinds).
    /// </summary>
    public class ProductItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int Stock { get; set; } = 10;
        public string Sizes { get; set; }
        public string ImageUrl { get; set; }
        public bool IsHot { get; set; }
    }
}
