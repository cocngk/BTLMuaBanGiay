
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class DangKy : ShopPage
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
                ShowFormError("Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.");
                return;
            }

            if (AuthHelper.EmailExists(email))
            {
                ShowFormError("Email này đã được đăng ký!");
                return;
            }
            // 8. Luu tai khoan moi — mat khau duoc bam ngay, khong luu ban tho
            UserAccount account = MemoryUserStore.NewUser(fullName, username, email, phone, password);
            AuthHelper.Add(account);

            // 9. Dang nhap ngay sau khi dang ky
            AuthHelper.SignIn(account);

            Toast.ShowAndRedirect(this, "RegisterSuccess_" + Guid.NewGuid().ToString("N"),
                "Dang ky thanh cong! Ban da dang nhap.",
                "success",
                "TrangChu.aspx");
        }

        /// <summary>Băng lỗi chung của form — thay cho việc mượn CustomValidator làm băng.</summary>
        private void ShowFormError(string message)
        {
            litFormError.Text = HttpUtility.HtmlEncode(message);
            pnlFormError.Visible = true;
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
