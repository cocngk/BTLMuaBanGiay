using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore
{
    // Người dùng. Mật khẩu lưu thẳng, không băm - giống ví dụ trong slide.
    public class NguoiDung
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }
    }

    // Một sản phẩm trong cửa hàng.
    public class SanPhamItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Sizes { get; set; }
        public string ImageUrl { get; set; }
        public bool IsHot { get; set; }
    }

    // Một dòng trong đơn hàng.
    public class DongDon
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    // Một đơn hàng.
    public class DonHang
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public List<DongDon> Lines = new List<DongDon>();
    }

    // Toàn bộ dữ liệu nằm trong bộ nhớ, không dùng cơ sở dữ liệu.
    // Dữ liệu mất khi ứng dụng khởi động lại (đóng Visual Studio hoặc sửa Web.config).
    public static class DuLieu
    {
        public static List<NguoiDung> NguoiDungs = new List<NguoiDung>();
        public static List<SanPhamItem> SanPhams = new List<SanPhamItem>();
        public static List<DonHang> DonHangs = new List<DonHang>();

        private static int maNguoiDung = 0;
        private static int maSanPham = 0;
        private static int maDonHang = 0;

        // Hàm này tự chạy một lần khi có chỗ đầu tiên dùng tới DuLieu.
        static DuLieu()
        {
            TaoDuLieuMau();
        }

        // ------------------------------------------------------------------
        //  Người dùng
        // ------------------------------------------------------------------

        public static NguoiDung TimNguoiDung(string tenDangNhap)
        {
            return NguoiDungs.FirstOrDefault(nd => nd.Username == tenDangNhap);
        }

        public static void ThemNguoiDung(NguoiDung nguoiDung)
        {
            maNguoiDung = maNguoiDung + 1;
            nguoiDung.Id = maNguoiDung;
            NguoiDungs.Add(nguoiDung);
        }

        // ------------------------------------------------------------------
        //  Sản phẩm
        // ------------------------------------------------------------------

        public static SanPhamItem TimSanPham(int id)
        {
            return SanPhams.FirstOrDefault(sp => sp.Id == id);
        }

        // Sản phẩm nổi bật cho trang chủ.
        public static List<SanPhamItem> SanPhamNoiBat(int soLuong)
        {
            return SanPhams.Where(sp => sp.IsHot).Take(soLuong).ToList();
        }

        // Lọc theo tên và theo hãng. Tham số rỗng thì bỏ qua điều kiện đó.
        public static List<SanPhamItem> LocSanPham(string tuKhoa, string hang)
        {
            IEnumerable<SanPhamItem> ketQua = SanPhams;

            if (!string.IsNullOrEmpty(tuKhoa))
                ketQua = ketQua.Where(sp => sp.Name.ToLower().Contains(tuKhoa.ToLower())
                    || sp.Brand.ToLower().Contains(tuKhoa.ToLower()));

            if (!string.IsNullOrEmpty(hang))
                ketQua = ketQua.Where(sp => sp.Brand == hang);

            return ketQua.ToList();
        }

        // Danh sách hãng để làm bộ lọc.
        public static List<string> DanhSachHang()
        {
            return SanPhams.Select(sp => sp.Brand).Distinct().OrderBy(hang => hang).ToList();
        }

        public static void ThemSanPham(SanPhamItem sanPham)
        {
            maSanPham = maSanPham + 1;
            sanPham.Id = maSanPham;
            SanPhams.Add(sanPham);
        }

        public static void XoaSanPham(int id)
        {
            SanPhams.RemoveAll(sp => sp.Id == id);
        }

        // ------------------------------------------------------------------
        //  Đơn hàng
        // ------------------------------------------------------------------

        public static void ThemDonHang(DonHang donHang)
        {
            maDonHang = maDonHang + 1;
            donHang.Id = maDonHang;
            DonHangs.Add(donHang);
        }

        public static DonHang TimDonHang(string maDon)
        {
            return DonHangs.FirstOrDefault(don => don.Code == maDon);
        }

        // Đơn hàng của một tài khoản, mới nhất trước.
        public static List<DonHang> DonHangCua(string tenDangNhap)
        {
            return DonHangs.Where(don => don.Username == tenDangNhap)
                .OrderByDescending(don => don.Id)
                .ToList();
        }

        // ------------------------------------------------------------------
        //  Dữ liệu mẫu
        // ------------------------------------------------------------------

        private static void TaoDuLieuMau()
        {
            // admin / admin123   và   khach / khach123
            ThemNguoiDung(new NguoiDung
            {
                Username = "admin",
                Password = "admin123",
                FullName = "Quản trị viên",
                Email = "admin@sneakerx.vn",
                Phone = "0900000000",
                IsAdmin = true
            });

            ThemNguoiDung(new NguoiDung
            {
                Username = "khach",
                Password = "khach123",
                FullName = "Nguyễn Văn Khách",
                Email = "khach@gmail.com",
                Phone = "0911111111",
                IsAdmin = false
            });

            ThemSanPham(new SanPhamItem { Name = "Nike Air Force 1", Brand = "Nike", Description = "Da chính hãng, đế cao su bám đường và đệm êm cả ngày.", Price = 2900000, Stock = 12, Sizes = "38,39,40,41", ImageUrl = "Content/images/products/nike-1-sm.jpg", IsHot = true });
            ThemSanPham(new SanPhamItem { Name = "Nike Air Jordan 1", Brand = "Nike", Description = "Dòng giày bóng rổ nổi tiếng nhất của Nike, da cao cấp và đệm êm.", Price = 4500000, Stock = 3, Sizes = "40,41,42,43", ImageUrl = "Content/images/products/nike-2-sm.jpg", IsHot = true });
            ThemSanPham(new SanPhamItem { Name = "Nike Air Max 90", Brand = "Nike", Description = "Đệm Air dày, hợp cả chạy bộ lẫn dạo phố.", Price = 4200000, Stock = 6, Sizes = "40,41,42,43", ImageUrl = "Content/images/products/nike-3-sm.jpg", IsHot = false });
            ThemSanPham(new SanPhamItem { Name = "NikeCourt Air Zoom Vapor Cage 4", Brand = "Nike", Description = "Thân lưới nhẹ thoáng, đế dẻo ôm chân khi vận động.", Price = 2750000, Stock = 14, Sizes = "39,40,41,42", ImageUrl = "Content/images/products/nike-4-sm.jpg", IsHot = false });
            ThemSanPham(new SanPhamItem { Name = "Converse Chuck 70 High", Brand = "Converse", Description = "Canvas cao cổ biểu tượng của phong cách skate từ năm 1970.", Price = 1950000, Stock = 0, Sizes = "38,39,40,41,42", ImageUrl = "Content/images/products/converse-5-sm.jpg", IsHot = true });
            ThemSanPham(new SanPhamItem { Name = "Converse Chuck Taylor All Star Low", Brand = "Converse", Description = "Mũi thấp dễ phối, canvas bền và đế cao su trắng cổ điển.", Price = 1850000, Stock = 11, Sizes = "38,39,40,41", ImageUrl = "Content/images/products/converse-6-sm.jpg", IsHot = false });
            ThemSanPham(new SanPhamItem { Name = "Vans Old Skool", Brand = "Vans", Description = "Dáng skate cổ điển, hợp quần jean và áo bomber.", Price = 1750000, Stock = 20, Sizes = "38,39,40,41", ImageUrl = "Content/images/products/vans-7-sm.jpg", IsHot = true });
            ThemSanPham(new SanPhamItem { Name = "New Balance 574", Brand = "New Balance", Description = "Chất liệu da lộn mềm, form classic bền qua nhiều mùa.", Price = 2450000, Stock = 10, Sizes = "39,40,41,42", ImageUrl = "Content/images/products/newbalance-8-sm.jpg", IsHot = false });
            ThemSanPham(new SanPhamItem { Name = "Puma RS-X", Brand = "Puma", Description = "Form chunky đặc trưng, phối lưới và da lộn, đế dày êm chân.", Price = 2450000, Stock = 12, Sizes = "39,40,41,42", ImageUrl = "Content/images/products/newbalance-9-sm.jpg", IsHot = false });
            ThemSanPham(new SanPhamItem { Name = "Adidas Superstar", Brand = "Adidas", Description = "Mũi giày vỏ sò biểu tượng, da cao cấp, đế cao su bám tốt.", Price = 2100000, Stock = 13, Sizes = "38,39,40,41", ImageUrl = "Content/images/products/adidas-10-sm.jpg", IsHot = false });
            ThemSanPham(new SanPhamItem { Name = "Nike LeBron Soldier 14", Brand = "Nike", Description = "Đệm Zoom Air, đai giữa bàn chân cố định khi di chuyển.", Price = 2650000, Stock = 9, Sizes = "40,41,42,43", ImageUrl = "Content/images/products/underarmour-11-sm.jpg", IsHot = false });
            ThemSanPham(new SanPhamItem { Name = "Common Projects Achilles Low", Brand = "Common Projects", Description = "Da Ý trắng tối giản, dãy số vàng đặc trưng của dòng Achilles.", Price = 10500000, Stock = 7, Sizes = "39,40,41,42", ImageUrl = "Content/images/products/commonprojects-12-sm.jpg", IsHot = true });
        }
    }
}
