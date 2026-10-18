using System.Collections.Generic;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>Single mock blog catalog (was disconnected from markup).</summary>
    public static class BlogStore
    {
        public static List<BlogItem> GetAll()
        {
            return new List<BlogItem>
            {
                new BlogItem {
                    Id = 1,
                    Title = "Cách chọn đúng size giày Sneaker chuẩn 100% khi đặt hàng Online",
                    Summary = "Không còn lo lắng việc đặt giày về bị chật hay rộng nữa. Dưới đây là quy chuẩn bảng đo chiều dài chân chuẩn từng milimet dành riêng cho bạn...",
                    Date = "12/10/2026",
                    Views = 1250,
                    Category = "Mẹo chọn Size giày",
                    ImageUrl = "Content/images/blogs/blog-1.jpg"
                },
                new BlogItem {
                    Id = 2,
                    Title = "5 Cách phối đồ cực đỉnh cùng Nike Air Force 1",
                    Summary = "Khám phá phong cách phối đồ từ đường phố năng động đến thanh lịch hiện đại cùng đôi giày quốc dân.",
                    Date = "11/10/2026",
                    Views = 980,
                    Category = "Phối đồ thời trang",
                    ImageUrl = "Content/images/blogs/blog-2.jpg"
                },
                new BlogItem {
                    Id = 3,
                    Title = "Bí quyết bảo quản giày thể thao không bị ố vàng mùa mưa",
                    Summary = "Những lưu ý quan trọng giúp đôi sneaker của bạn giữ nguyên độ mới ngay cả trong thời tiết ẩm ướt.",
                    Date = "09/10/2026",
                    Views = 870,
                    Category = "Hướng dẫn vệ sinh",
                    ImageUrl = "Content/images/blogs/blog-3.jpg"
                },
                new BlogItem {
                    Id = 4,
                    Title = "Lịch sử ra đời của dòng giày Jordan huyền thoại",
                    Summary = "Cùng nhìn lại chặng đường hơn 30 năm phát triển của đế chế Air Jordan đình đám thế giới.",
                    Date = "05/10/2026",
                    Views = 760,
                    Category = "Xu hướng Sneaker 2026",
                    ImageUrl = "Content/images/blogs/blog-4.jpg"
                },
                new BlogItem {
                    Id = 5,
                    Title = "Xu hướng Giày Chunky Sneaker càn quét sàn diễn 2026",
                    Summary = "Những đôi giày đế thô phá cách đang khẳng định vị thế dẫn đầu xu hướng thời trang giới trẻ.",
                    Date = "01/10/2026",
                    Views = 640,
                    Category = "Xu hướng Sneaker 2026",
                    ImageUrl = "Content/images/blogs/blog-5.jpg"
                },
                new BlogItem {
                    Id = 6,
                    Title = "Top 7 xịt nano chống nước bảo vệ giày tốt nhất hiện nay",
                    Summary = "Đánh giá chi tiết các thương hiệu xịt chống nước nổi tiếng giúp bảo vệ đôi sneaker yêu thích.",
                    Date = "28/09/2026",
                    Views = 520,
                    Category = "Hướng dẫn vệ sinh",
                    ImageUrl = "Content/images/blogs/blog-6.jpg"
                },
                new BlogItem {
                    Id = 7,
                    Title = "Hướng dẫn buộc dây giày Sneaker độc lạ thu hút mọi ánh nhìn",
                    Summary = "Tổng hợp 10 kiểu thắt dây giày thể thao độc đáo đơn giản ai cũng tự làm được.",
                    Date = "25/09/2026",
                    Views = 410,
                    Category = "Phối đồ thời trang",
                    ImageUrl = "Content/images/blogs/blog-7.jpg"
                }
            };
        }
    }
}
