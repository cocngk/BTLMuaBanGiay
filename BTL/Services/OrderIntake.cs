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
    /// Nhận input + giỏ hiện tại, tự validate, tạo đơn, lưu và xoá giỏ.
    /// Trang thanh toán không còn tự dựng Order hay tự quyết định luật hợp lệ.
    /// </summary>
    public sealed class OrderIntake
    {
        private readonly SessionCartStore cartStore;
        private readonly SessionOrderStore orderStore;

        public OrderIntake(SessionCartStore cartStore, SessionOrderStore orderStore)
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

            Order order = new Order
            {
                Code = Order.NewCode(),
                CreatedAt = System.DateTime.Now,
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
    }
}
