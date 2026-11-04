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
        public static SqlConnection Open()
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["ShoeStoreDb"];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString)) throw new ConfigurationErrorsException("Thiếu connectionStrings/ShoeStoreDb trong Web.config.");
            SqlConnection connection = new SqlConnection(setting.ConnectionString);
            connection.Open();
            return connection;
        }

        public static SqlParameter Parameter(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }
    }

    public sealed class SqlUserStore : IUserStore
    {
        public List<UserAccount> GetAll()
        {
            List<UserAccount> result = new List<UserAccount>();
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("SELECT Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin FROM Users ORDER BY Username", db))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read()) result.Add(Read(reader));
            }
            return result;
        }

        public UserAccount FindByUsernameOrEmail(string value)
        {
            return One("WHERE Username=@value OR Email=@value", SqlDb.Parameter("@value", value));
        }

        public UserAccount FindByUsername(string username)
        {
            return One("WHERE Username=@value", SqlDb.Parameter("@value", username));
        }

        public bool UsernameExists(string username) { return Exists("Username", username); }
        public bool EmailExists(string email) { return Exists("Email", email); }

        public AddUserOutcome TryAdd(UserAccount account)
        {
            if (account == null) return AddUserOutcome.UsernameTaken;
            if (UsernameExists(account.Username)) return AddUserOutcome.UsernameTaken;
            if (EmailExists(account.Email)) return AddUserOutcome.EmailTaken;
            try
            {
                using (SqlConnection db = SqlDb.Open())
                using (SqlCommand command = new SqlCommand("INSERT INTO Users(Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin) VALUES(@username,@name,@email,@phone,@hash,@salt,@admin)", db))
                {
                    command.Parameters.AddWithValue("@username", account.Username ?? "");
                    command.Parameters.AddWithValue("@name", (object)account.FullName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", account.Email ?? "");
                    command.Parameters.AddWithValue("@phone", (object)account.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@hash", account.PasswordHash ?? "");
                    command.Parameters.AddWithValue("@salt", account.PasswordSalt ?? "");
                    command.Parameters.AddWithValue("@admin", account.IsAdmin);
                    command.ExecuteNonQuery();
                }
                return AddUserOutcome.Added;
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return UsernameExists(account.Username) ? AddUserOutcome.UsernameTaken : AddUserOutcome.EmailTaken;
            }
        }

        public ProfileUpdateOutcome TryUpdateProfile(string username, string fullName, string email, string phone)
        {
            if (FindByUsername(username) == null) return ProfileUpdateOutcome.NotFound;
            if (EmailExistsForOther(email, username)) return ProfileUpdateOutcome.EmailTaken;
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("UPDATE Users SET FullName=@name,Email=@email,Phone=@phone WHERE Username=@username", db))
            {
                command.Parameters.AddWithValue("@name", (object)fullName ?? DBNull.Value);
                command.Parameters.AddWithValue("@email", email ?? "");
                command.Parameters.AddWithValue("@phone", (object)phone ?? DBNull.Value);
                command.Parameters.AddWithValue("@username", username ?? "");
                command.ExecuteNonQuery();
            }
            return ProfileUpdateOutcome.Updated;
        }

        public PasswordChangeOutcome TryChangePassword(string username, string currentPassword, string newPassword)
        {
            UserAccount account = FindByUsername(username);
            if (account == null) return PasswordChangeOutcome.NotFound;
            if (!PasswordHasher.Verify(currentPassword, account.PasswordSalt, account.PasswordHash)) return PasswordChangeOutcome.WrongPassword;
            string salt = PasswordHasher.NewSalt();
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("UPDATE Users SET PasswordHash=@hash,PasswordSalt=@salt WHERE Username=@username", db))
            {
                command.Parameters.AddWithValue("@hash", PasswordHasher.Hash(newPassword, salt));
                command.Parameters.AddWithValue("@salt", salt);
                command.Parameters.AddWithValue("@username", username);
                command.ExecuteNonQuery();
            }
            return PasswordChangeOutcome.Changed;
        }

        public RoleChangeOutcome TrySetAdmin(string username, bool isAdmin)
        {
            UserAccount account = FindByUsername(username);
            if (account == null) return RoleChangeOutcome.NotFound;
            if (account.IsAdmin == isAdmin) return RoleChangeOutcome.Unchanged;
            using (SqlConnection db = SqlDb.Open())
            using (SqlTransaction tx = db.BeginTransaction(IsolationLevel.Serializable))
            {
                if (!isAdmin)
                {
                    using (SqlCommand count = new SqlCommand("SELECT COUNT(*) FROM Users WHERE IsAdmin=1", db, tx))
                    {
                        if ((int)count.ExecuteScalar() <= 1) { tx.Rollback(); return RoleChangeOutcome.LastAdmin; }
                    }
                }
                using (SqlCommand update = new SqlCommand("UPDATE Users SET IsAdmin=@admin WHERE Username=@username", db, tx))
                {
                    update.Parameters.AddWithValue("@admin", isAdmin);
                    update.Parameters.AddWithValue("@username", username);
                    update.ExecuteNonQuery();
                }
                tx.Commit();
            }
            return RoleChangeOutcome.Changed;
        }

        private UserAccount One(string where, params SqlParameter[] parameters)
        {
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("SELECT TOP 1 Username,FullName,Email,Phone,PasswordHash,PasswordSalt,IsAdmin FROM Users " + where, db))
            {
                command.Parameters.AddRange(parameters);
                using (SqlDataReader reader = command.ExecuteReader()) return reader.Read() ? Read(reader) : null;
            }
        }

        private bool Exists(string column, string value)
        {
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE " + column + "=@value", db))
            {
                command.Parameters.AddWithValue("@value", value ?? "");
                return (int)command.ExecuteScalar() > 0;
            }
        }

        private bool EmailExistsForOther(string email, string username)
        {
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email=@email AND Username<>@username", db))
            {
                command.Parameters.AddWithValue("@email", email ?? "");
                command.Parameters.AddWithValue("@username", username ?? "");
                return (int)command.ExecuteScalar() > 0;
            }
        }

        private static UserAccount Read(SqlDataReader reader)
        {
            return new UserAccount
            {
                Username = reader["Username"].ToString(),
                FullName = reader["FullName"] as string,
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"] as string,
                PasswordHash = reader["PasswordHash"].ToString(),
                PasswordSalt = reader["PasswordSalt"].ToString(),
                IsAdmin = Convert.ToBoolean(reader["IsAdmin"])
            };
        }
    }

    public sealed class SqlOrderStore : IOrderStore
    {
        public void Save(Order order)
        {
            if (order == null) return;
            using (SqlConnection db = SqlDb.Open())
            using (SqlTransaction tx = db.BeginTransaction())
            {
                int id;
                using (SqlCommand command = new SqlCommand("INSERT INTO Orders(Code,CreatedAt,Username,Status,CustomerName,Phone,Address,PaymentMethod) OUTPUT INSERTED.Id VALUES(@code,@created,@username,@status,@name,@phone,@address,@payment)", db, tx))
                {
                    AddOrderParameters(command, order);
                    id = (int)command.ExecuteScalar();
                }
                InsertLines(db, tx, id, order.Lines);
                tx.Commit();
            }
        }

        public List<Order> All() { return ReadOrders("", null); }
        public List<Order> ByUser(string username) { return string.IsNullOrEmpty(username) ? new List<Order>() : ReadOrders(" WHERE Username=@username", SqlDb.Parameter("@username", username)); }
        public Order FindByCode(string code) { return string.IsNullOrWhiteSpace(code) ? null : ReadOrders(" WHERE Code=@code", SqlDb.Parameter("@code", code.Trim())).FirstOrDefault(); }

        public void UpdateStatus(Order order)
        {
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("UPDATE Orders SET Status=@status WHERE Code=@code", db))
            {
                command.Parameters.AddWithValue("@status", order.Status);
                command.Parameters.AddWithValue("@code", order.Code);
                command.ExecuteNonQuery();
            }
        }

        private static void AddOrderParameters(SqlCommand command, Order order)
        {
            command.Parameters.AddWithValue("@code", order.Code);
            command.Parameters.AddWithValue("@created", order.CreatedAt);
            command.Parameters.AddWithValue("@username", order.Username ?? "");
            command.Parameters.AddWithValue("@status", order.Status ?? Order.PendingStatus);
            command.Parameters.AddWithValue("@name", order.CustomerName ?? "");
            command.Parameters.AddWithValue("@phone", order.Phone ?? "");
            command.Parameters.AddWithValue("@address", order.Address ?? "");
            command.Parameters.AddWithValue("@payment", order.PaymentMethod ?? "");
        }

        private static void InsertLines(SqlConnection db, SqlTransaction tx, int orderId, IEnumerable<OrderLine> lines)
        {
            foreach (OrderLine line in lines ?? new List<OrderLine>())
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO OrderLines(OrderId,ProductId,Name,Size,Price,Quantity) VALUES(@order,@product,@name,@size,@price,@quantity)", db, tx))
                {
                    command.Parameters.AddWithValue("@order", orderId);
                    command.Parameters.AddWithValue("@product", line.ProductId);
                    command.Parameters.AddWithValue("@name", line.Name ?? "");
                    command.Parameters.AddWithValue("@size", line.Size ?? "");
                    command.Parameters.AddWithValue("@price", line.Price);
                    command.Parameters.AddWithValue("@quantity", line.Quantity);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static List<Order> ReadOrders(string filter, SqlParameter parameter)
        {
            List<Order> result = new List<Order>();
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("SELECT Id,Code,CreatedAt,Username,Status,CustomerName,Phone,Address,PaymentMethod FROM Orders" + filter + " ORDER BY CreatedAt DESC", db))
            {
                if (parameter != null) command.Parameters.Add(parameter);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) result.Add(new Order { Code = reader["Code"].ToString(), CreatedAt = Convert.ToDateTime(reader["CreatedAt"]), Username = reader["Username"].ToString(), Status = reader["Status"].ToString(), CustomerName = reader["CustomerName"].ToString(), Phone = reader["Phone"].ToString(), Address = reader["Address"].ToString(), PaymentMethod = reader["PaymentMethod"].ToString(), Lines = new List<OrderLine>() });
                }
            }
            foreach (Order order in result) order.Lines = ReadLines(order.Code);
            return result;
        }

        private static List<OrderLine> ReadLines(string code)
        {
            List<OrderLine> result = new List<OrderLine>();
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("SELECT l.ProductId,l.Name,l.Size,l.Price,l.Quantity FROM OrderLines l INNER JOIN Orders o ON o.Id=l.OrderId WHERE o.Code=@code ORDER BY l.Id", db))
            {
                command.Parameters.AddWithValue("@code", code);
                using (SqlDataReader reader = command.ExecuteReader()) while (reader.Read()) result.Add(new OrderLine { ProductId = Convert.ToInt32(reader["ProductId"]), Name = reader["Name"].ToString(), Size = reader["Size"].ToString(), Price = Convert.ToDecimal(reader["Price"]), Quantity = Convert.ToInt32(reader["Quantity"]) });
            }
            return result;
        }
    }

    public sealed class SqlCatalogStore
    {
        public ProductItem Find(int id) { return All().FirstOrDefault(x => x.Id == id); }
        public List<ProductItem> All()
        {
            List<ProductItem> result = new List<ProductItem>();
            using (SqlConnection db = SqlDb.Open())
            using (SqlCommand command = new SqlCommand("SELECT Id,Name,Brand,Description,Price,OldPrice,Stock,Sizes,ImageUrl,ImageUrlLarge,IsHot FROM Products ORDER BY Id", db))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read()) result.Add(new ProductItem { Id = Convert.ToInt32(reader["Id"]), Name = reader["Name"].ToString(), Brand = reader["Brand"].ToString(), Description = reader["Description"] as string, Price = Convert.ToDecimal(reader["Price"]), OldPrice = reader["OldPrice"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["OldPrice"]), Stock = Convert.ToInt32(reader["Stock"]), Sizes = reader["Sizes"].ToString(), ImageUrl = reader["ImageUrl"].ToString(), ImageUrlLarge = reader["ImageUrlLarge"] as string, IsHot = Convert.ToBoolean(reader["IsHot"]) });
            }
            foreach (ProductItem product in result) product.Colourways = ReadColourways(product.Id);
            return result;
        }

        public SaveProductOutcome Save(ProductItem product)
        {
            using (SqlConnection db = SqlDb.Open())
            {
                if (product.Id <= 0)
                {
                    using (SqlCommand command = new SqlCommand("INSERT INTO Products(Name,Brand,Description,Price,OldPrice,Stock,Sizes,ImageUrl,ImageUrlLarge,IsHot) OUTPUT INSERTED.Id VALUES(@name,@brand,@description,@price,@old,@stock,@sizes,@image,@large,@hot)", db))
                    {
                        AddProductParameters(command, product);
                        product.Id = Convert.ToInt32(command.ExecuteScalar());
                    }
                    SaveColourways(db, product.Id, product.Colourways);
                    return SaveProductOutcome.Created;
                }
                ProductItem old = Find(product.Id);
                if (old == null) return SaveProductOutcome.NotFound;
                using (SqlCommand command = new SqlCommand("UPDATE Products SET Name=@name,Brand=@brand,Description=@description,Price=@price,OldPrice=@old,Stock=@stock,Sizes=@sizes,ImageUrl=@image,ImageUrlLarge=@large WHERE Id=@id", db))
                {
                    AddProductParameters(command, product);
                    command.Parameters.AddWithValue("@id", product.Id);
                    command.ExecuteNonQuery();
                }
                if (product.Colourways != null || old.ImageUrl != product.ImageUrl || old.ImageUrlLarge != product.ImageUrlLarge)
                {
                    DeleteColourways(db, product.Id);
                    SaveColourways(db, product.Id, product.Colourways);
                }
                return SaveProductOutcome.Updated;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection db = SqlDb.Open()) using (SqlCommand command = new SqlCommand("DELETE FROM Products WHERE Id=@id", db)) { command.Parameters.AddWithValue("@id", id); return command.ExecuteNonQuery() > 0; }
        }

        public bool TakeStock(IEnumerable<KeyValuePair<int, int>> input)
        {
            Dictionary<int, int> wanted = Totals(input);
            using (SqlConnection db = SqlDb.Open()) using (SqlTransaction tx = db.BeginTransaction())
            {
                foreach (KeyValuePair<int, int> item in wanted)
                {
                    using (SqlCommand command = new SqlCommand("UPDATE Products SET Stock=Stock-@quantity WHERE Id=@id AND Stock>=@quantity", db, tx))
                    {
                        command.Parameters.AddWithValue("@quantity", item.Value); command.Parameters.AddWithValue("@id", item.Key);
                        if (command.ExecuteNonQuery() != 1) { tx.Rollback(); return false; }
                    }
                }
                tx.Commit(); return true;
            }
        }

        public void RestoreStock(IEnumerable<KeyValuePair<int, int>> input)
        {
            using (SqlConnection db = SqlDb.Open()) using (SqlTransaction tx = db.BeginTransaction())
            {
                foreach (KeyValuePair<int, int> item in Totals(input)) using (SqlCommand command = new SqlCommand("UPDATE Products SET Stock=Stock+@quantity WHERE Id=@id", db, tx)) { command.Parameters.AddWithValue("@quantity", item.Value); command.Parameters.AddWithValue("@id", item.Key); command.ExecuteNonQuery(); }
                tx.Commit();
            }
        }

        private static Dictionary<int, int> Totals(IEnumerable<KeyValuePair<int, int>> input)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> item in input ?? new List<KeyValuePair<int, int>>()) if (item.Value > 0) result[item.Key] = result.ContainsKey(item.Key) ? result[item.Key] + item.Value : item.Value;
            return result;
        }

        private static void AddProductParameters(SqlCommand command, ProductItem product)
        {
            command.Parameters.AddWithValue("@name", product.Name ?? ""); command.Parameters.AddWithValue("@brand", product.Brand ?? ""); command.Parameters.AddWithValue("@description", (object)product.Description ?? DBNull.Value); command.Parameters.AddWithValue("@price", product.Price); command.Parameters.AddWithValue("@old", (object)product.OldPrice ?? DBNull.Value); command.Parameters.AddWithValue("@stock", product.Stock); command.Parameters.AddWithValue("@sizes", product.Sizes ?? ""); command.Parameters.AddWithValue("@image", product.ImageUrl ?? ""); command.Parameters.AddWithValue("@large", (object)product.ImageUrlLarge ?? DBNull.Value); command.Parameters.AddWithValue("@hot", product.IsHot);
        }

        private static ProductItem FindBasic(SqlConnection db, int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT Id FROM Products WHERE Id=@id", db)) { command.Parameters.AddWithValue("@id", id); return command.ExecuteScalar() == null ? null : new ProductItem { Id = id }; }
        }

        private static List<ProductColourway> ReadColourways(int id)
        {
            List<ProductColourway> result = new List<ProductColourway>();
            using (SqlConnection db = SqlDb.Open()) using (SqlCommand command = new SqlCommand("SELECT Name,Small,Large FROM ProductColourways WHERE ProductId=@id ORDER BY Name", db)) { command.Parameters.AddWithValue("@id", id); using (SqlDataReader reader = command.ExecuteReader()) while (reader.Read()) result.Add(new ProductColourway { Name = reader["Name"].ToString(), Small = reader["Small"].ToString(), Large = reader["Large"].ToString() }); }
            return result;
        }

        private static void SaveColourways(SqlConnection db, int id, IEnumerable<ProductColourway> items)
        {
            foreach (ProductColourway item in items ?? new List<ProductColourway>()) using (SqlCommand command = new SqlCommand("INSERT INTO ProductColourways(ProductId,Name,Small,Large) VALUES(@id,@name,@small,@large)", db)) { command.Parameters.AddWithValue("@id", id); command.Parameters.AddWithValue("@name", item.Name ?? ""); command.Parameters.AddWithValue("@small", item.Small ?? ""); command.Parameters.AddWithValue("@large", item.Large ?? ""); command.ExecuteNonQuery(); }
        }

        private static void DeleteColourways(SqlConnection db, int id) { using (SqlCommand command = new SqlCommand("DELETE FROM ProductColourways WHERE ProductId=@id", db)) { command.Parameters.AddWithValue("@id", id); command.ExecuteNonQuery(); } }
    }

    public sealed class SqlReviewStore
    {
        public List<ProductReview> GetByProduct(int id)
        {
            List<ProductReview> result = new List<ProductReview>();
            using (SqlConnection db = SqlDb.Open()) using (SqlCommand command = new SqlCommand("SELECT ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt FROM ProductReviews WHERE ProductId=@id ORDER BY CreatedAt DESC", db)) { command.Parameters.AddWithValue("@id", id); using (SqlDataReader reader = command.ExecuteReader()) while (reader.Read()) result.Add(Read(reader)); }
            return result;
        }
        public ProductReview Find(int id, string userKey)
        {
            using (SqlConnection db = SqlDb.Open()) using (SqlCommand command = new SqlCommand("SELECT TOP 1 ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt FROM ProductReviews WHERE ProductId=@id AND UserKey=@user", db)) { command.Parameters.AddWithValue("@id", id); command.Parameters.AddWithValue("@user", userKey ?? ""); using (SqlDataReader reader = command.ExecuteReader()) return reader.Read() ? Read(reader) : null; }
        }
        public void Save(ProductReview review)
        {
            using (SqlConnection db = SqlDb.Open()) using (SqlCommand command = new SqlCommand("UPDATE ProductReviews SET CustomerName=@name,Rating=@rating,Comment=@comment,CreatedAt=@created WHERE ProductId=@id AND UserKey=@user", db))
            {
                command.Parameters.AddWithValue("@name", review.CustomerName ?? ""); command.Parameters.AddWithValue("@rating", review.Rating); command.Parameters.AddWithValue("@comment", review.Comment ?? ""); command.Parameters.AddWithValue("@created", DateTime.Now); command.Parameters.AddWithValue("@id", review.ProductId); command.Parameters.AddWithValue("@user", review.UserKey ?? "");
                if (command.ExecuteNonQuery() == 0) { command.CommandText = "INSERT INTO ProductReviews(ProductId,UserKey,CustomerName,Rating,Comment,CreatedAt) VALUES(@id,@user,@name,@rating,@comment,@created)"; command.ExecuteNonQuery(); }
            }
        }
        public double Average(int id) { List<ProductReview> list = GetByProduct(id); return list.Count == 0 ? 0 : list.Average(x => x.Rating); }
        private static ProductReview Read(SqlDataReader reader) { return new ProductReview { ProductId = Convert.ToInt32(reader["ProductId"]), UserKey = reader["UserKey"].ToString(), CustomerName = reader["CustomerName"].ToString(), Rating = Convert.ToInt32(reader["Rating"]), Comment = reader["Comment"].ToString(), CreatedAt = Convert.ToDateTime(reader["CreatedAt"]) }; }
    }

    public sealed class SqlCartStore
    {
        public List<CartItem> Load(string username)
        {
            List<CartItem> result = new List<CartItem>();
            using (SqlConnection db = SqlDb.Open()) using (SqlCommand command = new SqlCommand("SELECT ProductId,Name,Size,Price,Quantity,ImageUrl FROM CartItems WHERE Username=@user ORDER BY ProductId", db))
            {
                command.Parameters.AddWithValue("@user", username);
                using (SqlDataReader reader = command.ExecuteReader()) while (reader.Read()) result.Add(new CartItem { Id = Convert.ToInt32(reader["ProductId"]), Name = reader["Name"].ToString(), Size = reader["Size"].ToString(), Price = Convert.ToDecimal(reader["Price"]), Quantity = Convert.ToInt32(reader["Quantity"]), ImageUrl = reader["ImageUrl"] as string });
            }
            return result;
        }

        public void Save(string username, IEnumerable<CartItem> lines)
        {
            using (SqlConnection db = SqlDb.Open()) using (SqlTransaction tx = db.BeginTransaction())
            {
                using (SqlCommand delete = new SqlCommand("DELETE FROM CartItems WHERE Username=@user", db, tx)) { delete.Parameters.AddWithValue("@user", username); delete.ExecuteNonQuery(); }
                foreach (CartItem line in lines ?? new List<CartItem>()) using (SqlCommand insert = new SqlCommand("INSERT INTO CartItems(Username,ProductId,Size,Name,Price,Quantity,ImageUrl) VALUES(@user,@product,@size,@name,@price,@quantity,@image)", db, tx)) { insert.Parameters.AddWithValue("@user", username); insert.Parameters.AddWithValue("@product", line.Id); insert.Parameters.AddWithValue("@size", line.Size ?? ""); insert.Parameters.AddWithValue("@name", line.Name ?? ""); insert.Parameters.AddWithValue("@price", line.Price); insert.Parameters.AddWithValue("@quantity", line.Quantity); insert.Parameters.AddWithValue("@image", (object)line.ImageUrl ?? DBNull.Value); insert.ExecuteNonQuery(); }
                tx.Commit();
            }
        }
    }

    public sealed class SqlWishlistStore
    {
        public HashSet<int> Load(string username)
        {
            HashSet<int> result = new HashSet<int>();
            using (SqlConnection db = SqlDb.Open()) using (SqlCommand command = new SqlCommand("SELECT ProductId FROM Wishlists WHERE Username=@user", db)) { command.Parameters.AddWithValue("@user", username); using (SqlDataReader reader = command.ExecuteReader()) while (reader.Read()) result.Add(Convert.ToInt32(reader["ProductId"])); }
            return result;
        }

        public void Save(string username, IEnumerable<int> productIds)
        {
            using (SqlConnection db = SqlDb.Open()) using (SqlTransaction tx = db.BeginTransaction())
            {
                using (SqlCommand delete = new SqlCommand("DELETE FROM Wishlists WHERE Username=@user", db, tx)) { delete.Parameters.AddWithValue("@user", username); delete.ExecuteNonQuery(); }
                foreach (int id in productIds ?? new List<int>()) using (SqlCommand insert = new SqlCommand("INSERT INTO Wishlists(Username,ProductId) VALUES(@user,@product)", db, tx)) { insert.Parameters.AddWithValue("@user", username); insert.Parameters.AddWithValue("@product", id); insert.ExecuteNonQuery(); }
                tx.Commit();
            }
        }
    }

    public static class SqlDatabase
    {
        public static void SeedIfEmpty()
        {
            using (SqlConnection db = SqlDb.Open())
            {
                int products;
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Products", db)) products = (int)command.ExecuteScalar();
                if (products == 0) foreach (ProductItem product in ProductSeed.Items()) { product.Id = 0; new SqlCatalogStore().Save(product); }
                int users;
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users", db)) users = (int)command.ExecuteScalar();
                if (users == 0) foreach (UserAccount account in MemoryUserStore.Seed()) new SqlUserStore().TryAdd(account);
            }
        }
    }
}
