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
    internal static class SqlDb
    {
        // Thực hiện việc mở.
        public static SqlConnection Open()
        {
            ConnectionStringSettings cauHinh = ConfigurationManager.ConnectionStrings["ShoeStoreDb"];
            if (cauHinh == null || string.IsNullOrWhiteSpace(cauHinh.ConnectionString)) throw new ConfigurationErrorsException("Thiếu connectionStrings/ShoeStoreDb trong Web.config.");
            SqlConnection ketNoi = new SqlConnection(cauHinh.ConnectionString);
            ketNoi.Open();
            return ketNoi;
        }

        // Tạo tham số SQL.
        public static SqlParameter Parameter(string ten, object giaTri)
        {
            return new SqlParameter(ten, giaTri ?? DBNull.Value);
        }
    }

    public sealed class SqlUserStore : IUserStore
    {
        // Thực hiện việc lấy toàn bộ.
        public List<UserAccount> GetAll()
        {
            List<UserAccount> ketQua = new List<UserAccount>();
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("SELECT Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin FROM Users ORDER BY Username", coSoDuLieu))
            using (SqlDataReader boDoc = lenh.ExecuteReader())
            {
                while (boDoc.Read()) ketQua.Add(Read(boDoc));
            }
            return ketQua;
        }

        // Tìm theo tên đăng nhập hoặc email.
        public UserAccount FindByUsernameOrEmail(string giaTri)
        {
            return One("WHERE Username=@value OR Email=@value", SqlDb.Parameter("@value", giaTri));
        }

        // Thực hiện việc tìm theo tên đăng nhập.
        public UserAccount FindByUsername(string tenDangNhap)
        {
            return One("WHERE Username=@value", SqlDb.Parameter("@value", tenDangNhap));
        }

        // Thực hiện việc tên đăng nhập tồn tại.
        public bool UsernameExists(string tenDangNhap) { return Exists("Username", tenDangNhap); }
        // Thực hiện việc email tồn tại.
        public bool EmailExists(string email) { return Exists("Email", email); }

        // Thực hiện việc thử thêm.
        public AddUserOutcome TryAdd(UserAccount taiKhoan)
        {
            if (taiKhoan == null) return AddUserOutcome.UsernameTaken;
            if (UsernameExists(taiKhoan.Username)) return AddUserOutcome.UsernameTaken;
            if (EmailExists(taiKhoan.Email)) return AddUserOutcome.EmailTaken;
            try
            {
                using (SqlConnection coSoDuLieu = SqlDb.Open())
                using (SqlCommand lenh = new SqlCommand("INSERT INTO Users(Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin) VALUES(@username,@name,@email,@phone,@hash,@salt,@admin)", coSoDuLieu))
                {
                    lenh.Parameters.AddWithValue("@username", taiKhoan.Username ?? "");
                    lenh.Parameters.AddWithValue("@name", (object)taiKhoan.FullName ?? DBNull.Value);
                    lenh.Parameters.AddWithValue("@email", taiKhoan.Email ?? "");
                    lenh.Parameters.AddWithValue("@phone", (object)taiKhoan.Phone ?? DBNull.Value);
                    lenh.Parameters.AddWithValue("@hash", taiKhoan.PasswordHash ?? "");
                    lenh.Parameters.AddWithValue("@salt", taiKhoan.PasswordSalt ?? "");
                    lenh.Parameters.AddWithValue("@admin", taiKhoan.IsAdmin);
                    lenh.ExecuteNonQuery();
                }
                return AddUserOutcome.Added;
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return UsernameExists(taiKhoan.Username) ? AddUserOutcome.UsernameTaken : AddUserOutcome.EmailTaken;
            }
        }

        // Thực hiện việc thử cập nhật hồ sơ.
        public ProfileUpdateOutcome TryUpdateProfile(string tenDangNhap, string hoTen, string email, string soDienThoai)
        {
            if (FindByUsername(tenDangNhap) == null) return ProfileUpdateOutcome.NotFound;
            if (EmailExistsForOther(email, tenDangNhap)) return ProfileUpdateOutcome.EmailTaken;
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("UPDATE Users SET FullName=@name,Email=@email,Phone=@phone WHERE Username=@username", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@name", (object)hoTen ?? DBNull.Value);
                lenh.Parameters.AddWithValue("@email", email ?? "");
                lenh.Parameters.AddWithValue("@phone", (object)soDienThoai ?? DBNull.Value);
                lenh.Parameters.AddWithValue("@username", tenDangNhap ?? "");
                lenh.ExecuteNonQuery();
            }
            return ProfileUpdateOutcome.Updated;
        }

        // Thực hiện việc thử thay đổi mật khẩu.
        public PasswordChangeOutcome TryChangePassword(string tenDangNhap, string matKhauHienTai, string matKhauMoi)
        {
            UserAccount taiKhoan = FindByUsername(tenDangNhap);
            if (taiKhoan == null) return PasswordChangeOutcome.NotFound;
            if (!PasswordHasher.Verify(matKhauHienTai, taiKhoan.PasswordSalt, taiKhoan.PasswordHash)) return PasswordChangeOutcome.WrongPassword;
            string muoi = PasswordHasher.NewSalt();
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("UPDATE Users SET PasswordHash=@hash,PasswordSalt=@salt WHERE Username=@username", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@hash", PasswordHasher.Hash(matKhauMoi, muoi));
                lenh.Parameters.AddWithValue("@salt", muoi);
                lenh.Parameters.AddWithValue("@username", tenDangNhap);
                lenh.ExecuteNonQuery();
            }
            return PasswordChangeOutcome.Changed;
        }

        // Thực hiện việc thử đặt quản trị.
        public RoleChangeOutcome TrySetAdmin(string tenDangNhap, bool laQuanTri)
        {
            UserAccount taiKhoan = FindByUsername(tenDangNhap);
            if (taiKhoan == null) return RoleChangeOutcome.NotFound;
            if (taiKhoan.IsAdmin == laQuanTri) return RoleChangeOutcome.Unchanged;
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlTransaction giaoDich = coSoDuLieu.BeginTransaction(IsolationLevel.Serializable))
            {
                if (!laQuanTri)
                {
                    using (SqlCommand soLuong = new SqlCommand("SELECT COUNT(*) FROM Users WHERE IsAdmin=1", coSoDuLieu, giaoDich))
                    {
                        if ((int)soLuong.ExecuteScalar() <= 1) { giaoDich.Rollback(); return RoleChangeOutcome.LastAdmin; }
                    }
                }
                using (SqlCommand lenhCapNhat = new SqlCommand("UPDATE Users SET IsAdmin=@admin WHERE Username=@username", coSoDuLieu, giaoDich))
                {
                    lenhCapNhat.Parameters.AddWithValue("@admin", laQuanTri);
                    lenhCapNhat.Parameters.AddWithValue("@username", tenDangNhap);
                    lenhCapNhat.ExecuteNonQuery();
                }
                giaoDich.Commit();
            }
            return RoleChangeOutcome.Changed;
        }

        // Xử lý sự kiện vòng đời của điều khiển.
        private UserAccount One(string dieuKien, params SqlParameter[] thamSo)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("SELECT TOP 1 Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin FROM Users " + dieuKien, coSoDuLieu))
            {
                lenh.Parameters.AddRange(thamSo);
                using (SqlDataReader boDoc = lenh.ExecuteReader()) return boDoc.Read() ? Read(boDoc) : null;
            }
        }

        // Thực hiện việc tồn tại.
        private bool Exists(string cot, string giaTri)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("SELECT COUNT(*) FROM Users WHERE " + cot + "=@value", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@value", giaTri ?? "");
                return (int)lenh.ExecuteScalar() > 0;
            }
        }

        // Thực hiện việc email tồn tại for khác.
        private bool EmailExistsForOther(string email, string tenDangNhap)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email=@email AND Username<>@username", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@email", email ?? "");
                lenh.Parameters.AddWithValue("@username", tenDangNhap ?? "");
                return (int)lenh.ExecuteScalar() > 0;
            }
        }

        // Thực hiện việc đọc.
        private static UserAccount Read(SqlDataReader boDoc)
        {
            return new UserAccount
            {
                Username = boDoc["Username"].ToString(),
                FullName = boDoc["FullName"] as string,
                Email = boDoc["Email"].ToString(),
                Phone = boDoc["Phone"] as string,
                PasswordHash = boDoc["PasswordHash"].ToString(),
                PasswordSalt = boDoc["PasswordSalt"].ToString(),
                IsAdmin = Convert.ToBoolean(boDoc["IsAdmin"])
            };
        }
    }

    public sealed class SqlOrderStore : IOrderStore
    {
        // Thực hiện việc lưu.
        public void Save(Order donHang)
        {
            if (donHang == null) return;
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlTransaction giaoDich = coSoDuLieu.BeginTransaction())
            {
                int ma;
                using (SqlCommand lenh = new SqlCommand("INSERT INTO Orders(Code,CreatedAt,Username,Status,CustomerName,Phone,Address,PaymentMethod) OUTPUT INSERTED.Id VALUES(@code,@created,@username,@status,@name,@phone,@address,@payment)", coSoDuLieu, giaoDich))
                {
                    AddOrderParameters(lenh, donHang);
                    ma = (int)lenh.ExecuteScalar();
                }
                InsertLines(coSoDuLieu, giaoDich, ma, donHang.Lines);
                giaoDich.Commit();
            }
        }

        // Thực hiện việc toàn bộ.
        public List<Order> All() { return ReadOrders("", null); }
        // Thực hiện việc theo người dùng.
        public List<Order> ByUser(string tenDangNhap) { return string.IsNullOrEmpty(tenDangNhap) ? new List<Order>() : ReadOrders(" WHERE Username=@username", SqlDb.Parameter("@username", tenDangNhap)); }
        // Thực hiện việc tìm theo mã.
        public Order FindByCode(string ma) { return string.IsNullOrWhiteSpace(ma) ? null : ReadOrders(" WHERE Code=@code", SqlDb.Parameter("@code", ma.Trim())).FirstOrDefault(); }

        // Thực hiện việc cập nhật trạng thái.
        public void UpdateStatus(Order donHang)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("UPDATE Orders SET Status=@status WHERE Code=@code", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@status", donHang.Status);
                lenh.Parameters.AddWithValue("@code", donHang.Code);
                lenh.ExecuteNonQuery();
            }
        }

        // Thực hiện việc thêm đơn hàng tham số.
        private static void AddOrderParameters(SqlCommand lenh, Order donHang)
        {
            lenh.Parameters.AddWithValue("@code", donHang.Code);
            lenh.Parameters.AddWithValue("@created", donHang.CreatedAt);
            lenh.Parameters.AddWithValue("@username", donHang.Username ?? "");
            lenh.Parameters.AddWithValue("@status", donHang.Status ?? Order.PendingStatus);
            lenh.Parameters.AddWithValue("@name", donHang.CustomerName ?? "");
            lenh.Parameters.AddWithValue("@phone", donHang.Phone ?? "");
            lenh.Parameters.AddWithValue("@address", donHang.Address ?? "");
            lenh.Parameters.AddWithValue("@payment", donHang.PaymentMethod ?? "");
        }

        // Thực hiện việc insert dòng.
        private static void InsertLines(SqlConnection coSoDuLieu, SqlTransaction giaoDich, int maDonHang, IEnumerable<OrderLine> cacDong)
        {
            foreach (OrderLine dong in cacDong ?? new List<OrderLine>())
            {
                using (SqlCommand lenh = new SqlCommand("INSERT INTO OrderLines(OrderId,ProductId,Name,Size,Price,Quantity) VALUES(@order,@product,@name,@size,@price,@quantity)", coSoDuLieu, giaoDich))
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

        // Thực hiện việc đọc đơn hàng.
        private static List<Order> ReadOrders(string boLoc, SqlParameter thamSo)
        {
            List<Order> ketQua = new List<Order>();
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("SELECT Id,Code,CreatedAt,Username,Status,CustomerName,Phone,Address,PaymentMethod FROM Orders" + boLoc + " ORDER BY CreatedAt DESC", coSoDuLieu))
            {
                if (thamSo != null) lenh.Parameters.Add(thamSo);
                using (SqlDataReader boDoc = lenh.ExecuteReader())
                {
                    while (boDoc.Read()) ketQua.Add(new Order { Code = boDoc["Code"].ToString(), CreatedAt = Convert.ToDateTime(boDoc["CreatedAt"]), Username = boDoc["Username"].ToString(), Status = boDoc["Status"].ToString(), CustomerName = boDoc["CustomerName"].ToString(), Phone = boDoc["Phone"].ToString(), Address = boDoc["Address"].ToString(), PaymentMethod = boDoc["PaymentMethod"].ToString(), Lines = new List<OrderLine>() });
                }
            }
            foreach (Order donHang in ketQua) donHang.Lines = ReadLines(donHang.Code);
            return ketQua;
        }

        // Thực hiện việc đọc dòng.
        private static List<OrderLine> ReadLines(string ma)
        {
            List<OrderLine> ketQua = new List<OrderLine>();
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("SELECT l.ProductId,l.Name,l.Size,l.Price,l.Quantity FROM OrderLines l INNER JOIN Orders o ON o.Id=l.OrderId WHERE o.Code=@code ORDER BY l.Id", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@code", ma);
                using (SqlDataReader boDoc = lenh.ExecuteReader()) while (boDoc.Read()) ketQua.Add(new OrderLine { ProductId = Convert.ToInt32(boDoc["ProductId"]), Name = boDoc["Name"].ToString(), Size = boDoc["Size"].ToString(), Price = Convert.ToDecimal(boDoc["Price"]), Quantity = Convert.ToInt32(boDoc["Quantity"]) });
            }
            return ketQua;
        }
    }

    public sealed class SqlCatalogStore
    {
        // Thực hiện việc tìm.
        public ProductItem Find(int ma) { return All().FirstOrDefault(x => x.Id == ma); }
        // Thực hiện việc toàn bộ.
        public List<ProductItem> All()
        {
            List<ProductItem> ketQua = new List<ProductItem>();
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            using (SqlCommand lenh = new SqlCommand("SELECT Id,Name,Brand,Description,Price,OldPrice,Stock,Sizes,ImageUrl,ImageUrlLarge,IsHot FROM Products ORDER BY Id", coSoDuLieu))
            using (SqlDataReader boDoc = lenh.ExecuteReader())
            {
                while (boDoc.Read()) ketQua.Add(new ProductItem { Id = Convert.ToInt32(boDoc["Id"]), Name = boDoc["Name"].ToString(), Brand = boDoc["Brand"].ToString(), Description = boDoc["Description"] as string, Price = Convert.ToDecimal(boDoc["Price"]), OldPrice = boDoc["OldPrice"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(boDoc["OldPrice"]), Stock = Convert.ToInt32(boDoc["Stock"]), Sizes = boDoc["Sizes"].ToString(), ImageUrl = boDoc["ImageUrl"].ToString(), ImageUrlLarge = boDoc["ImageUrlLarge"] as string, IsHot = Convert.ToBoolean(boDoc["IsHot"]) });
            }
            foreach (ProductItem sanPham in ketQua) sanPham.Colourways = ReadColourways(sanPham.Id);
            return ketQua;
        }

        // Thực hiện việc lưu.
        public SaveProductOutcome Save(ProductItem sanPham)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            {
                if (sanPham.Id <= 0)
                {
                    using (SqlCommand lenh = new SqlCommand("INSERT INTO Products(Name,Brand,Description,Price,OldPrice,Stock,Sizes,ImageUrl,ImageUrlLarge,IsHot) OUTPUT INSERTED.Id VALUES(@name,@brand,@description,@price,@old,@stock,@sizes,@image,@large,@hot)", coSoDuLieu))
                    {
                        AddProductParameters(lenh, sanPham);
                        sanPham.Id = Convert.ToInt32(lenh.ExecuteScalar());
                    }
                    SaveColourways(coSoDuLieu, sanPham.Id, sanPham.Colourways);
                    return SaveProductOutcome.Created;
                }
                ProductItem cu = Find(sanPham.Id);
                if (cu == null) return SaveProductOutcome.NotFound;
                using (SqlCommand lenh = new SqlCommand("UPDATE Products SET Name=@name,Brand=@brand,Description=@description,Price=@price,OldPrice=@old,Stock=@stock,Sizes=@sizes,ImageUrl=@image,ImageUrlLarge=@large WHERE Id=@id", coSoDuLieu))
                {
                    AddProductParameters(lenh, sanPham);
                    lenh.Parameters.AddWithValue("@id", sanPham.Id);
                    lenh.ExecuteNonQuery();
                }
                if (sanPham.Colourways != null || cu.ImageUrl != sanPham.ImageUrl || cu.ImageUrlLarge != sanPham.ImageUrlLarge)
                {
                    DeleteColourways(coSoDuLieu, sanPham.Id);
                    SaveColourways(coSoDuLieu, sanPham.Id, sanPham.Colourways);
                }
                return SaveProductOutcome.Updated;
            }
        }

        // Thực hiện việc xóa.
        public bool Delete(int ma)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlCommand lenh = new SqlCommand("DELETE FROM Products WHERE Id=@id", coSoDuLieu)) { lenh.Parameters.AddWithValue("@id", ma); return lenh.ExecuteNonQuery() > 0; }
        }

        // Thực hiện việc trừ stock.
        public bool TakeStock(IEnumerable<KeyValuePair<int, int>> duLieuVao)
        {
            Dictionary<int, int> canDung = Totals(duLieuVao);
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlTransaction giaoDich = coSoDuLieu.BeginTransaction())
            {
                foreach (KeyValuePair<int, int> phanTu in canDung)
                {
                    using (SqlCommand lenh = new SqlCommand("UPDATE Products SET Stock=Stock-@quantity WHERE Id=@id AND Stock>=@quantity", coSoDuLieu, giaoDich))
                    {
                        lenh.Parameters.AddWithValue("@quantity", phanTu.Value); lenh.Parameters.AddWithValue("@id", phanTu.Key);
                        if (lenh.ExecuteNonQuery() != 1) { giaoDich.Rollback(); return false; }
                    }
                }
                giaoDich.Commit(); return true;
            }
        }

        // Thực hiện việc hoàn lại stock.
        public void RestoreStock(IEnumerable<KeyValuePair<int, int>> duLieuVao)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlTransaction giaoDich = coSoDuLieu.BeginTransaction())
            {
                foreach (KeyValuePair<int, int> phanTu in Totals(duLieuVao)) using (SqlCommand lenh = new SqlCommand("UPDATE Products SET Stock=Stock+@quantity WHERE Id=@id", coSoDuLieu, giaoDich)) { lenh.Parameters.AddWithValue("@quantity", phanTu.Value); lenh.Parameters.AddWithValue("@id", phanTu.Key); lenh.ExecuteNonQuery(); }
                giaoDich.Commit();
            }
        }

        // Thực hiện việc totals.
        private static Dictionary<int, int> Totals(IEnumerable<KeyValuePair<int, int>> duLieuVao)
        {
            Dictionary<int, int> ketQua = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> phanTu in duLieuVao ?? new List<KeyValuePair<int, int>>()) if (phanTu.Value > 0) ketQua[phanTu.Key] = ketQua.ContainsKey(phanTu.Key) ? ketQua[phanTu.Key] + phanTu.Value : phanTu.Value;
            return ketQua;
        }

        // Thực hiện việc thêm sản phẩm tham số.
        private static void AddProductParameters(SqlCommand lenh, ProductItem sanPham)
        {
            lenh.Parameters.AddWithValue("@name", sanPham.Name ?? ""); lenh.Parameters.AddWithValue("@brand", sanPham.Brand ?? ""); lenh.Parameters.AddWithValue("@description", (object)sanPham.Description ?? DBNull.Value); lenh.Parameters.AddWithValue("@price", sanPham.Price); lenh.Parameters.AddWithValue("@old", (object)sanPham.OldPrice ?? DBNull.Value); lenh.Parameters.AddWithValue("@stock", sanPham.Stock); lenh.Parameters.AddWithValue("@sizes", sanPham.Sizes ?? ""); lenh.Parameters.AddWithValue("@image", sanPham.ImageUrl ?? ""); lenh.Parameters.AddWithValue("@large", (object)sanPham.ImageUrlLarge ?? DBNull.Value); lenh.Parameters.AddWithValue("@hot", sanPham.IsHot);
        }

        // Thực hiện việc tìm cơ bản.
        private static ProductItem FindBasic(SqlConnection coSoDuLieu, int ma)
        {
            using (SqlCommand lenh = new SqlCommand("SELECT Id FROM Products WHERE Id=@id", coSoDuLieu)) { lenh.Parameters.AddWithValue("@id", ma); return lenh.ExecuteScalar() == null ? null : new ProductItem { Id = ma }; }
        }

        // Thực hiện việc đọc màu sắc.
        private static List<ProductColourway> ReadColourways(int ma)
        {
            List<ProductColourway> ketQua = new List<ProductColourway>();
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlCommand lenh = new SqlCommand("SELECT Name,Small,Large FROM ProductColourways WHERE ProductId=@id ORDER BY Name", coSoDuLieu)) { lenh.Parameters.AddWithValue("@id", ma); using (SqlDataReader boDoc = lenh.ExecuteReader()) while (boDoc.Read()) ketQua.Add(new ProductColourway { Name = boDoc["Name"].ToString(), Small = boDoc["Small"].ToString(), Large = boDoc["Large"].ToString() }); }
            return ketQua;
        }

        // Thực hiện việc lưu màu sắc.
        private static void SaveColourways(SqlConnection coSoDuLieu, int ma, IEnumerable<ProductColourway> danhSach)
        {
            foreach (ProductColourway phanTu in danhSach ?? new List<ProductColourway>()) using (SqlCommand lenh = new SqlCommand("INSERT INTO ProductColourways(ProductId,Name,Small,Large) VALUES(@id,@name,@small,@large)", coSoDuLieu)) { lenh.Parameters.AddWithValue("@id", ma); lenh.Parameters.AddWithValue("@name", phanTu.Name ?? ""); lenh.Parameters.AddWithValue("@small", phanTu.Small ?? ""); lenh.Parameters.AddWithValue("@large", phanTu.Large ?? ""); lenh.ExecuteNonQuery(); }
        }

        // Thực hiện việc xóa màu sắc.
        private static void DeleteColourways(SqlConnection coSoDuLieu, int ma) { using (SqlCommand lenh = new SqlCommand("DELETE FROM ProductColourways WHERE ProductId=@id", coSoDuLieu)) { lenh.Parameters.AddWithValue("@id", ma); lenh.ExecuteNonQuery(); } }
    }

    public sealed class SqlReviewStore
    {
        // Thực hiện việc lấy theo sản phẩm.
        public List<ProductReview> GetByProduct(int ma)
        {
            List<ProductReview> ketQua = new List<ProductReview>();
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlCommand lenh = new SqlCommand("SELECT ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt FROM ProductReviews WHERE ProductId=@id ORDER BY CreatedAt DESC", coSoDuLieu)) { lenh.Parameters.AddWithValue("@id", ma); using (SqlDataReader boDoc = lenh.ExecuteReader()) while (boDoc.Read()) ketQua.Add(Read(boDoc)); }
            return ketQua;
        }
        // Thực hiện việc tìm.
        public ProductReview Find(int ma, string khoaNguoiDung)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlCommand lenh = new SqlCommand("SELECT TOP 1 ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt FROM ProductReviews WHERE ProductId=@id AND UserKey=@user", coSoDuLieu)) { lenh.Parameters.AddWithValue("@id", ma); lenh.Parameters.AddWithValue("@user", khoaNguoiDung ?? ""); using (SqlDataReader boDoc = lenh.ExecuteReader()) return boDoc.Read() ? Read(boDoc) : null; }
        }
        // Thực hiện việc lưu.
        public void Save(ProductReview danhGia)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlCommand lenh = new SqlCommand("UPDATE ProductReviews SET CustomerName=@name,Rating=@rating,Comment=@comment,CreatedAt=@created WHERE ProductId=@id AND UserKey=@user", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@name", danhGia.CustomerName ?? ""); lenh.Parameters.AddWithValue("@rating", danhGia.Rating); lenh.Parameters.AddWithValue("@comment", danhGia.Comment ?? ""); lenh.Parameters.AddWithValue("@created", DateTime.Now); lenh.Parameters.AddWithValue("@id", danhGia.ProductId); lenh.Parameters.AddWithValue("@user", danhGia.UserKey ?? "");
                if (lenh.ExecuteNonQuery() == 0) { lenh.CommandText = "INSERT INTO ProductReviews(ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt) VALUES(@id,@user,@name,@rating,@comment,@created)"; lenh.ExecuteNonQuery(); }
            }
        }
        // Thực hiện việc tính trung bình.
        public double Average(int ma) { List<ProductReview> danhSach = GetByProduct(ma); return danhSach.Count == 0 ? 0 : danhSach.Average(x => x.Rating); }
        // Thực hiện việc đọc.
        private static ProductReview Read(SqlDataReader boDoc) { return new ProductReview { ProductId = Convert.ToInt32(boDoc["ProductId"]), UserKey = boDoc["UserKey"].ToString(), CustomerName = boDoc["CustomerName"].ToString(), Rating = Convert.ToInt32(boDoc["Rating"]), Comment = boDoc["Comment"].ToString(), CreatedAt = Convert.ToDateTime(boDoc["CreatedAt"]) }; }
    }

    public sealed class SqlCartStore
    {
        // Thực hiện việc tải.
        public List<CartItem> Load(string tenDangNhap)
        {
            List<CartItem> ketQua = new List<CartItem>();
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlCommand lenh = new SqlCommand("SELECT ProductId,Name,Size,Price,Quantity,ImageUrl FROM CartItems WHERE Username=@user ORDER BY ProductId", coSoDuLieu))
            {
                lenh.Parameters.AddWithValue("@user", tenDangNhap);
                using (SqlDataReader boDoc = lenh.ExecuteReader()) while (boDoc.Read()) ketQua.Add(new CartItem { Id = Convert.ToInt32(boDoc["ProductId"]), Name = boDoc["Name"].ToString(), Size = boDoc["Size"].ToString(), Price = Convert.ToDecimal(boDoc["Price"]), Quantity = Convert.ToInt32(boDoc["Quantity"]), ImageUrl = boDoc["ImageUrl"] as string });
            }
            return ketQua;
        }

        // Thực hiện việc lưu.
        public void Save(string tenDangNhap, IEnumerable<CartItem> cacDong)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlTransaction giaoDich = coSoDuLieu.BeginTransaction())
            {
                using (SqlCommand lenhXoa = new SqlCommand("DELETE FROM CartItems WHERE Username=@user", coSoDuLieu, giaoDich)) { lenhXoa.Parameters.AddWithValue("@user", tenDangNhap); lenhXoa.ExecuteNonQuery(); }
                foreach (CartItem dong in cacDong ?? new List<CartItem>()) using (SqlCommand lenhThem = new SqlCommand("INSERT INTO CartItems(Username,ProductId,Size,Name,Price,Quantity,ImageUrl) VALUES(@user,@product,@size,@name,@price,@quantity,@image)", coSoDuLieu, giaoDich)) { lenhThem.Parameters.AddWithValue("@user", tenDangNhap); lenhThem.Parameters.AddWithValue("@product", dong.Id); lenhThem.Parameters.AddWithValue("@size", dong.Size ?? ""); lenhThem.Parameters.AddWithValue("@name", dong.Name ?? ""); lenhThem.Parameters.AddWithValue("@price", dong.Price); lenhThem.Parameters.AddWithValue("@quantity", dong.Quantity); lenhThem.Parameters.AddWithValue("@image", (object)dong.ImageUrl ?? DBNull.Value); lenhThem.ExecuteNonQuery(); }
                giaoDich.Commit();
            }
        }
    }

    public sealed class SqlWishlistStore
    {
        // Thực hiện việc tải.
        public HashSet<int> Load(string tenDangNhap)
        {
            HashSet<int> ketQua = new HashSet<int>();
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlCommand lenh = new SqlCommand("SELECT ProductId FROM Wishlists WHERE Username=@user", coSoDuLieu)) { lenh.Parameters.AddWithValue("@user", tenDangNhap); using (SqlDataReader boDoc = lenh.ExecuteReader()) while (boDoc.Read()) ketQua.Add(Convert.ToInt32(boDoc["ProductId"])); }
            return ketQua;
        }

        // Thực hiện việc lưu.
        public void Save(string tenDangNhap, IEnumerable<int> maSanPham)
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open()) using (SqlTransaction giaoDich = coSoDuLieu.BeginTransaction())
            {
                using (SqlCommand lenhXoa = new SqlCommand("DELETE FROM Wishlists WHERE Username=@user", coSoDuLieu, giaoDich)) { lenhXoa.Parameters.AddWithValue("@user", tenDangNhap); lenhXoa.ExecuteNonQuery(); }
                foreach (int ma in maSanPham ?? new List<int>()) using (SqlCommand lenhThem = new SqlCommand("INSERT INTO Wishlists(Username,ProductId) VALUES(@user,@product)", coSoDuLieu, giaoDich)) { lenhThem.Parameters.AddWithValue("@user", tenDangNhap); lenhThem.Parameters.AddWithValue("@product", ma); lenhThem.ExecuteNonQuery(); }
                giaoDich.Commit();
            }
        }
    }

    public static class SqlDatabase
    {
        // Thực hiện việc khởi tạo dữ liệu if empty.
        public static void SeedIfEmpty()
        {
            using (SqlConnection coSoDuLieu = SqlDb.Open())
            {
                int danhSachSanPham;
                using (SqlCommand lenh = new SqlCommand("SELECT COUNT(*) FROM Products", coSoDuLieu)) danhSachSanPham = (int)lenh.ExecuteScalar();
                if (danhSachSanPham == 0) foreach (ProductItem sanPham in ProductSeed.Items()) { sanPham.Id = 0; new SqlCatalogStore().Save(sanPham); }
                int nguoiDungs;
                using (SqlCommand lenh = new SqlCommand("SELECT COUNT(*) FROM Users", coSoDuLieu)) nguoiDungs = (int)lenh.ExecuteScalar();
                if (nguoiDungs == 0) foreach (UserAccount taiKhoan in MemoryUserStore.Seed()) new SqlUserStore().TryAdd(taiKhoan);
            }
        }
    }
}
