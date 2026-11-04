# SQL Server setup

1. Mở `ShoeStore.sql` bằng SQL Server Management Studio và chạy một lần.
2. Sửa `connectionStrings/ShoeStoreDb` trong `Web.config` cho đúng SQL Server của máy.
3. Khởi động website. `Global.asax` sẽ tự thêm sản phẩm mẫu và hai tài khoản mẫu khi bảng còn trống.

Tài khoản mẫu:

- `admin` / `123`
- `sneakerx` / `123`

Không đưa mật khẩu SQL Server vào Git. Nếu dùng SQL Login thay vì Windows Authentication, đặt connection string ở môi trường triển khai hoặc biến cấu hình riêng.
