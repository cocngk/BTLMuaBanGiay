using System.Collections.Generic;
using System.Linq;

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

        /// <summary>Single source for "which size if none chosen". Replaces magic "40" scattered in pages.</summary>
        public List<string> SizeList()
        {
            return (Sizes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToList();
        }

        public string FirstSize()
        {
            List<string> sizes = SizeList();
            return sizes.Count > 0 ? sizes[0] : "40";
        }
    }
}
