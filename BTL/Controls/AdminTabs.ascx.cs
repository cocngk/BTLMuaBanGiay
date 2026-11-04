using System;
using System.IO;
using System.Web.UI.HtmlControls;

namespace ShoeStore
{
    public partial class AdminTabs : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
            link.Attributes["class"] = active ? "admin-tab active" : "admin-tab";
            if (active) link.Attributes["aria-current"] = "page";
        }
    }
}
