using System;
using System.Collections.Generic;
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
            // Repeater khong giu item qua postback: bind o MOI lan render,
            // neu chi bind lan !IsPostBack thi menu/footer bien mat sau
            // khi bam nut (them gio, luu so luong, dang ky nhan tin...).
            BindMenu();
            BindFooter();

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
            // Khong ep muc nao active khi khong co muc nao khop: trang Thanh toan,
            // Gio hang, Dang nhap... deu khong phai muc menu, to do Trang chu se
            // to do sai.
            rptMenu.DataSource = items;
            rptMenu.DataBind();

            if (cartIcon != null) cartIcon.Attributes["class"] = "cart-icon-btn";
            if (pageName.Contains("giohang") && cartIcon != null) cartIcon.Attributes["class"] = "cart-icon-btn active";
        }

        private void BindFooter()
        {
            rptSocial.DataSource = Models.ShopInfo.SocialLinks();
            rptSocial.DataBind();

            rptFooterPages.DataSource = Models.ShopInfo.PageLinks();
            rptFooterPages.DataBind();

            rptFooterPolicies.DataSource = Models.ShopInfo.PolicyLinks();
            rptFooterPolicies.DataBind();

            rptPartners.DataSource = Models.ShopInfo.Partners();
            rptPartners.DataBind();
        }

        protected void btnNewsletter_Click(object sender, EventArgs e)
        {
            // Button co ValidationGroup nen khong kich hoat loi cua cac form
            // khac tren trang (form dang nhap, dang ky, tu van...).
            if (!Page.IsValid) return;

            txtNewsletterEmail.Text = "";
            Toast.Success(this,
                "Đăng ký thành công! Chúng tôi sẽ gửi tin khuyến mãi tới bạn.");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthHelper.SignOut();
            CheckLoginState();
            Response.Redirect("TrangChu.aspx");
        }
    }
}