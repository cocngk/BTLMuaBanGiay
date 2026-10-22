using System.Collections.Generic;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore.Services
{
    /// <summary>Kết quả một lượt đặt hàng: hoặc có đơn, hoặc có lý do chưa đặt được.</summary>
    public sealed class OrderResult
    {
        public Order Order { get; private set; }
        public string Error { get; private set; }
        public bool IsEmptyCart { get; private set; }

        public bool IsPlaced
        {
            get { return Order != null; }
        }

        public static OrderResult Success(Order order)
        {
            return new OrderResult { Order = order };
        }

        public static OrderResult Invalid(string error)
        {
            return new OrderResult { Error = error };
        }

        public static OrderResult EmptyCart()
        {
            return new OrderResult { IsEmptyCart = true };
        }
    }

    /// <summary>
    /// Deep Order intake module: một cửa vào, một phán quyết.
    /// Nhận input + giỏ hiện tại, tự validate, trừ kho, tạo đơn, lưu và xoá giỏ.
    /// Trang thanh toán không còn tự dựng Order hay tự quyết định luật hợp lệ.
    /// </summary>
    public sealed class OrderIntake
    {
        private readonly SessionCartStore cartStore;
        private readonly IOrderStore orderStore;

        public OrderIntake(SessionCartStore cartStore, IOrderStore orderStore)
        {
            this.cartStore = cartStore;
            this.orderStore = orderStore;
        }

        public OrderResult Place(OrderInput input)
        {
            Cart cart = cartStore.Load();
            if (cart.IsEmpty)
            {
                return OrderResult.EmptyCart();
            }

            string error = OrderValidator.Validate(input);
            if (error != null)
            {
                return OrderResult.Invalid(error);
            }

            // Tồn kho có thể đã giảm sau lúc thêm vào giỏ (người khác đặt trước).
            // Trừ kho trước khi tạo đơn: thiếu hàng thì không đơn nào được ghi,
            // nhờ vậy không có đơn "ma" trỏ vào món đã hết.
            List<KeyValuePair<int, int>> wanted = cart.Lines
                .Select(line => new KeyValuePair<int, int>(line.Id, line.Quantity))
                .ToList();

            if (!Catalog.TakeStock(wanted))
            {
                return OrderResult.Invalid(
                    "Một sản phẩm trong giỏ đã hết hàng hoặc không đủ số lượng. Vui lòng kiểm tra lại giỏ hàng!");
            }

            Order order = new Order
            {
                Code = NextOrderCode(),
                CreatedAt = System.DateTime.Now,
                Username = input.Username,
                Status = Order.PendingStatus,
                CustomerName = input.CustomerName,
                Phone = input.Phone,
                Address = input.Address,
                PaymentMethod = input.PaymentMethod,
                Lines = OrderLine.FromCart(cart.Lines)
            };

            orderStore.Save(order);
            cartStore.Clear();

            return OrderResult.Success(order);
        }

        /// <summary>
        /// Mã đơn không được trùng: mã là thứ khách dùng để tra cứu, trùng nghĩa là
        /// tra ra đơn của người khác. Thử lại có giới hạn thay vì vòng lặp vô hạn.
        /// </summary>
        private string NextOrderCode()
        {
            string code = Order.NewCode();
            for (int attempt = 0; attempt < 10 && orderStore.FindByCode(code) != null; attempt++)
            {
                code = Order.NewCode();
            }
            return code;
        }
    }
}
