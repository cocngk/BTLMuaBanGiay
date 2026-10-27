using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Models;

namespace ShoeStore.Services
{
    /// <summary>Kết quả đổi trạng thái đơn. Kho/ dịch vụ chỉ nói lý do, trang tự viết câu.</summary>
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

        /// <summary>Câu giải thích khi bị từ chối; null khi thành công.</summary>
        public string Message { get; set; }

        public bool Ok
        {
            get
            {
                return Outcome == StatusChangeOutcome.Updated ||
                       Outcome == StatusChangeOutcome.Unchanged;
            }
        }
    }

    /// <summary>
    /// Luật quản trị đơn hàng. Tồn tại vì đổi trạng thái không chỉ là gán chuỗi:
    /// huỷ đơn phải trả hàng về kho, và mở lại đơn đã huỷ phải lấy lại hàng —
    /// hai việc này phải cùng một khóa với lần ghi trạng thái, nếu không hai
    /// quản trị viên bấm cùng lúc sẽ cộng/trừ kho hai lần cho một đơn.
    /// </summary>
    public sealed class OrderAdminService
    {
        private readonly ApplicationOrderStore store;

        public OrderAdminService() : this(new ApplicationOrderStore())
        {
        }

        public OrderAdminService(ApplicationOrderStore store)
        {
            this.store = store;
        }

        /// <summary>Mọi đơn, mới nhất trước (store chèn đơn mới lên đầu).</summary>
        public List<Order> All()
        {
            return store.All();
        }

        /// <summary>Lọc theo trạng thái; chuỗi rỗng hoặc lạ nghĩa là "tất cả".</summary>
        public List<Order> ByStatus(string status)
        {
            List<Order> all = store.All();
            if (string.IsNullOrEmpty(status) || !Order.IsKnownStatus(status))
            {
                return all;
            }

            return all.Where(o => string.Equals(o.Status, status, StringComparison.Ordinal)).ToList();
        }

        /// <summary>
        /// Đổi trạng thái một đơn theo mã. Toàn bộ đọc-sửa-ghi, kể cả phần tồn kho,
        /// nằm trong ApplicationStore.Gate — khóa này lồng lại được nên các store
        /// gọi bên trong vẫn an toàn.
        /// </summary>
        public StatusChangeResult ChangeStatus(string code, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new StatusChangeResult
                {
                    Outcome = StatusChangeOutcome.NotFound,
                    Message = "Thiếu mã đơn hàng."
                };
            }

            if (!Order.IsKnownStatus(newStatus))
            {
                return new StatusChangeResult
                {
                    Outcome = StatusChangeOutcome.InvalidStatus,
                    Message = "Trạng thái không hợp lệ."
                };
            }

            lock (ApplicationStore.Gate)
            {
                Order order = store.FindByCode(code);
                if (order == null)
                {
                    return new StatusChangeResult
                    {
                        Outcome = StatusChangeOutcome.NotFound,
                        Message = "Không tìm thấy đơn hàng có mã " + code.Trim() + "."
                    };
                }

                if (string.Equals(order.Status, newStatus, StringComparison.Ordinal))
                {
                    return new StatusChangeResult
                    {
                        Outcome = StatusChangeOutcome.Unchanged,
                        Message = "Đơn " + order.Code + " đang ở trạng thái này rồi."
                    };
                }

                bool wasCancelled = string.Equals(order.Status, Order.CancelledStatus, StringComparison.Ordinal);
                bool willCancel = string.Equals(newStatus, Order.CancelledStatus, StringComparison.Ordinal);
                string stockNote = "";

                if (willCancel && !wasCancelled)
                {
                    // Huỷ: trả hàng về kho đúng một lần. Không cần cờ riêng — trạng
                    // thái hiện tại chính là cờ, vì điều kiện đã loại trường hợp
                    // "đã huỷ rồi mà huỷ lại".
                    Catalog.RestoreStock(OrderedQuantities(order));
                    stockNote = " Đã trả " + order.TotalQuantity() + " đôi về kho.";
                }
                else if (wasCancelled && !willCancel)
                {
                    // Mở lại đơn: phải lấy lại hàng. Hết hàng thì không mở, và cũng
                    // không đổi trạng thái — nếu không đơn sẽ nói "đang giao" trong
                    // khi kho không có gì để giao.
                    List<string> shortages = FindShortages(order);
                    if (shortages.Count > 0)
                    {
                        return new StatusChangeResult
                        {
                            Outcome = StatusChangeOutcome.OutOfStock,
                            Message = "Không mở lại được đơn: không đủ tồn kho cho " +
                                      string.Join(", ", shortages.ToArray()) + "."
                        };
                    }

                    if (!Catalog.TakeStock(OrderedQuantities(order)))
                    {
                        return new StatusChangeResult
                        {
                            Outcome = StatusChangeOutcome.OutOfStock,
                            Message = "Không mở lại được đơn: tồn kho vừa thay đổi, vui lòng thử lại."
                        };
                    }

                    stockNote = " Đã lấy lại " + order.TotalQuantity() + " đôi từ kho.";
                }

                order.Status = newStatus;

                return new StatusChangeResult
                {
                    Outcome = StatusChangeOutcome.Updated,
                    Message = "Đơn " + order.Code + " → " + newStatus + "." + stockNote
                };
            }
        }

        /// <summary>Số lượng theo từng sản phẩm trong đơn — đúng dạng TakeStock/RestoreStock nhận.</summary>
        private static IEnumerable<KeyValuePair<int, int>> OrderedQuantities(Order order)
        {
            return (order.Lines ?? new List<OrderLine>())
                .Select(line => new KeyValuePair<int, int>(line.ProductId, line.Quantity))
                .ToList();
        }

        /// <summary>Tên những sản phẩm trong đơn không còn đủ hàng để mở lại.</summary>
        private static List<string> FindShortages(Order order)
        {
            Dictionary<int, int> wanted = new Dictionary<int, int>();
            foreach (OrderLine line in order.Lines ?? new List<OrderLine>())
            {
                int current;
                wanted[line.ProductId] = wanted.TryGetValue(line.ProductId, out current)
                    ? current + line.Quantity
                    : line.Quantity;
            }

            List<string> names = new List<string>();
            foreach (KeyValuePair<int, int> pair in wanted)
            {
                ProductItem product = Catalog.Find(pair.Key);
                if (product == null)
                {
                    OrderLine line = order.Lines.FirstOrDefault(l => l.ProductId == pair.Key);
                    names.Add(line != null ? line.Name + " (đã ngừng bán)" : "sản phẩm #" + pair.Key);
                }
                else if (product.Stock < pair.Value)
                {
                    names.Add(product.Name + " (còn " + product.Stock + ")");
                }
            }

            return names;
        }
    }
}
