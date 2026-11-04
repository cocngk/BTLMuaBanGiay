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
        // Thực hiện việc thông báo thành công.
        public static OrderResult Success(Order donHang) { return new OrderResult { Order = donHang }; }
        // Thực hiện việc invalid.
        public static OrderResult Invalid(string loi) { return new OrderResult { Error = loi }; }
        // Thực hiện việc empty giỏ hàng.
        public static OrderResult EmptyCart() { return new OrderResult { IsEmptyCart = true }; }
    }

    public sealed class OrderIntake
    {
        private readonly CartStore khoGioHang; private readonly IOrderStore khoDonHang;
        // Thực hiện việc đơn hàng intake.
        public OrderIntake(CartStore khoGioHang, IOrderStore khoDonHang) { this.khoGioHang = khoGioHang; this.khoDonHang = khoDonHang; }
        // Thực hiện việc tạo.
        public OrderResult Place(OrderInput duLieuVao)
        {
            Cart gioHang = khoGioHang.Load();
            if (gioHang.IsEmpty) return OrderResult.EmptyCart();
            string loi = OrderValidator.Validate(duLieuVao);
            if (loi != null) return OrderResult.Invalid(loi);
            List<KeyValuePair<int, int>> canDung = gioHang.Lines.Select(x => new KeyValuePair<int, int>(x.Id, x.Quantity)).ToList();
            if (!Catalog.TakeStock(canDung)) return OrderResult.Invalid("Một sản phẩm trong giỏ đã hết hàng hoặc không còn bán. Vui lòng kiểm tra lại giỏ hàng!");
            Order donHang = new Order { Code = NewCode(), CreatedAt = DateTime.Now, Username = duLieuVao.Username, Status = Order.PendingStatus, CustomerName = duLieuVao.CustomerName, Phone = duLieuVao.Phone, Address = duLieuVao.Address, PaymentMethod = duLieuVao.PaymentMethod, Lines = OrderLine.FromCart(gioHang.Lines) };
            khoDonHang.Save(donHang); khoGioHang.Clear(); return OrderResult.Success(donHang);
        }
        // Thực hiện việc tạo mới mã.
        private string NewCode() { string ma = Order.NewCode(); for (int chiSo = 0; chiSo < 10 && khoDonHang.FindByCode(ma) != null; chiSo++) ma = Order.NewCode(); return ma; }
    }

    public enum StatusChangeOutcome { Updated, Unchanged, NotFound, InvalidStatus, OutOfStock }
    public sealed class StatusChangeResult { public StatusChangeOutcome Outcome { get; set; } public string Message { get; set; } public bool Ok { get { return Outcome == StatusChangeOutcome.Updated || Outcome == StatusChangeOutcome.Unchanged; } } }

    public sealed class OrderAdminService
    {
        private readonly ApplicationOrderStore kho;
        // Tạo dịch vụ quản lý đơn hàng.
        public OrderAdminService() : this(new ApplicationOrderStore()) { }
        // Tạo dịch vụ quản lý đơn hàng.
        public OrderAdminService(ApplicationOrderStore kho) { this.kho = kho; }
        // Thực hiện việc toàn bộ.
        public List<Order> All() { return kho.All(); }
        // Thực hiện việc lọc theo trạng thái.
        public static List<Order> FilterByStatus(IEnumerable<Order> nguon, string trangThai) { List<Order> danhSachDonHang = nguon == null ? new List<Order>() : nguon.ToList(); return string.IsNullOrEmpty(trangThai) || !Order.IsKnownStatus(trangThai) ? danhSachDonHang : danhSachDonHang.Where(x => x.Status == trangThai).ToList(); }
        // Thực hiện việc thay đổi trạng thái.
        public StatusChangeResult ChangeStatus(string ma, string trangThai)
        {
            if (string.IsNullOrWhiteSpace(ma)) return Result(StatusChangeOutcome.NotFound, "Thiếu mã đơn hàng.");
            if (!Order.IsKnownStatus(trangThai)) return Result(StatusChangeOutcome.InvalidStatus, "Trạng thái không hợp lệ.");
            {
                Order donHang = kho.FindByCode(ma);
                if (donHang == null) return Result(StatusChangeOutcome.NotFound, "Không tìm thấy đơn hàng có mã " + ma.Trim() + ".");
                if (donHang.Status == trangThai) return Result(StatusChangeOutcome.Unchanged, "Đơn " + donHang.Code + " đang ở trạng thái này rồi.");
                bool daHuy = donHang.Status == Order.CancelledStatus; bool seHuy = trangThai == Order.CancelledStatus; string ghiChu = "";
                if (seHuy && !daHuy) { Catalog.RestoreStock(Quantities(donHang)); ghiChu = " Đã trả " + donHang.TotalQuantity() + " đôi về kho."; }
                else if (daHuy && !seHuy)
                {
                    List<string> shortages = Shortages(donHang);
                    if (shortages.Count > 0) return Result(StatusChangeOutcome.OutOfStock, "Không mở lại được đơn: không đủ tồn kho cho " + string.Join(", ", shortages.ToArray()) + ".");
                    if (!Catalog.TakeStock(Quantities(donHang))) return Result(StatusChangeOutcome.OutOfStock, "Không mở lại được đơn: tồn kho vừa thay đổi, vui lòng thử lại.");
                    ghiChu = " Đã lấy lại " + donHang.TotalQuantity() + " đôi từ kho.";
                }
                donHang.Status = trangThai;
                kho.Update(donHang);
                return Result(StatusChangeOutcome.Updated, "Đơn " + donHang.Code + " → " + trangThai + "." + ghiChu);
            }
        }
        // Tạo kết quả thay đổi trạng thái.
        private static StatusChangeResult Result(StatusChangeOutcome outcome, string thongBao) { return new StatusChangeResult { Outcome = outcome, Message = thongBao }; }
        // Thực hiện việc quantities.
        private static IEnumerable<KeyValuePair<int, int>> Quantities(Order donHang) { return (donHang.Lines ?? new List<OrderLine>()).Select(x => new KeyValuePair<int, int>(x.ProductId, x.Quantity)).ToList(); }
        // Thực hiện việc mặt hàng thiếu.
        private static List<string> Shortages(Order donHang)
        {
            Dictionary<int, int> canDung = new Dictionary<int, int>();
            foreach (OrderLine dong in donHang.Lines ?? new List<OrderLine>()) canDung[dong.ProductId] = canDung.ContainsKey(dong.ProductId) ? canDung[dong.ProductId] + dong.Quantity : dong.Quantity;
            List<string> ketQua = new List<string>();
            foreach (KeyValuePair<int, int> phanTu in canDung)
            {
                ProductItem sanPham = Catalog.Find(phanTu.Key);
                if (sanPham == null) { OrderLine dong = donHang.Lines.FirstOrDefault(x => x.ProductId == phanTu.Key); ketQua.Add(dong == null ? "sản phẩm #" + phanTu.Key : dong.Name + " (đã ngừng bán)"); }
                else if (sanPham.Stock < phanTu.Value) ketQua.Add(sanPham.Name + " (còn " + sanPham.Stock + ")");
            }
            return ketQua;
        }
    }
}
