using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    // Tập trung các thao tác kết nối và tạo tham số SQL.
    internal static class SqlDb
    {
        private const string TenKetNoi = "ShoeStoreDb";

        // Mở kết nối tới SQL Server.
        public static SqlConnection Open()
        {
            ConnectionStringSettings cauHinh =
                ConfigurationManager.ConnectionStrings[TenKetNoi];

            if (cauHinh == null || string.IsNullOrWhiteSpace(cauHinh.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "Thiếu connectionStrings/ShoeStoreDb trong Web.config.");
            }

            SqlConnection ketNoi = new SqlConnection(cauHinh.ConnectionString);
            ketNoi.Open();
            return ketNoi;
        }

        // Tạo tham số có giá trị DBNull khi dữ liệu rỗng.
        public static SqlParameter Parameter(string ten, object giaTri)
        {
            return new SqlParameter(ten, giaTri ?? DBNull.Value);
        }

        // Đọc chuỗi, trả về null nếu cột đang là DBNull.
        public static string StringValue(SqlDataReader boDoc, string tenCot)
        {
            return boDoc[tenCot] == DBNull.Value ? null : boDoc[tenCot].ToString();
        }
    }

    public sealed class SqlUserStore : IUserStore
    {
        private const string CotNguoiDung =
            "Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin";

        // Lấy tất cả tài khoản theo tên đăng nhập.
        public List<UserAccount> GetAll()
        {
            List<UserAccount> ketQua = new List<UserAccount>();
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT " + CotNguoiDung + " FROM Users ORDER BY Username", ketNoi))
            using (SqlDataReader boDoc = lenh.ExecuteReader())
            {
                while (boDoc.Read()) ketQua.Add(ChuyenTaiKhoan(boDoc));
            }

            return ketQua;
        }

        // Tìm theo tên đăng nhập hoặc email.
        public UserAccount FindByUsernameOrEmail(string giaTri)
        {
            return TimMot("WHERE Username=@giaTri OR Email=@giaTri",
                SqlDb.Parameter("@giaTri", giaTri ?? ""));
        }

        // Tìm theo tên đăng nhập.
        public UserAccount FindByUsername(string tenDangNhap)
        {
            return TimMot("WHERE Username=@giaTri",
                SqlDb.Parameter("@giaTri", tenDangNhap ?? ""));
        }

        // Kiểm tra tên đăng nhập đã được dùng chưa.
        public bool UsernameExists(string tenDangNhap)
        {
            return CotDaTonTai("Username", tenDangNhap);
        }

        // Kiểm tra email đã được dùng chưa.
        public bool EmailExists(string email)
        {
            return CotDaTonTai("Email", email);
        }

        // Thêm tài khoản mới, có xử lý trường hợp trùng khóa duy nhất.
        public AddUserOutcome TryAdd(UserAccount taiKhoan)
        {
            if (taiKhoan == null) return AddUserOutcome.UsernameTaken;
            if (UsernameExists(taiKhoan.Username)) return AddUserOutcome.UsernameTaken;
            if (EmailExists(taiKhoan.Email)) return AddUserOutcome.EmailTaken;

            const string cauLenh = @"
                INSERT INTO Users
                    (Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin)
                VALUES
                    (@username,@fullname,@email,@phone,@hash,@salt,@isadmin)";

            try
            {
                using (SqlConnection ketNoi = SqlDb.Open())
                using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi))
                {
                    ThemThamSoTaiKhoan(lenh, taiKhoan);
                    lenh.ExecuteNonQuery();
                }

                return AddUserOutcome.Added;
            }
            catch (SqlException loi) when (loi.Number == 2601 || loi.Number == 2627)
            {
                return UsernameExists(taiKhoan.Username)
                    ? AddUserOutcome.UsernameTaken
                    : AddUserOutcome.EmailTaken;
            }
        }

        // Cập nhật hồ sơ và không cho phép trùng email.
        public ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap,
            string hoTen, string email, string soDienThoai)
        {
            if (FindByUsername(tenDangNhap) == null)
                return ProfileUpdateOutcome.NotFound;
            if (EmailExistsForOther(email, tenDangNhap))
                return ProfileUpdateOutcome.EmailTaken;

            const string cauLenh = @"
                UPDATE Users
                SET FullName=@fullname, Email=@email, Phone=@phone
                WHERE Username=@username";

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi))
            {
                lenh.Parameters.AddWithValue("@fullname", (object)hoTen ?? DBNull.Value);
                lenh.Parameters.AddWithValue("@email", email ?? "");
                lenh.Parameters.AddWithValue("@phone", (object)soDienThoai ?? DBNull.Value);
                lenh.Parameters.AddWithValue("@username", tenDangNhap ?? "");
                lenh.ExecuteNonQuery();
            }

            return ProfileUpdateOutcome.Updated;
        }

        // Đổi mật khẩu sau khi kiểm tra mật khẩu hiện tại.
        public PasswordChangeOutcome TryChangePassword(string tenDangNhap,
            string matKhauHienTai, string matKhauMoi)
        {
            UserAccount taiKhoan = FindByUsername(tenDangNhap);
            if (taiKhoan == null) return PasswordChangeOutcome.NotFound;
            if (!PasswordHasher.Verify(matKhauHienTai, taiKhoan.PasswordSalt,
                taiKhoan.PasswordHash))
                return PasswordChangeOutcome.WrongPassword;

            string chuoiMuoi = PasswordHasher.NewSalt();
            const string cauLenh = @"
                UPDATE Users
                SET PasswordHash=@hash, PasswordSalt=@salt
                WHERE Username=@username";

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi))
            {
                lenh.Parameters.AddWithValue("@hash",
                    PasswordHasher.Hash(matKhauMoi, chuoiMuoi));
                lenh.Parameters.AddWithValue("@salt", chuoiMuoi);
                lenh.Parameters.AddWithValue("@username", tenDangNhap ?? "");
                lenh.ExecuteNonQuery();
            }

            return PasswordChangeOutcome.Changed;
        }

        // Cấp hoặc thu quyền quản trị, luôn giữ lại ít nhất một quản trị viên.
        public RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri)
        {
            UserAccount taiKhoan = FindByUsername(tenDangNhap);
            if (taiKhoan == null) return RoleChangeOutcome.NotFound;
            if (taiKhoan.IsAdmin == laQuanTri) return RoleChangeOutcome.Unchanged;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich =
                ketNoi.BeginTransaction(IsolationLevel.Serializable))
            {
                if (!laQuanTri)
                {
                    const string demQuanTri =
                        "SELECT COUNT(*) FROM Users WHERE IsAdmin=1";
                    using (SqlCommand lenh =
                        new SqlCommand(demQuanTri, ketNoi, giaoDich))
                    {
                        if (Convert.ToInt32(lenh.ExecuteScalar()) <= 1)
                        {
                            giaoDich.Rollback();
                            return RoleChangeOutcome.LastAdmin;
                        }
                    }
                }

                using (SqlCommand lenh = new SqlCommand(
                    "UPDATE Users SET IsAdmin=@isadmin WHERE Username=@username",
                    ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@isadmin", laQuanTri);
                    lenh.Parameters.AddWithValue("@username", tenDangNhap ?? "");
                    lenh.ExecuteNonQuery();
                }

                giaoDich.Commit();
            }

            return RoleChangeOutcome.Changed;
        }

        private static UserAccount TimMot(string dieuKien, params SqlParameter[] thamSo)
        {
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT TOP 1 " + CotNguoiDung + " FROM Users " + dieuKien,
                ketNoi))
            {
                lenh.Parameters.AddRange(thamSo);
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                    return boDoc.Read() ? ChuyenTaiKhoan(boDoc) : null;
            }
        }

        private static bool CotDaTonTai(string tenCot, string giaTri)
        {
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE " + tenCot + "=@giaTri", ketNoi))
            {
                lenh.Parameters.AddWithValue("@giaTri", giaTri ?? "");
                return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
            }
        }

        private static bool EmailExistsForOther(string email, string tenDangNhap)
        {
            const string cauLenh = @"
                SELECT COUNT(*) FROM Users
                WHERE Email=@email AND Username<>@username";

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi))
            {
                lenh.Parameters.AddWithValue("@email", email ?? "");
                lenh.Parameters.AddWithValue("@username", tenDangNhap ?? "");
                return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
            }
        }

        private static void ThemThamSoTaiKhoan(SqlCommand lenh, UserAccount taiKhoan)
        {
            lenh.Parameters.AddWithValue("@username", taiKhoan.Username ?? "");
            lenh.Parameters.AddWithValue("@fullname",
                (object)taiKhoan.FullName ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@email", taiKhoan.Email ?? "");
            lenh.Parameters.AddWithValue("@phone",
                (object)taiKhoan.Phone ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@hash", taiKhoan.PasswordHash ?? "");
            lenh.Parameters.AddWithValue("@salt", taiKhoan.PasswordSalt ?? "");
            lenh.Parameters.AddWithValue("@isadmin", taiKhoan.IsAdmin);
        }

        private static UserAccount ChuyenTaiKhoan(SqlDataReader boDoc)
        {
            return new UserAccount
            {
                Username = SqlDb.StringValue(boDoc, "Username"),
                FullName = SqlDb.StringValue(boDoc, "FullName"),
                Email = SqlDb.StringValue(boDoc, "Email"),
                Phone = SqlDb.StringValue(boDoc, "Phone"),
                PasswordHash = SqlDb.StringValue(boDoc, "PasswordHash"),
                PasswordSalt = SqlDb.StringValue(boDoc, "PasswordSalt"),
                IsAdmin = Convert.ToBoolean(boDoc["IsAdmin"])
            };
        }
    }

    public sealed class SqlOrderStore : IOrderStore
    {
        private const string CotDonHang =
            "Code,CreatedAt,Username,Status,CustomerName,Phone,Address,PaymentMethod";

        // Lưu đơn hàng và các dòng trong cùng một giao dịch.
        public void Save(Order donHang)
        {
            if (donHang == null) return;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
            {
                int maDonHang;
                const string cauLenh = @"
                    INSERT INTO Orders
                        (Code,CreatedAt,Username,Status,CustomerName,Phone,Address,PaymentMethod)
                    OUTPUT INSERTED.Id
                    VALUES
                        (@code,@created,@username,@status,@name,@phone,@address,@payment)";

                using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi, giaoDich))
                {
                    ThemThamSoDonHang(lenh, donHang);
                    maDonHang = Convert.ToInt32(lenh.ExecuteScalar());
                }

                ThemDongDonHang(ketNoi, giaoDich, maDonHang, donHang.Lines);
                giaoDich.Commit();
            }
        }

        // Lấy toàn bộ đơn hàng mới nhất trước.
        public List<Order> All()
        {
            return DocDonHang("", null);
        }

        // Lấy đơn hàng của một tài khoản.
        public List<Order> ByUser(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                return new List<Order>();

            return DocDonHang(" WHERE Username=@username",
                SqlDb.Parameter("@username", tenDangNhap));
        }

        // Tìm một đơn hàng theo mã.
        public Order FindByCode(string maDonHang)
        {
            if (string.IsNullOrWhiteSpace(maDonHang)) return null;
            return DocDonHang(" WHERE Code=@code",
                SqlDb.Parameter("@code", maDonHang.Trim())).FirstOrDefault();
        }

        // Cập nhật trạng thái đơn hàng.
        public void Update(Order donHang)
        {
            if (donHang == null || string.IsNullOrWhiteSpace(donHang.Code)) return;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "UPDATE Orders SET Status=@status WHERE Code=@code", ketNoi))
            {
                lenh.Parameters.AddWithValue("@status", donHang.Status ?? "");
                lenh.Parameters.AddWithValue("@code", donHang.Code.Trim());
                lenh.ExecuteNonQuery();
            }
        }

        // Tên cũ được giữ lại để tương thích với code tích hợp hiện có.
        public void UpdateStatus(Order donHang)
        {
            Update(donHang);
        }

        private static List<Order> DocDonHang(string boLoc, SqlParameter thamSo)
        {
            List<Order> ketQua = new List<Order>();
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT " + CotDonHang + " FROM Orders" + boLoc
                + " ORDER BY CreatedAt DESC", ketNoi))
            {
                if (thamSo != null) lenh.Parameters.Add(thamSo);
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read()) ketQua.Add(ChuyenDonHang(boDoc));
                }
            }

            foreach (Order donHang in ketQua)
                donHang.Lines = DocDongDonHang(donHang.Code);
            return ketQua;
        }

        private static List<OrderLine> DocDongDonHang(string maDonHang)
        {
            List<OrderLine> ketQua = new List<OrderLine>();
            const string cauLenh = @"
                SELECT l.ProductId,l.Name,l.Size,l.Price,l.Quantity
                FROM OrderLines l
                INNER JOIN Orders o ON o.Id=l.OrderId
                WHERE o.Code=@code
                ORDER BY l.Id";

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi))
            {
                lenh.Parameters.AddWithValue("@code", maDonHang ?? "");
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read())
                    {
                        ketQua.Add(new OrderLine
                        {
                            ProductId = Convert.ToInt32(boDoc["ProductId"]),
                            Name = SqlDb.StringValue(boDoc, "Name"),
                            Size = SqlDb.StringValue(boDoc, "Size"),
                            Price = Convert.ToDecimal(boDoc["Price"]),
                            Quantity = Convert.ToInt32(boDoc["Quantity"])
                        });
                    }
                }
            }

            return ketQua;
        }

        private static void ThemDongDonHang(SqlConnection ketNoi,
            SqlTransaction giaoDich, int maDonHang, IEnumerable<OrderLine> cacDong)
        {
            const string cauLenh = @"
                INSERT INTO OrderLines
                    (OrderId,ProductId,Name,Size,Price,Quantity)
                VALUES
                    (@order,@product,@name,@size,@price,@quantity)";

            foreach (OrderLine dong in cacDong ?? Enumerable.Empty<OrderLine>())
            {
                using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@order", maDonHang);
                    lenh.Parameters.AddWithValue("@product", dong.ProductId);
                    lenh.Parameters.AddWithValue("@name", dong.Name ?? "");
                    lenh.Parameters.AddWithValue("@size", dong.Size ?? "");
                    lenh.Parameters.AddWithValue("@price", dong.Price);
                    lenh.Parameters.AddWithValue("@quantity", dong.Quantity);
                    lenh.ExecuteNonQuery();
                }
            }
        }

        private static void ThemThamSoDonHang(SqlCommand lenh, Order donHang)
        {
            lenh.Parameters.AddWithValue("@code", donHang.Code ?? Order.NewCode());
            lenh.Parameters.AddWithValue("@created", donHang.CreatedAt);
            lenh.Parameters.AddWithValue("@username", donHang.Username ?? "");
            lenh.Parameters.AddWithValue("@status", donHang.Status ?? Order.PendingStatus);
            lenh.Parameters.AddWithValue("@name", donHang.CustomerName ?? "");
            lenh.Parameters.AddWithValue("@phone", donHang.Phone ?? "");
            lenh.Parameters.AddWithValue("@address", donHang.Address ?? "");
            lenh.Parameters.AddWithValue("@payment", donHang.PaymentMethod ?? "");
        }

        private static Order ChuyenDonHang(SqlDataReader boDoc)
        {
            return new Order
            {
                Code = SqlDb.StringValue(boDoc, "Code"),
                CreatedAt = Convert.ToDateTime(boDoc["CreatedAt"]),
                Username = SqlDb.StringValue(boDoc, "Username"),
                Status = SqlDb.StringValue(boDoc, "Status"),
                CustomerName = SqlDb.StringValue(boDoc, "CustomerName"),
                Phone = SqlDb.StringValue(boDoc, "Phone"),
                Address = SqlDb.StringValue(boDoc, "Address"),
                PaymentMethod = SqlDb.StringValue(boDoc, "PaymentMethod"),
                Lines = new List<OrderLine>()
            };
        }
    }

    public sealed class SqlCatalogStore
    {
        private const string CotSanPham =
            "Id,Name,Brand,Description,Price,OldPrice,Stock,Sizes,ImageUrl,ImageUrlLarge,IsHot";

        // Tìm sản phẩm theo mã.
        public ProductItem Find(int maSanPham)
        {
            if (maSanPham <= 0) return null;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT TOP 1 " + CotSanPham + " FROM Products WHERE Id=@id",
                ketNoi))
            {
                lenh.Parameters.AddWithValue("@id", maSanPham);
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    if (!boDoc.Read()) return null;
                    ProductItem sanPham = ChuyenSanPham(boDoc);
                    boDoc.Close();
                    sanPham.Colourways = DocMauSac(ketNoi, maSanPham);
                    return sanPham;
                }
            }
        }

        // Lấy toàn bộ sản phẩm theo mã tăng dần.
        public List<ProductItem> All()
        {
            List<ProductItem> ketQua = new List<ProductItem>();
            using (SqlConnection ketNoi = SqlDb.Open())
            {
                using (SqlCommand lenh = new SqlCommand(
                    "SELECT " + CotSanPham + " FROM Products ORDER BY Id", ketNoi))
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read()) ketQua.Add(ChuyenSanPham(boDoc));
                }

                GanMauSac(ketNoi, ketQua);
            }

            return ketQua;
        }

        // Thêm hoặc cập nhật sản phẩm và màu sắc trong một giao dịch.
        public SaveProductOutcome Save(ProductItem sanPham)
        {
            if (sanPham == null) return SaveProductOutcome.InvalidName;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
            {
                if (sanPham.Id <= 0)
                {
                    sanPham.Id = ThemSanPham(ketNoi, giaoDich, sanPham);
                    ThemMauSac(ketNoi, giaoDich, sanPham.Id, sanPham.Colourways);
                    giaoDich.Commit();
                    return SaveProductOutcome.Created;
                }

                if (!SanPhamTonTai(ketNoi, giaoDich, sanPham.Id))
                {
                    giaoDich.Rollback();
                    return SaveProductOutcome.NotFound;
                }

                CapNhatSanPham(ketNoi, giaoDich, sanPham);
                // Null nghĩa là form không chỉnh màu, cần giữ dữ liệu cũ.
                if (sanPham.Colourways != null)
                {
                    XoaMauSac(ketNoi, giaoDich, sanPham.Id);
                    ThemMauSac(ketNoi, giaoDich, sanPham.Id, sanPham.Colourways);
                }
                giaoDich.Commit();
                return SaveProductOutcome.Updated;
            }
        }

        // Xóa sản phẩm, màu sắc được xóa theo khóa ngoại.
        public bool Delete(int maSanPham)
        {
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "DELETE FROM Products WHERE Id=@id", ketNoi))
            {
                lenh.Parameters.AddWithValue("@id", maSanPham);
                return lenh.ExecuteNonQuery() > 0;
            }
        }

        // Trừ tồn kho nguyên tử cho các sản phẩm trong cùng một giao dịch.
        public bool TakeStock(IEnumerable<KeyValuePair<int, int>> duLieuVao)
        {
            Dictionary<int, int> canDung = GomSoLuong(duLieuVao);
            if (canDung.Count == 0) return true;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
            {
                foreach (KeyValuePair<int, int> phanTu in canDung)
                {
                    using (SqlCommand lenh = new SqlCommand(
                        "UPDATE Products SET Stock=Stock-@quantity "
                        + "WHERE Id=@id AND Stock>=@quantity", ketNoi, giaoDich))
                    {
                        lenh.Parameters.AddWithValue("@quantity", phanTu.Value);
                        lenh.Parameters.AddWithValue("@id", phanTu.Key);
                        if (lenh.ExecuteNonQuery() != 1)
                        {
                            giaoDich.Rollback();
                            return false;
                        }
                    }
                }

                giaoDich.Commit();
                return true;
            }
        }

        // Hoàn lại tồn kho khi đơn hàng bị hủy.
        public void RestoreStock(IEnumerable<KeyValuePair<int, int>> duLieuVao)
        {
            Dictionary<int, int> canHoan = GomSoLuong(duLieuVao);
            if (canHoan.Count == 0) return;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
            {
                foreach (KeyValuePair<int, int> phanTu in canHoan)
                {
                    using (SqlCommand lenh = new SqlCommand(
                        "UPDATE Products SET Stock=Stock+@quantity WHERE Id=@id",
                        ketNoi, giaoDich))
                    {
                        lenh.Parameters.AddWithValue("@quantity", phanTu.Value);
                        lenh.Parameters.AddWithValue("@id", phanTu.Key);
                        lenh.ExecuteNonQuery();
                    }
                }

                giaoDich.Commit();
            }
        }

        private static int ThemSanPham(SqlConnection ketNoi,
            SqlTransaction giaoDich, ProductItem sanPham)
        {
            const string cauLenh = @"
                INSERT INTO Products
                    (Name,Brand,Description,Price,OldPrice,Stock,Sizes,ImageUrl,ImageUrlLarge,IsHot)
                OUTPUT INSERTED.Id
                VALUES
                    (@name,@brand,@description,@price,@oldprice,@stock,@sizes,@image,@large,@ishot)";

            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi, giaoDich))
            {
                ThemThamSoSanPham(lenh, sanPham);
                return Convert.ToInt32(lenh.ExecuteScalar());
            }
        }

        private static void CapNhatSanPham(SqlConnection ketNoi,
            SqlTransaction giaoDich, ProductItem sanPham)
        {
            const string cauLenh = @"
                UPDATE Products SET
                    Name=@name, Brand=@brand, Description=@description,
                    Price=@price, OldPrice=@oldprice, Stock=@stock,
                    Sizes=@sizes, ImageUrl=@image, ImageUrlLarge=@large,
                    IsHot=@ishot
                WHERE Id=@id";

            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi, giaoDich))
            {
                ThemThamSoSanPham(lenh, sanPham);
                lenh.Parameters.AddWithValue("@id", sanPham.Id);
                lenh.ExecuteNonQuery();
            }
        }

        private static void ThemThamSoSanPham(SqlCommand lenh, ProductItem sanPham)
        {
            lenh.Parameters.AddWithValue("@name", sanPham.Name ?? "");
            lenh.Parameters.AddWithValue("@brand", sanPham.Brand ?? "");
            lenh.Parameters.AddWithValue("@description",
                (object)sanPham.Description ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@price", sanPham.Price);
            lenh.Parameters.AddWithValue("@oldprice",
                (object)sanPham.OldPrice ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@stock", sanPham.Stock);
            lenh.Parameters.AddWithValue("@sizes", sanPham.Sizes ?? "");
            lenh.Parameters.AddWithValue("@image", sanPham.ImageUrl ?? "");
            lenh.Parameters.AddWithValue("@large",
                (object)sanPham.ImageUrlLarge ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ishot", sanPham.IsHot);
        }

        private static bool SanPhamTonTai(SqlConnection ketNoi,
            SqlTransaction giaoDich, int maSanPham)
        {
            using (SqlCommand lenh = new SqlCommand(
                "SELECT COUNT(*) FROM Products WHERE Id=@id", ketNoi, giaoDich))
            {
                lenh.Parameters.AddWithValue("@id", maSanPham);
                return Convert.ToInt32(lenh.ExecuteScalar()) > 0;
            }
        }

        private static void GanMauSac(SqlConnection ketNoi,
            List<ProductItem> danhSach)
        {
            if (danhSach.Count == 0) return;

            const string cauLenh = @"
                SELECT ProductId,Name,Small,Large
                FROM ProductColourways
                ORDER BY ProductId,Name";
            Dictionary<int, List<ProductColourway>> mauSac =
                new Dictionary<int, List<ProductColourway>>();

            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi))
            using (SqlDataReader boDoc = lenh.ExecuteReader())
            {
                while (boDoc.Read())
                {
                    int maSanPham = Convert.ToInt32(boDoc["ProductId"]);
                    if (!mauSac.ContainsKey(maSanPham))
                        mauSac[maSanPham] = new List<ProductColourway>();
                    mauSac[maSanPham].Add(ChuyenMauSac(boDoc));
                }
            }

            foreach (ProductItem sanPham in danhSach)
            {
                List<ProductColourway> danhSachMau;
                sanPham.Colourways = mauSac.TryGetValue(sanPham.Id,
                    out danhSachMau)
                    ? danhSachMau
                    : new List<ProductColourway>();
            }
        }

        private static List<ProductColourway> DocMauSac(SqlConnection ketNoi,
            int maSanPham)
        {
            List<ProductColourway> ketQua = new List<ProductColourway>();
            using (SqlCommand lenh = new SqlCommand(
                "SELECT Name,Small,Large FROM ProductColourways "
                + "WHERE ProductId=@id ORDER BY Name", ketNoi))
            {
                lenh.Parameters.AddWithValue("@id", maSanPham);
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read()) ketQua.Add(ChuyenMauSac(boDoc));
                }
            }

            return ketQua;
        }

        private static void XoaMauSac(SqlConnection ketNoi,
            SqlTransaction giaoDich, int maSanPham)
        {
            using (SqlCommand lenh = new SqlCommand(
                "DELETE FROM ProductColourways WHERE ProductId=@id",
                ketNoi, giaoDich))
            {
                lenh.Parameters.AddWithValue("@id", maSanPham);
                lenh.ExecuteNonQuery();
            }
        }

        private static void ThemMauSac(SqlConnection ketNoi,
            SqlTransaction giaoDich, int maSanPham,
            IEnumerable<ProductColourway> danhSach)
        {
            const string cauLenh = @"
                INSERT INTO ProductColourways(ProductId,Name,Small,Large)
                VALUES(@id,@name,@small,@large)";

            foreach (ProductColourway mau in danhSach ?? Enumerable.Empty<ProductColourway>())
            {
                using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@id", maSanPham);
                    lenh.Parameters.AddWithValue("@name", mau.Name ?? "");
                    lenh.Parameters.AddWithValue("@small", mau.Small ?? "");
                    lenh.Parameters.AddWithValue("@large", mau.Large ?? "");
                    lenh.ExecuteNonQuery();
                }
            }
        }

        private static Dictionary<int, int> GomSoLuong(
            IEnumerable<KeyValuePair<int, int>> duLieuVao)
        {
            Dictionary<int, int> ketQua = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> phanTu in
                duLieuVao ?? Enumerable.Empty<KeyValuePair<int, int>>())
            {
                if (phanTu.Key <= 0 || phanTu.Value <= 0) continue;
                ketQua[phanTu.Key] = ketQua.ContainsKey(phanTu.Key)
                    ? ketQua[phanTu.Key] + phanTu.Value
                    : phanTu.Value;
            }

            return ketQua;
        }

        private static ProductItem ChuyenSanPham(SqlDataReader boDoc)
        {
            return new ProductItem
            {
                Id = Convert.ToInt32(boDoc["Id"]),
                Name = SqlDb.StringValue(boDoc, "Name"),
                Brand = SqlDb.StringValue(boDoc, "Brand"),
                Description = SqlDb.StringValue(boDoc, "Description"),
                Price = Convert.ToDecimal(boDoc["Price"]),
                OldPrice = boDoc["OldPrice"] == DBNull.Value
                    ? (decimal?)null
                    : Convert.ToDecimal(boDoc["OldPrice"]),
                Stock = Convert.ToInt32(boDoc["Stock"]),
                Sizes = SqlDb.StringValue(boDoc, "Sizes"),
                ImageUrl = SqlDb.StringValue(boDoc, "ImageUrl"),
                ImageUrlLarge = SqlDb.StringValue(boDoc, "ImageUrlLarge"),
                IsHot = Convert.ToBoolean(boDoc["IsHot"]),
                Colourways = new List<ProductColourway>()
            };
        }

        private static ProductColourway ChuyenMauSac(SqlDataReader boDoc)
        {
            return new ProductColourway
            {
                Name = SqlDb.StringValue(boDoc, "Name"),
                Small = SqlDb.StringValue(boDoc, "Small"),
                Large = SqlDb.StringValue(boDoc, "Large")
            };
        }
    }

    public sealed class SqlReviewStore
    {
        private const string CotDanhGia =
            "ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt";

        // Lấy đánh giá của một sản phẩm.
        public List<ProductReview> GetByProduct(int maSanPham)
        {
            List<ProductReview> ketQua = new List<ProductReview>();
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT " + CotDanhGia + " FROM ProductReviews "
                + "WHERE ProductId=@id ORDER BY CreatedAt DESC", ketNoi))
            {
                lenh.Parameters.AddWithValue("@id", maSanPham);
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read()) ketQua.Add(ChuyenDanhGia(boDoc));
                }
            }

            return ketQua;
        }

        // Tìm đánh giá của một người cho một sản phẩm.
        public ProductReview Find(int maSanPham, string khoaNguoiDung)
        {
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT TOP 1 " + CotDanhGia + " FROM ProductReviews "
                + "WHERE ProductId=@id AND UserKey=@user", ketNoi))
            {
                lenh.Parameters.AddWithValue("@id", maSanPham);
                lenh.Parameters.AddWithValue("@user", khoaNguoiDung ?? "");
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                    return boDoc.Read() ? ChuyenDanhGia(boDoc) : null;
            }
        }

        // Cập nhật đánh giá cũ hoặc thêm đánh giá mới.
        public void Save(ProductReview danhGia)
        {
            if (danhGia == null) return;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
            {
                const string cauCapNhat = @"
                    UPDATE ProductReviews SET
                        CustomerName=@name, Rating=@rating,
                        Comment=@comment, CreatedAt=@created
                    WHERE ProductId=@id AND UserKey=@user";

                int soDong;
                using (SqlCommand lenh = new SqlCommand(
                    cauCapNhat, ketNoi, giaoDich))
                {
                    ThemThamSoDanhGia(lenh, danhGia);
                    soDong = lenh.ExecuteNonQuery();
                }

                if (soDong == 0)
                {
                    const string cauThem = @"
                        INSERT INTO ProductReviews
                            (ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt)
                        VALUES
                            (@id,@user,@name,@rating,@comment,@created)";
                    using (SqlCommand lenh = new SqlCommand(
                        cauThem, ketNoi, giaoDich))
                    {
                        ThemThamSoDanhGia(lenh, danhGia);
                        lenh.ExecuteNonQuery();
                    }
                }

                giaoDich.Commit();
            }
        }

        // Tính điểm đánh giá trung bình.
        public double Average(int maSanPham)
        {
            List<ProductReview> danhSach = GetByProduct(maSanPham);
            return danhSach.Count == 0 ? 0 : danhSach.Average(x => x.Rating);
        }

        private static void ThemThamSoDanhGia(SqlCommand lenh,
            ProductReview danhGia)
        {
            lenh.Parameters.AddWithValue("@id", danhGia.ProductId);
            lenh.Parameters.AddWithValue("@user", danhGia.UserKey ?? "");
            lenh.Parameters.AddWithValue("@name", danhGia.CustomerName ?? "");
            lenh.Parameters.AddWithValue("@rating", danhGia.Rating);
            lenh.Parameters.AddWithValue("@comment", danhGia.Comment ?? "");
            lenh.Parameters.AddWithValue("@created",
                danhGia.CreatedAt == default(DateTime)
                    ? DateTime.Now
                    : danhGia.CreatedAt);
        }

        private static ProductReview ChuyenDanhGia(SqlDataReader boDoc)
        {
            return new ProductReview
            {
                ProductId = Convert.ToInt32(boDoc["ProductId"]),
                UserKey = SqlDb.StringValue(boDoc, "UserKey"),
                CustomerName = SqlDb.StringValue(boDoc, "CustomerName"),
                Rating = Convert.ToInt32(boDoc["Rating"]),
                Comment = SqlDb.StringValue(boDoc, "Comment"),
                CreatedAt = Convert.ToDateTime(boDoc["CreatedAt"])
            };
        }
    }

    public sealed class SqlCartStore
    {
        // Tải giỏ hàng của một tài khoản.
        public List<CartItem> Load(string tenDangNhap)
        {
            List<CartItem> ketQua = new List<CartItem>();
            const string cauLenh = @"
                SELECT ProductId,Name,Size,Price,Quantity,ImageUrl
                FROM CartItems WHERE Username=@user ORDER BY ProductId,Size";

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(cauLenh, ketNoi))
            {
                lenh.Parameters.AddWithValue("@user", tenDangNhap ?? "");
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read())
                    {
                        ketQua.Add(new CartItem
                        {
                            Id = Convert.ToInt32(boDoc["ProductId"]),
                            Name = SqlDb.StringValue(boDoc, "Name"),
                            Size = SqlDb.StringValue(boDoc, "Size"),
                            Price = Convert.ToDecimal(boDoc["Price"]),
                            Quantity = Convert.ToInt32(boDoc["Quantity"]),
                            ImageUrl = SqlDb.StringValue(boDoc, "ImageUrl")
                        });
                    }
                }
            }

            return ketQua;
        }

        // Thay toàn bộ giỏ hàng trong một giao dịch.
        public void Save(string tenDangNhap, IEnumerable<CartItem> cacDong)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
            {
                using (SqlCommand lenh = new SqlCommand(
                    "DELETE FROM CartItems WHERE Username=@user",
                    ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@user", tenDangNhap);
                    lenh.ExecuteNonQuery();
                }

                const string cauLenh = @"
                    INSERT INTO CartItems
                        (Username,ProductId,Size,Name,Price,Quantity,ImageUrl)
                    VALUES
                        (@user,@product,@size,@name,@price,@quantity,@image)";
                foreach (CartItem dong in cacDong ?? Enumerable.Empty<CartItem>())
                {
                    using (SqlCommand lenh = new SqlCommand(
                        cauLenh, ketNoi, giaoDich))
                    {
                        lenh.Parameters.AddWithValue("@user", tenDangNhap);
                        lenh.Parameters.AddWithValue("@product", dong.Id);
                        lenh.Parameters.AddWithValue("@size", dong.Size ?? "");
                        lenh.Parameters.AddWithValue("@name", dong.Name ?? "");
                        lenh.Parameters.AddWithValue("@price", dong.Price);
                        lenh.Parameters.AddWithValue("@quantity", dong.Quantity);
                        lenh.Parameters.AddWithValue("@image",
                            (object)dong.ImageUrl ?? DBNull.Value);
                        lenh.ExecuteNonQuery();
                    }
                }

                giaoDich.Commit();
            }
        }
    }

    public sealed class SqlWishlistStore
    {
        // Tải danh sách mã sản phẩm yêu thích.
        public HashSet<int> Load(string tenDangNhap)
        {
            HashSet<int> ketQua = new HashSet<int>();
            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand(
                "SELECT ProductId FROM Wishlists WHERE Username=@user", ketNoi))
            {
                lenh.Parameters.AddWithValue("@user", tenDangNhap ?? "");
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read())
                        ketQua.Add(Convert.ToInt32(boDoc["ProductId"]));
                }
            }

            return ketQua;
        }

        // Thay toàn bộ danh sách yêu thích trong một giao dịch.
        public void Save(string tenDangNhap, IEnumerable<int> maSanPham)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return;

            using (SqlConnection ketNoi = SqlDb.Open())
            using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
            {
                using (SqlCommand lenh = new SqlCommand(
                    "DELETE FROM Wishlists WHERE Username=@user",
                    ketNoi, giaoDich))
                {
                    lenh.Parameters.AddWithValue("@user", tenDangNhap);
                    lenh.ExecuteNonQuery();
                }

                foreach (int maTrongDanhSach in maSanPham ?? Enumerable.Empty<int>())
                {
                    using (SqlCommand lenh = new SqlCommand(
                        "INSERT INTO Wishlists(Username,ProductId) "
                        + "VALUES(@user,@product)", ketNoi, giaoDich))
                    {
                        lenh.Parameters.AddWithValue("@user", tenDangNhap);
                        lenh.Parameters.AddWithValue("@product", maTrongDanhSach);
                        lenh.ExecuteNonQuery();
                    }
                }

                giaoDich.Commit();
            }
        }
    }

    public static class SqlDatabase
    {
        // Tạo dữ liệu mẫu nếu cơ sở dữ liệu đang trống.
        public static void SeedIfEmpty()
        {
            bool chuaCoSanPham;
            bool chuaCoNguoiDung;
            using (SqlConnection ketNoi = SqlDb.Open())
            {
                chuaCoSanPham = Dem(ketNoi, "Products") == 0;
                chuaCoNguoiDung = Dem(ketNoi, "Users") == 0;
            }

            if (chuaCoSanPham)
            {
                SqlCatalogStore khoSanPham = new SqlCatalogStore();
                foreach (ProductItem sanPham in ProductSeed.Items())
                {
                    sanPham.Id = 0;
                    khoSanPham.Save(sanPham);
                }
            }

            if (chuaCoNguoiDung)
            {
                SqlUserStore khoNguoiDung = new SqlUserStore();
                foreach (UserAccount taiKhoan in MemoryUserStore.Seed())
                    khoNguoiDung.TryAdd(taiKhoan);
            }
        }

        private static int Dem(SqlConnection ketNoi, string tenBang)
        {
            using (SqlCommand lenh = new SqlCommand(
                "SELECT COUNT(*) FROM " + tenBang, ketNoi))
            {
                return Convert.ToInt32(lenh.ExecuteScalar());
            }
        }
    }
}
