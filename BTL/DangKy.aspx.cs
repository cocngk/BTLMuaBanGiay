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
            object daCo = KetNoi.Mot(
                "SELECT COUNT(*) FROM Users WHERE Username = @ten",
                KetNoi.P("@ten", tenDangNhap));

            if (Convert.ToInt32(daCo) > 0)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Tên đăng nhập này đã có người dùng.</span>";
                return;
            }

            KetNoi.ThucThi(
                "INSERT INTO Users (Username, PasswordHash, FullName, Email, Phone, IsAdmin) "
                + "VALUES (@ten, @matKhau, @hoTen, @email, @dienThoai, 0)",
                KetNoi.P("@ten", tenDangNhap),
                KetNoi.P("@matKhau", MatKhau.Bam(matKhau)),
                KetNoi.P("@hoTen", txtHoTen.Text.Trim()),
                KetNoi.P("@email", txtEmail.Text.Trim()),
                KetNoi.P("@dienThoai", txtDienThoai.Text.Trim()));

            Response.Redirect("DangNhap.aspx?dangky=1");
        }
    }
}
