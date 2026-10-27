using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
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

        /// <summary>
        /// Quyền quản trị. Trước đây tài khoản seed "admin" không khác gì tài khoản
        /// thường: muốn biết ai được vào khu quản trị thì phải so tên đăng nhập với
        /// một chuỗi viết cứng. Cờ nằm trên tài khoản để cấp/thu quyền được cho người
        /// khác mà không sửa code.
        /// </summary>
        public bool IsAdmin { get; set; }

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

    /// <summary>Kết quả sửa hồ sơ — trang tự dịch sang câu tiếng Việt.</summary>
    public enum ProfileUpdateOutcome
    {
        Updated,
        EmailTaken,
        NotFound
    }

    /// <summary>Kết quả đổi mật khẩu. Sai mật khẩu hiện tại là lý do riêng, không gộp vào "lỗi".</summary>
    public enum PasswordChangeOutcome
    {
        Changed,
        WrongPassword,
        NotFound
    }

    /// <summary>
    /// Kết quả đổi quyền quản trị. <see cref="LastAdmin"/> chặn trường hợp thu quyền
    /// của quản trị viên cuối cùng — lúc đó không còn ai vào được khu quản trị nữa
    /// và phải restart ứng dụng mới cứu được (kho chỉ nằm trong Application).
    /// </summary>
    public enum RoleChangeOutcome
    {
        Changed,
        Unchanged,
        LastAdmin,
        NotFound
    }

    /// <summary>
    /// Seam kho người dùng: test thay MemoryUserStore riêng, không đụng Application thật.
    /// </summary>
    public interface IUserStore
    {
        /// <summary>Bản sao tại thời điểm gọi — không phải list sống của kho.</summary>
        List<UserAccount> GetAll();
        UserAccount FindByUsernameOrEmail(string usernameOrEmail);

        /// <summary>Tìm đúng theo tên đăng nhập, không khớp email — dùng cho phiên đang đăng nhập.</summary>
        UserAccount FindByUsername(string username);
        bool UsernameExists(string username);
        bool EmailExists(string email);

        /// <summary>
        /// Kiểm tra trùng và thêm trong cùng một khóa. Gọi "kiểm tra" rồi "thêm" ở
        /// phía ngoài thì hai request đồng thời vẫn lọt cả hai — nên cổng chặn phải
        /// nằm gọn bên trong kho.
        /// </summary>
        AddUserOutcome TryAdd(UserAccount account);

        /// <summary>Sửa hồ sơ. Kiểm email trùng với người khác và ghi trong cùng một khóa.</summary>
        ProfileUpdateOutcome TryUpdateProfile(string username, string fullName, string email, string phone);

        /// <summary>Đổi mật khẩu: xác minh mật khẩu hiện tại rồi băm mật khẩu mới, cùng một khóa.</summary>
        PasswordChangeOutcome TryChangePassword(string username, string currentPassword, string newPassword);

        /// <summary>Cấp/thu quyền quản trị, có chặn thu quyền của admin cuối cùng.</summary>
        RoleChangeOutcome TrySetAdmin(string username, bool isAdmin);
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
            UserAccount admin = NewUser("Nguyễn Văn A", "admin", "admin@sneakerx.com", "0987654321", "123");
            admin.IsAdmin = true;

            return new List<UserAccount>
            {
                admin,
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

        public UserAccount FindByUsername(string username)
        {
            lock (gate)
            {
                return FindLocked(username);
            }
        }

        /// <summary>Bản dùng bên trong khóa — chỉ gọi khi đã giữ <see cref="gate"/>.</summary>
        private UserAccount FindLocked(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            return users.FirstOrDefault(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
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

        /// <summary>
        /// Sửa hồ sơ tại chỗ. Không nhận cả đối tượng UserAccount vì trang đang giữ
        /// bản đọc từ kho: gán thẳng đối tượng đó là ghi mà không qua khóa, và có thể
        /// ghi đè cả mật khẩu / quyền mà form không hề hiển thị.
        /// </summary>
        public ProfileUpdateOutcome TryUpdateProfile(string username, string fullName, string email, string phone)
        {
            lock (gate)
            {
                UserAccount account = FindLocked(username);
                if (account == null)
                {
                    return ProfileUpdateOutcome.NotFound;
                }

                bool emailTaken = users.Any(u =>
                    !ReferenceEquals(u, account) &&
                    string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

                if (emailTaken)
                {
                    return ProfileUpdateOutcome.EmailTaken;
                }

                account.FullName = fullName;
                account.Email = email;
                account.Phone = phone;
                return ProfileUpdateOutcome.Updated;
            }
        }

        /// <summary>
        /// Đổi mật khẩu: xác minh mật khẩu cũ và ghi bản băm mới trong cùng một khóa,
        /// nên không có khe giữa "kiểm tra đúng" và "đã đổi".
        /// </summary>
        public PasswordChangeOutcome TryChangePassword(string username, string currentPassword, string newPassword)
        {
            lock (gate)
            {
                UserAccount account = FindLocked(username);
                if (account == null)
                {
                    return PasswordChangeOutcome.NotFound;
                }

                if (!PasswordHasher.Verify(currentPassword, account.PasswordSalt, account.PasswordHash))
                {
                    return PasswordChangeOutcome.WrongPassword;
                }

                string salt = PasswordHasher.NewSalt();
                account.PasswordSalt = salt;
                account.PasswordHash = PasswordHasher.Hash(newPassword, salt);
                return PasswordChangeOutcome.Changed;
            }
        }

        /// <summary>
        /// Cấp/thu quyền quản trị. Đếm admin trong cùng khóa với lúc ghi: nếu đếm ở
        /// ngoài thì hai request thu quyền cùng lúc đều thấy "còn 2 admin" và kết quả
        /// là 0.
        /// </summary>
        public RoleChangeOutcome TrySetAdmin(string username, bool isAdmin)
        {
            lock (gate)
            {
                UserAccount account = FindLocked(username);
                if (account == null)
                {
                    return RoleChangeOutcome.NotFound;
                }

                if (account.IsAdmin == isAdmin)
                {
                    return RoleChangeOutcome.Unchanged;
                }

                if (!isAdmin && users.Count(u => u.IsAdmin) <= 1)
                {
                    return RoleChangeOutcome.LastAdmin;
                }

                account.IsAdmin = isAdmin;
                return RoleChangeOutcome.Changed;
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

        public static UserAccount FindByUsername(string username)
        {
            return Store.FindByUsername(username);
        }

        /// <summary>Toàn bộ tài khoản cho khu quản trị. Bản sao list, phần tử là tham chiếu đọc.</summary>
        public static List<UserAccount> AllAccounts()
        {
            return Store.GetAll();
        }

        public static ProfileUpdateOutcome TryUpdateProfile(string username, string fullName, string email, string phone)
        {
            return Store.TryUpdateProfile(username, fullName, email, phone);
        }

        public static PasswordChangeOutcome TryChangePassword(string username, string currentPassword, string newPassword)
        {
            return Store.TryChangePassword(username, currentPassword, newPassword);
        }

        public static RoleChangeOutcome TrySetAdmin(string username, bool isAdmin)
        {
            return Store.TrySetAdmin(username, isAdmin);
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
            return UserKeyOf(HttpContext.Current.Session);
        }

        /// <summary>
        /// Tài khoản của phiên đang đăng nhập, đọc tươi từ kho mỗi lần gọi — hồ sơ
        /// vừa sửa hoặc quyền vừa đổi phải có hiệu lực ngay, không đợi đăng nhập lại.
        /// Trả null khi chưa đăng nhập hoặc phiên còn giữ khóa cũ không còn trong kho.
        /// </summary>
        public static UserAccount CurrentAccount()
        {
            if (!IsLoggedIn())
            {
                return null;
            }

            return Store.FindByUsername(CurrentUserKey());
        }

        /// <summary>
        /// Quyền quản trị của phiên hiện tại. Mọi trang quản trị phải hỏi câu này ở
        /// server: ẩn link trên header chỉ là giao diện, gõ thẳng URL vẫn vào được.
        /// </summary>
        public static bool IsAdmin()
        {
            UserAccount account = CurrentAccount();
            return account != null && account.IsAdmin;
        }

        /// <summary>
        /// Cập nhật tên hiển thị trong session sau khi sửa họ tên. Session giữ bản
        /// chụp lúc đăng nhập, nên không gọi chỗ này thì header vẫn hiện tên cũ tới
        /// lần đăng nhập sau.
        /// </summary>
        public static void RefreshDisplayName()
        {
            UserAccount account = CurrentAccount();
            if (account != null)
            {
                HttpContext.Current.Session["Username"] = account.DisplayName;
            }
        }

        /// <summary>
        /// Cùng luật với CurrentUserKey nhưng đọc từ session truyền vào — để store
        /// nhận session giả lập trong test vẫn biết chủ sở hữu là ai.
        /// </summary>
        public static string UserKeyOf(HttpSessionState session)
        {
            if (session == null)
            {
                return "";
            }

            object key = session["UserKey"];
            if (key != null && key.ToString().Length > 0)
            {
                return key.ToString();
            }

            object name = session["Username"];
            return name != null ? name.ToString() : "";
        }

        /// <summary>
        /// Đăng nhập: ghi danh tính rồi gộp giỏ + tim mà khách vãng lai đã thêm trước
        /// đó vào dữ liệu của tài khoản. Phải sau khi ghi UserKey, vì store chọn chỗ
        /// lưu dựa trên UserKey.
        /// </summary>
        public static void SignIn(UserAccount account)
        {
            if (account == null)
            {
                return;
            }

            HttpSessionState session = HttpContext.Current.Session;
            session["Username"] = account.DisplayName;
            session["UserKey"] = account.Username;

            new CartStore(session).MergeGuestIntoAccount();
            new WishlistStore(session).MergeGuestIntoAccount();
        }

        /// <summary>
        /// Đăng xuất: xoá danh tính và dọn bản giỏ / tim trong session. KHÔNG xoá dữ
        /// liệu của tài khoản (nằm ở Application) — đăng nhập lại là có lại. Bản trong
        /// session phải dọn vì nó thuộc về trình duyệt: để lại thì người đăng nhập sau
        /// trên cùng máy thấy hàng của người trước và có thể đặt đơn đó dưới tên mình.
        /// </summary>
        public static void SignOut()
        {
            HttpSessionState session = HttpContext.Current.Session;
            session["Username"] = null;
            session["UserKey"] = null;

            new CartStore(session).ClearGuest();
            new WishlistStore(session).ClearGuest();
        }
    }
}
