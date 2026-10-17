using System;
using System.Security.Cryptography;
using System.Text;

namespace ShoeStore.Helpers
{
    /// <summary>
    /// Hash mật khẩu SHA-256 kèm salt riêng cho từng tài khoản.
    /// Trước đây UserAccount lưu mật khẩu thẳng và so sánh thẳng, nên đọc
    /// kho ra là xong. Khi thêm database thì đổi thuật toán ở đúng một chỗ này.
    /// </summary>
    public static class PasswordHasher
    {
        public static string NewSalt()
        {
            byte[] bytes = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        public static string Hash(string password, string salt)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] raw = Encoding.UTF8.GetBytes((password ?? "") + "|" + (salt ?? ""));
                return Convert.ToBase64String(sha.ComputeHash(raw));
            }
        }

        public static bool Verify(string password, string salt, string expectedHash)
        {
            if (string.IsNullOrEmpty(expectedHash))
            {
                return false;
            }
            return FixedTimeEquals(Hash(password, salt), expectedHash);
        }

        /// <summary>
        /// So sánh không rò thời gian: duyệt hết chuỗi thay vì dừng ngay ở ký tự
        /// lệch đầu tiên, để không đoán được độ dài/ký tự của hash.
        /// </summary>
        private static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}