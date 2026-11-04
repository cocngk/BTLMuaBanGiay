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
        private readonly SqlUserStore benTrong = new SqlUserStore();
        // Thực hiện việc lấy toàn bộ.
        public List<UserAccount> GetAll() { return benTrong.GetAll(); }
        // Tìm theo tên đăng nhập hoặc email.
        public UserAccount FindByUsernameOrEmail(string giaTri) { return benTrong.FindByUsernameOrEmail(giaTri); }
        // Thực hiện việc tìm theo tên đăng nhập.
        public UserAccount FindByUsername(string giaTri) { return benTrong.FindByUsername(giaTri); }
        // Thực hiện việc tên đăng nhập tồn tại.
        public bool UsernameExists(string giaTri) { return benTrong.UsernameExists(giaTri); }
        // Thực hiện việc email tồn tại.
        public bool EmailExists(string giaTri) { return benTrong.EmailExists(giaTri); }
        // Thực hiện việc thử thêm.
        public AddUserOutcome TryAdd(UserAccount giaTri) { return benTrong.TryAdd(giaTri); }
        // Thực hiện việc thử cập nhật hồ sơ.
        public ProfileUpdateOutcome TryUpdateProfile(string nguoiDung, string ten, string email, string soDienThoai) { return benTrong.TryUpdateProfile(nguoiDung, ten, email, soDienThoai); }
        // Thực hiện việc thử thay đổi mật khẩu.
        public PasswordChangeOutcome TryChangePassword(string nguoiDung, string hienTai, string moiCapNhat) { return benTrong.TryChangePassword(nguoiDung, hienTai, moiCapNhat); }
        // Thực hiện việc thử đặt quản trị.
        public RoleChangeOutcome TrySetAdmin(string nguoiDung, bool quanTri) { return benTrong.TrySetAdmin(nguoiDung, quanTri); }
    }
}



namespace ShoeStore.Data
{
    public interface IOrderStore
    {
        // Lưu đơn hàng.
        void Save(Order donHang);
        // Lấy toàn bộ đơn hàng.
        List<Order> All();
        // Lấy đơn hàng của một người dùng.
        List<Order> ByUser(string tenDangNhap);
        // Tìm đơn hàng theo mã.
        Order FindByCode(string ma);
    }

    public sealed class ApplicationOrderStore : IOrderStore
    {
        private readonly SqlOrderStore benTrong = new SqlOrderStore();
        // Thực hiện việc lưu.
        public void Save(Order donHang) { benTrong.Save(donHang); }
        // Thực hiện việc toàn bộ.
        public List<Order> All() { return benTrong.All(); }
        // Thực hiện việc theo người dùng.
        public List<Order> ByUser(string tenDangNhap) { return benTrong.ByUser(tenDangNhap); }
        // Thực hiện việc tìm theo mã.
        public Order FindByCode(string ma) { return benTrong.FindByCode(ma); }
        // Thực hiện việc cập nhật.
        public void Update(Order donHang) { benTrong.UpdateStatus(donHang); }
    }
}



namespace ShoeStore.Data
{
    public static class BlogStore
    {
        // Thực hiện việc lấy toàn bộ.
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
        private static readonly SqlReviewStore benTrong = new SqlReviewStore();
        // Thực hiện việc lấy theo sản phẩm.
        public static List<ProductReview> GetByProduct(int maSanPham) { return benTrong.GetByProduct(maSanPham); }
        // Thực hiện việc tìm.
        public static ProductReview Find(int maSanPham, string khoaNguoiDung) { return benTrong.Find(maSanPham, khoaNguoiDung); }
        // Thực hiện việc lưu.
        public static void Save(ProductReview danhGia) { benTrong.Save(danhGia); }
        // Thực hiện việc lấy tính trung bình.
        public static double GetAverage(int maSanPham) { return benTrong.Average(maSanPham); }
        // Thực hiện việc đếm.
        public static int Count(int maSanPham) { return benTrong.GetByProduct(maSanPham).Count; }
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

        // Thực hiện việc đối chiếu.
        public static bool Matches(string mucGia, decimal gia)
        {
            if (mucGia == Under2M) return gia < 2000000;
            if (mucGia == From2MTo4M) return gia >= 2000000 && gia <= 4000000;
            if (mucGia == Above4M) return gia > 4000000;
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

        // Thực hiện việc chuyển đổi từ query.
        public static CatalogQuery FromQuery(NameValueCollection truyVan)
        {
            CatalogQuery ketQua = new CatalogQuery
            {
                Keyword = truyVan["search"] ?? "",
                PriceRange = string.IsNullOrEmpty(truyVan["price"]) ? PriceBuckets.All : truyVan["price"],
                Size = truyVan["size"] ?? "",
                Sort = string.IsNullOrEmpty(truyVan["sort"]) ? "Default" : truyVan["sort"],
                Page = Paging.PageFromQuery(truyVan["page"])
            };
            ketQua.Brands = (truyVan["brand"] ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            return ketQua;
        }

        // Thực hiện việc chuyển đổi sang đường dẫn.
        public string ToUrl(string trang = "SanPham.aspx", int? soTrang = null)
        {
            int so = soTrang ?? Page;
            List<string> cacPhan = new List<string>();
            if (!string.IsNullOrEmpty(Keyword)) cacPhan.Add("search=" + HttpUtility.UrlEncode(Keyword));
            if (Brands.Count > 0) cacPhan.Add("brand=" + HttpUtility.UrlEncode(string.Join(",", Brands)));
            if (PriceRange != PriceBuckets.All) cacPhan.Add("price=" + HttpUtility.UrlEncode(PriceRange));
            if (!string.IsNullOrEmpty(Size)) cacPhan.Add("size=" + HttpUtility.UrlEncode(Size));
            if (Sort != "Default") cacPhan.Add("sort=" + HttpUtility.UrlEncode(Sort));
            if (so > 1) cacPhan.Add("page=" + so);
            return trang + (cacPhan.Count == 0 ? "" : "?" + string.Join("&", cacPhan));
        }

        // Thực hiện việc áp dụng.
        public IEnumerable<ProductItem> Apply(IEnumerable<ProductItem> nguon)
        {
            IEnumerable<ProductItem> ketQua = nguon ?? Enumerable.Empty<ProductItem>();
            if (!string.IsNullOrEmpty(Keyword)) ketQua = ketQua.Where(p => Contains(p.Name, Keyword) || Contains(p.Brand, Keyword) || Contains(p.Description, Keyword));
            if (Brands.Count > 0) ketQua = ketQua.Where(p => Brands.Contains(p.Brand, StringComparer.OrdinalIgnoreCase));
            ketQua = ketQua.Where(p => PriceBuckets.Matches(PriceRange, p.Price));
            if (!string.IsNullOrEmpty(Size)) ketQua = ketQua.Where(p => p.SizeList().Contains(Size));
            if (Sort == "PriceAsc") return ketQua.OrderBy(p => p.Price);
            if (Sort == "PriceDesc") return ketQua.OrderByDescending(p => p.Price);
            if (Sort == "NameAsc") return ketQua.OrderBy(p => p.Name);
            return ketQua;
        }

        // Thực hiện việc kiểm tra chứa.
        private static bool Contains(string giaTri, string tuKhoa)
        {
            return !string.IsNullOrEmpty(giaTri) && giaTri.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0;
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
        // Thực hiện việc trang chuyển đổi từ query.
        public static int PageFromQuery(string chuoiGoc)
        {
            int trang;
            return int.TryParse(chuoiGoc, out trang) && trang > 0 ? trang : 1;
        }

        // Thực hiện việc trang đường dẫn.
        public static string PageUrl(string trang, int so)
        {
            return so > 1 ? trang + "?page=" + so : trang;
        }

        public static PagedResult<T> Paginate<T>(IEnumerable<T> nguon, int trang, int kichThuocTrang)
        {
            List<T> phanTu = (nguon ?? Enumerable.Empty<T>()).ToList();
            int tong = kichThuocTrang <= 0 ? 1 : (int)Math.Ceiling((double)phanTu.Count / kichThuocTrang);
            tong = Math.Max(1, tong);
            trang = Math.Max(1, Math.Min(trang, tong));
            return new PagedResult<T> { Items = phanTu.Skip((trang - 1) * kichThuocTrang).Take(kichThuocTrang).ToList(), TotalPages = tong, Page = trang };
        }
    }
}
