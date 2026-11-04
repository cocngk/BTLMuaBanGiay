using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShoeStore.Models
{
    // Dữ liệu một bài viết trên trang tin tức.
    public class BlogItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Date { get; set; }
        public int Views { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }

    // Một dòng sản phẩm trong giỏ hàng.
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }

    // Giỏ hàng của khách hoặc tài khoản đã đăng nhập.
    public class Cart
    {
        private readonly List<CartItem> dongGioHang;

        // Tạo một giỏ hàng trống.
        public Cart() : this(null) { }

        // Tạo giỏ hàng từ các dòng có sẵn.
        public Cart(IEnumerable<CartItem> dongDaCo)
        {
            dongGioHang = dongDaCo == null
                ? new List<CartItem>()
                : new List<CartItem>(dongDaCo);
        }

        public IReadOnlyList<CartItem> Lines { get { return dongGioHang; } }
        public bool IsEmpty { get { return dongGioHang.Count == 0; } }

        // Giới hạn số lượng trong khoảng từ 1 đến tồn kho.
        public static int ClampQty(int soLuong, int gioiHan)
        {
            int gioiHanHopLe = gioiHan > 0 ? gioiHan : 1;
            return Math.Max(1, Math.Min(soLuong, gioiHanHopLe));
        }

        // Tính tổng số lượng của một sản phẩm trong giỏ.
        public int ProductQuantity(int maSanPham)
        {
            return dongGioHang
                .Where(dong => dong.Id == maSanPham)
                .Sum(dong => dong.Quantity);
        }

        // Tính số lượng còn có thể mua thêm.
        public int RemainingToBuy(int maSanPham, int tonKho, string boQuaKichThuoc)
        {
            int daCo = ProductQuantity(maSanPham);

            if (boQuaKichThuoc != null)
            {
                daCo -= dongGioHang
                    .Where(dong => dong.Id == maSanPham && dong.Size == boQuaKichThuoc)
                    .Sum(dong => dong.Quantity);
            }

            return tonKho - daCo;
        }

        // Thêm sản phẩm mới hoặc cộng vào dòng cùng cỡ.
        public void AddOrMerge(int maSanPham, string tenSanPham, decimal gia,
            string kichThuoc, int soLuong, string anh, int tonKho)
        {
            // Không thêm hàng đã hết tồn kho.
            if (tonKho <= 0) return;

            CartItem dongCu = dongGioHang.FirstOrDefault(dong =>
                dong.Id == maSanPham && dong.Size == kichThuoc);

            if (dongCu != null)
            {
                int soLuongKhac = ProductQuantity(maSanPham) - dongCu.Quantity;
                int gioiHanDong = Math.Max(1, tonKho - soLuongKhac);
                dongCu.Quantity = ClampQty(dongCu.Quantity + soLuong, gioiHanDong);
                return;
            }

            int gioiHanMoi = tonKho - ProductQuantity(maSanPham);
            if (gioiHanMoi <= 0) return;

            dongGioHang.Add(new CartItem
            {
                Id = maSanPham,
                Name = tenSanPham,
                Price = gia,
                Quantity = ClampQty(soLuong, gioiHanMoi),
                Size = kichThuoc,
                ImageUrl = anh
            });
        }

        // Xóa một dòng theo sản phẩm và cỡ.
        public bool Remove(int maSanPham, string kichThuoc)
        {
            return dongGioHang.RemoveAll(dong =>
                dong.Id == maSanPham && dong.Size == kichThuoc) > 0;
        }

        // Cập nhật số lượng một dòng trong giỏ.
        public void SetQuantity(int maSanPham, string kichThuoc,
            int soLuong, int tonKho)
        {
            CartItem dong = dongGioHang.FirstOrDefault(item =>
                item.Id == maSanPham && item.Size == kichThuoc);

            if (dong != null)
            {
                int soLuongKhac = ProductQuantity(maSanPham) - dong.Quantity;
                dong.Quantity = ClampQty(soLuong, Math.Max(1, tonKho - soLuongKhac));
            }
        }

        // Tính tiền hàng trước phí khác.
        public decimal SubTotal()
        {
            return dongGioHang.Sum(dong => dong.Price * dong.Quantity);
        }

        // Tính tổng số đôi giày trong giỏ.
        public int TotalQuantity()
        {
            return dongGioHang.Sum(dong => dong.Quantity);
        }

        // Xóa toàn bộ giỏ hàng.
        public void Clear()
        {
            dongGioHang.Clear();
        }
    }

    public static class PaymentMethods
    {
        public const string Cod = "COD";
        public const string Qr = "QR";
    }

    // Thông tin khách nhập khi đặt hàng.
    public class OrderInput
    {
        public string Username { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
    }

    public static class OrderValidator
    {
        private static readonly Regex BieuThucSoDienThoai =
            new Regex(@"^[0-9]{9,11}$");

        // Kiểm tra số điện thoại có 9 đến 11 chữ số.
        public static bool IsValidPhone(string soDienThoai)
        {
            return !string.IsNullOrEmpty(soDienThoai)
                && BieuThucSoDienThoai.IsMatch(soDienThoai);
        }

        // Kiểm tra các thông tin bắt buộc của đơn hàng.
        public static string Validate(OrderInput thongTin)
        {
            if (thongTin == null) return "Thiếu thông tin đơn hàng!";

            if (string.IsNullOrEmpty(thongTin.CustomerName)
                || string.IsNullOrEmpty(thongTin.Phone)
                || string.IsNullOrEmpty(thongTin.Address))
            {
                return "Bạn chưa nhập đủ thông tin nhận hàng!";
            }

            if (thongTin.PaymentMethod != PaymentMethods.Cod
                && thongTin.PaymentMethod != PaymentMethods.Qr)
            {
                return "Phương thức thanh toán chưa hợp lệ!";
            }

            return IsValidPhone(thongTin.Phone)
                ? null
                : "Số điện thoại chưa hợp lệ. Vui lòng kiểm tra lại!";
        }
    }

    public class FooterLink
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class SocialLink
    {
        public string Label { get; set; }
        public string Url { get; set; }
        public string IconClass { get; set; }
    }

    public class ProductColourway
    {
        public string Name { get; set; }
        public string Small { get; set; }
        public string Large { get; set; }
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }

        // Tạo các mục menu chính của website.
        public static List<MenuItem> MainMenu()
        {
            return new List<MenuItem>
            {
                new MenuItem { Title = "Trang chủ", Url = "TrangChu.aspx", IconClass = "fa-solid fa-house" },
                new MenuItem { Title = "Sản phẩm", Url = "SanPham.aspx", IconClass = "fa-solid fa-shoe-prints" },
                new MenuItem { Title = "Tin tức", Url = "TinTuc.aspx", IconClass = "fa-solid fa-newspaper" },
                new MenuItem { Title = "Tư vấn", Url = "TuVan.aspx", IconClass = "fa-solid fa-headset" }
            };
        }
    }

    public class OrderLine
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Tính thành tiền của một dòng đơn hàng.
        public decimal LineTotal()
        {
            return Price * Quantity;
        }

        // Chuyển các dòng giỏ hàng thành dòng đơn hàng.
        public static List<OrderLine> FromCart(IEnumerable<CartItem> dongGioHang)
        {
            return (dongGioHang ?? Enumerable.Empty<CartItem>())
                .Select(dong => new OrderLine
                {
                    ProductId = dong.Id,
                    Name = dong.Name,
                    Size = dong.Size,
                    Price = dong.Price,
                    Quantity = dong.Quantity
                })
                .ToList();
        }
    }

    public class Order
    {
        public const string PendingStatus = "Chờ xác nhận";
        public const string ShippingStatus = "Đang giao";
        public const string DeliveredStatus = "Đã giao";
        public const string CancelledStatus = "Đã huỷ";

        private static readonly Random BoSinhMa = new Random();

        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string Status { get; set; } = PendingStatus;
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderLine> Lines { get; set; }

        // Trả về các trạng thái đơn hàng được phép sử dụng.
        public static List<string> AllStatuses()
        {
            return new List<string>
            {
                PendingStatus,
                ShippingStatus,
                DeliveredStatus,
                CancelledStatus
            };
        }

        // Kiểm tra một trạng thái có hợp lệ không.
        public static bool IsKnownStatus(string trangThai)
        {
            return AllStatuses().Contains(trangThai);
        }

        // Tính tổng tiền đơn hàng.
        public decimal Total()
        {
            return (Lines ?? new List<OrderLine>())
                .Sum(dong => dong.LineTotal());
        }

        // Tính tổng số lượng trong đơn hàng.
        public int TotalQuantity()
        {
            return (Lines ?? new List<OrderLine>())
                .Sum(dong => dong.Quantity);
        }

        // Tạo mã đơn hàng gần như duy nhất.
        public static string NewCode()
        {
            lock (BoSinhMa)
            {
                return "SX-"
                    + DateTime.Now.ToString("yyMMdd")
                    + "-"
                    + BoSinhMa.Next(1000, 10000);
            }
        }
    }

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
        public string ImageUrlLarge { get; set; }
        public List<ProductColourway> Colourways { get; set; }
        public bool IsHot { get; set; }

        // Chọn ảnh lớn nếu có, nếu không dùng ảnh mặc định.
        public string LargeImageUrl()
        {
            return string.IsNullOrEmpty(ImageUrlLarge)
                ? ImageUrl
                : ImageUrlLarge;
        }

        // Chuyển chuỗi kích thước thành danh sách.
        public List<string> SizeList()
        {
            return (Sizes ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(kichThuoc => kichThuoc.Trim())
                .ToList();
        }

        // Lấy kích thước đầu tiên hoặc dùng cỡ 40 mặc định.
        public string FirstSize()
        {
            List<string> danhSach = SizeList();
            return danhSach.Count == 0 ? "40" : danhSach[0];
        }
    }

    public class ProductReview
    {
        public int ProductId { get; set; }
        public string UserKey { get; set; }
        public string CustomerName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Stars
        {
            get { return new string('★', Rating) + new string('☆', 5 - Rating); }
        }
    }

    public static class ShopInfo
    {
        public static string BrandName { get { return "SneakerX"; } }
        public static string Tagline { get { return "Giày thể thao chính hãng, đổi trả 30 ngày, giao hàng toàn quốc."; } }
        public static string Hotline { get { return "1900 6789"; } }
        public static string Mobile { get { return "0988 123 456"; } }
        public static string Email { get { return "hotro@sneakerx.com"; } }
        public static string Address { get { return "128 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh"; } }
        public static string OpeningHours { get { return "8:00 – 21:00, cả 7 ngày trong tuần"; } }
        public static string CurrentYear { get { return DateTime.Now.Year.ToString(); } }

        // Tạo danh sách liên kết mạng xã hội ở chân trang.
        public static List<SocialLink> SocialLinks()
        {
            return new List<SocialLink>
            {
                new SocialLink { Label = "Facebook", IconClass = "fa-brands fa-facebook-f", Url = "#" },
                new SocialLink { Label = "TikTok", IconClass = "fa-brands fa-tiktok", Url = "#" },
                new SocialLink { Label = "Instagram", IconClass = "fa-brands fa-instagram", Url = "#" },
                new SocialLink { Label = "YouTube", IconClass = "fa-brands fa-youtube", Url = "#" },
                new SocialLink { Label = "Zalo", IconClass = "fa-solid fa-comments", Url = "#" }
            };
        }

        // Tạo các liên kết trang ở chân trang.
        public static List<FooterLink> PageLinks()
        {
            return new List<FooterLink>
            {
                new FooterLink { Title = "Sản phẩm", Url = "SanPham.aspx" },
                new FooterLink { Title = "Tin tức", Url = "TinTuc.aspx" },
                new FooterLink { Title = "Tư vấn", Url = "TuVan.aspx" },
                new FooterLink { Title = "Giỏ hàng", Url = "GioHang.aspx" }
            };
        }

        // Tạo các liên kết chính sách ở chân trang.
        public static List<FooterLink> PolicyLinks()
        {
            return new List<FooterLink>
            {
                new FooterLink { Title = "Chính sách đổi trả", Url = "ChinhSach.aspx#doi-tra" },
                new FooterLink { Title = "Bảo hành sản phẩm", Url = "ChinhSach.aspx#bao-hanh" },
                new FooterLink { Title = "Vận chuyển & giao hàng", Url = "ChinhSach.aspx#van-chuyen" },
                new FooterLink { Title = "Thanh toán & bảo mật", Url = "ChinhSach.aspx#thanh-toan" },
                new FooterLink { Title = "Điều khoản sử dụng", Url = "ChinhSach.aspx#dieu-khoan" }
            };
        }
    }
}
