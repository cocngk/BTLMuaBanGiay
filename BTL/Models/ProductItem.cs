using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore.Models
{
    /// <summary>Model sản phẩm dùng chung cho mọi trang.</summary>
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
        /// <summary>
        /// Đường dẫn bắt đầu bằng ~ để ResolveUrl() luôn ra gốc ứng dụng.
        /// Không có ~ thì ResolveUrl bên trong UserControl sẽ ghép vào /Controls/,
        /// và ảnh trong thẻ sản phẩm sẽ hỏng.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>Ảnh lớn cho trang chi tiết (ảnh phóng to 1.8×), mặc định lấy luôn ảnh nhỏ.</summary>
        public string ImageUrlLarge { get; set; }

        /// <summary>
        /// Các màu của sản phẩm (màu đầu tiên là màu đang hiển thị, và cũng
        /// chính là ImageUrlLarge). Rỗng thì trang chi tiết chỉ hiện một ảnh —
        /// đừng dựng ảnh giả chỉ để lấp chỗ trống.
        /// </summary>
        public List<ProductColourway> Colourways { get; set; }

        /// <summary>Ảnh lớn có sẵn không — nếu không thì dùng ảnh nhỏ.</summary>
        public string LargeImageUrl()
        {
            return string.IsNullOrEmpty(ImageUrlLarge) ? ImageUrl : ImageUrlLarge;
        }
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
