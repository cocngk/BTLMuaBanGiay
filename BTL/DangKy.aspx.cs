using System;
using System.Text.RegularExpressions;

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
            lblTenDangNhapError.Text = string.Empty;
            lblHoTenError.Text = string.Empty;
            lblEmailError.Text = string.Empty;
            lblDienThoaiError.Text = string.Empty;
            lblMatKhauError.Text = string.Empty;
            lblNhapLaiError.Text = string.Empty;

            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string dienThoai = txtDienThoai.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (tenDangNhap.Length == 0)
            {
                lblTenDangNhapError.Text = "Vui lòng nhập tên đăng nhập.";
                return;
            }
            if (tenDangNhap.Length < 3)
            {
                lblTenDangNhapError.Text = "Tên đăng nhập phải có ít nhất 3 ký tự.";
                return;
            }

            if (hoTen.Length == 0)
            {
                lblHoTenError.Text = "Vui lòng nhập họ tên.";
                return;
            }
            if (hoTen.Length > 100)
            {
                lblHoTenError.Text = "Họ tên không được vượt quá 100 ký tự.";
                return;
            }
            if (email.Length == 0)
            {
                lblEmailError.Text = "Vui lòng nhập email.";
                return;
            }
            if (email.Length > 254)
            {
                lblEmailError.Text = "Email không được vượt quá 254 ký tự.";
                return;
            }
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblEmailError.Text = "Địa chỉ email không hợp lệ.";
                return;
            }
            if (dienThoai.Length == 0)
            {
                lblDienThoaiError.Text = "Vui lòng nhập số điện thoại.";
                return;
            }
            if (!Regex.IsMatch(dienThoai, @"^[0-9]{9,11}$"))
            {
                lblDienThoaiError.Text = "Số điện thoại phải gồm từ 9 đến 11 chữ số.";
                return;
            }

            if (matKhau.Length == 0)
            {
                lblMatKhauError.Text = "Vui lòng nhập mật khẩu.";
                return;
            }
            if (matKhau.Length < 6)
            {
                lblMatKhauError.Text = "Mật khẩu phải có ít nhất 6 ký tự.";
                return;
            }

            if (matKhau.Length > 128)
            {
                lblMatKhauError.Text = "Mật khẩu không được vượt quá 128 ký tự.";
                return;
            }

            if (tenDangNhap.Length > 50)
            {
                lblTenDangNhapError.Text = "Tên đăng nhập không được vượt quá 50 ký tự.";
                return;
            }

            if (txtNhapLai.Text.Length == 0)
            {
                lblNhapLaiError.Text = "Vui lòng nhập lại mật khẩu.";
                return;
            }
            if (matKhau != txtNhapLai.Text)
            {
                lblNhapLaiError.Text = "Hai lần nhập mật khẩu không giống nhau.";
                return;
            }

            NguoiDung nguoiDung = new NguoiDung();
            nguoiDung.Username = tenDangNhap;
            nguoiDung.Password = matKhau;
            nguoiDung.FullName = hoTen;
            nguoiDung.Email = email;
            nguoiDung.Phone = dienThoai;
            nguoiDung.IsAdmin = false;

            bool trungTen;
            bool trungEmail;
            if (!DuLieu.ThuThemTaiKhoan(nguoiDung, out trungTen, out trungEmail))
            {
                if (trungTen)
                    lblTenDangNhapError.Text = "Tên đăng nhập này đã có người dùng.";
                else
                    lblEmailError.Text = "Email này đã được đăng ký.";
                return;
            }

            Response.Redirect("DangNhap.aspx?dangky=1", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
