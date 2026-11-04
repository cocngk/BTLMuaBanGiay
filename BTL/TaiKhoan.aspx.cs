using System;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class TaiKhoan : ShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để xem tài khoản của bạn.")) return;
            pnlPage.Visible = true;
            if (!IsPostBack) LoadAccount();
        }

        private void LoadAccount()
        {
            UserAccount account = AuthHelper.CurrentAccount();
            if (account == null)
            {
                pnlPage.Visible = false;
                Toast.ShowAndRedirect(this, "Phiên đăng nhập không còn hợp lệ. Vui lòng đăng nhập lại.", "info", "DangNhap.aspx");
                return;
            }
            litName.Text = account.DisplayName;
            litUsername.Text = account.Username;
            litEmail.Text = account.Email;
            litPhone.Text = string.IsNullOrEmpty(account.Phone) ? "Chưa cập nhật" : account.Phone;
            litRole.Text = account.IsAdmin ? "Quản trị viên" : "Khách hàng";
            spnRole.Attributes["class"] = account.IsAdmin ? "account-role is-admin" : "account-role";
            pnlAdminEntry.Visible = account.IsAdmin;
            txtFullName.Text = account.FullName;
            txtUsername.Text = account.Username;
            txtEmail.Text = account.Email;
            txtPhone.Text = account.Phone;
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để sửa hồ sơ.") || !Page.IsValid) return;
            ProfileUpdateOutcome result = AuthHelper.TryUpdateProfile(AuthHelper.CurrentUserKey(), txtFullName.Text.Trim(), txtEmail.Text.Trim(), txtPhone.Text.Trim());
            if (result == ProfileUpdateOutcome.Updated)
            {
                AuthHelper.RefreshDisplayName();
                Toast.ShowAndRedirect(this, "Đã cập nhật hồ sơ.", "success", "TaiKhoan.aspx");
            }
            else ErrorProfile(result == ProfileUpdateOutcome.EmailTaken ? "Email này đã được một tài khoản khác dùng. Vui lòng chọn email khác." : "Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại.");
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để đổi mật khẩu.") || !Page.IsValid) return;
            PasswordChangeOutcome result = AuthHelper.TryChangePassword(AuthHelper.CurrentUserKey(), txtCurrentPassword.Text, txtNewPassword.Text);
            if (result == PasswordChangeOutcome.Changed) Toast.ShowAndRedirect(this, "Đã đổi mật khẩu. Hãy dùng mật khẩu mới ở lần đăng nhập sau.", "success", "TaiKhoan.aspx");
            else ErrorPassword(result == PasswordChangeOutcome.WrongPassword ? "Mật khẩu hiện tại không đúng." : "Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại.");
        }

        private void ErrorProfile(string text) { litProfileError.Text = text; pnlProfileError.Visible = true; }
        private void ErrorPassword(string text) { litPasswordError.Text = text; pnlPasswordError.Visible = true; }
    }
}
