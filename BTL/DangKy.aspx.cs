using System;
using System.Web;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class DangKy : ShopPage
    {
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            UserAccount account = MemoryUserStore.NewUser(txtFullName.Text.Trim(), txtUsername.Text.Trim(), txtEmail.Text.Trim(), txtPhone.Text.Trim(), txtPassword.Text);
            AddUserOutcome result = AuthHelper.TryAdd(account);
            if (result == AddUserOutcome.UsernameTaken) { ShowError("Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác."); return; }
            if (result == AddUserOutcome.EmailTaken) { ShowError("Email này đã được đăng ký!"); return; }
            AuthHelper.SignIn(account);
            Toast.ShowAndRedirect(this, "Đăng ký thành công! Bạn đã đăng nhập.", "success", "TrangChu.aspx");
        }

        private void ShowError(string message)
        {
            litFormError.Text = HttpUtility.HtmlEncode(message);
            pnlFormError.Visible = true;
        }

        [System.Web.Services.WebMethod]
        public static object CheckDuplicate(string username, string email)
        {
            return new { userTaken = !string.IsNullOrEmpty(username) && AuthHelper.UsernameExists(username), emailTaken = !string.IsNullOrEmpty(email) && AuthHelper.EmailExists(email) };
        }
    }
}
