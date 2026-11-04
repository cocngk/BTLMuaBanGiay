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
        protected void Page_Load(object sender, EventArgs e)
        {
            BindMenu();
            BindFooter();
            UpdateUserState();
        }

        private void BindMenu()
        {
            string page = Path.GetFileName(Request.Url.AbsolutePath).ToLowerInvariant();
            List<ShoeStore.Models.MenuItem> items = ShoeStore.Models.MenuItem.MainMenu();
            foreach (ShoeStore.Models.MenuItem item in items) item.IsActive = item.Url.Equals(page, StringComparison.OrdinalIgnoreCase);
            rptMenu.DataSource = items;
            rptMenu.DataBind();
            cartIcon.Attributes["class"] = page.Contains("giohang") ? "cart-icon-btn active" : "cart-icon-btn";
        }

        private void BindFooter()
        {
            rptSocial.DataSource = ShopInfo.SocialLinks();
            rptSocial.DataBind();
            rptFooterPages.DataSource = ShopInfo.PageLinks();
            rptFooterPages.DataBind();
            rptFooterPolicies.DataSource = ShopInfo.PolicyLinks();
            rptFooterPolicies.DataBind();
        }

        private void UpdateUserState()
        {
            Panel guest = FindControl("pnlGuest") as Panel;
            Panel user = FindControl("pnlUser") as Panel;
            Literal name = FindControl("litUsername") as Literal;
            bool logged = AuthHelper.IsLoggedIn();
            if (guest != null) guest.Visible = !logged;
            if (user != null) user.Visible = logged;
            if (name != null && logged) name.Text = AuthHelper.CurrentUserName();
            lnkAdmin.Visible = AuthHelper.IsAdmin();
            int count = new CartStore(Session).Load().TotalQuantity();
            cartCount.Visible = count > 0;
            cartCount.InnerText = count > 99 ? "99+" : count.ToString();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthHelper.SignOut();
            Response.Redirect("TrangChu.aspx");
        }
    }
}
