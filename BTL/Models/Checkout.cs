using System.Text.RegularExpressions;

namespace ShoeStore.Models
{
    /// <summary>
    /// Payment vocabulary, shared by markup values and order intake.
    /// Add a method here and both sides follow.
    /// </summary>
    public static class PaymentMethods
    {
        public const string Cod = "COD";
        public const string Qr = "QR";
    }

    /// <summary>Order intake: one object in, one verdict out.</summary>
    public class OrderInput
    {
        /// <summary>Khóa tài khoản đang đặt — OrderIntake ghi vào đơn để lọc về sau.</summary>
        public string Username { get; set; }

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
    }

    /// <summary>Single validation ruleset (was hand-rolled per page).</summary>
    public static class OrderValidator
    {
        private static readonly Regex PhoneRegex = new Regex(@"^[0-9]{9,11}$");

        public static bool IsValidPhone(string phone)
        {
            return !string.IsNullOrEmpty(phone) && PhoneRegex.IsMatch(phone);
        }

        /// <summary>Returns null when valid, otherwise the error message.</summary>
        public static string Validate(OrderInput order)
        {
            if (order == null) return "Thiếu thông tin đơn hàng!";
            if (string.IsNullOrEmpty(order.CustomerName) ||
                string.IsNullOrEmpty(order.Phone) ||
                string.IsNullOrEmpty(order.Address))
            {
                return "Bạn chưa nhập đủ thông tin nhận hàng!";
            }
            if (!IsValidPhone(order.Phone))
            {
                return "Số điện thoại chưa hợp lệ. Vui lòng kiểm tra lại!";
            }
            return null;
        }
    }
}
