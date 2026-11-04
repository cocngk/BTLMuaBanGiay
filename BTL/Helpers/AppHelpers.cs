using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using ShoeStore.Data;

namespace ShoeStore.Helpers
{
    public static class Money
    {
        public const string Suffix = "đ";
        public const string JsLocale = "vi-VN";
        public static string Format(decimal value) { return string.Format("{0:N0}{1}", value, Suffix); }
        public static string JsFormatter { get { return "function formatMoney(n){return Number(n).toLocaleString('" + JsLocale + "')+'" + Suffix + "';}"; } }
    }

    public static class PasswordHasher
    {
        public static string NewSalt()
        {
            byte[] bytes = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        public static string Hash(string password, string salt)
        {
            using (SHA256 sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes((password ?? "") + "|" + (salt ?? ""))));
        }
        public static bool Verify(string password, string salt, string expected)
        {
            if (string.IsNullOrEmpty(expected)) return false;
            string actual = Hash(password, salt);
            if (actual.Length != expected.Length) return false;
            int diff = 0;
            for (int i = 0; i < actual.Length; i++) diff |= actual[i] ^ expected[i];
            return diff == 0;
        }
    }

    public static class Toast
    {
        private static string Key() { return "toast_" + Guid.NewGuid().ToString("N"); }
        private static void Register(Control control, string script)
        {
            Page page = control as Page ?? (control == null ? null : control.Page);
            ScriptManager manager = page == null ? null : ScriptManager.GetCurrent(page);
            if (manager != null) ScriptManager.RegisterStartupScript(control, control.GetType(), Key(), script, true);
            else if (page != null) page.ClientScript.RegisterStartupScript(control.GetType(), Key(), script, true);
        }
        public static void Show(Control control, string message, string type) { Register(control, "showToast('" + HttpUtility.JavaScriptStringEncode(message) + "','" + type + "');"); }
        public static void Success(Control control, string message) { Show(control, message, "success"); }
        public static void Info(Control control, string message) { Show(control, message, "info"); }
        public static void Navigate(Control control, string url) { Register(control, "window.location.href='" + HttpUtility.JavaScriptStringEncode(url) + "';"); }
        public static void ShowAndRedirect(Control control, string message, string type, string url) { Register(control, "showToast('" + HttpUtility.JavaScriptStringEncode(message) + "','" + type + "');setTimeout(function(){window.location.href='" + HttpUtility.JavaScriptStringEncode(url) + "';},1200);"); }
        public static void RequireLogin(Control control, string message) { Register(control, "showToast('" + HttpUtility.JavaScriptStringEncode(message) + "','info');openLoginModal();"); }
    }

    [Serializable]
    public class UserAccount
    {
        public string FullName { get; set; } public string Username { get; set; } public string Email { get; set; } public string Phone { get; set; } public string PasswordHash { get; set; } public string PasswordSalt { get; set; } public bool IsAdmin { get; set; }
        public string DisplayName { get { return string.IsNullOrEmpty(FullName) ? Username : FullName; } }
    }

    public enum AddUserOutcome { Added, UsernameTaken, EmailTaken }
    public enum ProfileUpdateOutcome { Updated, EmailTaken, NotFound }
    public enum PasswordChangeOutcome { Changed, WrongPassword, NotFound }
    public enum RoleChangeOutcome { Changed, Unchanged, LastAdmin, NotFound }

    public interface IUserStore
    {
        List<UserAccount> GetAll(); UserAccount FindByUsernameOrEmail(string value); UserAccount FindByUsername(string username); bool UsernameExists(string username); bool EmailExists(string email); AddUserOutcome TryAdd(UserAccount account); ProfileUpdateOutcome TryUpdateProfile(string username, string fullName, string email, string phone); PasswordChangeOutcome TryChangePassword(string username, string currentPassword, string newPassword); RoleChangeOutcome TrySetAdmin(string username, bool isAdmin);
    }

    public sealed class MemoryUserStore : IUserStore
    {
        private readonly List<UserAccount> users; private readonly object gate = new object();
        public MemoryUserStore() : this(Seed()) { }
        public MemoryUserStore(IEnumerable<UserAccount> source) { users = source == null ? new List<UserAccount>() : new List<UserAccount>(source); }
        public static UserAccount NewUser(string name, string username, string email, string phone, string password)
        {
            string salt = PasswordHasher.NewSalt();
            return new UserAccount { FullName = name, Username = username, Email = email, Phone = phone, PasswordSalt = salt, PasswordHash = PasswordHasher.Hash(password, salt) };
        }
        public static List<UserAccount> Seed()
        {
            UserAccount admin = NewUser("Nguyễn Văn A", "admin", "admin@sneakerx.com", "0987654321", "123"); admin.IsAdmin = true;
            return new List<UserAccount> { admin, NewUser("Lê Thu Hà", "sneakerx", "thuha@sneakerx.com", "0912345678", "123") };
        }
        public List<UserAccount> GetAll() { lock (gate) return new List<UserAccount>(users); }
        public UserAccount FindByUsernameOrEmail(string value) { lock (gate) return users.FirstOrDefault(x => Same(x.Username, value) || Same(x.Email, value)); }
        public UserAccount FindByUsername(string value) { lock (gate) return FindLocked(value); }
        public bool UsernameExists(string value) { lock (gate) return users.Any(x => Same(x.Username, value)); }
        public bool EmailExists(string value) { lock (gate) return users.Any(x => Same(x.Email, value)); }
        public AddUserOutcome TryAdd(UserAccount account)
        {
            if (account == null) return AddUserOutcome.UsernameTaken;
            lock (gate)
            {
                if (users.Any(x => Same(x.Username, account.Username))) return AddUserOutcome.UsernameTaken;
                if (users.Any(x => Same(x.Email, account.Email))) return AddUserOutcome.EmailTaken;
                users.Add(account); return AddUserOutcome.Added;
            }
        }
        public ProfileUpdateOutcome TryUpdateProfile(string username, string name, string email, string phone)
        {
            lock (gate)
            {
                UserAccount account = FindLocked(username);
                if (account == null) return ProfileUpdateOutcome.NotFound;
                if (users.Any(x => !ReferenceEquals(x, account) && Same(x.Email, email))) return ProfileUpdateOutcome.EmailTaken;
                account.FullName = name; account.Email = email; account.Phone = phone; return ProfileUpdateOutcome.Updated;
            }
        }
        public PasswordChangeOutcome TryChangePassword(string username, string current, string updated)
        {
            lock (gate)
            {
                UserAccount account = FindLocked(username);
                if (account == null) return PasswordChangeOutcome.NotFound;
                if (!PasswordHasher.Verify(current, account.PasswordSalt, account.PasswordHash)) return PasswordChangeOutcome.WrongPassword;
                account.PasswordSalt = PasswordHasher.NewSalt(); account.PasswordHash = PasswordHasher.Hash(updated, account.PasswordSalt); return PasswordChangeOutcome.Changed;
            }
        }
        public RoleChangeOutcome TrySetAdmin(string username, bool isAdmin)
        {
            lock (gate)
            {
                UserAccount account = FindLocked(username);
                if (account == null) return RoleChangeOutcome.NotFound;
                if (account.IsAdmin == isAdmin) return RoleChangeOutcome.Unchanged;
                if (!isAdmin && users.Count(x => x.IsAdmin) <= 1) return RoleChangeOutcome.LastAdmin;
                account.IsAdmin = isAdmin; return RoleChangeOutcome.Changed;
            }
        }
        private UserAccount FindLocked(string username) { return string.IsNullOrEmpty(username) ? null : users.FirstOrDefault(x => Same(x.Username, username)); }
        private static bool Same(string a, string b) { return string.Equals(a, b, StringComparison.OrdinalIgnoreCase); }
    }

    public static class AuthHelper
    {
        private static IUserStore store;
        public static IUserStore Store { get { return store ?? (store = new ApplicationUserStore()); } set { store = value; } }
        public static UserAccount FindByUsernameOrEmail(string value) { return Store.FindByUsernameOrEmail(value); }
        public static UserAccount FindByUsername(string value) { return Store.FindByUsername(value); }
        public static bool UsernameExists(string value) { return Store.UsernameExists(value); }
        public static bool EmailExists(string value) { return Store.EmailExists(value); }
        public static AddUserOutcome TryAdd(UserAccount value) { return Store.TryAdd(value); }
        public static List<UserAccount> AllAccounts() { return Store.GetAll(); }
        public static ProfileUpdateOutcome TryUpdateProfile(string user, string name, string email, string phone) { return Store.TryUpdateProfile(user, name, email, phone); }
        public static PasswordChangeOutcome TryChangePassword(string user, string current, string updated) { return Store.TryChangePassword(user, current, updated); }
        public static RoleChangeOutcome TrySetAdmin(string user, bool admin) { return Store.TrySetAdmin(user, admin); }
        public static bool IsLoggedIn() { return HttpContext.Current != null && HttpContext.Current.Session["Username"] != null; }
        public static string CurrentUserName() { object value = HttpContext.Current == null ? null : HttpContext.Current.Session["Username"]; return value == null ? "" : value.ToString(); }
        public static string CurrentUserKey() { return UserKeyOf(HttpContext.Current == null ? null : HttpContext.Current.Session); }
        public static string UserKeyOf(HttpSessionState session) { if (session == null) return ""; object key = session["UserKey"] ?? session["Username"]; return key == null ? "" : key.ToString(); }
        public static UserAccount CurrentAccount() { return IsLoggedIn() ? Store.FindByUsername(CurrentUserKey()) : null; }
        public static bool IsAdmin() { UserAccount account = CurrentAccount(); return account != null && account.IsAdmin; }
        public static void RefreshDisplayName() { UserAccount account = CurrentAccount(); if (account != null) HttpContext.Current.Session["Username"] = account.DisplayName; }
        public static void SignIn(UserAccount account)
        {
            if (account == null || HttpContext.Current == null) return;
            HttpSessionState session = HttpContext.Current.Session; session["Username"] = account.DisplayName; session["UserKey"] = account.Username; new CartStore(session).MergeGuestIntoAccount(); new WishlistStore(session).MergeGuestIntoAccount();
        }
        public static void SignOut()
        {
            if (HttpContext.Current == null) return;
            HttpSessionState session = HttpContext.Current.Session; session["Username"] = null; session["UserKey"] = null; new CartStore(session).ClearGuest(); new WishlistStore(session).ClearGuest();
        }
    }
}
