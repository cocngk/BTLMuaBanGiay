using System;
using System.Collections.Generic;

namespace ShoeStore.Models
{
    /// <summary>
    /// Thông tin cửa hàng — nguồn duy nhất cho số liệu liên hệ.
    /// Trước đây hotline nằm rải rác trong từng trang nên dễ lệch nhau;
    /// giờ thêm/sửa chỉ ở đây, footer và các trang khác cùng đọc một nguồn.
    /// </summary>
    public static class ShopInfo
    {
        public static string BrandName
        {
            get { return "SneakerX"; }
        }

        public static string Tagline
        {
            get { return "Giày thể thao chính hãng, đổi trả 30 ngày, giao hàng toàn quốc."; }
        }

        public static string Hotline
        {
            get { return "1900 6789"; }
        }

        public static string Mobile
        {
            get { return "0988 123 456"; }
        }

        public static string Email
        {
            get { return "hotro@sneakerx.com"; }
        }

        public static string Address
        {
            get { return "128 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh"; }
        }

        public static string OpeningHours
        {
            get { return "8:00 – 21:00, cả 7 ngày trong tuần"; }
        }

        public static string CurrentYear
        {
            get { return DateTime.Now.Year.ToString(); }
        }

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

        /// <summary>
        /// Các trang trong site. Không kèm Thanh toán: đó là bước trong phễu, vào
        /// từ menu khi giỏ trống chỉ ra trang trống — đường đi hợp lý là qua nút
        /// thanh toán ở trang Giỏ hàng.
        /// </summary>
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

        /// <summary>
        /// Mỗi chính sách là một mục trên trang ChinhSach; footer trỏ thẳng tới
        /// mục đó. Nhãn nói "chính sách" thì đích phải là văn bản chính sách,
        /// không phải form tư vấn (form dành cho câu hỏi, không phải để đọc).
        /// </summary>
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