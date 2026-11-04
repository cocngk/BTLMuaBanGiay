using System;
using System.IO;
using System.Web.UI.HtmlControls;

namespace ShoeStore
{
    public partial class TabQuanTri : System.Web.UI.UserControl
    {
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            lnkProducts.HRef = ResolveUrl("~/QuanTriSanPham.aspx");
            lnkOrders.HRef = ResolveUrl("~/QuanTriDonHang.aspx");
            lnkUsers.HRef = ResolveUrl("~/QuanTriNguoiDung.aspx");
            string trang = Path.GetFileName(Request.Url.AbsolutePath).ToLowerInvariant();
            Mark(lnkProducts, trang == "quantrisanpham.aspx" || trang == "suasanpham.aspx");
            Mark(lnkOrders, trang == "quantridonhang.aspx");
            Mark(lnkUsers, trang == "quantringuoidung.aspx");
        }

        // Thực hiện việc mark.
        private static void Mark(HtmlAnchor link, bool dangChon)
        {
            link.Attributes["class"] = dangChon ? "admin-tab active" : "admin-tab";
            if (dangChon) link.Attributes["aria-current"] = "page";
        }
    }
}
