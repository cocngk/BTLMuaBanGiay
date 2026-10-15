using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeStore.Helpers
{
    [Serializable]
    public class UserAccount
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Demo-only auth store (was triplicated in SiteMaster / DangNhap / DangKy).
    /// Users live in Application state; passwords are plaintext — never use as-is
    /// outside this school project.
    /// </summary>
    public static class AuthHelper
    {
        private const string AppKey = "UserList";

        public static List<UserAccount> GetUsers()
        {
            var users = HttpContext.Current.Application[AppKey] as List<UserAccount>;
            if (users == null)
            {
                users = new List<UserAccount>
                {
                    new UserAccount { FullName = "Nguyễn Văn A", Username = "admin", Email = "admin@sneakerx.com", Phone = "0987654321", Password = "123" },
                    new UserAccount { FullName = "Khách Hàng Demo", Username = "sneakerx", Email = "demo@sneakerx.com", Phone = "0912345678", Password = "123" }
                };
                HttpContext.Current.Application[AppKey] = users;
            }
            return users;
        }

        public static UserAccount FindByUsernameOrEmail(string usernameOrEmail)
        {
            return GetUsers().FirstOrDefault(u =>
                string.Equals(u.Username, usernameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Email, usernameOrEmail, StringComparison.OrdinalIgnoreCase));
        }

        public static bool UsernameExists(string username)
        {
            return GetUsers().Any(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EmailExists(string email)
        {
            return GetUsers().Any(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public static void Add(UserAccount account)
        {
            GetUsers().Add(account);
        }

        public static bool IsLoggedIn()
        {
            return HttpContext.Current.Session["Username"] != null;
        }

        public static string CurrentUserName()
        {
            object name = HttpContext.Current.Session["Username"];
            return name != null ? name.ToString() : "";
        }

        public static void SignIn(string displayName)
        {
            HttpContext.Current.Session["Username"] = displayName;
        }

        public static void SignOut()
        {
            HttpContext.Current.Session["Username"] = null;
        }
    }
}
