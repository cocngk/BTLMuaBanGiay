using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class DangNhap : ShopPage
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            UserAccount account = AuthHelper.FindByUsernameOrEmail(txtUsername.Text.Trim());
            if (account == null) { ShowError("Tài khoản chưa được đăng ký! Vui lòng đăng ký tài khoản mới."); return; }
            if (!PasswordHasher.Verify(txtPassword.Text.Trim(), account.PasswordSalt, account.PasswordHash)) { ShowError("Sai mật khẩu! Vui lòng kiểm tra lại."); return; }
            AuthHelper.SignIn(account);
            Toast.ShowAndRedirect(this, "Đăng nhập thành công! Đang chuyển đến trang chủ...", "success", "TrangChu.aspx");
        }

        protected void cvUsername_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string value = (txtUsername.Text ?? "").Trim();
            args.IsValid = value.Contains("@") ? Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") : value.Length >= 3;
            if (!args.IsValid) cvUsername.ErrorMessage = value.Contains("@") ? "Địa chỉ email không hợp lệ." : "Tên đăng nhập phải có ít nhất 3 ký tự.";
        }

        private void ShowError(string message)
        {
            litFormError.Text = HttpUtility.HtmlEncode(message);
            pnlFormError.Visible = true;
        }
    }
}
