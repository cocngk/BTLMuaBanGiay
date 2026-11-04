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

        // Định dạng số tiền theo kiểu Việt Nam.
        public static string Format(decimal soTien)
        {
            return string.Format("{0:N0}{1}", soTien, Suffix);
        }

        public static string JsFormatter
        {
            get
            {
                return "function formatMoney(n){return Number(n).toLocaleString('"
                    + JsLocale + "')+'" + Suffix + "';}";
            }
        }
    }

    public static class PasswordHasher
    {
        // Tạo chuỗi muối ngẫu nhiên cho mật khẩu.
        public static string NewSalt()
        {
            byte[] duLieu = new byte[16];
            using (RandomNumberGenerator boSinh = RandomNumberGenerator.Create())
            {
                boSinh.GetBytes(duLieu);
            }

            return Convert.ToBase64String(duLieu);
        }

        // Băm mật khẩu bằng SHA-256.
        public static string Hash(string matKhau, string chuoiMuoi)
        {
            string vanBan = (matKhau ?? "") + "|" + (chuoiMuoi ?? "");
            using (SHA256 boBam = SHA256.Create())
            {
                byte[] ketQua = boBam.ComputeHash(Encoding.UTF8.GetBytes(vanBan));
                return Convert.ToBase64String(ketQua);
            }
        }

        // Kiểm tra mật khẩu nhập vào có khớp mật khẩu đã lưu.
        public static bool Verify(string matKhau, string chuoiMuoi, string maBamMongDoi)
        {
            if (string.IsNullOrEmpty(maBamMongDoi)) return false;

            string maBamThucTe = Hash(matKhau, chuoiMuoi);
            if (maBamThucTe.Length != maBamMongDoi.Length) return false;

            int saiKhac = 0;
            for (int chiSo = 0; chiSo < maBamThucTe.Length; chiSo++)
            {
                saiKhac |= maBamThucTe[chiSo] ^ maBamMongDoi[chiSo];
            }

            return saiKhac == 0;
        }
    }

    public static class Toast
    {
        // Tạo khóa duy nhất cho đoạn mã JavaScript.
        private static string TaoKhoa()
        {
            return "toast_" + Guid.NewGuid().ToString("N");
        }

        // Đăng ký một đoạn JavaScript chạy sau khi trang tải xong.
        private static void DangKy(Control dieuKhien, string maKichBan)
        {
            Page trang = dieuKhien as Page
                ?? (dieuKhien == null ? null : dieuKhien.Page);
            ScriptManager quanLy = trang == null
                ? null
                : ScriptManager.GetCurrent(trang);

            if (quanLy != null)
            {
                ScriptManager.RegisterStartupScript(
                    dieuKhien,
                    dieuKhien.GetType(),
                    TaoKhoa(),
                    maKichBan,
                    true);
            }
            else if (trang != null)
            {
                trang.ClientScript.RegisterStartupScript(
                    dieuKhien.GetType(),
                    TaoKhoa(),
                    maKichBan,
                    true);
            }
        }

        // Hiển thị thông báo dạng toast.
        public static void Show(Control dieuKhien, string thongBao, string loai)
        {
            string noiDung = HttpUtility.JavaScriptStringEncode(thongBao);
            DangKy(dieuKhien, "showToast('" + noiDung + "','" + loai + "');");
        }

        // Hiển thị thông báo thành công.
        public static void Success(Control dieuKhien, string thongBao)
        {
            Show(dieuKhien, thongBao, "success");
        }

        // Hiển thị thông báo thông tin.
        public static void Info(Control dieuKhien, string thongBao)
        {
            Show(dieuKhien, thongBao, "info");
        }

        // Chuyển trình duyệt sang một địa chỉ khác.
        public static void Navigate(Control dieuKhien, string duongDan)
        {
            string diaChi = HttpUtility.JavaScriptStringEncode(duongDan);
            DangKy(dieuKhien, "window.location.href='" + diaChi + "';");
        }

        // Hiển thị thông báo rồi chuyển trang.
        public static void ShowAndRedirect(Control dieuKhien, string thongBao,
            string loai, string duongDan)
        {
            string noiDung = HttpUtility.JavaScriptStringEncode(thongBao);
            string diaChi = HttpUtility.JavaScriptStringEncode(duongDan);
            string maKichBan = "showToast('" + noiDung + "','" + loai
                + "');setTimeout(function(){window.location.href='"
                + diaChi + "';},1200);";
            DangKy(dieuKhien, maKichBan);
        }

        // Yêu cầu khách đăng nhập và mở hộp đăng nhập.
        public static void RequireLogin(Control dieuKhien, string thongBao)
        {
            string noiDung = HttpUtility.JavaScriptStringEncode(thongBao);
            DangKy(dieuKhien,
                "showToast('" + noiDung + "','info');openLoginModal();");
        }
    }

    [Serializable]
    public class UserAccount
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsAdmin { get; set; }

        public string DisplayName
        {
            get { return string.IsNullOrEmpty(FullName) ? Username : FullName; }
        }
    }

    public enum AddUserOutcome { Added, UsernameTaken, EmailTaken }
    public enum ProfileUpdateOutcome { Updated, EmailTaken, NotFound }
    public enum PasswordChangeOutcome { Changed, WrongPassword, NotFound }
    public enum RoleChangeOutcome { Changed, Unchanged, LastAdmin, NotFound }

    public interface IUserStore
    {
        List<UserAccount> GetAll();
        UserAccount FindByUsernameOrEmail(string giaTri);
        UserAccount FindByUsername(string tenDangNhap);
        bool UsernameExists(string tenDangNhap);
        bool EmailExists(string email);
        AddUserOutcome TryAdd(UserAccount taiKhoan);
        ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap, string hoTen,
            string email, string soDienThoai);
        PasswordChangeOutcome TryChangePassword(string tenDangNhap,
            string matKhauCu, string matKhauMoi);
        RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri);
    }

    // Kho người dùng cũ chỉ còn dùng để tạo dữ liệu mẫu ban đầu.
    public sealed class MemoryUserStore : IUserStore
    {
        private readonly List<UserAccount> nguoiDungs;
        private readonly object khoaDongBo = new object();

        // Tạo kho người dùng bằng dữ liệu mẫu.
        public MemoryUserStore() : this(Seed()) { }

        // Tạo kho người dùng từ danh sách có sẵn.
        public MemoryUserStore(IEnumerable<UserAccount> danhSach)
        {
            nguoiDungs = danhSach == null
                ? new List<UserAccount>()
                : new List<UserAccount>(danhSach);
        }

        // Tạo tài khoản mới và băm mật khẩu.
        public static UserAccount NewUser(string hoTen, string tenDangNhap,
            string email, string soDienThoai, string matKhau)
        {
            string chuoiMuoi = PasswordHasher.NewSalt();
            return new UserAccount
            {
                FullName = hoTen,
                Username = tenDangNhap,
                Email = email,
                Phone = soDienThoai,
                PasswordSalt = chuoiMuoi,
                PasswordHash = PasswordHasher.Hash(matKhau, chuoiMuoi)
            };
        }

        // Trả về hai tài khoản mẫu của hệ thống.
        public static List<UserAccount> Seed()
        {
            UserAccount quanTri = NewUser(
                "Nguyễn Văn A", "admin", "admin@sneakerx.com",
                "0987654321", "123");
            quanTri.IsAdmin = true;

            return new List<UserAccount>
            {
                quanTri,
                NewUser("Lê Thu Hà", "sneakerx", "thuha@sneakerx.com",
                    "0912345678", "123")
            };
        }

        public List<UserAccount> GetAll()
        {
            lock (khoaDongBo)
            {
                return new List<UserAccount>(nguoiDungs);
            }
        }

        public UserAccount FindByUsernameOrEmail(string giaTri)
        {
            lock (khoaDongBo)
            {
                return nguoiDungs.FirstOrDefault(taiKhoan =>
                    GiongNhau(taiKhoan.Username, giaTri)
                    || GiongNhau(taiKhoan.Email, giaTri));
            }
        }

        public UserAccount FindByUsername(string tenDangNhap)
        {
            lock (khoaDongBo)
            {
                return TimTrongKhoa(tenDangNhap);
            }
        }

        public bool UsernameExists(string tenDangNhap)
        {
            return FindByUsername(tenDangNhap) != null;
        }

        public bool EmailExists(string email)
        {
            lock (khoaDongBo)
            {
                return nguoiDungs.Any(taiKhoan => GiongNhau(taiKhoan.Email, email));
            }
        }

        public AddUserOutcome TryAdd(UserAccount taiKhoan)
        {
            if (taiKhoan == null) return AddUserOutcome.UsernameTaken;

            lock (khoaDongBo)
            {
                if (nguoiDungs.Any(item => GiongNhau(item.Username, taiKhoan.Username)))
                    return AddUserOutcome.UsernameTaken;
                if (nguoiDungs.Any(item => GiongNhau(item.Email, taiKhoan.Email)))
                    return AddUserOutcome.EmailTaken;

                nguoiDungs.Add(taiKhoan);
                return AddUserOutcome.Added;
            }
        }

        public ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap,
            string hoTen, string email, string soDienThoai)
        {
            lock (khoaDongBo)
            {
                UserAccount taiKhoan = TimTrongKhoa(tenDangNhap);
                if (taiKhoan == null) return ProfileUpdateOutcome.NotFound;

                bool trungEmail = nguoiDungs.Any(item =>
                    !ReferenceEquals(item, taiKhoan)
                    && GiongNhau(item.Email, email));
                if (trungEmail) return ProfileUpdateOutcome.EmailTaken;

                taiKhoan.FullName = hoTen;
                taiKhoan.Email = email;
                taiKhoan.Phone = soDienThoai;
                return ProfileUpdateOutcome.Updated;
            }
        }

        public PasswordChangeOutcome TryChangePassword(string tenDangNhap,
            string matKhauCu, string matKhauMoi)
        {
            lock (khoaDongBo)
            {
                UserAccount taiKhoan = TimTrongKhoa(tenDangNhap);
                if (taiKhoan == null) return PasswordChangeOutcome.NotFound;
                if (!PasswordHasher.Verify(matKhauCu, taiKhoan.PasswordSalt,
                    taiKhoan.PasswordHash))
                    return PasswordChangeOutcome.WrongPassword;

                taiKhoan.PasswordSalt = PasswordHasher.NewSalt();
                taiKhoan.PasswordHash = PasswordHasher.Hash(
                    matKhauMoi, taiKhoan.PasswordSalt);
                return PasswordChangeOutcome.Changed;
            }
        }

        public RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri)
        {
            lock (khoaDongBo)
            {
                UserAccount taiKhoan = TimTrongKhoa(tenDangNhap);
                if (taiKhoan == null) return RoleChangeOutcome.NotFound;
                if (taiKhoan.IsAdmin == laQuanTri) return RoleChangeOutcome.Unchanged;
                if (!laQuanTri && nguoiDungs.Count(item => item.IsAdmin) <= 1)
                    return RoleChangeOutcome.LastAdmin;

                taiKhoan.IsAdmin = laQuanTri;
                return RoleChangeOutcome.Changed;
            }
        }

        private UserAccount TimTrongKhoa(string tenDangNhap)
        {
            return string.IsNullOrEmpty(tenDangNhap)
                ? null
                : nguoiDungs.FirstOrDefault(taiKhoan =>
                    GiongNhau(taiKhoan.Username, tenDangNhap));
        }

        private static bool GiongNhau(string trai, string phai)
        {
            return string.Equals(trai, phai, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class AuthHelper
    {
        private static IUserStore khoNguoiDung;

        public static IUserStore Store
        {
            get
            {
                if (khoNguoiDung == null)
                    khoNguoiDung = new ApplicationUserStore();
                return khoNguoiDung;
            }
            set { khoNguoiDung = value; }
        }

        public static UserAccount FindByUsernameOrEmail(string giaTri)
        {
            return Store.FindByUsernameOrEmail(giaTri);
        }

        public static UserAccount FindByUsername(string tenDangNhap)
        {
            return Store.FindByUsername(tenDangNhap);
        }

        public static bool UsernameExists(string tenDangNhap)
        {
            return Store.UsernameExists(tenDangNhap);
        }

        public static bool EmailExists(string email)
        {
            return Store.EmailExists(email);
        }

        public static AddUserOutcome TryAdd(UserAccount taiKhoan)
        {
            return Store.TryAdd(taiKhoan);
        }

        public static List<UserAccount> AllAccounts()
        {
            return Store.GetAll();
        }

        public static ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap,
            string hoTen, string email, string soDienThoai)
        {
            return Store.TryUpdateProfile(tenDangNhap, hoTen, email, soDienThoai);
        }

        public static PasswordChangeOutcome TryChangePassword(string tenDangNhap,
            string matKhauCu, string matKhauMoi)
        {
            return Store.TryChangePassword(tenDangNhap, matKhauCu, matKhauMoi);
        }

        public static RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri)
        {
            return Store.TrySetAdmin(tenDangNhap, laQuanTri);
        }

        public static bool IsLoggedIn()
        {
            return HttpContext.Current != null
                && HttpContext.Current.Session["Username"] != null;
        }

        public static string CurrentUserName()
        {
            object tenHienThi = HttpContext.Current == null
                ? null
                : HttpContext.Current.Session["Username"];
            return tenHienThi == null ? "" : tenHienThi.ToString();
        }

        public static string CurrentUserKey()
        {
            HttpSessionState phien = HttpContext.Current == null
                ? null
                : HttpContext.Current.Session;
            return UserKeyOf(phien);
        }

        public static string UserKeyOf(HttpSessionState phien)
        {
            if (phien == null) return "";
            object khoaNguoiDung = phien["UserKey"] ?? phien["Username"];
            return khoaNguoiDung == null ? "" : khoaNguoiDung.ToString();
        }

        public static UserAccount CurrentAccount()
        {
            return IsLoggedIn() ? Store.FindByUsername(CurrentUserKey()) : null;
        }

        public static bool IsAdmin()
        {
            UserAccount taiKhoan = CurrentAccount();
            return taiKhoan != null && taiKhoan.IsAdmin;
        }

        public static void RefreshDisplayName()
        {
            UserAccount taiKhoan = CurrentAccount();
            if (taiKhoan != null)
                HttpContext.Current.Session["Username"] = taiKhoan.DisplayName;
        }

        public static void SignIn(UserAccount taiKhoan)
        {
            if (taiKhoan == null || HttpContext.Current == null) return;

            HttpSessionState phien = HttpContext.Current.Session;
            phien["Username"] = taiKhoan.DisplayName;
            phien["UserKey"] = taiKhoan.Username;
            new CartStore(phien).MergeGuestIntoAccount();
            new WishlistStore(phien).MergeGuestIntoAccount();
        }

        public static void SignOut()
        {
            if (HttpContext.Current == null) return;

            HttpSessionState phien = HttpContext.Current.Session;
            phien["Username"] = null;
            phien["UserKey"] = null;
            new CartStore(phien).ClearGuest();
            new WishlistStore(phien).ClearGuest();
        }
    }
}
