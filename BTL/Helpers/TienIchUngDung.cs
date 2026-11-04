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
        // Thực hiện việc định dạng.
        public static string Format(decimal giaTri) { return string.Format("{0:N0}{1}", giaTri, Suffix); }
        public static string JsFormatter { get { return "function formatMoney(n){return Number(n).toLocaleString('" + JsLocale + "')+'" + Suffix + "';}"; } }
    }

    public static class PasswordHasher
    {
        // Thực hiện việc tạo mới salt.
        public static string NewSalt()
        {
            byte[] bytes = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        // Thực hiện việc băm.
        public static string Hash(string matKhau, string muoi)
        {
            using (SHA256 sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes((matKhau ?? "") + "|" + (muoi ?? ""))));
        }
        // Thực hiện việc xác thực.
        public static bool Verify(string matKhau, string muoi, string mongDoi)
        {
            if (string.IsNullOrEmpty(mongDoi)) return false;
            string thucTe = Hash(matKhau, muoi);
            if (thucTe.Length != mongDoi.Length) return false;
            int chenhLech = 0;
            for (int chiSo = 0; chiSo < thucTe.Length; chiSo++) chenhLech |= thucTe[chiSo] ^ mongDoi[chiSo];
            return chenhLech == 0;
        }
    }

    public static class Toast
    {
        // Thực hiện việc khóa.
        private static string Key() { return "toast_" + Guid.NewGuid().ToString("N"); }
        // Thực hiện việc register.
        private static void Register(Control dieuKhien, string maKichBan)
        {
            Page trang = dieuKhien as Page ?? (dieuKhien == null ? null : dieuKhien.Page);
            ScriptManager manager = trang == null ? null : ScriptManager.GetCurrent(trang);
            if (manager != null) ScriptManager.RegisterStartupScript(dieuKhien, dieuKhien.GetType(), Key(), maKichBan, true);
            else if (trang != null) trang.ClientScript.RegisterStartupScript(dieuKhien.GetType(), Key(), maKichBan, true);
        }
        // Thực hiện việc hiển thị.
        public static void Show(Control dieuKhien, string thongBao, string loai) { Register(dieuKhien, "showToast('" + HttpUtility.JavaScriptStringEncode(thongBao) + "','" + loai + "');"); }
        // Thực hiện việc thông báo thành công.
        public static void Success(Control dieuKhien, string thongBao) { Show(dieuKhien, thongBao, "success"); }
        // Thực hiện việc thông báo thông tin.
        public static void Info(Control dieuKhien, string thongBao) { Show(dieuKhien, thongBao, "info"); }
        // Thực hiện việc điều hướng.
        public static void Navigate(Control dieuKhien, string duongDan) { Register(dieuKhien, "window.location.href='" + HttpUtility.JavaScriptStringEncode(duongDan) + "';"); }
        // Thực hiện việc hiển thị and chuyển trang.
        public static void ShowAndRedirect(Control dieuKhien, string thongBao, string loai, string duongDan) { Register(dieuKhien, "showToast('" + HttpUtility.JavaScriptStringEncode(thongBao) + "','" + loai + "');setTimeout(function(){window.location.href='" + HttpUtility.JavaScriptStringEncode(duongDan) + "';},1200);"); }
        // Thực hiện việc kiểm tra yêu cầu login.
        public static void RequireLogin(Control dieuKhien, string thongBao) { Register(dieuKhien, "showToast('" + HttpUtility.JavaScriptStringEncode(thongBao) + "','info');openLoginModal();"); }
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
        // Lấy toàn bộ tài khoản.
        List<UserAccount> GetAll();
        // Tìm tài khoản theo tên đăng nhập hoặc email.
        UserAccount FindByUsernameOrEmail(string giaTri);
        // Tìm tài khoản theo tên đăng nhập.
        UserAccount FindByUsername(string tenDangNhap);
        // Kiểm tra tên đăng nhập đã tồn tại.
        bool UsernameExists(string tenDangNhap);
        // Kiểm tra email đã tồn tại.
        bool EmailExists(string email);
        // Thêm một tài khoản.
        AddUserOutcome TryAdd(UserAccount taiKhoan);
        // Cập nhật thông tin cá nhân.
        ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap, string hoTen, string email, string soDienThoai);
        // Đổi mật khẩu.
        PasswordChangeOutcome TryChangePassword(string tenDangNhap, string matKhauHienTai, string matKhauMoi);
        // Thay đổi quyền quản trị.
        RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri);
    }

    public sealed class MemoryUserStore : IUserStore
    {
        private readonly List<UserAccount> nguoiDungs; private readonly object khoaDongBo = new object();
        // Khởi tạo kho người dùng trong bộ nhớ.
        public MemoryUserStore() : this(Seed()) { }
        // Khởi tạo kho người dùng trong bộ nhớ.
        public MemoryUserStore(IEnumerable<UserAccount> nguon) { nguoiDungs = nguon == null ? new List<UserAccount>() : new List<UserAccount>(nguon); }
        // Thực hiện việc tạo mới người dùng.
        public static UserAccount NewUser(string ten, string tenDangNhap, string email, string soDienThoai, string matKhau)
        {
            string muoi = PasswordHasher.NewSalt();
            return new UserAccount { FullName = ten, Username = tenDangNhap, Email = email, Phone = soDienThoai, PasswordSalt = muoi, PasswordHash = PasswordHasher.Hash(matKhau, muoi) };
        }
        // Thực hiện việc khởi tạo dữ liệu.
        public static List<UserAccount> Seed()
        {
            UserAccount quanTri = NewUser("Nguyễn Văn A", "admin", "admin@sneakerx.com", "0987654321", "123"); quanTri.IsAdmin = true;
            return new List<UserAccount> { quanTri, NewUser("Lê Thu Hà", "sneakerx", "thuha@sneakerx.com", "0912345678", "123") };
        }
        // Thực hiện việc lấy toàn bộ.
        public List<UserAccount> GetAll() { lock (khoaDongBo) return new List<UserAccount>(nguoiDungs); }
        // Tìm theo tên đăng nhập hoặc email.
        public UserAccount FindByUsernameOrEmail(string giaTri) { lock (khoaDongBo) return nguoiDungs.FirstOrDefault(x => Same(x.Username, giaTri) || Same(x.Email, giaTri)); }
        // Thực hiện việc tìm theo tên đăng nhập.
        public UserAccount FindByUsername(string giaTri) { lock (khoaDongBo) return FindLocked(giaTri); }
        // Thực hiện việc tên đăng nhập tồn tại.
        public bool UsernameExists(string giaTri) { lock (khoaDongBo) return nguoiDungs.Any(x => Same(x.Username, giaTri)); }
        // Thực hiện việc email tồn tại.
        public bool EmailExists(string giaTri) { lock (khoaDongBo) return nguoiDungs.Any(x => Same(x.Email, giaTri)); }
        // Thực hiện việc thử thêm.
        public AddUserOutcome TryAdd(UserAccount taiKhoan)
        {
            if (taiKhoan == null) return AddUserOutcome.UsernameTaken;
            lock (khoaDongBo)
            {
                if (nguoiDungs.Any(x => Same(x.Username, taiKhoan.Username))) return AddUserOutcome.UsernameTaken;
                if (nguoiDungs.Any(x => Same(x.Email, taiKhoan.Email))) return AddUserOutcome.EmailTaken;
                nguoiDungs.Add(taiKhoan); return AddUserOutcome.Added;
            }
        }
        // Thực hiện việc thử cập nhật hồ sơ.
        public ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap, string ten, string email, string soDienThoai)
        {
            lock (khoaDongBo)
            {
                UserAccount taiKhoan = FindLocked(tenDangNhap);
                if (taiKhoan == null) return ProfileUpdateOutcome.NotFound;
                if (nguoiDungs.Any(x => !ReferenceEquals(x, taiKhoan) && Same(x.Email, email))) return ProfileUpdateOutcome.EmailTaken;
                taiKhoan.FullName = ten; taiKhoan.Email = email; taiKhoan.Phone = soDienThoai; return ProfileUpdateOutcome.Updated;
            }
        }
        // Thực hiện việc thử thay đổi mật khẩu.
        public PasswordChangeOutcome TryChangePassword(string tenDangNhap, string hienTai, string moiCapNhat)
        {
            lock (khoaDongBo)
            {
                UserAccount taiKhoan = FindLocked(tenDangNhap);
                if (taiKhoan == null) return PasswordChangeOutcome.NotFound;
                if (!PasswordHasher.Verify(hienTai, taiKhoan.PasswordSalt, taiKhoan.PasswordHash)) return PasswordChangeOutcome.WrongPassword;
                taiKhoan.PasswordSalt = PasswordHasher.NewSalt(); taiKhoan.PasswordHash = PasswordHasher.Hash(moiCapNhat, taiKhoan.PasswordSalt); return PasswordChangeOutcome.Changed;
            }
        }
        // Thực hiện việc thử đặt quản trị.
        public RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri)
        {
            lock (khoaDongBo)
            {
                UserAccount taiKhoan = FindLocked(tenDangNhap);
                if (taiKhoan == null) return RoleChangeOutcome.NotFound;
                if (taiKhoan.IsAdmin == laQuanTri) return RoleChangeOutcome.Unchanged;
                if (!laQuanTri && nguoiDungs.Count(x => x.IsAdmin) <= 1) return RoleChangeOutcome.LastAdmin;
                taiKhoan.IsAdmin = laQuanTri; return RoleChangeOutcome.Changed;
            }
        }
        // Thực hiện việc tìm locked.
        private UserAccount FindLocked(string tenDangNhap) { return string.IsNullOrEmpty(tenDangNhap) ? null : nguoiDungs.FirstOrDefault(x => Same(x.Username, tenDangNhap)); }
        // Thực hiện việc same.
        private static bool Same(string a, string b) { return string.Equals(a, b, StringComparison.OrdinalIgnoreCase); }
    }

    public static class AuthHelper
    {
        private static IUserStore kho;
        public static IUserStore Store { get { return kho ?? (kho = new ApplicationUserStore()); } set { kho = value; } }
        // Tìm theo tên đăng nhập hoặc email.
        public static UserAccount FindByUsernameOrEmail(string giaTri) { return Store.FindByUsernameOrEmail(giaTri); }
        // Thực hiện việc tìm theo tên đăng nhập.
        public static UserAccount FindByUsername(string giaTri) { return Store.FindByUsername(giaTri); }
        // Thực hiện việc tên đăng nhập tồn tại.
        public static bool UsernameExists(string giaTri) { return Store.UsernameExists(giaTri); }
        // Thực hiện việc email tồn tại.
        public static bool EmailExists(string giaTri) { return Store.EmailExists(giaTri); }
        // Thực hiện việc thử thêm.
        public static AddUserOutcome TryAdd(UserAccount giaTri) { return Store.TryAdd(giaTri); }
        // Thực hiện việc toàn bộ accounts.
        public static List<UserAccount> AllAccounts() { return Store.GetAll(); }
        // Thực hiện việc thử cập nhật hồ sơ.
        public static ProfileUpdateOutcome TryUpdateProfile(string nguoiDung, string ten, string email, string soDienThoai) { return Store.TryUpdateProfile(nguoiDung, ten, email, soDienThoai); }
        // Thực hiện việc thử thay đổi mật khẩu.
        public static PasswordChangeOutcome TryChangePassword(string nguoiDung, string hienTai, string moiCapNhat) { return Store.TryChangePassword(nguoiDung, hienTai, moiCapNhat); }
        // Thực hiện việc thử đặt quản trị.
        public static RoleChangeOutcome TrySetAdmin(string nguoiDung, bool quanTri) { return Store.TrySetAdmin(nguoiDung, quanTri); }
        // Thực hiện việc is logged in.
        public static bool IsLoggedIn() { return HttpContext.Current != null && HttpContext.Current.Session["Username"] != null; }
        // Thực hiện việc hiện tại người dùng tên.
        public static string CurrentUserName() { object giaTri = HttpContext.Current == null ? null : HttpContext.Current.Session["Username"]; return giaTri == null ? "" : giaTri.ToString(); }
        // Thực hiện việc hiện tại người dùng khóa.
        public static string CurrentUserKey() { return UserKeyOf(HttpContext.Current == null ? null : HttpContext.Current.Session); }
        // Thực hiện việc người dùng khóa của.
        public static string UserKeyOf(HttpSessionState phien) { if (phien == null) return ""; object khoa = phien["UserKey"] ?? phien["Username"]; return khoa == null ? "" : khoa.ToString(); }
        // Thực hiện việc hiện tại tài khoản.
        public static UserAccount CurrentAccount() { return IsLoggedIn() ? Store.FindByUsername(CurrentUserKey()) : null; }
        // Thực hiện việc is quản trị.
        public static bool IsAdmin() { UserAccount taiKhoan = CurrentAccount(); return taiKhoan != null && taiKhoan.IsAdmin; }
        // Thực hiện việc làm mới display tên.
        public static void RefreshDisplayName() { UserAccount taiKhoan = CurrentAccount(); if (taiKhoan != null) HttpContext.Current.Session["Username"] = taiKhoan.DisplayName; }
        // Thực hiện việc sign in.
        public static void SignIn(UserAccount taiKhoan)
        {
            if (taiKhoan == null || HttpContext.Current == null) return;
            HttpSessionState phien = HttpContext.Current.Session; phien["Username"] = taiKhoan.DisplayName; phien["UserKey"] = taiKhoan.Username; new CartStore(phien).MergeGuestIntoAccount(); new WishlistStore(phien).MergeGuestIntoAccount();
        }
        // Thực hiện việc sign out.
        public static void SignOut()
        {
            if (HttpContext.Current == null) return;
            HttpSessionState phien = HttpContext.Current.Session; phien["Username"] = null; phien["UserKey"] = null; new CartStore(phien).ClearGuest(); new WishlistStore(phien).ClearGuest();
        }
    }
}
