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

        /// <summary>Tên hiển thị duy nhất một chỗ (trước rải ở 2 trang).</summary>
        public string DisplayName
        {
            get { return !string.IsNullOrEmpty(FullName) ? FullName : Username; }
        }
    }

    /// <summary>
    /// Seam kho người dùng: test thay MemoryUserStore riêng, không đụng Application thật.
    /// </summary>
    public interface IUserStore
    {
        List<UserAccount> GetAll();
        UserAccount FindByUsernameOrEmail(string usernameOrEmail);
        bool UsernameExists(string username);
        bool EmailExists(string email);
        void Add(UserAccount account);
    }

    public sealed class MemoryUserStore : IUserStore
    {
        private const string AppKey = "UserList";

        public List<UserAccount> GetAll()
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

        public UserAccount FindByUsernameOrEmail(string usernameOrEmail)
        {
            return GetAll().FirstOrDefault(u =>
                string.Equals(u.Username, usernameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Email, usernameOrEmail, StringComparison.OrdinalIgnoreCase));
        }

        public bool UsernameExists(string username)
        {
            return GetAll().Any(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public bool EmailExists(string email)
        {
            return GetAll().Any(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(UserAccount account)
        {
            GetAll().Add(account);
        }
    }

    /// <summary>
    /// Demo-only auth (was triplicated in SiteMaster / DangNhap / DangKy).
    /// Passwords are plaintext — never use as-is outside this school project.
    /// </summary>
    public static class AuthHelper
    {
        public static IUserStore Store { get; set; } = new MemoryUserStore();

        public static List<UserAccount> GetUsers()
        {
            return Store.GetAll();
        }

        public static UserAccount FindByUsernameOrEmail(string usernameOrEmail)
        {
            return Store.FindByUsernameOrEmail(usernameOrEmail);
        }

        public static bool UsernameExists(string username)
        {
            return Store.UsernameExists(username);
        }

        public static bool EmailExists(string email)
        {
            return Store.EmailExists(email);
        }

        public static void Add(UserAccount account)
        {
            Store.Add(account);
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

        public static void SignIn(UserAccount account)
        {
            HttpContext.Current.Session["Username"] = account != null ? account.DisplayName : "";
        }

        public static void SignOut()
        {
            HttpContext.Current.Session["Username"] = null;
        }
    }
}
