using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore.Services
{
    public sealed class OrderResult
    {
        public Order Order { get; private set; } public string Error { get; private set; } public bool IsEmptyCart { get; private set; } public bool IsPlaced { get { return Order != null; } }
        public static OrderResult Success(Order order) { return new OrderResult { Order = order }; }
        public static OrderResult Invalid(string error) { return new OrderResult { Error = error }; }
        public static OrderResult EmptyCart() { return new OrderResult { IsEmptyCart = true }; }
    }

    public sealed class OrderIntake
    {
        private readonly CartStore cartStore; private readonly IOrderStore orderStore;
        public OrderIntake(CartStore cartStore, IOrderStore orderStore) { this.cartStore = cartStore; this.orderStore = orderStore; }
        public OrderResult Place(OrderInput input)
        {
            Cart cart = cartStore.Load();
            if (cart.IsEmpty) return OrderResult.EmptyCart();
            string error = OrderValidator.Validate(input);
            if (error != null) return OrderResult.Invalid(error);
            List<KeyValuePair<int, int>> wanted = cart.Lines.Select(x => new KeyValuePair<int, int>(x.Id, x.Quantity)).ToList();
            if (!Catalog.TakeStock(wanted)) return OrderResult.Invalid("Một sản phẩm trong giỏ đã hết hàng hoặc không còn bán. Vui lòng kiểm tra lại giỏ hàng!");
            Order order = new Order { Code = NewCode(), CreatedAt = DateTime.Now, Username = input.Username, Status = Order.PendingStatus, CustomerName = input.CustomerName, Phone = input.Phone, Address = input.Address, PaymentMethod = input.PaymentMethod, Lines = OrderLine.FromCart(cart.Lines) };
            orderStore.Save(order); cartStore.Clear(); return OrderResult.Success(order);
        }
        private string NewCode() { string code = Order.NewCode(); for (int i = 0; i < 10 && orderStore.FindByCode(code) != null; i++) code = Order.NewCode(); return code; }
    }

    public enum StatusChangeOutcome { Updated, Unchanged, NotFound, InvalidStatus, OutOfStock }
    public sealed class StatusChangeResult { public StatusChangeOutcome Outcome { get; set; } public string Message { get; set; } public bool Ok { get { return Outcome == StatusChangeOutcome.Updated || Outcome == StatusChangeOutcome.Unchanged; } } }

    public sealed class OrderAdminService
    {
        private readonly ApplicationOrderStore store;
        public OrderAdminService() : this(new ApplicationOrderStore()) { }
        public OrderAdminService(ApplicationOrderStore store) { this.store = store; }
        public List<Order> All() { return store.All(); }
        public static List<Order> FilterByStatus(IEnumerable<Order> source, string status) { List<Order> orders = source == null ? new List<Order>() : source.ToList(); return string.IsNullOrEmpty(status) || !Order.IsKnownStatus(status) ? orders : orders.Where(x => x.Status == status).ToList(); }
        public StatusChangeResult ChangeStatus(string code, string status)
        {
            if (string.IsNullOrWhiteSpace(code)) return Result(StatusChangeOutcome.NotFound, "Thiếu mã đơn hàng.");
            if (!Order.IsKnownStatus(status)) return Result(StatusChangeOutcome.InvalidStatus, "Trạng thái không hợp lệ.");
            lock (ApplicationStore.Gate)
            {
                Order order = store.FindByCode(code);
                if (order == null) return Result(StatusChangeOutcome.NotFound, "Không tìm thấy đơn hàng có mã " + code.Trim() + ".");
                if (order.Status == status) return Result(StatusChangeOutcome.Unchanged, "Đơn " + order.Code + " đang ở trạng thái này rồi.");
                bool wasCancelled = order.Status == Order.CancelledStatus; bool willCancel = status == Order.CancelledStatus; string note = "";
                if (willCancel && !wasCancelled) { Catalog.RestoreStock(Quantities(order)); note = " Đã trả " + order.TotalQuantity() + " đôi về kho."; }
                else if (wasCancelled && !willCancel)
                {
                    List<string> shortages = Shortages(order);
                    if (shortages.Count > 0) return Result(StatusChangeOutcome.OutOfStock, "Không mở lại được đơn: không đủ tồn kho cho " + string.Join(", ", shortages.ToArray()) + ".");
                    if (!Catalog.TakeStock(Quantities(order))) return Result(StatusChangeOutcome.OutOfStock, "Không mở lại được đơn: tồn kho vừa thay đổi, vui lòng thử lại.");
                    note = " Đã lấy lại " + order.TotalQuantity() + " đôi từ kho.";
                }
                order.Status = status; return Result(StatusChangeOutcome.Updated, "Đơn " + order.Code + " → " + status + "." + note);
            }
        }
        private static StatusChangeResult Result(StatusChangeOutcome outcome, string message) { return new StatusChangeResult { Outcome = outcome, Message = message }; }
        private static IEnumerable<KeyValuePair<int, int>> Quantities(Order order) { return (order.Lines ?? new List<OrderLine>()).Select(x => new KeyValuePair<int, int>(x.ProductId, x.Quantity)).ToList(); }
        private static List<string> Shortages(Order order)
        {
            Dictionary<int, int> wanted = new Dictionary<int, int>();
            foreach (OrderLine line in order.Lines ?? new List<OrderLine>()) wanted[line.ProductId] = wanted.ContainsKey(line.ProductId) ? wanted[line.ProductId] + line.Quantity : line.Quantity;
            List<string> result = new List<string>();
            foreach (KeyValuePair<int, int> item in wanted)
            {
                ProductItem product = Catalog.Find(item.Key);
                if (product == null) { OrderLine line = order.Lines.FirstOrDefault(x => x.ProductId == item.Key); result.Add(line == null ? "sản phẩm #" + item.Key : line.Name + " (đã ngừng bán)"); }
                else if (product.Stock < item.Value) result.Add(product.Name + " (còn " + product.Stock + ")");
            }
            return result;
        }
    }
}
