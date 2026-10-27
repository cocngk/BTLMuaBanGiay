using System;
using System.IO;
using System.Web.UI.HtmlControls;

namespace ShoeStore
{
    /// <summary>
    /// Thanh tab quản trị. Tự đánh dấu tab đang mở theo tên trang, nên trang chỉ
    /// việc đặt control vào chỗ cần hiện — không phải truyền tham số "active".
    /// Trang sửa sản phẩm nằm trong cùng nhánh với danh sách sản phẩm.
    /// </summary>
    public partial class AdminTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Phải là URL từ gốc ứng dụng. Control này nằm ở /Controls/ nên ASP.NET
            // coi href tương đối trong markup là tương đối với THƯ MỤC CỦA CONTROL:
            // href="QuanTriDonHang.aspx" bị render thành "Controls/QuanTriDonHang.aspx",
            // bấm vào là 404 ở /Controls/. Dùng ResolveUrl("~/...") để chạy đúng cả
            // khi site nằm trong thư mục con.
            lnkProducts.HRef = ResolveUrl("~/QuanTriSanPham.aspx");
            lnkOrders.HRef = ResolveUrl("~/QuanTriDonHang.aspx");
            lnkUsers.HRef = ResolveUrl("~/QuanTriNguoiDung.aspx");

            string page = Path.GetFileName(Request.Url.AbsolutePath).ToLowerInvariant();

            Mark(lnkProducts, page == "quantrisanpham.aspx" || page == "suasanpham.aspx");
            Mark(lnkOrders, page == "quantridonhang.aspx");
            Mark(lnkUsers, page == "quantringuoidung.aspx");
        }

        private static void Mark(HtmlAnchor link, bool active)
        {
            if (link == null)
            {
                return;
            }

            link.Attributes["class"] = active ? "admin-tab active" : "admin-tab";
            if (active)
            {
                link.Attributes["aria-current"] = "page";
            }
        }
    }
}
