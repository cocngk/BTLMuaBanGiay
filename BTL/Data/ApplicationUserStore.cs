using System.Collections.Generic;
using ShoeStore.Helpers;

namespace ShoeStore.Data
{
    /// <summary>
    /// Kho người dùng ở cấp ứng dụng: tài khoản đăng ký ở một phiên vẫn còn khi
    /// phiên đó đóng, vì dữ liệu nằm trong Application — không phải trong Session,
    /// cũng không phải trong biến static của lớp.
    /// Bọc MemoryUserStore (đã có lock và TryAdd nguyên tử) thay vì viết lại luật trùng.
    /// </summary>
    public sealed class ApplicationUserStore : IUserStore
    {
        private static MemoryUserStore Inner
        {
            get
            {
                return ApplicationStore.GetOrCreate(
                    ApplicationStore.UsersKey,
                    () => new MemoryUserStore());
            }
        }

        public List<UserAccount> GetAll()
        {
            return Inner.GetAll();
        }

        public UserAccount FindByUsernameOrEmail(string usernameOrEmail)
        {
            return Inner.FindByUsernameOrEmail(usernameOrEmail);
        }

        public UserAccount FindByUsername(string username)
        {
            return Inner.FindByUsername(username);
        }

        public bool UsernameExists(string username)
        {
            return Inner.UsernameExists(username);
        }

        public bool EmailExists(string email)
        {
            return Inner.EmailExists(email);
        }

        public AddUserOutcome TryAdd(UserAccount account)
        {
            return Inner.TryAdd(account);
        }

        public ProfileUpdateOutcome TryUpdateProfile(string username, string fullName, string email, string phone)
        {
            return Inner.TryUpdateProfile(username, fullName, email, phone);
        }

        public PasswordChangeOutcome TryChangePassword(string username, string currentPassword, string newPassword)
        {
            return Inner.TryChangePassword(username, currentPassword, newPassword);
        }

        public RoleChangeOutcome TrySetAdmin(string username, bool isAdmin)
        {
            return Inner.TrySetAdmin(username, isAdmin);
        }
    }
}
