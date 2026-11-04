using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;
using System.Web.SessionState;
using ShoeStore.Helpers;


namespace ShoeStore.Data
{
    public sealed class CartLineView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int Max { get; set; }
    }

    public static class CartService
    {
        // Thực hiện việc thêm or gộp.
        public static void AddOrMerge(Cart gioHang, ProductItem sanPham, string kichThuoc, int soLuong)
        {
            if (gioHang != null && sanPham != null) gioHang.AddOrMerge(sanPham.Id, sanPham.Name, sanPham.Price, kichThuoc, soLuong, sanPham.ImageUrl, sanPham.Stock);
        }

        // Thực hiện việc đặt số lượng.
        public static bool SetQuantity(Cart gioHang, int ma, string kichThuoc, int soLuong)
        {
            if (gioHang == null) return false;
            CartItem dong = gioHang.Lines.FirstOrDefault(x => x.Id == ma && x.Size == kichThuoc);
            if (dong == null) return false;
            int cu = dong.Quantity;
            gioHang.SetQuantity(ma, kichThuoc, soLuong, StockOf(ma));
            return cu != dong.Quantity;
        }

        // Thực hiện việc max buyable.
        public static int MaxBuyable(Cart gioHang, int ma)
        {
            int giaTri = gioHang.RemainingToBuy(ma, StockOf(ma), null);
            return giaTri > 0 ? giaTri : 1;
        }

        // Thực hiện việc dòng.
        public static List<CartLineView> Lines(Cart gioHang)
        {
            return gioHang.Lines.Select(x => new CartLineView
            {
                Id = x.Id, Name = x.Name, Price = x.Price, Size = x.Size,
                ImageUrl = x.ImageUrl, Quantity = x.Quantity,
                Max = Math.Max(1, gioHang.RemainingToBuy(x.Id, StockOf(x.Id), x.Size))
            }).ToList();
        }

        // Thực hiện việc stock của.
        private static int StockOf(int ma)
        {
            ProductItem sanPham = Catalog.Find(ma);
            return sanPham == null ? int.MaxValue : sanPham.Stock;
        }
    }
}



namespace ShoeStore.Data
{
    public sealed class CartStore
    {
        private const string KhoaPhien = "Cart";
        private readonly HttpSessionState phien;

        // Thực hiện việc giỏ hàng kho.
        public CartStore(HttpSessionState phien) { this.phien = phien; }

        private string Owner
        {
            get
            {
                string giaTri = AuthHelper.UserKeyOf(phien);
                return giaTri.Length == 0 ? null : giaTri;
            }
        }

        // Thực hiện việc tải.
        public Cart Load() { return new Cart(Read()); }
        // Thực hiện việc lưu.
        public void Save(Cart gioHang) { SaveLines(gioHang == null ? null : gioHang.Lines.ToList()); }
        // Thực hiện việc xóa.
        public void Clear() { SaveLines(null); }
        // Thực hiện việc xóa khách.
        public void ClearGuest() { phien.Remove(KhoaPhien); }

        // Thực hiện việc thêm sản phẩm.
        public void AddProduct(ProductItem sanPham, string kichThuoc, int soLuong)
        {
            Cart gioHang = Load();
            CartService.AddOrMerge(gioHang, sanPham, kichThuoc, soLuong);
            Save(gioHang);
        }

        // Thực hiện việc gộp khách vào tài khoản.
        public void MergeGuestIntoAccount()
        {
            if (Owner == null) return;
            List<CartItem> guest = phien[KhoaPhien] as List<CartItem>;
            phien.Remove(KhoaPhien);
            if (guest == null) return;
            Cart gioHang = Load();
            foreach (CartItem phanTu in guest)
            {
                ProductItem sanPham = Catalog.Find(phanTu.Id);
                if (sanPham != null) CartService.AddOrMerge(gioHang, sanPham, phanTu.Size, phanTu.Quantity);
            }
            Save(gioHang);
        }

        // Thực hiện việc đọc.
        private List<CartItem> Read()
        {
            if (Owner == null)
            {
                List<CartItem> guest = phien[KhoaPhien] as List<CartItem>;
                return guest == null ? null : new List<CartItem>(guest);
            }

            return new SqlCartStore().Load(Owner);
        }

        // Thực hiện việc lưu dòng.
        private void SaveLines(List<CartItem> cacDong)
        {
            bool rong = cacDong == null || cacDong.Count == 0;
            if (Owner == null)
            {
                phien[KhoaPhien] = rong ? null : new List<CartItem>(cacDong);
                return;
            }

            new SqlCartStore().Save(Owner, cacDong);
        }
    }
}



namespace ShoeStore.Data
{
    public sealed class WishlistStore
    {
        private const string KhoaPhien = "Wishlist";
        private readonly HttpSessionState phien;

        // Thực hiện việc yêu thích kho.
        public WishlistStore(HttpSessionState phien) { this.phien = phien; }

        private string Owner
        {
            get
            {
                string giaTri = AuthHelper.UserKeyOf(phien);
                return giaTri.Length == 0 ? null : giaTri;
            }
        }

        // Thực hiện việc kiểm tra chứa.
        public bool Contains(int ma) { return Read().Contains(ma); }

        // Thực hiện việc đổi trạng thái.
        public bool Toggle(int ma)
        {
            HashSet<int> phanTu = Read();
            bool daThich = !phanTu.Remove(ma);
            if (daThich) phanTu.Add(ma);
            Save(phanTu);
            return daThich;
        }

        // Thực hiện việc xóa.
        public void Clear() { Save(new HashSet<int>()); }
        // Thực hiện việc xóa khách.
        public void ClearGuest() { phien.Remove(KhoaPhien); }

        // Thực hiện việc gộp khách vào tài khoản.
        public void MergeGuestIntoAccount()
        {
            if (Owner == null) return;
            HashSet<int> guest = phien[KhoaPhien] as HashSet<int>;
            phien.Remove(KhoaPhien);
            if (guest == null) return;
            HashSet<int> phanTu = Read();
            phanTu.UnionWith(guest);
            Save(phanTu);
        }

        // Thực hiện việc đọc.
        private HashSet<int> Read()
        {
            if (Owner == null)
            {
                HashSet<int> guest = phien[KhoaPhien] as HashSet<int>;
                return guest == null ? new HashSet<int>() : new HashSet<int>(guest);
            }

            return new SqlWishlistStore().Load(Owner);
        }

        // Thực hiện việc lưu.
        private void Save(HashSet<int> phanTu)
        {
            bool rong = phanTu == null || phanTu.Count == 0;
            if (Owner == null)
            {
                phien[KhoaPhien] = rong ? null : new HashSet<int>(phanTu);
                return;
            }

            new SqlWishlistStore().Save(Owner, phanTu);
        }
    }
}
