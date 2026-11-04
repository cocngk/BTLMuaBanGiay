using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            BindMenu();
            BindFooter();
            UpdateUserState();
        }

        // Thực hiện việc nạp menu.
        private void BindMenu()
        {
            string trang = Path.GetFileName(Request.Url.AbsolutePath).ToLowerInvariant();
            List<ShoeStore.Models.MenuItem> danhSachMenu = ShoeStore.Models.MenuItem.MainMenu();
            foreach (ShoeStore.Models.MenuItem menu in danhSachMenu)
                menu.IsActive = menu.Url.Equals(trang, StringComparison.OrdinalIgnoreCase);
            rptMenu.DataSource = danhSachMenu;
            rptMenu.DataBind();
            cartIcon.Attributes["class"] = trang.Contains("giohang") ? "cart-icon-btn active" : "cart-icon-btn";
        }

        // Thực hiện việc nạp chân trang.
        private void BindFooter()
        {
            rptSocial.DataSource = ShopInfo.SocialLinks();
            rptSocial.DataBind();
            rptFooterPages.DataSource = ShopInfo.PageLinks();
            rptFooterPages.DataBind();
            rptFooterPolicies.DataSource = ShopInfo.PolicyLinks();
            rptFooterPolicies.DataBind();
        }

        // Thực hiện việc cập nhật người dùng trạng thái.
        private void UpdateUserState()
        {
            Panel guest = FindControl("pnlGuest") as Panel;
            Panel nguoiDung = FindControl("pnlUser") as Panel;
            Literal ten = FindControl("litUsername") as Literal;
            bool logged = AuthHelper.IsLoggedIn();
            if (guest != null) guest.Visible = !logged;
            if (nguoiDung != null) nguoiDung.Visible = logged;
            if (ten != null && logged) ten.Text = AuthHelper.CurrentUserName();
            lnkAdmin.Visible = AuthHelper.IsAdmin();

            int soLuong = new CartStore(Session).Load().TotalQuantity();
            cartCount.Visible = soLuong > 0;
            cartCount.InnerText = soLuong > 99 ? "99+" : soLuong.ToString();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnLogout_Click(object nguonSuKien, EventArgs e)
        {
            AuthHelper.SignOut();
            Response.Redirect("TrangChu.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
