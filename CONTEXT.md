# CONTEXT.md — SneakerX (BTL)

Từ điển domain dùng chung cho mọi review/refactor. Tên module = tên dùng trong hội thoại kiến trúc.

| Khái niệm | Sống ở | Ghi chú |
|---|---|---|
| Catalog | `Data/ProductStore.cs`, `Models/ProductItem.cs`, TrangChu/SanPham | 1 catalog chuẩn; lọc qua query `search/brand/price/size/sort/page` |
| Cart (Giỏ hàng) | `Models/Cart.cs` (thuần, test được), `Models/CartItem.cs`, `Data/SessionCartStore.cs` (adapter Session duy nhất) | Line key = `(Id, Size)`; tổng/chặn kho trong module; toast qua `Helpers/Toast.cs` |
| Checkout | `ThanhToan.aspx(.cs)` | Đọc giỏ qua store, xóa giỏ sau đặt hàng; validation còn nằm trong handler (chưa gom) |
| Auth | `Helpers/AuthHelper.cs` (demo, plaintext — không dùng ngoài đồ án) | `Session["Username"]` chứa tên hiển thị |
| Content | `TinTuc`, `TuVan` | Tĩnh, drift với codebehind — để sau |
| Shell | `Site.Master(.cs)` | Header/nav/search/toast dùng chung; badge giỏ qua store |

Quy ước seam: không ai chạm `Session["Cart"]` trực tiếp ngoài `SessionCartStore`; không toast nào tự nối chuỗi JS ngoài `Toast`.
