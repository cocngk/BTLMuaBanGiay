
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class DangNhap : ShopPage
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
                ShowFormError("Tài khoản chưa được đăng ký! Vui lòng đăng ký tài khoản mới.");
                return;
            }

            // 3. Kiem tra mat khau — so bang hash, khong so bang chuoi goc
            if (!PasswordHasher.Verify(password, account.PasswordSalt, account.PasswordHash))
            {
                ShowFormError("Sai mật khẩu! Vui lòng kiểm tra lại.");
                return;
            }

            // 4. Dang nhap thanh cong
            AuthHelper.SignIn(account);

            Toast.ShowAndRedirect(this, "LoginSuccess_" + Guid.NewGuid().ToString("N"),
                "Dang nhap thanh cong! Dang chuyen den trang chu...",
                "success",
                "TrangChu.aspx");
        }

        protected void cvUsername_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string value = (txtUsername.Text ?? "").Trim();
            if (value.Contains("@"))
            {
                args.IsValid = System.Text.RegularExpressions.Regex.IsMatch(
                    value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!args.IsValid)
                {
                    cvUsername.ErrorMessage = "Địa chỉ email không hợp lệ.";
                }
            }
            else
            {
                args.IsValid = value.Length >= 3;
                if (!args.IsValid)
                {
                    cvUsername.ErrorMessage = "Tên đăng nhập phải có ít nhất 3 ký tự.";
                }
            }
        }

        /// <summary>Băng lỗi chung của form — thay cho việc mượn CustomValidator làm băng.</summary>
        private void ShowFormError(string message)
        {
            litFormError.Text = HttpUtility.HtmlEncode(message);
            pnlFormError.Visible = true;
        }

    }
}
