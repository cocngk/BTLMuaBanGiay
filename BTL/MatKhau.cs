using System;
using System.Security.Cryptography;
using System.Text;

namespace ShoeStore
{
    // Băm mật khẩu bằng SHA256 rồi đổi sang chuỗi hex.
    // Không lưu mật khẩu gốc vào cơ sở dữ liệu.
    public static class MatKhau
    {
        public static string Bam(string matKhau)
        {
            using (SHA256 bam = SHA256.Create())
            {
                byte[] ketQua = bam.ComputeHash(Encoding.UTF8.GetBytes(matKhau));

                StringBuilder chuoi = new StringBuilder();
                foreach (byte b in ketQua) chuoi.Append(b.ToString("x2"));
                return chuoi.ToString();
            }
        }
    }
}
