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

        /// <summary>Các trang trong site — footer là chỗ tự nhiên cho những trang không lên header.</summary>
        public static List<FooterLink> PageLinks()
        {
            return new List<FooterLink>
            {
                new FooterLink { Title = "Sản phẩm", Url = "SanPham.aspx" },
                new FooterLink { Title = "Tin tức", Url = "TinTuc.aspx" },
                new FooterLink { Title = "Tư vấn", Url = "TuVan.aspx" },
                new FooterLink { Title = "Thanh toán", Url = "ThanhToan.aspx" },
                new FooterLink { Title = "Giỏ hàng", Url = "GioHang.aspx" }
            };
        }

        /// <summary>
        /// Chính sách chưa có trang riêng nên trỏ về form tư vấn với đúng chủ đề,
        /// thay vì để một liên kết chết.
        /// </summary>
        public static List<FooterLink> PolicyLinks()
        {
            return new List<FooterLink>
            {
                new FooterLink { Title = "Chính sách đổi trả", Url = "TuVan.aspx?topic=Return" },
                new FooterLink { Title = "Bảo hành sản phẩm", Url = "TuVan.aspx?topic=Warranty" },
                new FooterLink { Title = "Vận chuyển & giao hàng", Url = "TuVan.aspx?topic=Order" },
                new FooterLink { Title = "Thanh toán & bảo mật", Url = "TuVan.aspx?topic=Payment" },
                new FooterLink { Title = "Điều khoản sử dụng", Url = "TuVan.aspx?topic=Other" }
            };
        }

        public static List<string> Partners()
        {
            return new List<string>
            {
                "COD", "MoMo", "VNPay", "ZaloPay", "ViettelPost", "Giao Hàng Nhanh"
            };
        }
    }
}