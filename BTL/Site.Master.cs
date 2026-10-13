using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        [Serializable]
        public class UserAccount
        {
            public string FullName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Password { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitDefaultAccounts();
                HighlightCurrentMenu();

                TextBox txtSearch = FindControl("txtHeaderSearch") as TextBox;
                if (txtSearch != null && Request.QueryString["search"] != null)
                {
                    txtSearch.Text = Request.QueryString["search"];
                }
            }

            CheckLoginState();
        }

        private void InitDefaultAccounts()
        {
            if (Application["UserList"] == null)
            {
                List<UserAccount> users = new List<UserAccount>
                {
                    new UserAccount { FullName = "Nguyễn Văn A", Username = "admin", Email = "admin@sneakerx.com", Phone = "0987654321", Password = "123" },
                    new UserAccount { FullName = "Khách Hàng Demo", Username = "sneakerx", Email = "demo@sneakerx.com", Phone = "0912345678", Password = "123" }
                };
                Application["UserList"] = users;
            }
        }

        private void CheckLoginState()
        {
            Panel pnlGuest = FindControl("pnlGuest") as Panel;
            Panel pnlUser = FindControl("pnlUser") as Panel;
            Literal litUsername = FindControl("litUsername") as Literal;

            if (Session["Username"] != null)
            {
                if (pnlGuest != null) pnlGuest.Visible = false;
                if (pnlUser != null) pnlUser.Visible = true;
                if (litUsername != null) litUsername.Text = Session["Username"].ToString();
            }
            else
            {
                if (pnlGuest != null) pnlGuest.Visible = true;
                if (pnlUser != null) pnlUser.Visible = false;
            }
        }

        private void HighlightCurrentMenu()
        {
            string pageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath).ToLower();

            var navHome = FindControl("navHome") as System.Web.UI.HtmlControls.HtmlAnchor;
            var navProduct = FindControl("navProduct") as System.Web.UI.HtmlControls.HtmlAnchor;
            var navNews = FindControl("navNews") as System.Web.UI.HtmlControls.HtmlAnchor;
            var navConsult = FindControl("navConsult") as System.Web.UI.HtmlControls.HtmlAnchor;
            var navCart = FindControl("navCart") as System.Web.UI.HtmlControls.HtmlAnchor;
            var navCheckout = FindControl("navCheckout") as System.Web.UI.HtmlControls.HtmlAnchor;

            if (navHome != null) navHome.Attributes["class"] = "";
            if (navProduct != null) navProduct.Attributes["class"] = "";
            if (navNews != null) navNews.Attributes["class"] = "";
            if (navConsult != null) navConsult.Attributes["class"] = "";
            if (navCart != null) navCart.Attributes["class"] = "";
            if (navCheckout != null) navCheckout.Attributes["class"] = "";

            if (pageName.Contains("sanpham") && navProduct != null) navProduct.Attributes["class"] = "active";
            else if (pageName.Contains("tintuc") && navNews != null) navNews.Attributes["class"] = "active";
            else if (pageName.Contains("tuvan") && navConsult != null) navConsult.Attributes["class"] = "active";
            else if (pageName.Contains("giohang") && navCart != null) navCart.Attributes["class"] = "active";
            else if (pageName.Contains("thanhtoan") && navCheckout != null) navCheckout.Attributes["class"] = "active";
            else if (navHome != null) navHome.Attributes["class"] = "active";
        }

        protected void btnHeaderSearch_Click(object sender, EventArgs e)
        {
            TextBox txtSearch = FindControl("txtHeaderSearch") as TextBox;
            string keyword = txtSearch != null ? txtSearch.Text.Trim() : "";

            if (!string.IsNullOrEmpty(keyword))
            {
                Response.Redirect("SanPham.aspx?search=" + Server.UrlEncode(keyword));
            }
            else
            {
                Response.Redirect("SanPham.aspx");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session["Username"] = null;
            CheckLoginState();
            Response.Redirect("TrangChu.aspx");
        }
    }
}