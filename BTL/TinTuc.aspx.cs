using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class TinTuc : System.Web.UI.Page
    {
        public class BlogItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Summary { get; set; }
            public string Date { get; set; }
            public string ImageUrl { get; set; }
        }

        // ĐÃ ĐIỀU CHỈNH: 3 BÀI VIẾT / TRANG
        private const int PageSize = 3;

        public int CurrentNewsPage
        {
            get { return ViewState["CurrentNewsPage"] != null ? (int)ViewState["CurrentNewsPage"] : 1; }
            set { ViewState["CurrentNewsPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentNewsPage = 1;
                LoadBlogs();
            }
        }

        private void LoadBlogs()
        {
            List<BlogItem> allBlogs = GetMockBlogs();

            int totalItems = allBlogs.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            if (totalPages < 1) totalPages = 1;
            if (CurrentNewsPage > totalPages) CurrentNewsPage = totalPages;
            if (CurrentNewsPage < 1) CurrentNewsPage = 1;

            var pagedBlogs = allBlogs.Skip((CurrentNewsPage - 1) * PageSize).Take(PageSize).ToList();

            rptBlogs.DataSource = pagedBlogs;
            rptBlogs.DataBind();

            List<int> pages = Enumerable.Range(1, totalPages).ToList();
            rptNewsPages.DataSource = pages;
            rptNewsPages.DataBind();

            pnlNewsPagination.Visible = totalPages > 1;

            btnNewsPrev.CssClass = CurrentNewsPage == 1 ? "page-link-custom disabled" : "page-link-custom";
            btnNewsNext.CssClass = CurrentNewsPage == totalPages ? "page-link-custom disabled" : "page-link-custom";
        }

        protected void rptNewsPages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentNewsPage = Convert.ToInt32(e.CommandArgument);
                LoadBlogs();
            }
        }

        protected void btnNewsPrev_Click(object sender, EventArgs e)
        {
            if (CurrentNewsPage > 1)
            {
                CurrentNewsPage--;
                LoadBlogs();
            }
        }

        protected void btnNewsNext_Click(object sender, EventArgs e)
        {
            CurrentNewsPage++;
            LoadBlogs();
        }

        // BỔ SUNG ĐỦ 6 BÀI VIẾT ĐỂ CHẠY CHUẨN PHÂN TRANG (2 TRANG X 3 TIN)
        private List<BlogItem> GetMockBlogs()
        {
            return new List<BlogItem>
            {
                new BlogItem {
                    Id = 1,
                    Title = "5 Cách phối đồ cực đỉnh cùng Nike Air Force 1",
                    Summary = "Khám phá phong cách phối đồ từ đường phố năng động đến thanh lịch hiện đại cùng đôi giày quốc dân.",
                    Date = "11/10/2026",
                    ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?q=80&w=400"
                },
                new BlogItem {
                    Id = 2,
                    Title = "Bí quyết bảo quản giày thể thao không bị ố vàng mùa mưa",
                    Summary = "Những lưu ý quan trọng giúp đôi sneaker của bạn giữ nguyên độ mới ngay cả trong thời tiết ẩm ướt.",
                    Date = "09/10/2026",
                    ImageUrl = "https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?q=80&w=400"
                },
                new BlogItem {
                    Id = 3,
                    Title = "Lịch sử ra đời của dòng giày Jordan huyền thoại",
                    Summary = "Cùng nhìn lại chặng đường hơn 30 năm phát triển của đế chế Air Jordan đình đám thế giới.",
                    Date = "05/10/2026",
                    ImageUrl = "https://images.unsplash.com/photo-1556906781-9a412961c28c?q=80&w=400"
                },
                new BlogItem {
                    Id = 4,
                    Title = "Xu hướng Giày Chunky Sneaker càn quét sàn diễn 2026",
                    Summary = "Những đôi giày đế thô phá cách đang khẳng định vị thế dẫn đầu xu hướng thời trang giới trẻ.",
                    Date = "01/10/2026",
                    ImageUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?q=80&w=400"
                },
                new BlogItem {
                    Id = 5,
                    Title = "Top 7 xịt nano chống nước bảo vệ giày tốt nhất hiện nay",
                    Summary = "Đánh giá chi tiết các thương hiệu xịt chống nước nổi tiếng giúp bảo vệ đôi sneaker yêu thích.",
                    Date = "28/09/2026",
                    ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?q=80&w=400"
                },
                new BlogItem {
                    Id = 6,
                    Title = "Hướng dẫn buộc dây giày Sneaker độc lạ thu hút mọi ánh nhìn",
                    Summary = "Tổng hợp 10 kiểu thắt dây giày thể thao độc đáo đơn giản ai cũng tự làm được.",
                    Date = "25/09/2026",
                    ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=400"
                }
            };
        }
    }
}