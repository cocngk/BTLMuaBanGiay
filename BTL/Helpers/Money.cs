namespace ShoeStore.Helpers
{
    /// <summary>
    /// Một nơi duy nhất quy định cách hiện tiền. Trước đây "{0:N0}đ" rải rác 11 chỗ
    /// và client tự thêm 'đ' riêng — đổi khung giá là phải sửa khắp site.
    /// </summary>
    public static class Money
    {
        public const string Suffix = "đ";

        /// <summary>Locale dùng ở client để khớp đúng với server.</summary>
        public const string JsLocale = "vi-VN";

        public static string Format(decimal value)
        {
            return string.Format("{0:N0}" + Suffix, value);
        }

        /// <summary>
        /// Khai báo formatMoney() cho client. Sinh từ cùng hằng số với Format
        /// để hai bên không lệch nhau.
        /// </summary>
        public static string JsFormatter
        {
            get
            {
                return "function formatMoney(n){"
                    + " return Number(n).toLocaleString('" + JsLocale + "') + '" + Suffix + "';"
                    + " }";
            }
        }
    }
}