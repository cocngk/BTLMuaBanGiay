using System;
using System.Data;
using System.Web;

namespace ShoeStore
{
    // Đăng nhập: kiểm tra mật khẩu trong bảng Users, lưu tên vào Session.
    // Nếu chọn "Nhớ đăng nhập" thì lưu tên vào Cookie 7 ngày.
    public partial class DangNhap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Request.QueryString["dangky"] == "1")
                litThongBao.Text = "<span class=\"thong-bao tot\">Đăng ký thành công, mời bạn đăng nhập.</span>";

            // Lần sau mở trang, nếu có cookie thì điền sẵn tên đăng nhập
            HttpCookie cookie = Request.Cookies["nhodangnhap"];
            if (cookie != null)
            {
                txtTenDangNhap.Text = cookie.Value;
                chkNho.Checked = true;
            }
        }

        protected void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (tenDangNhap == "" || matKhau == "")
            {
                litThongBao.Text = "<span class=\"thong-bao\">Vui lòng nhập tên đăng nhập và mật khẩu.</span>";
                return;
            }

            DataTable bang = KetNoi.Bang(
                "SELECT * FROM Users WHERE Username = @ten",
                KetNoi.P("@ten", tenDangNhap));

            if (bang.Rows.Count == 0)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Tài khoản không tồn tại.</span>";
                return;
            }

            DataRow nguoiDung = bang.Rows[0];

            if (MatKhau.Bam(matKhau) != Convert.ToString(nguoiDung["PasswordHash"]))
            {
                litThongBao.Text = "<span class=\"thong-bao\">Mật khẩu không đúng.</span>";
                return;
            }

            // Lưu trạng thái đăng nhập vào Session
            Session["Username"] = Convert.ToString(nguoiDung["Username"]);
            Session["FullName"] = Convert.ToString(nguoiDung["FullName"]);
            Session["IsAdmin"] = Convert.ToBoolean(nguoiDung["IsAdmin"]);

            // Nhớ đăng nhập bằng Cookie
            if (chkNho.Checked)
            {
                HttpCookie cookie = new HttpCookie("nhodangnhap");
                cookie.Value = tenDangNhap;
                cookie.Expires = DateTime.Now.AddDays(7);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
            }

            // Quay lại trang người dùng đang xem, hoặc về trang chủ
            string quayLai = Request.QueryString["return"];
            if (string.IsNullOrEmpty(quayLai) || quayLai.Contains("://"))
                quayLai = "TrangChu.aspx";

            Response.Redirect(quayLai);
        }
    }
}
