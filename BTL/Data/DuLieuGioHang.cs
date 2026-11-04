using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    // Dữ liệu đã chuẩn bị để hiển thị một dòng giỏ hàng.
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
        // Thêm sản phẩm hoặc gộp vào dòng cùng cỡ.
        public static void AddOrMerge(Cart gioHang, ProductItem sanPham,
            string kichThuoc, int soLuong)
        {
            if (gioHang == null || sanPham == null) return;
            gioHang.AddOrMerge(sanPham.Id, sanPham.Name, sanPham.Price,
                kichThuoc, soLuong, sanPham.ImageUrl, sanPham.Stock);
        }

        // Đặt lại số lượng một dòng, trả về true nếu có thay đổi.
        public static bool SetQuantity(Cart gioHang, int maSanPham,
            string kichThuoc, int soLuong)
        {
            if (gioHang == null) return false;

            CartItem dong = gioHang.Lines.FirstOrDefault(item =>
                item.Id == maSanPham && item.Size == kichThuoc);
            if (dong == null) return false;

            int soLuongCu = dong.Quantity;
            gioHang.SetQuantity(maSanPham, kichThuoc, soLuong, StockOf(maSanPham));
            return soLuongCu != dong.Quantity;
        }

        // Tính số lượng tối đa còn có thể thêm vào giỏ.
        public static int MaxBuyable(Cart gioHang, int maSanPham)
        {
            if (gioHang == null) return 1;

            int conLai = gioHang.RemainingToBuy(
                maSanPham, StockOf(maSanPham), null);
            return Math.Max(1, conLai);
        }

        // Tạo dữ liệu hiển thị cho repeater giỏ hàng.
        public static List<CartLineView> Lines(Cart gioHang)
        {
            if (gioHang == null) return new List<CartLineView>();

            return gioHang.Lines.Select(dong => new CartLineView
            {
                Id = dong.Id,
                Name = dong.Name,
                Price = dong.Price,
                Size = dong.Size,
                ImageUrl = dong.ImageUrl,
                Quantity = dong.Quantity,
                Max = Math.Max(1, gioHang.RemainingToBuy(
                    dong.Id, StockOf(dong.Id), dong.Size))
            }).ToList();
        }

        // Đọc tồn kho mới nhất của sản phẩm.
        private static int StockOf(int maSanPham)
        {
            ProductItem sanPham = Catalog.Find(maSanPham);
            return sanPham == null ? 0 : sanPham.Stock;
        }
    }

    // Kho giỏ hàng chọn session cho khách và SQL cho tài khoản.
    public sealed class CartStore
    {
        private const string KhoaPhien = "Cart";
        private readonly HttpSessionState phien;

        public CartStore(HttpSessionState phien)
        {
            this.phien = phien;
        }

        private string ChuTaiKhoan
        {
            get
            {
                string khoa = AuthHelper.UserKeyOf(phien);
                return string.IsNullOrEmpty(khoa) ? null : khoa;
            }
        }

        // Đọc giỏ hàng hiện tại.
        public Cart Load()
        {
            return new Cart(DocDong());
        }

        // Lưu toàn bộ giỏ hàng.
        public void Save(Cart gioHang)
        {
            SaveLines(gioHang == null ? null : gioHang.Lines.ToList());
        }

        // Xóa giỏ hàng của tài khoản hoặc khách.
        public void Clear()
        {
            SaveLines(null);
        }

        // Xóa dữ liệu giỏ tạm của khách sau khi đăng nhập.
        public void ClearGuest()
        {
            if (phien != null) phien.Remove(KhoaPhien);
        }

        // Thêm một sản phẩm vào giỏ hiện tại.
        public void AddProduct(ProductItem sanPham, string kichThuoc,
            int soLuong)
        {
            if (sanPham == null) return;

            Cart gioHang = Load();
            CartService.AddOrMerge(gioHang, sanPham, kichThuoc, soLuong);
            Save(gioHang);
        }

        // Gộp giỏ tạm của khách vào giỏ tài khoản.
        public void MergeGuestIntoAccount()
        {
            if (ChuTaiKhoan == null || phien == null) return;

            List<CartItem> gioKhach = phien[KhoaPhien] as List<CartItem>;
            phien.Remove(KhoaPhien);
            if (gioKhach == null || gioKhach.Count == 0) return;

            Cart gioHang = Load();
            foreach (CartItem dong in gioKhach)
            {
                ProductItem sanPham = Catalog.Find(dong.Id);
                if (sanPham != null)
                    CartService.AddOrMerge(gioHang, sanPham, dong.Size, dong.Quantity);
            }

            Save(gioHang);
        }

        private List<CartItem> DocDong()
        {
            if (ChuTaiKhoan == null)
            {
                List<CartItem> gioKhach = phien == null
                    ? null
                    : phien[KhoaPhien] as List<CartItem>;
                return gioKhach == null
                    ? new List<CartItem>()
                    : new List<CartItem>(gioKhach);
            }

            return new SqlCartStore().Load(ChuTaiKhoan);
        }

        private void SaveLines(List<CartItem> cacDong)
        {
            bool rong = cacDong == null || cacDong.Count == 0;
            if (ChuTaiKhoan == null)
            {
                if (phien != null)
                    phien[KhoaPhien] = rong
                        ? null
                        : new List<CartItem>(cacDong);
                return;
            }

            new SqlCartStore().Save(ChuTaiKhoan, cacDong);
        }
    }

    // Kho yêu thích chọn session cho khách và SQL cho tài khoản.
    public sealed class WishlistStore
    {
        private const string KhoaPhien = "Wishlist";
        private readonly HttpSessionState phien;

        public WishlistStore(HttpSessionState phien)
        {
            this.phien = phien;
        }

        private string ChuTaiKhoan
        {
            get
            {
                string khoa = AuthHelper.UserKeyOf(phien);
                return string.IsNullOrEmpty(khoa) ? null : khoa;
            }
        }

        // Kiểm tra sản phẩm đã nằm trong danh sách yêu thích chưa.
        public bool Contains(int maSanPham)
        {
            return DocDanhSach().Contains(maSanPham);
        }

        // Đổi trạng thái yêu thích, trả về trạng thái sau khi đổi.
        public bool Toggle(int maSanPham)
        {
            HashSet<int> danhSach = DocDanhSach();
            bool daThem = !danhSach.Remove(maSanPham);
            if (daThem) danhSach.Add(maSanPham);
            LuuDanhSach(danhSach);
            return daThem;
        }

        // Xóa toàn bộ danh sách yêu thích.
        public void Clear()
        {
            LuuDanhSach(new HashSet<int>());
        }

        // Xóa danh sách tạm của khách.
        public void ClearGuest()
        {
            if (phien != null) phien.Remove(KhoaPhien);
        }

        // Gộp danh sách tạm của khách vào tài khoản.
        public void MergeGuestIntoAccount()
        {
            if (ChuTaiKhoan == null || phien == null) return;

            HashSet<int> yeuThichKhach = phien[KhoaPhien] as HashSet<int>;
            phien.Remove(KhoaPhien);
            if (yeuThichKhach == null || yeuThichKhach.Count == 0) return;

            HashSet<int> danhSach = DocDanhSach();
            danhSach.UnionWith(yeuThichKhach);
            LuuDanhSach(danhSach);
        }

        private HashSet<int> DocDanhSach()
        {
            if (ChuTaiKhoan == null)
            {
                HashSet<int> tam = phien == null
                    ? null
                    : phien[KhoaPhien] as HashSet<int>;
                return tam == null ? new HashSet<int>() : new HashSet<int>(tam);
            }

            return new SqlWishlistStore().Load(ChuTaiKhoan);
        }

        private void LuuDanhSach(HashSet<int> danhSach)
        {
            bool rong = danhSach == null || danhSach.Count == 0;
            if (ChuTaiKhoan == null)
            {
                if (phien != null)
                    phien[KhoaPhien] = rong
                        ? null
                        : new HashSet<int>(danhSach);
                return;
            }

            new SqlWishlistStore().Save(ChuTaiKhoan, danhSach);
        }
    }
}
