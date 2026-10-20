
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
            // Trùng tài khoản thì để kho chặn: nó kiểm tra và thêm trong cùng một
            // khóa. CheckDuplicate bên dưới chỉ là gợi ý tức thời cho người dùng.
            if (!Page.IsValid) return;

            // 8. Luu tai khoan moi — mat khau duoc bam ngay, khong luu ban tho
            UserAccount account = MemoryUserStore.NewUser(fullName, username, email, phone, password);

            switch (AuthHelper.TryAdd(account))
            {
                case AddUserOutcome.UsernameTaken:
                    ShowFormError("Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.");
                    return;
                case AddUserOutcome.EmailTaken:
                    ShowFormError("Email này đã được đăng ký!");
                    return;
            }

            // 9. Dang nhap ngay sau khi dang ky
            AuthHelper.SignIn(account);

            Toast.ShowAndRedirect(this,
                "Đăng ký thành công! Bạn đã đăng nhập.",
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
