using System;
using System.Web;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class DangKy : ShopPage
    {
        // Xử lý khi người dùng bấm nút.
        protected void btnRegister_Click(object nguonSuKien, EventArgs e)
        {
            if (!Page.IsValid) return;

            UserAccount taiKhoan = MemoryUserStore.NewUser(
                txtFullName.Text.Trim(),
                txtUsername.Text.Trim(),
                txtEmail.Text.Trim(),
                txtPhone.Text.Trim(),
                txtPassword.Text);
            AddUserOutcome ketQua = AuthHelper.TryAdd(taiKhoan);
            if (ketQua == AddUserOutcome.UsernameTaken)
            {
                ShowError("Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.");
                return;
            }
            if (ketQua == AddUserOutcome.EmailTaken)
            {
                ShowError("Email này đã được đăng ký!");
                return;
            }

            AuthHelper.SignIn(taiKhoan);
            Toast.ShowAndRedirect(this, "Đăng ký thành công! Bạn đã đăng nhập.", "success", "TrangChu.aspx");
        }

        // Thực hiện việc hiển thị error.
        private void ShowError(string thongBao)
        {
            litFormError.Text = HttpUtility.HtmlEncode(thongBao);
            pnlFormError.Visible = true;
        }

        [System.Web.Services.WebMethod]
        // Kiểm tra trùng tên đăng nhập và email khi người dùng nhập form.
        public static object CheckDuplicate(string tenDangNhap, string email)
        {
            string ten = (tenDangNhap ?? "").Trim();
            string diaChi = (email ?? "").Trim();
            return new
            {
                userTaken = ten.Length > 0 && AuthHelper.UsernameExists(ten),
                emailTaken = diaChi.Length > 0 && AuthHelper.EmailExists(diaChi)
            };
        }
    }
}
