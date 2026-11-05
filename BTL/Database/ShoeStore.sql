-- =====================================================================
--  Cơ sở dữ liệu cho website bán giày SneakerX
--  Chỉ có 4 bảng: Users, Products, Orders, OrderLines
--  Chạy script này một lần trong SQL Server Management Studio.
--  Script xóa bảng cũ rồi tạo lại, nên chạy lại được nhiều lần.
-- =====================================================================

IF DB_ID(N'ShoeStore') IS NULL
    CREATE DATABASE ShoeStore;
GO

USE ShoeStore;
GO

-- Xóa bảng cũ nếu đã tồn tại (xóa con trước, cha sau)
IF OBJECT_ID(N'dbo.OrderLines', N'U') IS NOT NULL DROP TABLE dbo.OrderLines;
IF OBJECT_ID(N'dbo.Orders', N'U') IS NOT NULL DROP TABLE dbo.Orders;
-- Bốn bảng của bản cũ, bản này không dùng nữa
IF OBJECT_ID(N'dbo.CartItems', N'U') IS NOT NULL DROP TABLE dbo.CartItems;
IF OBJECT_ID(N'dbo.Wishlists', N'U') IS NOT NULL DROP TABLE dbo.Wishlists;
IF OBJECT_ID(N'dbo.ProductReviews', N'U') IS NOT NULL DROP TABLE dbo.ProductReviews;
IF OBJECT_ID(N'dbo.ProductColourways', N'U') IS NOT NULL DROP TABLE dbo.ProductColourways;
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- ---------------------------------------------------------------------
-- Bảng người dùng
-- ---------------------------------------------------------------------
CREATE TABLE dbo.Users
(
    Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(64)  NOT NULL,          -- SHA256 của mật khẩu, dạng hex
    FullName     NVARCHAR(120) NULL,
    Email        NVARCHAR(255) NULL,
    Phone        NVARCHAR(30)  NULL,
    IsAdmin      BIT           NOT NULL DEFAULT 0
);
GO

-- ---------------------------------------------------------------------
-- Bảng sản phẩm
-- ---------------------------------------------------------------------
CREATE TABLE dbo.Products
(
    Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name        NVARCHAR(120)   NOT NULL,
    Brand       NVARCHAR(60)    NOT NULL,
    Description NVARCHAR(1000)  NULL,
    Price       DECIMAL(18,2)   NOT NULL,
    Stock       INT             NOT NULL DEFAULT 0,
    Sizes       NVARCHAR(100)   NOT NULL,         -- danh sách size, cách nhau bằng dấu phẩy
    ImageUrl    NVARCHAR(400)   NOT NULL,
    IsHot       BIT             NOT NULL DEFAULT 0
);
GO

-- ---------------------------------------------------------------------
-- Bảng đơn hàng
-- ---------------------------------------------------------------------
CREATE TABLE dbo.Orders
(
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Code          NVARCHAR(40)  NOT NULL UNIQUE,  -- mã đơn, ví dụ DH20260315143000
    CreatedAt     DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    Username      NVARCHAR(50)  NOT NULL,
    CustomerName  NVARCHAR(120) NOT NULL,
    Phone         NVARCHAR(30)  NOT NULL,
    Address       NVARCHAR(500) NOT NULL,
    PaymentMethod NVARCHAR(20)  NOT NULL,         -- COD hoặc ChuyenKhoan
    Total         DECIMAL(18,2) NOT NULL
);
GO

-- ---------------------------------------------------------------------
-- Bảng chi tiết đơn hàng
-- ---------------------------------------------------------------------
CREATE TABLE dbo.OrderLines
(
    Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    OrderId     INT            NOT NULL,
    ProductId   INT            NOT NULL,
    ProductName NVARCHAR(120)  NOT NULL,
    Size        NVARCHAR(20)   NOT NULL,
    Price       DECIMAL(18,2)  NOT NULL,
    Quantity    INT            NOT NULL,
    CONSTRAINT FK_OrderLines_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE
);
GO

-- =====================================================================
--  Dữ liệu mẫu
-- =====================================================================

-- admin / admin123   và   khach / khach123
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, Phone, IsAdmin) VALUES
(N'admin', N'240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', N'Quản trị viên', N'admin@sneakerx.vn', N'0900000000', 1),
(N'khach', N'79c4de8c8577ebfbebe7ecf1951d0b51e66f80cf7b7255d20798917f4886de87', N'Nguyễn Văn Khách', N'khach@gmail.com', N'0911111111', 0);
GO

INSERT INTO dbo.Products (Name, Brand, Description, Price, Stock, Sizes, ImageUrl, IsHot) VALUES
(N'Nike Air Force 1', N'Nike', N'Da chính hãng, đế cao su bám đường và đệm êm cả ngày.', 2900000, 12, N'38,39,40,41', N'Content/images/products/nike-1-sm.jpg', 1),
(N'Nike Air Jordan 1', N'Nike', N'Dòng giày bóng rổ nổi tiếng nhất của Nike, da cao cấp và đệm êm.', 4500000, 3, N'40,41,42,43', N'Content/images/products/nike-2-sm.jpg', 1),
(N'Nike Air Max 90', N'Nike', N'Đệm Air dày, hợp cả chạy bộ lẫn dạo phố.', 4200000, 6, N'40,41,42,43', N'Content/images/products/nike-3-sm.jpg', 0),
(N'NikeCourt Air Zoom Vapor Cage 4', N'Nike', N'Thân lưới nhẹ thoáng, đế dẻo ôm chân khi vận động.', 2750000, 14, N'39,40,41,42', N'Content/images/products/nike-4-sm.jpg', 0),
(N'Converse Chuck 70 High', N'Converse', N'Canvas cao cổ biểu tượng của phong cách skate từ năm 1970.', 1950000, 0, N'38,39,40,41,42', N'Content/images/products/converse-5-sm.jpg', 1),
(N'Converse Chuck Taylor All Star Low', N'Converse', N'Mũi thấp dễ phối, canvas bền và đế cao su trắng cổ điển.', 1850000, 11, N'38,39,40,41', N'Content/images/products/converse-6-sm.jpg', 0),
(N'Vans Old Skool', N'Vans', N'Dáng skate cổ điển, hợp quần jean và áo bomber.', 1750000, 20, N'38,39,40,41', N'Content/images/products/vans-7-sm.jpg', 1),
(N'New Balance 574', N'New Balance', N'Chất liệu da lộn mềm, form classic bền qua nhiều mùa.', 2450000, 10, N'39,40,41,42', N'Content/images/products/newbalance-8-sm.jpg', 0),
(N'Puma RS-X', N'Puma', N'Form chunky đặc trưng, phối lưới và da lộn, đế dày êm chân.', 2450000, 12, N'39,40,41,42', N'Content/images/products/newbalance-9-sm.jpg', 0),
(N'Adidas Superstar', N'Adidas', N'Mũi giày vỏ sò biểu tượng, da cao cấp, đế cao su bám tốt.', 2100000, 13, N'38,39,40,41', N'Content/images/products/adidas-10-sm.jpg', 0),
(N'Nike LeBron Soldier 14', N'Nike', N'Đệm Zoom Air, đai giữa bàn chân cố định khi di chuyển.', 2650000, 9, N'40,41,42,43', N'Content/images/products/underarmour-11-sm.jpg', 0),
(N'Common Projects Achilles Low', N'Common Projects', N'Da Ý trắng tối giản, dãy số vàng đặc trưng của dòng Achilles.', 10500000, 7, N'39,40,41,42', N'Content/images/products/commonprojects-12-sm.jpg', 1);
GO

SELECT N'Xong: 4 bảng và 12 sản phẩm mẫu đã sẵn sàng.' AS KetQua;
GO
