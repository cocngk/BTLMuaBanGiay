using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AuthHelper.GetUsers();
                BindMenu();
            }

            CheckLoginState();
        }

        private void CheckLoginState()
        {
            Panel pnlGuest = FindControl("pnlGuest") as Panel;
            Panel pnlUser = FindControl("pnlUser") as Panel;
            Literal litUsername = FindControl("litUsername") as Literal;

            if (AuthHelper.IsLoggedIn())
            {
                if (pnlGuest != null) pnlGuest.Visible = false;
                if (pnlUser != null) pnlUser.Visible = true;
                if (litUsername != null) litUsername.Text = AuthHelper.CurrentUserName();
            }
            else
            {
                if (pnlGuest != null) pnlGuest.Visible = true;
                if (pnlUser != null) pnlUser.Visible = false;
            }

            UpdateCartBadge();
        }

        private void UpdateCartBadge()
        {
            if (cartCount == null) return;
            int total = new SessionCartStore(Session).Load().TotalQuantity();
            cartCount.Visible = total > 0;
            cartCount.InnerText = total > 99 ? "99+" : total.ToString();
        }

        private void BindMenu()
        {
            string pageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath).ToLower();

            List<Models.MenuItem> items = Models.MenuItem.MainMenu();
            foreach (Models.MenuItem item in items)
            {
                item.IsActive = string.Equals(item.Url, pageName, StringComparison.OrdinalIgnoreCase);
            }
            if (!items.Any(i => i.IsActive) && !pageName.Contains("giohang"))
            {
                items[0].IsActive = true;
            }
            rptMenu.DataSource = items;
            rptMenu.DataBind();

            if (cartIcon != null) cartIcon.Attributes["class"] = "cart-icon-btn";
            if (pageName.Contains("giohang") && cartIcon != null) cartIcon.Attributes["class"] = "cart-icon-btn active";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthHelper.SignOut();
            CheckLoginState();
            Response.Redirect("TrangChu.aspx");
        }
    }
}