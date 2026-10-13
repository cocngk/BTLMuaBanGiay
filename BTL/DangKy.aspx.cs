
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;

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

            // 1. Kiem tra bo trong
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowToast("Vui long nhap ho va ten!", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowToast("Vui long nhap ten dang nhap!", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowToast("Vui long nhap dia chi Email!", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowToast("Vui long nhap so dien thoai!", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowToast("Vui long nhap mat khau!", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(confirm))
            {
                ShowToast("Vui long xac nhan mat khau!", "error");
                return;
            }

            // 2. Kiem tra do dai
            if (username.Length < 3)
            {
                ShowToast(
                    "Ten dang nhap phai co it nhat 3 ky tu!",
                    "error");
                return;
            }

            if (password.Length < 6)
            {
                ShowToast(
                    "Mat khau phai co it nhat 6 ky tu!",
                    "error");
                return;
            }

            // 3. Kiem tra Email va so dien thoai
            if (!Regex.IsMatch(
                email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowToast("Dia chi Email khong hop le!", "error");
                return;
            }

            if (!Regex.IsMatch(phone, @"^[0-9]{9,11}$"))
            {
                ShowToast(
                    "So dien thoai phai co tu 9 den 11 chu so!",
                    "error");
                return;
            }

            // 4. Kiem tra mat khau xac nhan
            if (password != confirm)
            {
                ShowToast(
                    "Mat khau xac nhan khong trung khop!",
                    "error");
                return;
            }

            // 5. Lay danh sach tai khoan
            List<SiteMaster.UserAccount> users =
                Application["UserList"] as List<SiteMaster.UserAccount>;

            if (users == null)
            {
                users = new List<SiteMaster.UserAccount>
                {
                    new SiteMaster.UserAccount
                    {
                        FullName = "Nguyen Van A",
                        Username = "admin",
                        Email = "admin@sneakerx.com",
                        Phone = "0987654321",
                        Password = "123"
                    },
                    new SiteMaster.UserAccount
                    {
                        FullName = "Khach Hang Demo",
                        Username = "sneakerx",
                        Email = "demo@sneakerx.com",
                        Phone = "0912345678",
                        Password = "123"
                    }
                };
            }

            // 6. Kiem tra trung ten dang nhap
            if (users.Any(u =>
                string.Equals(u.Username, username,
                    StringComparison.OrdinalIgnoreCase)))
            {
                ShowToast(
                    "Ten dang nhap da ton tai! Vui long chon ten khac.",
                    "error");
                return;
            }

            // 7. Kiem tra trung Email
            if (users.Any(u =>
                string.Equals(u.Email, email,
                    StringComparison.OrdinalIgnoreCase)))
            {
                ShowToast(
                    "Email nay da duoc dang ky!",
                    "error");
                return;
            }

            // 8. Luu tai khoan moi
            users.Add(new SiteMaster.UserAccount
            {
                FullName = fullName,
                Username = username,
                Email = email,
                Phone = phone,
                Password = password
            });

            Application["UserList"] = users;

            // 9. Dang nhap ngay sau khi dang ky
            Session["Username"] = fullName;

            ShowToastAndRedirect(
                "Dang ky thanh cong! Ban da dang nhap.",
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
                "RegisterSuccess_" + Guid.NewGuid().ToString("N"),
                script,
                true);
        }
    }
}
