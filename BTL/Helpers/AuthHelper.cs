using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoeStore.Data;

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

    /// <summary>Kết quả thêm tài khoản. Store chỉ nói lý do; trang tự quyết chữ hiển thị.</summary>
    public enum AddUserOutcome
    {
        Added,
        UsernameTaken,
        EmailTaken
    }

    /// <summary>
    /// Seam kho người dùng: test thay MemoryUserStore riêng, không đụng Application thật.
    /// </summary>
    public interface IUserStore
    {
        /// <summary>Bản sao tại thời điểm gọi — không phải list sống của kho.</summary>
        List<UserAccount> GetAll();
        UserAccount FindByUsernameOrEmail(string usernameOrEmail);
        bool UsernameExists(string username);
        bool EmailExists(string email);

        /// <summary>
        /// Kiểm tra trùng và thêm trong cùng một khóa. Gọi "kiểm tra" rồi "thêm" ở
        /// phía ngoài thì hai request đồng thời vẫn lọt cả hai — nên cổng chặn phải
        /// nằm gọn bên trong kho.
        /// </summary>
        AddUserOutcome TryAdd(UserAccount account);
    }

    public sealed class MemoryUserStore : IUserStore
    {
        private readonly List<UserAccount> users;

        /// <summary>
        /// Khóa cho mọi lần đọc/ghi. Application["UserList"] ngày trước được
        /// HttpRuntime tự lock; kho thuần thì phải tự làm — List.Add không
        /// thread-safe, hai request đăng ký cùng lúc có thể làm hỏng list.
        /// </summary>
        private readonly object gate = new object();

        /// <summary>Kho seed mặc định của app — bộ nhớ thuần, không side-effect lên Application.</summary>
        public MemoryUserStore() : this(Seed())
        {
        }

        /// <summary>
        /// Cho phép truyền kho có sẵn. Test dựng store mà không cần HttpContext,
        /// và hai test không dùng chung state.
        /// </summary>
        public MemoryUserStore(IEnumerable<UserAccount> users)
        {
            this.users = users != null ? new List<UserAccount>(users) : new List<UserAccount>();
        }

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

        /// <summary>
        /// Dữ liệu demo. Mật khẩu thô chỉ xuất hiện đúng ở chỗ này,
        /// vào là được băm ngay — không có UserAccount nào giữ bản thô.
        /// </summary>
        public static List<UserAccount> Seed()
        {
            return new List<UserAccount>
            {
                NewUser("Nguyễn Văn A", "admin", "admin@sneakerx.com", "0987654321", "123"),
                NewUser("Khách Hàng Demo", "sneakerx", "demo@sneakerx.com", "0912345678", "123")
            };
        }

        /// <summary>
        /// Trả bản sao chứ không trả list thật: người gọi duyệt list trong lúc
        /// request khác đang Add sẽ ném "Collection was modified", và nếu trả
        /// list thật thì họ ghi được mà không đi qua khóa.
        /// </summary>
        public List<UserAccount> GetAll()
        {
            lock (gate)
            {
                return new List<UserAccount>(users);
            }
        }

        public UserAccount FindByUsernameOrEmail(string usernameOrEmail)
        {
            lock (gate)
            {
                return users.FirstOrDefault(u =>
                    string.Equals(u.Username, usernameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(u.Email, usernameOrEmail, StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool UsernameExists(string username)
        {
            lock (gate)
            {
                return users.Any(u =>
                    string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool EmailExists(string email)
        {
            lock (gate)
            {
                return users.Any(u =>
                    string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            }
        }

        public AddUserOutcome TryAdd(UserAccount account)
        {
            lock (gate)
            {
                if (users.Any(u =>
                    string.Equals(u.Username, account.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    return AddUserOutcome.UsernameTaken;
                }

                if (users.Any(u =>
                    string.Equals(u.Email, account.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return AddUserOutcome.EmailTaken;
                }

                users.Add(account);
                return AddUserOutcome.Added;
            }
        }
    }

    /// <summary>
    /// Demo-only auth (was triplicated in SiteMaster / DangNhap / DangKy).
    /// Kho là bộ nhớ thuần sau IUserStore; khi thêm database chỉ thay adapter.
    /// </summary>
    public static class AuthHelper
    {
        private static IUserStore store;

        /// <summary>
        /// Kho mặc định là cấp ứng dụng (Application), không phải biến static của
        /// lớp: tài khoản đăng ký ở phiên này phải còn khi phiên khác mở ra.
        /// Tạo lazy vì Application chỉ có khi đang chạy trong pipeline ASP.NET.
        /// </summary>
        public static IUserStore Store
        {
            get { return store ?? (store = new ApplicationUserStore()); }
            set { store = value; }
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

        public static AddUserOutcome TryAdd(UserAccount account)
        {
            return Store.TryAdd(account);
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

        /// <summary>
        /// Khóa định danh bền của người đang đăng nhập — dùng để gắn đơn hàng với
        /// chủ sở hữu. Không dùng tên hiển thị: hai tài khoản có thể trùng họ tên.
        /// Phiên cũ (trước khi có khóa riêng) rơi về tên hiển thị để không mất đơn.
        /// </summary>
        public static string CurrentUserKey()
        {
            object key = HttpContext.Current.Session["UserKey"];
            if (key != null && key.ToString().Length > 0)
            {
                return key.ToString();
            }
            return CurrentUserName();
        }

        public static void SignIn(UserAccount account)
        {
            if (account == null)
            {
                return;
            }

            HttpContext.Current.Session["Username"] = account.DisplayName;
            HttpContext.Current.Session["UserKey"] = account.Username;
        }

        public static void SignOut()
        {
            HttpContext.Current.Session["Username"] = null;
            HttpContext.Current.Session["UserKey"] = null;
        }
    }
}
