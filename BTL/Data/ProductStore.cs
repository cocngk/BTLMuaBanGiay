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
                new ProductItem { Id = 1, Name = "Nike Air Force 1 Low Retro", Brand = "Nike", Description = "Da trắng kinh điển, đế cao su bám đường và đệm êm cả ngày.", Price = 2900000, OldPrice = 3500000, Stock = 12, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/nike-1-sm.jpg", ImageUrlLarge = "~/Content/images/products/nike-1-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/nike-1-sm.jpg", Large = "~/Content/images/products/nike-1-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/u16-t.jpg", Large = "~/Content/images/gallery/u16.jpg" } }, IsHot = true },
                new ProductItem { Id = 2, Name = "Nike Air Force 1 Low Tím", Brand = "Nike", Description = "Màu tím trẻ trung, phối dễ với quần jean và áo basic.", Price = 3200000, Stock = 9, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/nike-2-sm.jpg", ImageUrlLarge = "~/Content/images/products/nike-2-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/nike-2-sm.jpg", Large = "~/Content/images/products/nike-2-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/v01-t.jpg", Large = "~/Content/images/gallery/v01.jpg" } }, IsHot = false },
                new ProductItem { Id = 3, Name = "Nike Air Force 1 High", Brand = "Nike", Description = "Cổ cao bảo vệ cổ chân, phối trắng nền cực dễ mặc.", Price = 3400000, Stock = 8, Sizes = "39,40,41,42", ImageUrl = "~/Content/images/products/nike-3-sm.jpg", ImageUrlLarge = "~/Content/images/products/nike-3-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/nike-3-sm.jpg", Large = "~/Content/images/products/nike-3-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/n00-t.jpg", Large = "~/Content/images/gallery/n00.jpg" } }, IsHot = true },
                new ProductItem { Id = 4, Name = "Nike Air Max", Brand = "Nike", Description = "Đệm Air dày, hợp cả chạy bộ lẫn dạo phố.", Price = 4200000, Stock = 6, Sizes = "40,41,42,43", ImageUrl = "~/Content/images/products/nike-4-sm.jpg", ImageUrlLarge = "~/Content/images/products/nike-4-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/nike-4-sm.jpg", Large = "~/Content/images/products/nike-4-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/u17-t.jpg", Large = "~/Content/images/gallery/u17.jpg" } }, IsHot = true },
                new ProductItem { Id = 5, Name = "Nike Air Max Day", Brand = "Nike", Description = "Thân giày lưới thoáng khí, nhẹ chân khi vận động.", Price = 3800000, OldPrice = 4300000, Stock = 14, Sizes = "39,40,41,42", ImageUrl = "~/Content/images/products/nike-5-sm.jpg", ImageUrlLarge = "~/Content/images/products/nike-5-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/nike-5-sm.jpg", Large = "~/Content/images/products/nike-5-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/u06-t.jpg", Large = "~/Content/images/gallery/u06.jpg" } }, IsHot = false },
                new ProductItem { Id = 6, Name = "Converse Chuck 70", Brand = "Converse", Description = "Canvas cao cổ biểu tượng của phong cách skate từ năm 1970.", Price = 1950000, Stock = 0, Sizes = "38,39,40,41,42", ImageUrl = "~/Content/images/products/converse-6-sm.jpg", ImageUrlLarge = "~/Content/images/products/converse-6-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/converse-6-sm.jpg", Large = "~/Content/images/products/converse-6-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/b11-t.jpg", Large = "~/Content/images/gallery/b11.jpg" } }, IsHot = true },
                new ProductItem { Id = 7, Name = "Converse Chuck 70 Low", Brand = "Converse", Description = "Mũi thấp dễ phối, đế gum bám tốt trên mặt đất.", Price = 1850000, Stock = 11, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/converse-7-sm.jpg", ImageUrlLarge = "~/Content/images/products/converse-7-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/converse-7-sm.jpg", Large = "~/Content/images/products/converse-7-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/u12-t.jpg", Large = "~/Content/images/gallery/u12.jpg" } }, IsHot = false },
                new ProductItem { Id = 8, Name = "Vans Old Skool", Brand = "Vans", Description = "Dáng skate cổ điển, hợp quần jean và áo bomber.", Price = 1750000, Stock = 20, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/vans-8-sm.jpg", ImageUrlLarge = "~/Content/images/products/vans-8-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/vans-8-sm.jpg", Large = "~/Content/images/products/vans-8-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/u14-t.jpg", Large = "~/Content/images/gallery/u14.jpg" } }, IsHot = true },
                new ProductItem { Id = 9, Name = "Vans Classic Low", Brand = "Vans", Description = "Màu trắng dễ phối, form ôm chân cho cả nam lẫn nữ.", Price = 1690000, Stock = 16, Sizes = "38,39,40,41,42", ImageUrl = "~/Content/images/products/vans-9-sm.jpg", ImageUrlLarge = "~/Content/images/products/vans-9-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/vans-9-sm.jpg", Large = "~/Content/images/products/vans-9-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/u05-t.jpg", Large = "~/Content/images/gallery/u05.jpg" } }, IsHot = false },
                new ProductItem { Id = 10, Name = "New Balance 574", Brand = "New Balance", Description = "Chất liệu da lộn mềm, form classic bền qua nhiều mùa.", Price = 2450000, Stock = 10, Sizes = "39,40,41,42", ImageUrl = "~/Content/images/products/newbalance-10-sm.jpg", ImageUrlLarge = "~/Content/images/products/newbalance-10-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/newbalance-10-sm.jpg", Large = "~/Content/images/products/newbalance-10-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/u01-t.jpg", Large = "~/Content/images/gallery/u01.jpg" } }, IsHot = false },
                new ProductItem { Id = 11, Name = "SneakerX Basic", Brand = "SneakerX", Description = "Mẫu basic giá rẻ, da dễ lau chùi và thay linh kiện.", Price = 990000, Stock = 25, Sizes = "38,39,40,41", ImageUrl = "~/Content/images/products/sneakersx-11-sm.jpg", ImageUrlLarge = "~/Content/images/products/sneakersx-11-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/sneakersx-11-sm.jpg", Large = "~/Content/images/products/sneakersx-11-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/n05-t.jpg", Large = "~/Content/images/gallery/n05.jpg" } }, IsHot = false },
                new ProductItem { Id = 12, Name = "SneakerX Studio", Brand = "SneakerX", Description = "Phiên bản màu tím nổi bật, dành cho khách thích cá tính.", Price = 1290000, Stock = 18, Sizes = "38,39,40,41,42", ImageUrl = "~/Content/images/products/sneakersx-12-sm.jpg", ImageUrlLarge = "~/Content/images/products/sneakersx-12-lg.jpg", Gallery = new List<ProductImage> { new ProductImage { Small = "~/Content/images/products/sneakersx-12-sm.jpg", Large = "~/Content/images/products/sneakersx-12-lg.jpg" }, new ProductImage { Small = "~/Content/images/gallery/v03-t.jpg", Large = "~/Content/images/gallery/v03.jpg" } }, IsHot = false }

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
