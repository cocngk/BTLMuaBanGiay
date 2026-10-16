using System.Collections.Generic;

namespace ShoeStore.Models
{
    /// <summary>
    /// Menu data — thêm mục menu chỉ cần thêm 1 dòng ở đây, không đụng markup/codebehind.
    /// </summary>
    public class MenuItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }

        public static List<MenuItem> MainMenu()
        {
            return new List<MenuItem>
            {
                new MenuItem { Title = "Trang chủ", Url = "TrangChu.aspx", IconClass = "fa-solid fa-house" },
                new MenuItem { Title = "Sản phẩm", Url = "SanPham.aspx", IconClass = "fa-solid fa-shoe-prints" },
                new MenuItem { Title = "Tin tức", Url = "TinTuc.aspx", IconClass = "fa-solid fa-newspaper" },
                new MenuItem { Title = "Tư vấn", Url = "TuVan.aspx", IconClass = "fa-solid fa-headset" },
                new MenuItem { Title = "Thanh toán", Url = "ThanhToan.aspx", IconClass = "fa-solid fa-credit-card" }
            };
        }
    }
}
