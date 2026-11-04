IF DB_ID(N'ShoeStore') IS NULL CREATE DATABASE ShoeStore;
GO
USE ShoeStore;
GO

CREATE TABLE Users
(
    Id INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    FullName NVARCHAR(120) NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(30) NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    PasswordSalt NVARCHAR(256) NOT NULL,
    IsAdmin BIT NOT NULL DEFAULT 0
);

CREATE TABLE Products
(
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Brand NVARCHAR(60) NOT NULL,
    Description NVARCHAR(2000) NULL,
    Price DECIMAL(18,2) NOT NULL,
    OldPrice DECIMAL(18,2) NULL,
    Stock INT NOT NULL DEFAULT 0,
    Sizes NVARCHAR(200) NOT NULL,
    ImageUrl NVARCHAR(400) NOT NULL,
    ImageUrlLarge NVARCHAR(400) NULL,
    IsHot BIT NOT NULL DEFAULT 0
);

CREATE TABLE ProductColourways
(
    Id INT IDENTITY PRIMARY KEY,
    ProductId INT NOT NULL,
    Name NVARCHAR(80) NOT NULL,
    Small NVARCHAR(400) NOT NULL,
    Large NVARCHAR(400) NOT NULL,
    CONSTRAINT FK_ProductColourways_Product FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

CREATE TABLE Orders
(
    Id INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(40) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    CustomerName NVARCHAR(120) NOT NULL,
    Phone NVARCHAR(30) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    PaymentMethod NVARCHAR(20) NOT NULL
);

CREATE TABLE OrderLines
(
    Id INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Name NVARCHAR(120) NOT NULL,
    Size NVARCHAR(20) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Quantity INT NOT NULL,
    CONSTRAINT FK_OrderLines_Order FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
);

CREATE TABLE CartItems
(
    Username NVARCHAR(50) NOT NULL,
    ProductId INT NOT NULL,
    Size NVARCHAR(20) NOT NULL,
    Name NVARCHAR(120) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Quantity INT NOT NULL,
    ImageUrl NVARCHAR(400) NULL,
    CONSTRAINT PK_CartItems PRIMARY KEY (Username, ProductId, Size)
);

CREATE TABLE Wishlists
(
    Username NVARCHAR(50) NOT NULL,
    ProductId INT NOT NULL,
    CONSTRAINT PK_Wishlists PRIMARY KEY (Username, ProductId)
);

CREATE TABLE ProductReviews
(
    ProductId INT NOT NULL,
    UserKey NVARCHAR(50) NOT NULL,
    CustomerName NVARCHAR(120) NOT NULL,
    Rating INT NOT NULL,
    Comment NVARCHAR(1000) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    CONSTRAINT PK_ProductReviews PRIMARY KEY (ProductId, UserKey),
    CONSTRAINT CK_ProductReviews_Rating CHECK (Rating BETWEEN 1 AND 5)
);
GO
