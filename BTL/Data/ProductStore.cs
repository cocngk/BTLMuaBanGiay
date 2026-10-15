using System.Collections.Generic;
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
                new ProductItem { Id = 1, Name = "Nike Air Force 1 '07", Brand = "Nike", Description = "Thiết kế kinh điển, đệm Air siêu êm.", Price = 2900000, OldPrice = 3500000, Stock = 12, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?q=80&w=400", IsHot = true },
                new ProductItem { Id = 2, Name = "Adidas Ultraboost Light", Brand = "Adidas", Description = "Trọng lượng siêu nhẹ, êm ái khi chạy bộ.", Price = 3800000, Stock = 8, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400", IsHot = true },
                new ProductItem { Id = 3, Name = "Jordan 1 Retro High Chicago", Brand = "Jordan", Description = "Phối màu huyền thoại dành cho tín đồ Sneaker.", Price = 4500000, Stock = 3, Sizes = "40,41,42,43", ImageUrl = "https://images.unsplash.com/photo-1556906781-9a412961c28c?q=80&w=400", IsHot = false },
                new ProductItem { Id = 4, Name = "Puma RS-X Reinvent Red", Brand = "Puma", Description = "Cá tính, phối màu năng động cá tính.", Price = 1800000, OldPrice = 2200000, Stock = 15, Sizes = "38,39,40", ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400", IsHot = false },
                new ProductItem { Id = 5, Name = "Converse Chuck 70 High", Brand = "Converse", Description = "Phong cách Retro Vintage tối giản cổ điển.", Price = 1950000, Stock = 0, Sizes = "38,39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?q=80&w=400", IsHot = true },
                new ProductItem { Id = 6, Name = "Vans Old Skool Black", Brand = "Vans", Description = "Mẫu giày Skateboard trường tồn với thời gian.", Price = 1850000, Stock = 20, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400", IsHot = false },
                new ProductItem { Id = 7, Name = "Nike Dunk Low Retro", Brand = "Nike", Description = "Phong cách bóng rổ đường phố 80s quyến rũ.", Price = 3100000, Stock = 10, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1512374382149-233c42b6a83b?q=80&w=400", IsHot = true },
                new ProductItem { Id = 8, Name = "Adidas Forum Low White", Brand = "Adidas", Description = "Quai dán cổ điển mang đậm chất cổ điển.", Price = 2500000, Stock = 10, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1582588678413-dbf45f4823e9?q=80&w=400", IsHot = false },
                new ProductItem { Id = 9, Name = "Jordan 4 Retro Thunder", Brand = "Jordan", Description = "Phối màu vàng đen cực kỳ cuốn hút.", Price = 5200000, Stock = 10, Sizes = "41,42,43", ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400", IsHot = true },
                new ProductItem { Id = 10, Name = "Puma Suede Classic", Brand = "Puma", Description = "Chất liệu da lộn êm ái sang trọng.", Price = 1750000, Stock = 10, Sizes = "38,39,40", ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400", IsHot = false },
                new ProductItem { Id = 11, Name = "Converse Run Star Hike", Brand = "Converse", Description = "Đế răng cưa hầm hố tôn dáng ấn tượng.", Price = 2450000, Stock = 10, Sizes = "38,39,40,41", ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?q=80&w=400", IsHot = false },
                new ProductItem { Id = 12, Name = "Vans Sk8-Hi Reissue", Brand = "Vans", Description = "Giày cổ cao bảo vệ cổ chân thể thao năng động.", Price = 2100000, Stock = 10, Sizes = "39,40,41,42", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400", IsHot = true }
            };
        }
    }
}
