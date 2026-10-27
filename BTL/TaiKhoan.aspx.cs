using System;
using System.Web.UI;
using ShoeStore.Helpers;

namespace ShoeStore
{
    /// <summary>
    /// Trang tài khoản tự phục vụ: xem hồ sơ, sửa họ tên / email / số điện thoại và
    /// đổi mật khẩu. Trước đây người dùng chỉ thấy tên hiển thị ở header — không có
    /// chỗ nào xem lại mình đã đăng ký bằng email nào, càng không đổi được gì.
    /// </summary>
    public partial class TaiKhoan : ShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để xem tài khoản của bạn."))
            {
                return;
            }

            pnlPage.Visible = true;

            if (!IsPostBack)
            {
                LoadAccount();
            }
        }

        /// <summary>
        /// Đọc tươi từ kho thay vì từ session: hồ sơ vừa sửa (hoặc quyền vừa được cấp)
        /// phải hiện đúng ngay lần tải kế tiếp, kể cả khi quản trị viên đổi quyền cho
        /// người đang đăng nhập ở máy khác.
        /// </summary>
        private void LoadAccount()
        {
            UserAccount account = AuthHelper.CurrentAccount();
            if (account == null)
            {
                pnlPage.Visible = false;
                Toast.ShowAndRedirect(this,
                    "Phiên đăng nhập không còn hợp lệ. Vui lòng đăng nhập lại.",
                    "info",
                    "DangNhap.aspx");
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
            if (!RequireLogin("Vui lòng đăng nhập để sửa hồ sơ."))
            {
                return;
            }

            if (!Page.IsValid)
            {
                return;
            }

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            switch (AuthHelper.TryUpdateProfile(AuthHelper.CurrentUserKey(), fullName, email, phone))
            {
                case ProfileUpdateOutcome.Updated:
                    // Header lấy tên từ session; đổi tên mà không cập nhật session thì
                    // header vẫn hiện tên cũ. Chuyển trang để mọi chỗ đọc lại từ kho.
                    AuthHelper.RefreshDisplayName();
                    Toast.ShowAndRedirect(this, "Đã cập nhật hồ sơ.", "success", "TaiKhoan.aspx");
                    return;

                case ProfileUpdateOutcome.EmailTaken:
                    ShowProfileError("Email này đã được một tài khoản khác dùng. Vui lòng chọn email khác.");
                    return;

                default:
                    ShowProfileError("Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại.");
                    return;
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để đổi mật khẩu."))
            {
                return;
            }

            if (!Page.IsValid)
            {
                return;
            }

            string current = txtCurrentPassword.Text;
            string updated = txtNewPassword.Text;

            switch (AuthHelper.TryChangePassword(AuthHelper.CurrentUserKey(), current, updated))
            {
                case PasswordChangeOutcome.Changed:
                    Toast.ShowAndRedirect(this, "Đã đổi mật khẩu. Hãy dùng mật khẩu mới ở lần đăng nhập sau.", "success", "TaiKhoan.aspx");
                    return;

                case PasswordChangeOutcome.WrongPassword:
                    ShowPasswordError("Mật khẩu hiện tại không đúng.");
                    return;

                default:
                    ShowPasswordError("Không tìm thấy tài khoản của bạn. Vui lòng đăng nhập lại.");
                    return;
            }
        }

        private void ShowProfileError(string message)
        {
            litProfileError.Text = message;
            pnlProfileError.Visible = true;
        }

        private void ShowPasswordError(string message)
        {
            litPasswordError.Text = message;
            pnlPasswordError.Visible = true;
        }
    }
}
