using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class DangNhap : ShopPage
    {
        // Xử lý khi người dùng bấm nút.
        protected void btnLogin_Click(object nguonSuKien, EventArgs e)
        {
            if (!Page.IsValid) return;
            UserAccount taiKhoan = AuthHelper.FindByUsernameOrEmail(txtUsername.Text.Trim());
            if (taiKhoan == null) { ShowError("Tài khoản chưa được đăng ký! Vui lòng đăng ký tài khoản mới."); return; }
            if (!PasswordHasher.Verify(txtPassword.Text.Trim(), taiKhoan.PasswordSalt, taiKhoan.PasswordHash)) { ShowError("Sai mật khẩu! Vui lòng kiểm tra lại."); return; }
            AuthHelper.SignIn(taiKhoan);
            Toast.ShowAndRedirect(this, "Đăng nhập thành công! Đang chuyển đến trang chủ...", "success", "TrangChu.aspx");
        }

        // Kiểm tra dữ liệu ở phía máy chủ.
        protected void cvUsername_ServerValidate(object nguon, ServerValidateEventArgs thamSoSuKien)
        {
            string giaTri = (txtUsername.Text ?? "").Trim();
            thamSoSuKien.IsValid = giaTri.Contains("@") ? Regex.IsMatch(giaTri, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") : giaTri.Length >= 3;
            if (!thamSoSuKien.IsValid) cvUsername.ErrorMessage = giaTri.Contains("@") ? "Địa chỉ email không hợp lệ." : "Tên đăng nhập phải có ít nhất 3 ký tự.";
        }

        // Thực hiện việc hiển thị error.
        private void ShowError(string thongBao)
        {
            litFormError.Text = HttpUtility.HtmlEncode(thongBao);
            pnlFormError.Visible = true;
        }
    }
}
