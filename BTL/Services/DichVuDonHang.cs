using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore.Services
{
    // Kết quả đặt hàng để code-behind hiển thị đúng thông báo.
    public sealed class OrderResult
    {
        public Order Order { get; private set; }
        public string Error { get; private set; }
        public bool IsEmptyCart { get; private set; }
        public bool IsPlaced { get { return Order != null; } }

        // Tạo kết quả đặt hàng thành công.
        public static OrderResult Success(Order donHang)
        {
            return new OrderResult { Order = donHang };
        }

        // Tạo kết quả có lỗi dữ liệu.
        public static OrderResult Invalid(string loi)
        {
            return new OrderResult { Error = loi };
        }

        // Tạo kết quả giỏ hàng trống.
        public static OrderResult EmptyCart()
        {
            return new OrderResult { IsEmptyCart = true };
        }
    }

    public sealed class OrderIntake
    {
        private readonly CartStore khoGioHang;
        private readonly IOrderStore khoDonHang;

        public OrderIntake(CartStore khoGioHang, IOrderStore khoDonHang)
        {
            this.khoGioHang = khoGioHang;
            this.khoDonHang = khoDonHang;
        }

        // Kiểm tra giỏ, trừ kho và lưu đơn hàng.
        public OrderResult Place(OrderInput duLieuVao)
        {
            if (khoGioHang == null || khoDonHang == null)
                return OrderResult.Invalid("Không thể kết nối với dịch vụ đặt hàng.");

            Cart gioHang = khoGioHang.Load();
            if (gioHang == null || gioHang.IsEmpty)
                return OrderResult.EmptyCart();

            string loi = OrderValidator.Validate(duLieuVao);
            if (loi != null) return OrderResult.Invalid(loi);

            List<KeyValuePair<int, int>> canDung = gioHang.Lines
                .Select(dong => new KeyValuePair<int, int>(dong.Id, dong.Quantity))
                .ToList();
            if (!Catalog.TakeStock(canDung))
            {
                return OrderResult.Invalid(
                    "Một sản phẩm trong giỏ đã hết hàng hoặc không còn bán. "
                    + "Vui lòng kiểm tra lại giỏ hàng!");
            }

            Order donHang = new Order
            {
                Code = TaoMaDonHang(),
                CreatedAt = DateTime.Now,
                Username = duLieuVao.Username,
                Status = Order.PendingStatus,
                CustomerName = duLieuVao.CustomerName,
                Phone = duLieuVao.Phone,
                Address = duLieuVao.Address,
                PaymentMethod = duLieuVao.PaymentMethod,
                Lines = OrderLine.FromCart(gioHang.Lines)
            };

            try
            {
                khoDonHang.Save(donHang);
                khoGioHang.Clear();
                return OrderResult.Success(donHang);
            }
            catch
            {
                // Không để mất tồn kho nếu ghi đơn hàng thất bại.
                Catalog.RestoreStock(canDung);
                throw;
            }
        }

        // Sinh mã mới và kiểm tra trùng trong kho đơn hàng.
        private string TaoMaDonHang()
        {
            string maDonHang = Order.NewCode();
            for (int lan = 0; lan < 10; lan++)
            {
                if (khoDonHang.FindByCode(maDonHang) == null) return maDonHang;
                maDonHang = Order.NewCode();
            }

            return maDonHang + "-" + Guid.NewGuid().ToString("N").Substring(0, 4);
        }
    }

    public enum StatusChangeOutcome
    {
        Updated,
        Unchanged,
        NotFound,
        InvalidStatus,
        OutOfStock
    }

    public sealed class StatusChangeResult
    {
        public StatusChangeOutcome Outcome { get; set; }
        public string Message { get; set; }
        public bool Ok
        {
            get
            {
                return Outcome == StatusChangeOutcome.Updated
                    || Outcome == StatusChangeOutcome.Unchanged;
            }
        }
    }

    public sealed class OrderAdminService
    {
        private readonly ApplicationOrderStore kho;

        public OrderAdminService() : this(new ApplicationOrderStore()) { }

        public OrderAdminService(ApplicationOrderStore kho)
        {
            this.kho = kho;
        }

        // Lấy toàn bộ đơn cho trang quản trị.
        public List<Order> All()
        {
            return kho.All();
        }

        // Lọc đơn theo trạng thái nếu trạng thái hợp lệ.
        public static List<Order> FilterByStatus(IEnumerable<Order> nguon,
            string trangThai)
        {
            List<Order> danhSach = (nguon ?? Enumerable.Empty<Order>()).ToList();
            if (string.IsNullOrEmpty(trangThai)
                || !Order.IsKnownStatus(trangThai))
                return danhSach;

            return danhSach.Where(donHang => donHang.Status == trangThai).ToList();
        }

        // Đổi trạng thái và điều chỉnh tồn kho khi đơn bị hủy/mở lại.
        public StatusChangeResult ChangeStatus(string maDonHang,
            string trangThaiMoi)
        {
            if (string.IsNullOrWhiteSpace(maDonHang))
                return KetQua(StatusChangeOutcome.NotFound,
                    "Thiếu mã đơn hàng.");
            if (!Order.IsKnownStatus(trangThaiMoi))
                return KetQua(StatusChangeOutcome.InvalidStatus,
                    "Trạng thái không hợp lệ.");

            Order donHang = kho.FindByCode(maDonHang.Trim());
            if (donHang == null)
            {
                return KetQua(StatusChangeOutcome.NotFound,
                    "Không tìm thấy đơn hàng có mã " + maDonHang.Trim() + ".");
            }
            if (donHang.Status == trangThaiMoi)
            {
                return KetQua(StatusChangeOutcome.Unchanged,
                    "Đơn " + donHang.Code + " đang ở trạng thái này rồi.");
            }

            bool daHuy = donHang.Status == Order.CancelledStatus;
            bool seHuy = trangThaiMoi == Order.CancelledStatus;
            IEnumerable<KeyValuePair<int, int>> soLuong = SoLuong(donHang);
            string ghiChu = "";

            if (seHuy && !daHuy)
            {
                Catalog.RestoreStock(soLuong);
                ghiChu = " Đã trả " + donHang.TotalQuantity() + " đôi về kho.";
            }
            else if (daHuy && !seHuy)
            {
                List<string> thieu = TimSanPhamThieu(donHang);
                if (thieu.Count > 0)
                {
                    return KetQua(StatusChangeOutcome.OutOfStock,
                        "Không mở lại được đơn: không đủ tồn kho cho "
                        + string.Join(", ", thieu.ToArray()) + ".");
                }
                if (!Catalog.TakeStock(soLuong))
                {
                    return KetQua(StatusChangeOutcome.OutOfStock,
                        "Không mở lại được đơn: tồn kho vừa thay đổi, vui lòng thử lại.");
                }
                ghiChu = " Đã lấy lại " + donHang.TotalQuantity() + " đôi từ kho.";
            }

            try
            {
                donHang.Status = trangThaiMoi;
                kho.Update(donHang);
                return KetQua(StatusChangeOutcome.Updated,
                    "Đơn " + donHang.Code + " → " + trangThaiMoi + "." + ghiChu);
            }
            catch
            {
                // Hoàn tác thay đổi kho nếu cập nhật đơn không thành công.
                if (seHuy && !daHuy) Catalog.TakeStock(soLuong);
                if (daHuy && !seHuy) Catalog.RestoreStock(soLuong);
                throw;
            }
        }

        private static StatusChangeResult KetQua(StatusChangeOutcome ketQua,
            string thongBao)
        {
            return new StatusChangeResult
            {
                Outcome = ketQua,
                Message = thongBao
            };
        }

        private static IEnumerable<KeyValuePair<int, int>> SoLuong(Order donHang)
        {
            return (donHang.Lines ?? new List<OrderLine>())
                .Select(dong => new KeyValuePair<int, int>(
                    dong.ProductId, dong.Quantity))
                .ToList();
        }

        // Tìm các sản phẩm không đủ tồn khi mở lại đơn đã hủy.
        private static List<string> TimSanPhamThieu(Order donHang)
        {
            List<OrderLine> cacDong = donHang.Lines ?? new List<OrderLine>();
            Dictionary<int, int> canDung = new Dictionary<int, int>();
            foreach (OrderLine dong in cacDong)
            {
                canDung[dong.ProductId] = canDung.ContainsKey(dong.ProductId)
                    ? canDung[dong.ProductId] + dong.Quantity
                    : dong.Quantity;
            }

            List<string> ketQua = new List<string>();
            foreach (KeyValuePair<int, int> phanTu in canDung)
            {
                ProductItem sanPham = Catalog.Find(phanTu.Key);
                if (sanPham == null)
                {
                    OrderLine dong = cacDong.FirstOrDefault(
                        item => item.ProductId == phanTu.Key);
                    ketQua.Add(dong == null
                        ? "sản phẩm #" + phanTu.Key
                        : dong.Name + " (đã ngừng bán)");
                }
                else if (sanPham.Stock < phanTu.Value)
                {
                    ketQua.Add(sanPham.Name + " (còn " + sanPham.Stock + ")");
                }
            }

            return ketQua;
        }
    }
}
