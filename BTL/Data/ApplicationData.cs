using System;
using System.Collections.Generic;
using ShoeStore.Helpers;
using ShoeStore.Models;
using System.Linq;
using System.Collections.Specialized;
using System.Web;


namespace ShoeStore.Data
{
    public sealed class ApplicationUserStore : IUserStore
    {
        private readonly SqlUserStore inner = new SqlUserStore();
        public List<UserAccount> GetAll() { return inner.GetAll(); }
        public UserAccount FindByUsernameOrEmail(string value) { return inner.FindByUsernameOrEmail(value); }
        public UserAccount FindByUsername(string value) { return inner.FindByUsername(value); }
        public bool UsernameExists(string value) { return inner.UsernameExists(value); }
        public bool EmailExists(string value) { return inner.EmailExists(value); }
        public AddUserOutcome TryAdd(UserAccount value) { return inner.TryAdd(value); }
        public ProfileUpdateOutcome TryUpdateProfile(string user, string name, string email, string phone) { return inner.TryUpdateProfile(user, name, email, phone); }
        public PasswordChangeOutcome TryChangePassword(string user, string current, string updated) { return inner.TryChangePassword(user, current, updated); }
        public RoleChangeOutcome TrySetAdmin(string user, bool admin) { return inner.TrySetAdmin(user, admin); }
    }
}



namespace ShoeStore.Data
{
    public interface IOrderStore
    {
        void Save(Order order);
        List<Order> All();
        List<Order> ByUser(string username);
        Order FindByCode(string code);
    }

    public sealed class ApplicationOrderStore : IOrderStore
    {
        private readonly SqlOrderStore inner = new SqlOrderStore();
        public void Save(Order order) { inner.Save(order); }
        public List<Order> All() { return inner.All(); }
        public List<Order> ByUser(string username) { return inner.ByUser(username); }
        public Order FindByCode(string code) { return inner.FindByCode(code); }
        public void Update(Order order) { inner.UpdateStatus(order); }
    }
}



namespace ShoeStore.Data
{
    public static class BlogStore
    {
        public static List<BlogItem> GetAll()
        {
            return new List<BlogItem>
            {
                new BlogItem { Id = 1, Title = "Cách chọn đúng size giày Sneaker chuẩn 100% khi đặt hàng Online", Summary = "Không còn lo lắng việc đặt giày về bị chật hay rộng nữa. Dưới đây là quy chuẩn bảng đo chiều dài chân chuẩn từng milimet dành riêng cho bạn...", Date = "12/10/2026", Views = 1250, Category = "Mẹo chọn Size giày", ImageUrl = "~/Content/images/blogs/blog-1.jpg" },
                new BlogItem { Id = 2, Title = "5 Cách phối đồ cực đỉnh cùng Nike Air Force 1", Summary = "Khám phá phong cách phối đồ từ đường phố năng động đến thanh lịch hiện đại cùng đôi giày quốc dân.", Date = "11/10/2026", Views = 980, Category = "Phối đồ thời trang", ImageUrl = "~/Content/images/blogs/blog-2.jpg" },
                new BlogItem { Id = 3, Title = "Bí quyết bảo quản giày thể thao không bị ố vàng mùa mưa", Summary = "Những lưu ý quan trọng giúp đôi sneaker của bạn giữ nguyên độ mới ngay cả trong thời tiết ẩm ướt.", Date = "09/10/2026", Views = 870, Category = "Hướng dẫn vệ sinh", ImageUrl = "~/Content/images/blogs/blog-3.jpg" },
                new BlogItem { Id = 4, Title = "Vì sao dòng giày da trắng lại bền màu hơn mẫu khác", Summary = "Cách chọn đôi giày da trắng ít bị bong màu, dễ lau chùi và giữ được vẻ sạch lâu dài.", Date = "05/10/2026", Views = 760, Category = "Xu hướng Sneaker 2026", ImageUrl = "~/Content/images/blogs/blog-4.jpg" },
                new BlogItem { Id = 5, Title = "Xu hướng Giày Chunky Sneaker càn quét sàn diễn 2026", Summary = "Những đôi giày đế thô phá cách đang khẳng định vị thế dẫn đầu xu hướng thời trang giới trẻ.", Date = "01/10/2026", Views = 640, Category = "Xu hướng Sneaker 2026", ImageUrl = "~/Content/images/blogs/blog-5.jpg" },
                new BlogItem { Id = 6, Title = "Top 7 xịt nano chống nước bảo vệ giày tốt nhất hiện nay", Summary = "Đánh giá chi tiết các thương hiệu xịt chống nước nổi tiếng giúp bảo vệ đôi sneaker yêu thích.", Date = "28/09/2026", Views = 520, Category = "Hướng dẫn vệ sinh", ImageUrl = "~/Content/images/blogs/blog-6.jpg" },
                new BlogItem { Id = 7, Title = "Hướng dẫn buộc dây giày Sneaker độc lạ thu hút mọi ánh nhìn", Summary = "Tổng hợp 10 kiểu thắt dây giày thể thao độc đáo đơn giản ai cũng tự làm được.", Date = "25/09/2026", Views = 410, Category = "Phối đồ thời trang", ImageUrl = "~/Content/images/blogs/blog-7.jpg" }
            };
        }
    }
}



namespace ShoeStore.Data
{
    public static class ProductReviewStore
    {
        private static readonly SqlReviewStore Inner = new SqlReviewStore();
        public static List<ProductReview> GetByProduct(int productId) { return Inner.GetByProduct(productId); }
        public static ProductReview Find(int productId, string userKey) { return Inner.Find(productId, userKey); }
        public static void Save(ProductReview review) { Inner.Save(review); }
        public static double GetAverage(int productId) { return Inner.Average(productId); }
        public static int Count(int productId) { return Inner.GetByProduct(productId).Count; }
    }
}



namespace ShoeStore.Data
{
    public static class PriceBuckets
    {
        public const string All = "All";
        public const string Under2M = "Under2M";
        public const string From2MTo4M = "2Mto4M";
        public const string Above4M = "Above4M";

        public static bool Matches(string bucket, decimal price)
        {
            if (bucket == Under2M) return price < 2000000;
            if (bucket == From2MTo4M) return price >= 2000000 && price <= 4000000;
            if (bucket == Above4M) return price > 4000000;
            return true;
        }
    }

    public class CatalogQuery
    {
        public string Keyword = "";
        public List<string> Brands = new List<string>();
        public string PriceRange = PriceBuckets.All;
        public string Size = "";
        public string Sort = "Default";
        public int Page = 1;

        public static CatalogQuery FromQuery(NameValueCollection query)
        {
            CatalogQuery result = new CatalogQuery
            {
                Keyword = query["search"] ?? "",
                PriceRange = string.IsNullOrEmpty(query["price"]) ? PriceBuckets.All : query["price"],
                Size = query["size"] ?? "",
                Sort = string.IsNullOrEmpty(query["sort"]) ? "Default" : query["sort"],
                Page = Paging.PageFromQuery(query["page"])
            };
            result.Brands = (query["brand"] ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            return result;
        }

        public string ToUrl(string page = "SanPham.aspx", int? pageNumber = null)
        {
            int number = pageNumber ?? Page;
            List<string> parts = new List<string>();
            if (!string.IsNullOrEmpty(Keyword)) parts.Add("search=" + HttpUtility.UrlEncode(Keyword));
            if (Brands.Count > 0) parts.Add("brand=" + HttpUtility.UrlEncode(string.Join(",", Brands)));
            if (PriceRange != PriceBuckets.All) parts.Add("price=" + HttpUtility.UrlEncode(PriceRange));
            if (!string.IsNullOrEmpty(Size)) parts.Add("size=" + HttpUtility.UrlEncode(Size));
            if (Sort != "Default") parts.Add("sort=" + HttpUtility.UrlEncode(Sort));
            if (number > 1) parts.Add("page=" + number);
            return page + (parts.Count == 0 ? "" : "?" + string.Join("&", parts));
        }

        public IEnumerable<ProductItem> Apply(IEnumerable<ProductItem> source)
        {
            IEnumerable<ProductItem> result = source ?? Enumerable.Empty<ProductItem>();
            if (!string.IsNullOrEmpty(Keyword)) result = result.Where(p => Contains(p.Name, Keyword) || Contains(p.Brand, Keyword) || Contains(p.Description, Keyword));
            if (Brands.Count > 0) result = result.Where(p => Brands.Contains(p.Brand, StringComparer.OrdinalIgnoreCase));
            result = result.Where(p => PriceBuckets.Matches(PriceRange, p.Price));
            if (!string.IsNullOrEmpty(Size)) result = result.Where(p => p.SizeList().Contains(Size));
            if (Sort == "PriceAsc") return result.OrderBy(p => p.Price);
            if (Sort == "PriceDesc") return result.OrderByDescending(p => p.Price);
            if (Sort == "NameAsc") return result.OrderBy(p => p.Name);
            return result;
        }

        private static bool Contains(string value, string keyword)
        {
            return !string.IsNullOrEmpty(value) && value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items;
        public int TotalPages;
        public int Page;
    }

    public static class Paging
    {
        public static int PageFromQuery(string raw)
        {
            int page;
            return int.TryParse(raw, out page) && page > 0 ? page : 1;
        }

        public static string PageUrl(string page, int number)
        {
            return number > 1 ? page + "?page=" + number : page;
        }

        public static PagedResult<T> Paginate<T>(IEnumerable<T> source, int page, int pageSize)
        {
            List<T> items = (source ?? Enumerable.Empty<T>()).ToList();
            int total = pageSize <= 0 ? 1 : (int)Math.Ceiling((double)items.Count / pageSize);
            total = Math.Max(1, total);
            page = Math.Max(1, Math.Min(page, total));
            return new PagedResult<T> { Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(), TotalPages = total, Page = page };
        }
    }
}
