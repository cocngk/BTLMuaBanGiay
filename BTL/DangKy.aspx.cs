using System;

namespace ShoeStore
{
    // Đăng ký tài khoản mới.
    public partial class DangKy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnDangKy_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (tenDangNhap.Length < 3)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Tên đăng nhập phải có ít nhất 3 ký tự.</span>";
                return;
            }

            if (matKhau.Length < 6)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Mật khẩu phải có ít nhất 6 ký tự.</span>";
                return;
            }

            if (matKhau != txtNhapLai.Text)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Hai lần nhập mật khẩu không giống nhau.</span>";
                return;
            }

            // Kiểm tra trùng tên đăng nhập
            if (DuLieu.TimNguoiDung(tenDangNhap) != null)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Tên đăng nhập này đã có người dùng.</span>";
                return;
            }

            NguoiDung nguoiDung = new NguoiDung();
            nguoiDung.Username = tenDangNhap;
            nguoiDung.Password = matKhau;
            nguoiDung.FullName = txtHoTen.Text.Trim();
            nguoiDung.Email = txtEmail.Text.Trim();
            nguoiDung.Phone = txtDienThoai.Text.Trim();
            nguoiDung.IsAdmin = false;

            DuLieu.ThemNguoiDung(nguoiDung);

            Response.Redirect("DangNhap.aspx?dangky=1");
        }
    }
}
