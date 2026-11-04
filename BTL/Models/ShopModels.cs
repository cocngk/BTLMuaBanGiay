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
        private readonly List<CartItem> lines;
        public Cart() : this(null) { }
        public Cart(IEnumerable<CartItem> existing) { lines = existing == null ? new List<CartItem>() : new List<CartItem>(existing); }
        public IReadOnlyList<CartItem> Lines { get { return lines; } }
        public bool IsEmpty { get { return lines.Count == 0; } }
        public static int ClampQty(int qty, int max) { max = max > 0 ? max : 1; return Math.Max(1, Math.Min(qty, max)); }
        public int ProductQuantity(int id) { return lines.Where(x => x.Id == id).Sum(x => x.Quantity); }
        public int RemainingToBuy(int id, int stock, string excludeSize) { int count = ProductQuantity(id); if (excludeSize != null) count -= lines.Where(x => x.Id == id && x.Size == excludeSize).Sum(x => x.Quantity); return stock - count; }
        public void AddOrMerge(int id, string name, decimal price, string size, int qty, string image, int stock)
        {
            CartItem item = lines.FirstOrDefault(x => x.Id == id && x.Size == size);
            if (item != null) item.Quantity = ClampQty(item.Quantity + qty, RemainingToBuy(id, stock, size));
            else lines.Add(new CartItem { Id = id, Name = name, Price = price, Quantity = ClampQty(qty, RemainingToBuy(id, stock, null)), Size = size, ImageUrl = image });
        }
        public bool Remove(int id, string size) { return lines.RemoveAll(x => x.Id == id && x.Size == size) > 0; }
        public void SetQuantity(int id, string size, int qty, int stock) { CartItem item = lines.FirstOrDefault(x => x.Id == id && x.Size == size); if (item != null) item.Quantity = ClampQty(qty, RemainingToBuy(id, stock, size)); }
        public decimal SubTotal() { return lines.Sum(x => x.Price * x.Quantity); }
        public int TotalQuantity() { return lines.Sum(x => x.Quantity); }
        public void Clear() { lines.Clear(); }
    }

    public static class PaymentMethods { public const string Cod = "COD"; public const string Qr = "QR"; }
    public class OrderInput { public string Username { get; set; } public string CustomerName { get; set; } public string Phone { get; set; } public string Address { get; set; } public string PaymentMethod { get; set; } }
    public static class OrderValidator
    {
        private static readonly Regex PhoneRegex = new Regex(@"^[0-9]{9,11}$");
        public static bool IsValidPhone(string phone) { return !string.IsNullOrEmpty(phone) && PhoneRegex.IsMatch(phone); }
        public static string Validate(OrderInput order)
        {
            if (order == null) return "Thiếu thông tin đơn hàng!";
            if (string.IsNullOrEmpty(order.CustomerName) || string.IsNullOrEmpty(order.Phone) || string.IsNullOrEmpty(order.Address)) return "Bạn chưa nhập đủ thông tin nhận hàng!";
            return IsValidPhone(order.Phone) ? null : "Số điện thoại chưa hợp lệ. Vui lòng kiểm tra lại!";
        }
    }

    public class FooterLink { public string Title { get; set; } public string Url { get; set; } }
    public class SocialLink { public string Label { get; set; } public string Url { get; set; } public string IconClass { get; set; } }
    public class ProductColourway { public string Name { get; set; } public string Small { get; set; } public string Large { get; set; } }

    public class MenuItem
    {
        public string Title { get; set; } public string Url { get; set; } public string IconClass { get; set; } public bool IsActive { get; set; }
        public static List<MenuItem> MainMenu() { return new List<MenuItem> { new MenuItem { Title = "Trang chủ", Url = "TrangChu.aspx", IconClass = "fa-solid fa-house" }, new MenuItem { Title = "Sản phẩm", Url = "SanPham.aspx", IconClass = "fa-solid fa-shoe-prints" }, new MenuItem { Title = "Tin tức", Url = "TinTuc.aspx", IconClass = "fa-solid fa-newspaper" }, new MenuItem { Title = "Tư vấn", Url = "TuVan.aspx", IconClass = "fa-solid fa-headset" } }; }
    }

    public class OrderLine
    {
        public int ProductId { get; set; } public string Name { get; set; } public string Size { get; set; } public decimal Price { get; set; } public int Quantity { get; set; }
        public decimal LineTotal() { return Price * Quantity; }
        public static List<OrderLine> FromCart(IEnumerable<CartItem> lines) { return lines.Select(x => new OrderLine { ProductId = x.Id, Name = x.Name, Size = x.Size, Price = x.Price, Quantity = x.Quantity }).ToList(); }
    }

    public class Order
    {
        public string Code { get; set; } public DateTime CreatedAt { get; set; } public string Username { get; set; } public string Status { get; set; } = PendingStatus;
        public const string PendingStatus = "Chờ xác nhận"; public const string ShippingStatus = "Đang giao"; public const string DeliveredStatus = "Đã giao"; public const string CancelledStatus = "Đã huỷ";
        public static List<string> AllStatuses() { return new List<string> { PendingStatus, ShippingStatus, DeliveredStatus, CancelledStatus }; }
        public static bool IsKnownStatus(string status) { return AllStatuses().Contains(status); }
        public string CustomerName { get; set; } public string Phone { get; set; } public string Address { get; set; } public string PaymentMethod { get; set; } public List<OrderLine> Lines { get; set; }
        public decimal Total() { return Lines.Sum(x => x.LineTotal()); }
        public int TotalQuantity() { return Lines.Sum(x => x.Quantity); }
        public static string NewCode() { lock (CodeRandom) return "SX-" + DateTime.Now.ToString("yyMMdd") + "-" + CodeRandom.Next(1000, 10000); }
        private static readonly Random CodeRandom = new Random();
    }

    public class ProductItem
    {
        public int Id { get; set; } public string Name { get; set; } public string Brand { get; set; } public string Description { get; set; } public decimal Price { get; set; } public decimal? OldPrice { get; set; } public int Stock { get; set; } = 10; public string Sizes { get; set; } public string ImageUrl { get; set; } public string ImageUrlLarge { get; set; } public List<ProductColourway> Colourways { get; set; } public bool IsHot { get; set; }
        public string LargeImageUrl() { return string.IsNullOrEmpty(ImageUrlLarge) ? ImageUrl : ImageUrlLarge; }
        public List<string> SizeList() { return (Sizes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList(); }
        public string FirstSize() { List<string> sizes = SizeList(); return sizes.Count == 0 ? "40" : sizes[0]; }
    }

    public class ProductReview
    {
        public int ProductId { get; set; } public string UserKey { get; set; } public string CustomerName { get; set; } public int Rating { get; set; } public string Comment { get; set; } public DateTime CreatedAt { get; set; }
        public string Stars { get { return new string('★', Rating) + new string('☆', 5 - Rating); } }
    }

    public static class ShopInfo
    {
        public static string BrandName { get { return "SneakerX"; } } public static string Tagline { get { return "Giày thể thao chính hãng, đổi trả 30 ngày, giao hàng toàn quốc."; } } public static string Hotline { get { return "1900 6789"; } } public static string Mobile { get { return "0988 123 456"; } } public static string Email { get { return "hotro@sneakerx.com"; } } public static string Address { get { return "128 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh"; } } public static string OpeningHours { get { return "8:00 – 21:00, cả 7 ngày trong tuần"; } } public static string CurrentYear { get { return DateTime.Now.Year.ToString(); } }
        public static List<SocialLink> SocialLinks() { return new List<SocialLink> { new SocialLink { Label = "Facebook", IconClass = "fa-brands fa-facebook-f", Url = "#" }, new SocialLink { Label = "TikTok", IconClass = "fa-brands fa-tiktok", Url = "#" }, new SocialLink { Label = "Instagram", IconClass = "fa-brands fa-instagram", Url = "#" }, new SocialLink { Label = "YouTube", IconClass = "fa-brands fa-youtube", Url = "#" }, new SocialLink { Label = "Zalo", IconClass = "fa-solid fa-comments", Url = "#" } }; }
        public static List<FooterLink> PageLinks() { return new List<FooterLink> { new FooterLink { Title = "Sản phẩm", Url = "SanPham.aspx" }, new FooterLink { Title = "Tin tức", Url = "TinTuc.aspx" }, new FooterLink { Title = "Tư vấn", Url = "TuVan.aspx" }, new FooterLink { Title = "Giỏ hàng", Url = "GioHang.aspx" } }; }
        public static List<FooterLink> PolicyLinks() { return new List<FooterLink> { new FooterLink { Title = "Chính sách đổi trả", Url = "ChinhSach.aspx#doi-tra" }, new FooterLink { Title = "Bảo hành sản phẩm", Url = "ChinhSach.aspx#bao-hanh" }, new FooterLink { Title = "Vận chuyển & giao hàng", Url = "ChinhSach.aspx#van-chuyen" }, new FooterLink { Title = "Thanh toán & bảo mật", Url = "ChinhSach.aspx#thanh-toan" }, new FooterLink { Title = "Điều khoản sử dụng", Url = "ChinhSach.aspx#dieu-khoan" } }; }
    }
}
