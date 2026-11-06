using System;
using System.Web.UI;

namespace ShoeStore
{
    public partial class TabQuanTri : UserControl
    {
        public string ActiveTab { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkProducts.HRef = ResolveUrl("~/QuanTriSanPham.aspx");
            lnkOrders.HRef = ResolveUrl("~/QuanTriDonHang.aspx");
            lnkUsers.HRef = ResolveUrl("~/QuanTriNguoiDung.aspx");
            lnkProducts.Attributes["class"] = ActiveTab == "products" ? "admin-tab active" : "admin-tab";
            lnkOrders.Attributes["class"] = ActiveTab == "orders" ? "admin-tab active" : "admin-tab";
            lnkUsers.Attributes["class"] = ActiveTab == "users" ? "admin-tab active" : "admin-tab";
        }
    }
}
