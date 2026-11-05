# Cơ sở dữ liệu

Chỉ có **một file**: `ShoeStore.sql`.

## Cách chạy

1. Mở SQL Server Management Studio.
2. Mở file `ShoeStore.sql`.
3. Bấm **Execute** (F5).

Script tự làm hết: tạo database `ShoeStore` nếu chưa có, xóa 4 bảng cũ, tạo lại 4 bảng, thêm 2 tài khoản và 12 sản phẩm mẫu. Chạy lại nhiều lần cũng được.

## 4 bảng

| Bảng | Nội dung |
|---|---|
| `Users` | Tài khoản: tên đăng nhập, mật khẩu đã băm, họ tên, email, điện thoại, cờ quản trị |
| `Products` | Sản phẩm: tên, hãng, giá, tồn kho, danh sách size, ảnh |
| `Orders` | Đơn hàng: mã đơn, người nhận, địa chỉ, hình thức thanh toán, tổng tiền |
| `OrderLines` | Chi tiết đơn: mỗi dòng là một sản phẩm trong đơn |

## Tài khoản mẫu

| Tên đăng nhập | Mật khẩu | Quyền |
|---|---|---|
| `admin` | `admin123` | Quản trị |
| `khach` | `khach123` | Khách |

## Kết nối

Chuỗi kết nối nằm trong `BTL/Web.config`, dùng Windows Authentication:

```
Data Source=localhost;Initial Catalog=ShoeStore;Integrated Security=True;TrustServerCertificate=True
```

Máy khác dùng SQL Server khác thì sửa `Data Source` tại đây.
