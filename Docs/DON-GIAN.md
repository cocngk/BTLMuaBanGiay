# Bản đơn giản — đối chiếu với slide môn Lập trình web

Nhánh `don-gian-slide`: viết lại toàn bộ phần code theo đúng mức của 5 file PDF trong thư mục `Docs/`.
Không có lớp trung gian, không có thư viện riêng, không có cơ sở dữ liệu — dữ liệu nằm trong bộ nhớ.

## 1. Mỗi khái niệm trong slide nằm ở đâu

| Trong slide | Trong project | File |
|---|---|---|
| Tạo trang `.aspx`, code-behind | 10 trang, mỗi trang 1 file `.aspx` + 1 file `.aspx.cs` | `*.aspx`, `*.aspx.cs` |
| `Page_Load` và `IsPostBack` | Mọi trang đều có | `TrangChu.aspx.cs`, … |
| `web.config`: `appSettings` | Cấu hình chung (không còn chuỗi kết nối) | `Web.config` |
| `web.config`: `maxRequestLength`, `executionTimeout` | Cấu hình trong `httpRuntime` | `Web.config` |
| `web.config`: `X-Frame-Options`, `X-XSS-Protection` | Hai header trong `httpProtocol` | `Web.config` |
| `web.config`: `sessionState` | Phiên làm việc 30 phút | `Web.config` |
| `Session` | Đăng nhập, giỏ hàng | `DangNhap.aspx.cs`, `Gio.cs` |
| `Cookie` | "Nhớ đăng nhập" 7 ngày | `DangNhap.aspx.cs`, `Site.Master.cs` |
| `Application` | Đếm lượt truy cập, số người online | `Global.asax.cs`, `Site.Master.cs` |
| `Global.asax` | `Application_Start`, `Session_Start`, `Session_End` | `Global.asax.cs` |
| `Request.QueryString` | `?id=`, `?search=`, `?brand=`, `?ma=`, `?return=` | `ChiTietSanPham.aspx.cs`, `SanPham.aspx.cs`, `ThanhToan.aspx.cs`, `DangNhap.aspx.cs` |
| `Request.Form` | Không cần — đọc qua `TextBox.Text` | — |
| `Response.Redirect` | Chuyển trang sau khi lưu, đăng nhập, đặt hàng | tất cả các trang |
| `Server.HtmlEncode` | Mọi chỗ in dữ liệu người dùng nhập | `SanPham.aspx.cs`, `DangNhap.aspx.cs`, … |
| `Server.UrlEncode` | Mã hóa từ khóa và đường dẫn quay lại | `Site.Master.cs`, `ChiTietSanPham.aspx.cs` |
| Điều khiển dữ liệu (`Repeater`, `GridView`) | Danh sách sản phẩm, giỏ hàng, bảng quản trị | `TrangChu.aspx`, `SanPham.aspx`, `GioHang.aspx`, `QuanTriSanPham.aspx` |
| Mảng, danh sách, vòng lặp | Toàn bộ dữ liệu của cửa hàng | `DuLieu.cs`, `Gio.cs` |

## 2. Ba bài tập trong slide, làm thật trong project

| Bài tập trong slide | Trong project |
|---|---|
| Đăng nhập bằng `Session` | `DangNhap.aspx` → `Session["Username"]`, `Session["IsAdmin"]` |
| Nhớ đăng nhập bằng `Cookie` | Ô "Nhớ đăng nhập 7 ngày" → `Response.Cookies["nhodangnhap"]`; mở lại trang tự điền tên |
| Đếm lượt truy cập bằng `Application` | `Global.asax.cs` giữ `Application["LuotTruyCap"]`, hiện ở chân trang mọi trang |

## 3. Dữ liệu: ba danh sách trong bộ nhớ

Không dùng SQL Server, không cần cài đặt gì thêm. `DuLieu.cs` giữ ba danh sách:

| Danh sách | Nội dung |
|---|---|
| `NguoiDungs` | Tài khoản: tên đăng nhập, mật khẩu, họ tên, email, điện thoại, cờ quản trị |
| `SanPhams` | Sản phẩm: tên, hãng, giá, tồn kho, danh sách size, ảnh |
| `DonHangs` | Đơn hàng, mỗi đơn có danh sách dòng `Lines` |

Dữ liệu mẫu (2 tài khoản, 12 sản phẩm) được tạo một lần trong hàm khởi tạo tĩnh của `DuLieu`.
Mật khẩu lưu thẳng, so sánh thẳng — đúng như ví dụ đăng nhập trong slide.

**Lưu ý:** vì dữ liệu nằm trong bộ nhớ nên mất khi ứng dụng khởi động lại (đóng Visual Studio, sửa `Web.config`, hoặc IIS tự recycle). Đơn hàng tạo trong lúc chạy chỉ sống trong phiên làm việc đó.

## 4. Mười trang

| Trang | Việc nó làm | Chức năng trong đề cương |
|---|---|---|
| `TrangChu.aspx` | 8 sản phẩm nổi bật | 1. Trang chủ |
| `SanPham.aspx` | Danh sách, tìm theo tên, lọc theo hãng | 2. Danh sách sản phẩm + 4. Tìm kiếm (lọc) |
| `ChiTietSanPham.aspx` | Ảnh, mô tả, chọn size, thêm vào giỏ | 3. Chi tiết sản phẩm |
| `DangKy.aspx` | Tạo tài khoản mới | 5. Đăng ký |
| `DangNhap.aspx` | Đăng nhập, nhớ đăng nhập | 6. Đăng nhập |
| `TaiKhoan.aspx` | Xem/sửa thông tin, đổi mật khẩu, xem đơn | 7. Xem, quản trị tài khoản |
| `GioHang.aspx` | Xem giỏ, sửa số lượng, xóa dòng | (bắt buộc để mua hàng) |
| `ThanhToan.aspx` | Nhập thông tin nhận hàng, ghi đơn, trừ tồn kho | (bắt buộc để mua hàng) |
| `QuanTriSanPham.aspx` | Bảng sản phẩm, xóa | 8. Danh sách sản phẩm (quản trị) |
| `SuaSanPham.aspx` | Thêm mới hoặc sửa sản phẩm | 9. Sửa sản phẩm |

## 5. Cách chạy

1. Mở `BTL.sln` bằng Visual Studio.
2. Bấm F5.
3. Đăng nhập `admin / admin123` để vào khu quản trị, hoặc `khach / khach123` để mua hàng.

Không cần chạy script SQL, không cần cấu hình chuỗi kết nối.

## 6. Đã bỏ so với bản cũ

Bản cũ (nhánh `viet-lai-webforms`) có 18 trang, các tầng `Data/`, `Services/`, `Models/`, `Helpers/`, `Controls/`, và SQL Server với 8 bảng.
Bản này bỏ: cơ sở dữ liệu, băm mật khẩu, tin tức, tư vấn, chính sách, tra cứu đơn hàng,
quản trị người dùng, quản trị đơn hàng, đánh giá sản phẩm, danh sách yêu thích,
nhiều màu sắc cho một sản phẩm, giỏ hàng lưu cho tài khoản, gộp giỏ khách vào giỏ tài khoản.

Muốn xem lại bản cũ: `git checkout viet-lai-webforms`.
