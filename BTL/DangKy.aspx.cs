
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class DangKy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            // Validators (required/regex/compare) đã chạy ở client + server.
            // Chỉ còn kiểm tra trùng tài khoản ở đây.
            if (!Page.IsValid) return;

            if (AuthHelper.UsernameExists(username))
            {
                cvServerCheck.ErrorMessage = "Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.";
                cvServerCheck.IsValid = false;
                return;
            }

            if (AuthHelper.EmailExists(email))
            {
                cvServerCheck.ErrorMessage = "Email này đã được đăng ký!";
                cvServerCheck.IsValid = false;
                return;
            }
            // 8. Luu tai khoan moi
            UserAccount account = new UserAccount
            {
                FullName = fullName,
                Username = username,
                Email = email,
                Phone = phone,
                Password = password
            };
            AuthHelper.Add(account);

            // 9. Dang nhap ngay sau khi dang ky
            AuthHelper.SignIn(account);

            Toast.ShowAndRedirect(this, "RegisterSuccess_" + Guid.NewGuid().ToString("N"),
                "Dang ky thanh cong! Ban da dang nhap.",
                "success",
                "TrangChu.aspx");
        }

        protected void cvServerCheck_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
        }

        [System.Web.Services.WebMethod]
        public static object CheckDuplicate(string username, string email)
        {
            return new
            {
                userTaken = !string.IsNullOrEmpty(username) && AuthHelper.UsernameExists(username),
                emailTaken = !string.IsNullOrEmpty(email) && AuthHelper.EmailExists(email)
            };
        }

    }
}
