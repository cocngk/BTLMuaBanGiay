using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Single canonical mock catalog (was triplicated with diverging data).
    /// Replace with real DB access when a database is introduced.
    /// </summary>
    public static class ProductStore
    {
        public static List<ProductItem> GetAll()
        {
            return new List<ProductItem>
            {
                new ProductItem { Id = 1, Name = "Nike Air Force 1 '07", Brand = "Nike", Description = "Thiết kế kinh điển, đệm Air siêu êm.", Price = 2900000, OldPrice = 3500000, Stock = 12, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/nike-1-sm.jpg", ImageUrlLarge = "~/Content/images/products/nike-1-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/nike-1-sm.jpg", Large = "~/Content/images/products/nike-1-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g12-t.jpg", Large = "~/Content/images/gallery/g12.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g44-t.jpg", Large = "~/Content/images/gallery/g44.jpg" }, }, IsHot = true },
                new ProductItem { Id = 2, Name = "Adidas Ultraboost Light", Brand = "Adidas", Description = "Trọng lượng siêu nhẹ, êm ái khi chạy bộ.", Price = 3800000, Stock = 8, Sizes = "39,40,41,42", ImageUrl = "~/Content/images/products/adidas-2-sm.jpg", ImageUrlLarge = "~/Content/images/products/adidas-2-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/adidas-2-sm.jpg", Large = "~/Content/images/products/adidas-2-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g00-t.jpg", Large = "~/Content/images/gallery/g00.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g01-t.jpg", Large = "~/Content/images/gallery/g01.jpg" }, }, IsHot = true },
                new ProductItem { Id = 3, Name = "Jordan 1 Retro High Chicago", Brand = "Jordan", Description = "Phối màu huyền thoại dành cho tín đồ Sneaker.", Price = 4500000, Stock = 3, Sizes = "40,41,42,43", ImageUrl = "~/Content/images/products/jordan-3-sm.jpg", ImageUrlLarge = "~/Content/images/products/jordan-3-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/jordan-3-sm.jpg", Large = "~/Content/images/products/jordan-3-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g37-t.jpg", Large = "~/Content/images/gallery/g37.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g38-t.jpg", Large = "~/Content/images/gallery/g38.jpg" }, }, IsHot = false },
                new ProductItem { Id = 4, Name = "Puma RS-X Reinvent Red", Brand = "Puma", Description = "Cá tính, phối màu năng động cá tính.", Price = 1800000, OldPrice = 2200000, Stock = 15, Sizes = "38,39,40", ImageUrl = "~/Content/images/products/puma-4-sm.jpg", ImageUrlLarge = "~/Content/images/products/puma-4-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/puma-4-sm.jpg", Large = "~/Content/images/products/puma-4-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/m00-t.jpg", Large = "~/Content/images/gallery/m00.jpg" }, new ProductImage { Small = "~/Content/images/gallery/m01-t.jpg", Large = "~/Content/images/gallery/m01.jpg" }, }, IsHot = false },
                new ProductItem { Id = 5, Name = "Converse Chuck 70 High", Brand = "Converse", Description = "Phong cách Retro Vintage tối giản cổ điển.", Price = 1950000, Stock = 0, Sizes = "38,39,40,41,42", ImageUrl = "~/Content/images/products/converse-5-sm.jpg", ImageUrlLarge = "~/Content/images/products/converse-5-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/converse-5-sm.jpg", Large = "~/Content/images/products/converse-5-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/c02-t.jpg", Large = "~/Content/images/gallery/c02.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g14-t.jpg", Large = "~/Content/images/gallery/g14.jpg" }, }, IsHot = true },
                new ProductItem { Id = 6, Name = "Vans Old Skool Black", Brand = "Vans", Description = "Mẫu giày Skateboard trường tồn với thời gian.", Price = 1850000, Stock = 20, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/vans-6-sm.jpg", ImageUrlLarge = "~/Content/images/products/vans-6-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/vans-6-sm.jpg", Large = "~/Content/images/products/vans-6-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g07-t.jpg", Large = "~/Content/images/gallery/g07.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g11-t.jpg", Large = "~/Content/images/gallery/g11.jpg" }, }, IsHot = false },
                new ProductItem { Id = 7, Name = "Nike Dunk Low Retro", Brand = "Nike", Description = "Phong cách bóng rổ đường phố 80s quyến rũ.", Price = 3100000, Stock = 10, Sizes = "39,40,41,42", ImageUrl = "~/Content/images/products/nike-7-sm.jpg", ImageUrlLarge = "~/Content/images/products/nike-7-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/nike-7-sm.jpg", Large = "~/Content/images/products/nike-7-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g41-t.jpg", Large = "~/Content/images/gallery/g41.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g43-t.jpg", Large = "~/Content/images/gallery/g43.jpg" }, }, IsHot = true },
                new ProductItem { Id = 8, Name = "Adidas Forum Low White", Brand = "Adidas", Description = "Quai dán cổ điển mang đậm chất cổ điển.", Price = 2500000, Stock = 10, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/adidas-8-sm.jpg", ImageUrlLarge = "~/Content/images/products/adidas-8-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/adidas-8-sm.jpg", Large = "~/Content/images/products/adidas-8-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g02-t.jpg", Large = "~/Content/images/gallery/g02.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g03-t.jpg", Large = "~/Content/images/gallery/g03.jpg" }, }, IsHot = false },
                new ProductItem { Id = 9, Name = "Jordan 4 Retro Thunder", Brand = "Jordan", Description = "Phối màu vàng đen cực kỳ cuốn hút.", Price = 5200000, Stock = 10, Sizes = "41,42,43", ImageUrl = "~/Content/images/products/jordan-9-sm.jpg", ImageUrlLarge = "~/Content/images/products/jordan-9-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/jordan-9-sm.jpg", Large = "~/Content/images/products/jordan-9-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g39-t.jpg", Large = "~/Content/images/gallery/g39.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g40-t.jpg", Large = "~/Content/images/gallery/g40.jpg" }, }, IsHot = true },
                new ProductItem { Id = 10, Name = "Puma Suede Classic", Brand = "Puma", Description = "Chất liệu da lộn êm ái sang trọng.", Price = 1750000, Stock = 10, Sizes = "38,39,40", ImageUrl = "~/Content/images/products/puma-10-sm.jpg", ImageUrlLarge = "~/Content/images/products/puma-10-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/puma-10-sm.jpg", Large = "~/Content/images/products/puma-10-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/m02-t.jpg", Large = "~/Content/images/gallery/m02.jpg" }, new ProductImage { Small = "~/Content/images/gallery/m00-t.jpg", Large = "~/Content/images/gallery/m00.jpg" }, }, IsHot = false },
                new ProductItem { Id = 11, Name = "Converse Run Star Hike", Brand = "Converse", Description = "Đế răng cưa hầm hố tôn dáng ấn tượng.", Price = 2450000, Stock = 10, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/converse-11-sm.jpg", ImageUrlLarge = "~/Content/images/products/converse-11-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/converse-11-sm.jpg", Large = "~/Content/images/products/converse-11-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/x01-t.jpg", Large = "~/Content/images/gallery/x01.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g08-t.jpg", Large = "~/Content/images/gallery/g08.jpg" }, }, IsHot = false },
                new ProductItem { Id = 12, Name = "Vans Sk8-Hi Reissue", Brand = "Vans", Description = "Giày cổ cao bảo vệ cổ chân thể thao năng động.", Price = 2100000, Stock = 10, Sizes = "39,40,41,42", ImageUrl = "~/Content/images/products/vans-12-sm.jpg", ImageUrlLarge = "~/Content/images/products/vans-12-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/vans-12-sm.jpg", Large = "~/Content/images/products/vans-12-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g07-t.jpg", Large = "~/Content/images/gallery/g07.jpg" }, new ProductImage { Small = "~/Content/images/gallery/g11-t.jpg", Large = "~/Content/images/gallery/g11.jpg" }, }, IsHot = true }
            };
        }

        /// <summary>
        /// Sản phẩm liên quan: ưu tiên cùng thương hiệu, thiếu thì lấp từ phần còn lại.
        /// Trước đây trang chi tiết tự sắp bằng OrderByDescending(p =&gt; p.Brand == ...) —
        /// xếp theo bool để đẩy cùng brand lên đầu, khó đọc và dễ sai khi thêm quy tắc.
        /// </summary>
        public static List<ProductItem> RelatedTo(ProductItem product, int count)
        {
            if (product == null || count <= 0)
            {
                return new List<ProductItem>();
            }

            List<ProductItem> others = GetAll().Where(p => p.Id != product.Id).ToList();

            return others.Where(p => p.Brand == product.Brand)
                .Concat(others.Where(p => p.Brand != product.Brand))
                .OrderBy(p => p.Id)
                .Take(count)
                .ToList();
        }
    }
}
