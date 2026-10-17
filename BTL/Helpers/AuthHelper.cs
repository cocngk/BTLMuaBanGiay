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

        /// <summary>Không bao giờ lưu mật khẩu thô — xem PasswordHasher.</summary>
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

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

        /// <summary>Sinh tài khoản đã băm mật khẩu — dùng cho cả seed lẫn đăng ký.</summary>
        public static UserAccount NewUser(string fullName, string username, string email, string phone, string plainPassword)
        {
            string salt = PasswordHasher.NewSalt();
            return new UserAccount
            {
                FullName = fullName,
                Username = username,
                Email = email,
                Phone = phone,
                PasswordSalt = salt,
                PasswordHash = PasswordHasher.Hash(plainPassword, salt)
            };
        }

        public List<UserAccount> GetAll()
        {
            var users = HttpContext.Current.Application[AppKey] as List<UserAccount>;
            if (users == null)
            {
                // Mật khẩu thô chỉ xuất hiện đúng ở chỗ seed này, vào là được
                // băm ngay — không có UserAccount nào giữ bản thô.
                users = new List<UserAccount>
                {
                    NewUser("Nguyễn Văn A", "admin", "admin@sneakerx.com", "0987654321", "123"),
                    NewUser("Khách Hàng Demo", "sneakerx", "demo@sneakerx.com", "0912345678", "123")
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
    /// Kho vẫn là bộ nhớ trong Application; khi thêm database chỉ thay MemoryUserStore.
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
