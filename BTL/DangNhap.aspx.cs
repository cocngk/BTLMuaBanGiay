
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public partial class DangNhap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 2. Tim tai khoan (dùng chung qua AuthHelper)
            UserAccount account = AuthHelper.FindByUsernameOrEmail(username);

            if (account == null)
            {
                cvLogin.ErrorMessage = "Tài khoản chưa được đăng ký! Vui lòng đăng ký tài khoản mới.";
                cvLogin.IsValid = false;
                return;
            }

            // 3. Kiem tra mat khau
            if (account.Password != password)
            {
                cvLogin.ErrorMessage = "Sai mật khẩu! Vui lòng kiểm tra lại.";
                cvLogin.IsValid = false;
                return;
            }

            // 4. Dang nhap thanh cong
            AuthHelper.SignIn(!string.IsNullOrEmpty(account.FullName)
                ? account.FullName
                : account.Username);

            ShowToastAndRedirect(
                "Dang nhap thanh cong! Dang chuyen den trang chu...",
                "success",
                "TrangChu.aspx");
        }

        protected void cvLogin_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
        }

        private void ShowToastAndRedirect(
            string message, string type, string url)
        {
            string cleanMessage =
                System.Web.HttpUtility.JavaScriptStringEncode(message);

            string cleanUrl =
                System.Web.HttpUtility.JavaScriptStringEncode(url);

            string script =
                "if (typeof showToast === 'function') {" +
                "showToast('" + cleanMessage + "', '" + type + "');" +
                "}" +
                "setTimeout(function() {" +
                "window.location.href='" + cleanUrl + "';" +
                "}, 1200);";

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "LoginSuccess_" + Guid.NewGuid().ToString("N"),
                script,
                true);
        }
    }
}
