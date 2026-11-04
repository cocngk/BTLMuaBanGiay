using System;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class TaiKhoan : ShopPage
    {
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để xem tài khoản của bạn.")) return;
            pnlPage.Visible = true;
            if (!IsPostBack) LoadAccount();
        }

        // Thực hiện việc tải tài khoản.
        private void LoadAccount()
        {
            UserAccount taiKhoan = AuthHelper.CurrentAccount();
            if (taiKhoan == null)
            {
                pnlPage.Visible = false;
                Toast.ShowAndRedirect(this, "Phiên đăng nhập không còn hợp lệ. Vui lòng đăng nhập lại.", "info", "DangNhap.aspx");
                return;
            }
            litName.Text = taiKhoan.DisplayName;
            litUsername.Text = taiKhoan.Username;
            litEmail.Text = taiKhoan.Email;
            litPhone.Text = string.IsNullOrEmpty(taiKhoan.Phone) ? "Chưa cập nhật" : taiKhoan.Phone;
            litRole.Text = taiKhoan.IsAdmin ? "Quản trị viên" : "Khách hàng";
            spnRole.Attributes["class"] = taiKhoan.IsAdmin ? "account-role is-admin" : "account-role";
            pnlAdminEntry.Visible = taiKhoan.IsAdmin;
            txtFullName.Text = taiKhoan.FullName;
            txtUsername.Text = taiKhoan.Username;
            txtEmail.Text = taiKhoan.Email;
            txtPhone.Text = taiKhoan.Phone;
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnSaveProfile_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để sửa hồ sơ.") || !Page.IsValid) return;
            ProfileUpdateOutcome ketQua = AuthHelper.TryUpdateProfile(AuthHelper.CurrentUserKey(), txtFullName.Text.Trim(), txtEmail.Text.Trim(), txtPhone.Text.Trim());
            if (ketQua == ProfileUpdateOutcome.Updated)
            {
                AuthHelper.RefreshDisplayName();
                Toast.ShowAndRedirect(this, "Đã cập nhật hồ sơ.", "success", "TaiKhoan.aspx");
            }
            else ErrorProfile(ketQua == ProfileUpdateOutcome.EmailTaken ? "Email này đã được một tài khoản khác dùng. Vui lòng chọn email khác." : "Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại.");
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnChangePassword_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để đổi mật khẩu.") || !Page.IsValid) return;
            PasswordChangeOutcome ketQua = AuthHelper.TryChangePassword(AuthHelper.CurrentUserKey(), txtCurrentPassword.Text, txtNewPassword.Text);
            if (ketQua == PasswordChangeOutcome.Changed) Toast.ShowAndRedirect(this, "Đã đổi mật khẩu. Hãy dùng mật khẩu mới ở lần đăng nhập sau.", "success", "TaiKhoan.aspx");
            else ErrorPassword(ketQua == PasswordChangeOutcome.WrongPassword ? "Mật khẩu hiện tại không đúng." : "Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại.");
        }

        // Thực hiện việc error hồ sơ.
        private void ErrorProfile(string vanBan) { litProfileError.Text = vanBan; pnlProfileError.Visible = true; }
        // Thực hiện việc error mật khẩu.
        private void ErrorPassword(string vanBan) { litPasswordError.Text = vanBan; pnlPasswordError.Visible = true; }
    }
}
