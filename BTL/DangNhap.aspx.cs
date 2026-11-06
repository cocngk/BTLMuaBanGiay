using System;
using System.Web;
using System.Web.Security;

namespace ShoeStore
{
    // Đăng nhập bằng tên/email; tùy chọn ghi nhớ dùng Forms Authentication ticket 7 ngày.
    public partial class DangNhap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Request.QueryString["dangky"] == "1")
                litThongBao.Text = "<span class=\"thong-bao tot\">Đăng ký thành công, mời bạn đăng nhập.</span>";

            chkNho.Checked = Request.Cookies[FormsAuthentication.FormsCookieName] != null;
        }

        protected void btnDangNhap_Click(object sender, EventArgs e)
        {
            lblTenDangNhapError.Text = string.Empty;
            lblMatKhauError.Text = string.Empty;
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (tenDangNhap == "")
            {
                lblTenDangNhapError.Text = "Vui lòng nhập tên đăng nhập hoặc email.";
            }
            if (matKhau == "")
            {
                lblMatKhauError.Text = "Vui lòng nhập mật khẩu.";
            }
            if (tenDangNhap == "" || matKhau == "")
                return;

            NguoiDung nguoiDung = DuLieu.TimNguoiDung(tenDangNhap)
                ?? DuLieu.TimNguoiDungTheoEmail(tenDangNhap);

            if (nguoiDung == null)
            {
                lblTenDangNhapError.Text = "Tài khoản hoặc email không tồn tại.";
                return;
            }

            if (nguoiDung.Password != matKhau)
            {
                lblMatKhauError.Text = "Mật khẩu không đúng.";
                return;
            }

            // Lưu trạng thái đăng nhập vào Session
            Session["Username"] = nguoiDung.Username;
            Session["FullName"] = nguoiDung.FullName;
            Session["IsAdmin"] = nguoiDung.IsAdmin;

            // Cookie xác thực có chữ ký/mã hóa của ASP.NET, không lưu tên hay mật khẩu dạng thô.
            if (chkNho.Checked)
            {
                DateTime hetHan = DateTime.Now.AddDays(7);
                var ticket = new FormsAuthenticationTicket(
                    1, nguoiDung.Username, DateTime.Now, hetHan, true, string.Empty,
                    FormsAuthentication.FormsCookiePath);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(ticket));
                cookie.Expires = hetHan;
                cookie.HttpOnly = true;
                cookie.Secure = Request.IsSecureConnection;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                Response.Cookies.Add(cookie);
            }
            else
            {
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty);
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.HttpOnly = true;
                cookie.Secure = Request.IsSecureConnection;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                Response.Cookies.Add(cookie);
            }

            string trangTruoc = Request.QueryString["return"];
            if (string.IsNullOrWhiteSpace(trangTruoc))
                trangTruoc = Request.QueryString["ReturnUrl"];

            string noiChuyenDen = LaDuongDanNoiBo(trangTruoc) ? trangTruoc : "TrangChu.aspx";
            Response.Redirect(noiChuyenDen, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private bool LaDuongDanNoiBo(string duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan)) return false;
            if (duongDan.StartsWith("//", StringComparison.Ordinal)
                || duongDan.StartsWith("\\\\", StringComparison.Ordinal)
                || duongDan.IndexOf('\\') >= 0
                || duongDan.IndexOf('\r') >= 0
                || duongDan.IndexOf('\n') >= 0)
                return false;

            Uri uriTuyetDoi;
            if (Uri.TryCreate(duongDan, UriKind.Absolute, out uriTuyetDoi))
                return uriTuyetDoi.Scheme == Request.Url.Scheme
                    && uriTuyetDoi.Host == Request.Url.Host
                    && uriTuyetDoi.Port == Request.Url.Port;

            Uri uriTuongDoi;
            return Uri.TryCreate(duongDan, UriKind.Relative, out uriTuongDoi);
        }
    }
}
