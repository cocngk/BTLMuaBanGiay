
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;
using ShoeStore.Models;

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
                cvLoginSummary.ErrorMessage = "Tài khoản chưa được đăng ký! Vui lòng đăng ký tài khoản mới.";
                cvLoginSummary.IsValid = false;
                return;
            }

            // 3. Kiem tra mat khau
            if (account.Password != password)
            {
                cvLoginSummary.ErrorMessage = "Sai mật khẩu! Vui lòng kiểm tra lại.";
                cvLoginSummary.IsValid = false;
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

        protected void cvLoginSummary_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
        }

    }
}
