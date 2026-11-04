using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    // Lớp trung gian để trang làm việc với kho người dùng SQL.
    public sealed class ApplicationUserStore : IUserStore
    {
        private readonly SqlUserStore kho = new SqlUserStore();

        public List<UserAccount> GetAll() { return kho.GetAll(); }

        public UserAccount FindByUsernameOrEmail(string giaTri)
        {
            return kho.FindByUsernameOrEmail(giaTri);
        }

        public UserAccount FindByUsername(string tenDangNhap)
        {
            return kho.FindByUsername(tenDangNhap);
        }

        public bool UsernameExists(string tenDangNhap)
        {
            return kho.UsernameExists(tenDangNhap);
        }

        public bool EmailExists(string email)
        {
            return kho.EmailExists(email);
        }

        public AddUserOutcome TryAdd(UserAccount taiKhoan)
        {
            return kho.TryAdd(taiKhoan);
        }

        public ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap,
            string hoTen, string email, string soDienThoai)
        {
            return kho.TryUpdateProfile(tenDangNhap, hoTen, email, soDienThoai);
        }

        public PasswordChangeOutcome TryChangePassword(string tenDangNhap,
            string matKhauCu, string matKhauMoi)
        {
            return kho.TryChangePassword(tenDangNhap, matKhauCu, matKhauMoi);
        }

        public RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri)
        {
            return kho.TrySetAdmin(tenDangNhap, laQuanTri);
        }
    }

    public interface IOrderStore
    {
        void Save(Order donHang);
        List<Order> All();
        List<Order> ByUser(string tenDangNhap);
        Order FindByCode(string maDonHang);
    }

    // Lớp trung gian để các trang không phải biết chi tiết SQL.
    public sealed class ApplicationOrderStore : IOrderStore
    {
        private readonly SqlOrderStore kho = new SqlOrderStore();

        public void Save(Order donHang) { kho.Save(donHang); }
        public List<Order> All() { return kho.All(); }
        public List<Order> ByUser(string tenDangNhap) { return kho.ByUser(tenDangNhap); }
        public Order FindByCode(string maDonHang) { return kho.FindByCode(maDonHang); }
        public void Update(Order donHang) { kho.Update(donHang); }
    }

    public static class BlogStore
    {
        // Trả về dữ liệu tin tức tĩnh đang hiển thị trên website.
        public static List<BlogItem> GetAll()
        {
            return new List<BlogItem>
            {
                new BlogItem
                {
                    Id = 1,
                    Title = "Cách chọn đúng size giày Sneaker chuẩn 100% khi đặt hàng Online",
                    Summary = "Không còn lo lắng việc đặt giày về bị chật hay rộng nữa. Dưới đây là quy chuẩn bảng đo chiều dài chân chuẩn từng milimet dành riêng cho bạn...",
                    Date = "12/10/2026",
                    Views = 1250,
                    Category = "Mẹo chọn Size giày",
                    ImageUrl = "~/Content/images/blogs/blog-1.jpg"
                },
                new BlogItem
                {
                    Id = 2,
                    Title = "5 Cách phối đồ cực đỉnh cùng Nike Air Force 1",
                    Summary = "Khám phá phong cách phối đồ từ đường phố năng động đến thanh lịch hiện đại cùng đôi giày quốc dân.",
                    Date = "11/10/2026",
                    Views = 980,
                    Category = "Phối đồ thời trang",
                    ImageUrl = "~/Content/images/blogs/blog-2.jpg"
                },
                new BlogItem
                {
                    Id = 3,
                    Title = "Bí quyết bảo quản giày thể thao không bị ố vàng mùa mưa",
                    Summary = "Những lưu ý quan trọng giúp đôi sneaker của bạn giữ nguyên độ mới ngay cả trong thời tiết ẩm ướt.",
                    Date = "09/10/2026",
                    Views = 870,
                    Category = "Hướng dẫn vệ sinh",
                    ImageUrl = "~/Content/images/blogs/blog-3.jpg"
                },
                new BlogItem
                {
                    Id = 4,
                    Title = "Vì sao dòng giày da trắng lại bền màu hơn mẫu khác",
                    Summary = "Cách chọn đôi giày da trắng ít bị bong màu, dễ lau chùi và giữ được vẻ sạch lâu dài.",
                    Date = "05/10/2026",
                    Views = 760,
                    Category = "Xu hướng Sneaker 2026",
                    ImageUrl = "~/Content/images/blogs/blog-4.jpg"
                },
                new BlogItem
                {
                    Id = 5,
                    Title = "Xu hướng Giày Chunky Sneaker càn quét sàn diễn 2026",
                    Summary = "Những đôi giày đế thô phá cách đang khẳng định vị thế dẫn đầu xu hướng thời trang giới trẻ.",
                    Date = "01/10/2026",
                    Views = 640,
                    Category = "Xu hướng Sneaker 2026",
                    ImageUrl = "~/Content/images/blogs/blog-5.jpg"
                },
                new BlogItem
                {
                    Id = 6,
                    Title = "Top 7 xịt nano chống nước bảo vệ giày tốt nhất hiện nay",
                    Summary = "Đánh giá chi tiết các thương hiệu xịt chống nước nổi tiếng giúp bảo vệ đôi sneaker yêu thích.",
                    Date = "28/09/2026",
                    Views = 520,
                    Category = "Hướng dẫn vệ sinh",
                    ImageUrl = "~/Content/images/blogs/blog-6.jpg"
                },
                new BlogItem
                {
                    Id = 7,
                    Title = "Hướng dẫn buộc dây giày Sneaker độc lạ thu hút mọi ánh nhìn",
                    Summary = "Tổng hợp 10 kiểu thắt dây giày thể thao độc đáo đơn giản ai cũng tự làm được.",
                    Date = "25/09/2026",
                    Views = 410,
                    Category = "Phối đồ thời trang",
                    ImageUrl = "~/Content/images/blogs/blog-7.jpg"
                }
            };
        }
    }

    public static class ProductReviewStore
    {
        private static readonly SqlReviewStore kho = new SqlReviewStore();

        public static List<ProductReview> GetByProduct(int maSanPham)
        {
            return kho.GetByProduct(maSanPham);
        }

        public static ProductReview Find(int maSanPham, string khoaNguoiDung)
        {
            return kho.Find(maSanPham, khoaNguoiDung);
        }

        public static void Save(ProductReview danhGia) { kho.Save(danhGia); }

        public static double GetAverage(int maSanPham)
        {
            return kho.Average(maSanPham);
        }

        public static int Count(int maSanPham)
        {
            return kho.GetByProduct(maSanPham).Count;
        }
    }

    public static class PriceBuckets
    {
        public const string All = "All";
        public const string Under2M = "Under2M";
        public const string From2MTo4M = "2Mto4M";
        public const string Above4M = "Above4M";

        // Kiểm tra giá có nằm trong nhóm đã chọn.
        public static bool Matches(string mucGia, decimal gia)
        {
            if (mucGia == Under2M) return gia < 2000000;
            if (mucGia == From2MTo4M)
                return gia >= 2000000 && gia <= 4000000;
            if (mucGia == Above4M) return gia > 4000000;
            return true;
        }
    }

    public sealed class CatalogQuery
    {
        public string Keyword = "";
        public List<string> Brands = new List<string>();
        public string PriceRange = PriceBuckets.All;
        public string Size = "";
        public string Sort = "Default";
        public int Page = 1;

        // Đọc bộ lọc sản phẩm từ query string.
        public static CatalogQuery FromQuery(NameValueCollection thamSo)
        {
            CatalogQuery boLoc = new CatalogQuery
            {
                Keyword = thamSo["search"] ?? "",
                PriceRange = string.IsNullOrEmpty(thamSo["price"])
                    ? PriceBuckets.All
                    : thamSo["price"],
                Size = thamSo["size"] ?? "",
                Sort = string.IsNullOrEmpty(thamSo["sort"])
                    ? "Default"
                    : thamSo["sort"],
                Page = Paging.PageFromQuery(thamSo["page"])
            };

            boLoc.Brands = (thamSo["brand"] ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(thuongHieu => thuongHieu.Trim())
                .ToList();
            return boLoc;
        }

        // Tạo URL giữ lại các bộ lọc hiện tại.
        public string ToUrl(string trang = "SanPham.aspx", int? soTrang = null)
        {
            int so = soTrang ?? Page;
            List<string> thamSo = new List<string>();

            if (!string.IsNullOrEmpty(Keyword))
                thamSo.Add("search=" + HttpUtility.UrlEncode(Keyword));
            if (Brands.Count > 0)
                thamSo.Add("brand=" + HttpUtility.UrlEncode(string.Join(",", Brands)));
            if (PriceRange != PriceBuckets.All)
                thamSo.Add("price=" + HttpUtility.UrlEncode(PriceRange));
            if (!string.IsNullOrEmpty(Size))
                thamSo.Add("size=" + HttpUtility.UrlEncode(Size));
            if (Sort != "Default")
                thamSo.Add("sort=" + HttpUtility.UrlEncode(Sort));
            if (so > 1) thamSo.Add("page=" + so);

            return trang + (thamSo.Count == 0
                ? ""
                : "?" + string.Join("&", thamSo));
        }

        // Áp dụng bộ lọc và sắp xếp lên danh sách sản phẩm.
        public IEnumerable<ProductItem> Apply(IEnumerable<ProductItem> nguon)
        {
            IEnumerable<ProductItem> ketQua = nguon
                ?? Enumerable.Empty<ProductItem>();

            if (!string.IsNullOrEmpty(Keyword))
            {
                ketQua = ketQua.Where(sanPham =>
                    CoChua(sanPham.Name, Keyword)
                    || CoChua(sanPham.Brand, Keyword)
                    || CoChua(sanPham.Description, Keyword));
            }

            if (Brands.Count > 0)
            {
                ketQua = ketQua.Where(sanPham =>
                    Brands.Contains(sanPham.Brand, StringComparer.OrdinalIgnoreCase));
            }

            ketQua = ketQua.Where(sanPham =>
                PriceBuckets.Matches(PriceRange, sanPham.Price));

            if (!string.IsNullOrEmpty(Size))
            {
                ketQua = ketQua.Where(sanPham =>
                    sanPham.SizeList().Contains(Size));
            }

            if (Sort == "PriceAsc") return ketQua.OrderBy(sanPham => sanPham.Price);
            if (Sort == "PriceDesc") return ketQua.OrderByDescending(sanPham => sanPham.Price);
            if (Sort == "NameAsc") return ketQua.OrderBy(sanPham => sanPham.Name);
            return ketQua;
        }

        private static bool CoChua(string vanBan, string tuKhoa)
        {
            return !string.IsNullOrEmpty(vanBan)
                && vanBan.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public sealed class PagedResult<T>
    {
        public List<T> Items;
        public int TotalPages;
        public int Page;
    }

    public static class Paging
    {
        // Đọc số trang từ query string, mặc định là trang 1.
        public static int PageFromQuery(string chuoi)
        {
            int trang;
            return int.TryParse(chuoi, out trang) && trang > 0 ? trang : 1;
        }

        // Tạo URL đơn giản cho phân trang.
        public static string PageUrl(string trang, int soTrang)
        {
            return soTrang > 1 ? trang + "?page=" + soTrang : trang;
        }

        // Chia danh sách thành một trang kết quả.
        public static PagedResult<T> Paginate<T>(IEnumerable<T> nguon,
            int trang, int kichThuocTrang)
        {
            List<T> danhSach = (nguon ?? Enumerable.Empty<T>()).ToList();
            int coTrang = Math.Max(1, kichThuocTrang);
            int tongTrang = (int)Math.Ceiling((double)danhSach.Count / coTrang);
            tongTrang = Math.Max(1, tongTrang);
            trang = Math.Max(1, Math.Min(trang, tongTrang));

            return new PagedResult<T>
            {
                Items = danhSach
                    .Skip((trang - 1) * coTrang)
                    .Take(coTrang)
                    .ToList(),
                TotalPages = tongTrang,
                Page = trang
            };
        }
    }
}
