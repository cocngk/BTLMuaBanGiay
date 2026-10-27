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
