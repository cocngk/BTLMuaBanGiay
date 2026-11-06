using System;
using System.Web;
using System.Web.Security;

namespace ShoeStore
{
    // Site.Master: phần đầu trang, menu, ô tìm kiếm và chân trang.
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["Username"] != null) return;

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value)) return;

            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null || ticket.Expired || !ticket.IsPersistent)
                {
                    ExpireAuthCookie();
                    return;
                }

                NguoiDung user = DuLieu.TimNguoiDung(ticket.Name);
                if (user == null)
                {
                    ExpireAuthCookie();
                    return;
                }

                Session["Username"] = user.Username;
                Session["FullName"] = user.FullName;
                Session["IsAdmin"] = user.IsAdmin;
            }
            catch
            {
                ExpireAuthCookie();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Hai biến Application đếm lượt truy cập, khai báo trong Global.asax
            litLuotTruyCap.Text = Convert.ToString(Application["LuotTruyCap"]);
            litDangOnline.Text = Convert.ToString(Application["DangOnline"]);

            // Trạng thái đăng nhập lấy từ Session
            string tenDangNhap = Session["Username"] as string;

            if (tenDangNhap == null)
            {
                litTaiKhoan.Text = "<a href=\"DangNhap.aspx\" class=\"btn-header-login\">Đăng nhập</a>" +
                    "<a href=\"DangKy.aspx\" class=\"btn-header-register\">Đăng ký</a>";
                btnThoat.Visible = false;
                phnQuanTri.Visible = false;
            }
            else
            {
                litTaiKhoan.Text = "<span class=\"user-logged-box\"><span class=\"user-avatar-circle\" aria-hidden=\"true\">" +
                    Server.HtmlEncode(tenDangNhap.Substring(0, 1).ToUpperInvariant()) + "</span><span class=\"user-name-text\">Xin chào, " +
                    Server.HtmlEncode(tenDangNhap) + "</span></span>";
                btnThoat.Visible = true;
                phnQuanTri.Visible = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            }
        }

        // Cập nhật sau sự kiện của trang con (thêm/xóa/sửa giỏ hàng).
        protected void Page_PreRender(object sender, EventArgs e)
        {
            litSoGio.Text = Gio.SoMon().ToString();
        }

        // Bấm nút Tìm: chuyển sang trang sản phẩm kèm từ khóa.
        protected void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();
            Response.Redirect("SanPham.aspx?search=" + Server.UrlEncode(tuKhoa));
        }

        // Bấm Thoát: xóa Session và xóa cookie nhớ đăng nhập.
        protected void btnThoat_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            ExpireAuthCookie();

            HttpCookie cookie = Request.Cookies["nhodangnhap"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            Response.Redirect("TrangChu.aspx");
        }

        private void ExpireAuthCookie()
        {
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.HttpOnly = true;
            cookie.Secure = Request.IsSecureConnection;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            Response.Cookies.Add(cookie);
        }
    }
}
