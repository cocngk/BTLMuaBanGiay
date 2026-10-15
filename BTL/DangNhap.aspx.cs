
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
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
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 1. Kiem tra bo trong
            if (string.IsNullOrEmpty(username) &&
                string.IsNullOrEmpty(password))
            {
                ShowToast("Vui long dien ten dang nhap va mat khau!", "error");
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                ShowToast("Vui long dien ten dang nhap hoac Email!", "error");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowToast("Vui long dien mat khau!", "error");
                return;
            }

            // 2. Lay danh sach tai khoan (dùng chung qua AuthHelper)
            List<UserAccount> users = AuthHelper.GetUsers();

            // 3. Tim tai khoan
            UserAccount account = AuthHelper.FindByUsernameOrEmail(username);

            if (account == null)
            {
                ShowToast(
                    "Tai khoan chua duoc dang ky! Vui long dang ky tai khoan moi.",
                    "error");
                return;
            }

            // 4. Kiem tra mat khau
            if (account.Password != password)
            {
                ShowToast("Sai mat khau! Vui long kiem tra lai.", "error");
                return;
            }

            // 5. Dang nhap thanh cong
            AuthHelper.SignIn(!string.IsNullOrEmpty(account.FullName)
                ? account.FullName
                : account.Username);

            ShowToastAndRedirect(
                "Dang nhap thanh cong! Dang chuyen den trang chu...",
                "success",
                "TrangChu.aspx");
        }

        private void ShowToast(string message, string type)
        {
            string cleanMessage =
                System.Web.HttpUtility.JavaScriptStringEncode(message);

            string script =
                "if (typeof showToast === 'function') {" +
                "showToast('" + cleanMessage + "', '" + type + "');" +
                "}";

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "Toast_" + Guid.NewGuid().ToString("N"),
                script,
                true);
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
