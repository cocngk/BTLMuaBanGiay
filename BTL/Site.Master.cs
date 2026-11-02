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
            // Repeater không giữ item qua postback: bind ở MỌI lần render,
            // nếu chỉ bind lần !IsPostBack thì menu/footer biến mất sau
            // khi bấm nút (thêm giỏ, lưu số lượng, đăng ký nhận tin...).
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

            // Quyền đọc tươi từ kho mỗi lần render: vừa được cấp quyền thì link hiện
            // ra ngay, vừa bị thu thì biến mất — không phải đăng xuất rồi vào lại.
            lnkAdmin.Visible = AuthHelper.IsAdmin();

            UpdateCartBadge();
        }

        private void UpdateCartBadge()
        {
            if (cartCount == null) return;
            int total = new CartStore(Session).Load().TotalQuantity();
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
            // Không ép mục nào active khi không có mục nào khớp: trang Thanh toán,
            // Giỏ hàng, Đăng nhập... đều không phải mục menu, tô đỏ Trang chủ sẽ
            // tô đỏ sai.
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
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthHelper.SignOut();
            CheckLoginState();
            Response.Redirect("TrangChu.aspx");
        }
    }
}