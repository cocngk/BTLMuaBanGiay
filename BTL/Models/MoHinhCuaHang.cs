using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShoeStore.Models
{
    public class BlogItem { public int Id { get; set; } public string Title { get; set; } public string Summary { get; set; } public string Date { get; set; } public int Views { get; set; } public string Category { get; set; } public string ImageUrl { get; set; } }
    public class CartItem { public int Id { get; set; } public string Name { get; set; } public string Size { get; set; } public decimal Price { get; set; } public int Quantity { get; set; } public string ImageUrl { get; set; } }

    public class Cart
    {
        private readonly List<CartItem> cacDong;
        // Thực hiện việc giỏ hàng.
        public Cart() : this(null) { }
        // Thực hiện việc giỏ hàng.
        public Cart(IEnumerable<CartItem> existing) { cacDong = existing == null ? new List<CartItem>() : new List<CartItem>(existing); }
        public IReadOnlyList<CartItem> Lines { get { return cacDong; } }
        public bool IsEmpty { get { return cacDong.Count == 0; } }
        // Thực hiện việc clamp qty.
        public static int ClampQty(int soLuong, int max) { max = max > 0 ? max : 1; return Math.Max(1, Math.Min(soLuong, max)); }
        // Thực hiện việc sản phẩm số lượng.
        public int ProductQuantity(int ma) { return cacDong.Where(x => x.Id == ma).Sum(x => x.Quantity); }
        // Thực hiện việc remaining chuyển đổi sang buy.
        public int RemainingToBuy(int ma, int stock, string excludeSize) { int soLuong = ProductQuantity(ma); if (excludeSize != null) soLuong -= cacDong.Where(x => x.Id == ma && x.Size == excludeSize).Sum(x => x.Quantity); return stock - soLuong; }
        // Thực hiện việc thêm or gộp.
        public void AddOrMerge(int ma, string ten, decimal gia, string kichThuoc, int soLuong, string anh, int stock)
        {
            CartItem phanTu = cacDong.FirstOrDefault(x => x.Id == ma && x.Size == kichThuoc);
            if (phanTu != null) phanTu.Quantity = ClampQty(phanTu.Quantity + soLuong, RemainingToBuy(ma, stock, kichThuoc));
            else cacDong.Add(new CartItem { Id = ma, Name = ten, Price = gia, Quantity = ClampQty(soLuong, RemainingToBuy(ma, stock, null)), Size = kichThuoc, ImageUrl = anh });
        }
        // Thực hiện việc xóa.
        public bool Remove(int ma, string kichThuoc) { return cacDong.RemoveAll(x => x.Id == ma && x.Size == kichThuoc) > 0; }
        // Thực hiện việc đặt số lượng.
        public void SetQuantity(int ma, string kichThuoc, int soLuong, int stock) { CartItem phanTu = cacDong.FirstOrDefault(x => x.Id == ma && x.Size == kichThuoc); if (phanTu != null) phanTu.Quantity = ClampQty(soLuong, RemainingToBuy(ma, stock, kichThuoc)); }
        // Thực hiện việc sub total.
        public decimal SubTotal() { return cacDong.Sum(x => x.Price * x.Quantity); }
        // Thực hiện việc total số lượng.
        public int TotalQuantity() { return cacDong.Sum(x => x.Quantity); }
        // Thực hiện việc xóa.
        public void Clear() { cacDong.Clear(); }
    }

    public static class PaymentMethods { public const string Cod = "COD"; public const string Qr = "QR"; }
    public class OrderInput { public string Username { get; set; } public string CustomerName { get; set; } public string Phone { get; set; } public string Address { get; set; } public string PaymentMethod { get; set; } }
    public static class OrderValidator
    {
        private static readonly Regex BieuThucSoDienThoai = new Regex(@"^[0-9]{9,11}$");
        // Thực hiện việc is valid phone.
        public static bool IsValidPhone(string soDienThoai) { return !string.IsNullOrEmpty(soDienThoai) && BieuThucSoDienThoai.IsMatch(soDienThoai); }
        // Thực hiện việc kiểm tra.
        public static string Validate(OrderInput donHang)
        {
            if (donHang == null) return "Thiếu thông tin đơn hàng!";
            if (string.IsNullOrEmpty(donHang.CustomerName) || string.IsNullOrEmpty(donHang.Phone) || string.IsNullOrEmpty(donHang.Address)) return "Bạn chưa nhập đủ thông tin nhận hàng!";
            return IsValidPhone(donHang.Phone) ? null : "Số điện thoại chưa hợp lệ. Vui lòng kiểm tra lại!";
        }
    }

    public class FooterLink { public string Title { get; set; } public string Url { get; set; } }
    public class SocialLink { public string Label { get; set; } public string Url { get; set; } public string IconClass { get; set; } }
    public class ProductColourway { public string Name { get; set; } public string Small { get; set; } public string Large { get; set; } }

    public class MenuItem
    {
        public string Title { get; set; } public string Url { get; set; } public string IconClass { get; set; } public bool IsActive { get; set; }
        // Thực hiện việc chính menu.
        public static List<MenuItem> MainMenu() { return new List<MenuItem> { new MenuItem { Title = "Trang chủ", Url = "TrangChu.aspx", IconClass = "fa-solid fa-house" }, new MenuItem { Title = "Sản phẩm", Url = "SanPham.aspx", IconClass = "fa-solid fa-shoe-prints" }, new MenuItem { Title = "Tin tức", Url = "TinTuc.aspx", IconClass = "fa-solid fa-newspaper" }, new MenuItem { Title = "Tư vấn", Url = "TuVan.aspx", IconClass = "fa-solid fa-headset" } }; }
    }

    public class OrderLine
    {
        public int ProductId { get; set; } public string Name { get; set; } public string Size { get; set; } public decimal Price { get; set; } public int Quantity { get; set; }
        // Thực hiện việc dòng total.
        public decimal LineTotal() { return Price * Quantity; }
        // Thực hiện việc chuyển đổi từ giỏ hàng.
        public static List<OrderLine> FromCart(IEnumerable<CartItem> cacDong) { return cacDong.Select(x => new OrderLine { ProductId = x.Id, Name = x.Name, Size = x.Size, Price = x.Price, Quantity = x.Quantity }).ToList(); }
    }

    public class Order
    {
        public string Code { get; set; } public DateTime CreatedAt { get; set; } public string Username { get; set; } public string Status { get; set; } = PendingStatus;
        public const string PendingStatus = "Chờ xác nhận"; public const string ShippingStatus = "Đang giao"; public const string DeliveredStatus = "Đã giao"; public const string CancelledStatus = "Đã huỷ";
        // Thực hiện việc toàn bộ statuses.
        public static List<string> AllStatuses() { return new List<string> { PendingStatus, ShippingStatus, DeliveredStatus, CancelledStatus }; }
        // Thực hiện việc is known trạng thái.
        public static bool IsKnownStatus(string trangThai) { return AllStatuses().Contains(trangThai); }
        public string CustomerName { get; set; } public string Phone { get; set; } public string Address { get; set; } public string PaymentMethod { get; set; } public List<OrderLine> Lines { get; set; }
        // Thực hiện việc total.
        public decimal Total() { return Lines.Sum(x => x.LineTotal()); }
        // Thực hiện việc total số lượng.
        public int TotalQuantity() { return Lines.Sum(x => x.Quantity); }
        // Thực hiện việc tạo mới mã.
        public static string NewCode() { lock (BoSinhMa) return "SX-" + DateTime.Now.ToString("yyMMdd") + "-" + BoSinhMa.Next(1000, 10000); }
        private static readonly Random BoSinhMa = new Random();
    }

    public class ProductItem
    {
        public int Id { get; set; } public string Name { get; set; } public string Brand { get; set; } public string Description { get; set; } public decimal Price { get; set; } public decimal? OldPrice { get; set; } public int Stock { get; set; } = 10; public string Sizes { get; set; } public string ImageUrl { get; set; } public string ImageUrlLarge { get; set; } public List<ProductColourway> Colourways { get; set; } public bool IsHot { get; set; }
        // Thực hiện việc large image đường dẫn.
        public string LargeImageUrl() { return string.IsNullOrEmpty(ImageUrlLarge) ? ImageUrl : ImageUrlLarge; }
        // Tạo danh sách kích thước.
        public List<string> SizeList() { return (Sizes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList(); }
        // Lấy kích thước đầu tiên.
        public string FirstSize() { List<string> kichThuoc = SizeList(); return kichThuoc.Count == 0 ? "40" : kichThuoc[0]; }
    }

    public class ProductReview
    {
        public int ProductId { get; set; } public string UserKey { get; set; } public string CustomerName { get; set; } public int Rating { get; set; } public string Comment { get; set; } public DateTime CreatedAt { get; set; }
        public string Stars { get { return new string('★', Rating) + new string('☆', 5 - Rating); } }
    }

    public static class ShopInfo
    {
        public static string BrandName { get { return "SneakerX"; } } public static string Tagline { get { return "Giày thể thao chính hãng, đổi trả 30 ngày, giao hàng toàn quốc."; } } public static string Hotline { get { return "1900 6789"; } } public static string Mobile { get { return "0988 123 456"; } } public static string Email { get { return "hotro@sneakerx.com"; } } public static string Address { get { return "128 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh"; } } public static string OpeningHours { get { return "8:00 – 21:00, cả 7 ngày trong tuần"; } } public static string CurrentYear { get { return DateTime.Now.Year.ToString(); } }
        // Thực hiện việc mạng xã hội liên kết.
        public static List<SocialLink> SocialLinks() { return new List<SocialLink> { new SocialLink { Label = "Facebook", IconClass = "fa-brands fa-facebook-f", Url = "#" }, new SocialLink { Label = "TikTok", IconClass = "fa-brands fa-tiktok", Url = "#" }, new SocialLink { Label = "Instagram", IconClass = "fa-brands fa-instagram", Url = "#" }, new SocialLink { Label = "YouTube", IconClass = "fa-brands fa-youtube", Url = "#" }, new SocialLink { Label = "Zalo", IconClass = "fa-solid fa-comments", Url = "#" } }; }
        // Thực hiện việc trang liên kết.
        public static List<FooterLink> PageLinks() { return new List<FooterLink> { new FooterLink { Title = "Sản phẩm", Url = "SanPham.aspx" }, new FooterLink { Title = "Tin tức", Url = "TinTuc.aspx" }, new FooterLink { Title = "Tư vấn", Url = "TuVan.aspx" }, new FooterLink { Title = "Giỏ hàng", Url = "GioHang.aspx" } }; }
        // Thực hiện việc chính sách liên kết.
        public static List<FooterLink> PolicyLinks() { return new List<FooterLink> { new FooterLink { Title = "Chính sách đổi trả", Url = "ChinhSach.aspx#doi-tra" }, new FooterLink { Title = "Bảo hành sản phẩm", Url = "ChinhSach.aspx#bao-hanh" }, new FooterLink { Title = "Vận chuyển & giao hàng", Url = "ChinhSach.aspx#van-chuyen" }, new FooterLink { Title = "Thanh toán & bảo mật", Url = "ChinhSach.aspx#thanh-toan" }, new FooterLink { Title = "Điều khoản sử dụng", Url = "ChinhSach.aspx#dieu-khoan" } }; }
    }
}
