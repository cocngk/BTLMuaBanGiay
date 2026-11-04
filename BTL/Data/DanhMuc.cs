using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    public enum SaveProductOutcome
    {
        Created,
        Updated,
        NotFound,
        InvalidName,
        InvalidBrand,
        InvalidPrice,
        InvalidStock,
        InvalidSizes,
        InvalidImage,
        InvalidDescription
    }

    // Facade sản phẩm dùng chung cho các trang WebForms.
    public static class Catalog
    {
        private static readonly SqlCatalogStore Kho = new SqlCatalogStore();

        // Tìm một sản phẩm theo mã.
        public static ProductItem Find(int maSanPham)
        {
            return Kho.Find(maSanPham);
        }

        // Lấy toàn bộ sản phẩm.
        public static List<ProductItem> All()
        {
            return Kho.All();
        }

        // Lấy sản phẩm nổi bật, sau đó bổ sung sản phẩm thường nếu chưa đủ.
        public static List<ProductItem> Featured(int soLuong)
        {
            if (soLuong <= 0) return new List<ProductItem>();

            List<ProductItem> tatCa = All();
            return tatCa.Where(sanPham => sanPham.IsHot)
                .Concat(tatCa.Where(sanPham => !sanPham.IsHot))
                .Take(soLuong)
                .ToList();
        }

        // Lấy sản phẩm cùng thương hiệu trước các sản phẩm liên quan khác.
        public static List<ProductItem> RelatedTo(int maSanPham, int soLuong)
        {
            if (soLuong <= 0) return new List<ProductItem>();

            ProductItem hienTai = Find(maSanPham);
            if (hienTai == null) return new List<ProductItem>();

            List<ProductItem> khac = All()
                .Where(sanPham => sanPham.Id != maSanPham)
                .ToList();

            return khac.Where(sanPham => GiongThuongHieu(sanPham, hienTai))
                .Concat(khac.Where(sanPham => !GiongThuongHieu(sanPham, hienTai)))
                .OrderBy(sanPham => sanPham.Id)
                .Take(soLuong)
                .ToList();
        }

        // Lấy danh sách thương hiệu không trùng nhau.
        public static List<string> Brands()
        {
            return All()
                .Select(sanPham => sanPham.Brand)
                .Where(thuongHieu => !string.IsNullOrWhiteSpace(thuongHieu))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(thuongHieu => thuongHieu)
                .ToList();
        }

        // Tìm kiếm, lọc và phân trang sản phẩm.
        public static PagedResult<ProductItem> Search(CatalogQuery truyVan,
            int kichThuocTrang)
        {
            CatalogQuery boLoc = truyVan ?? new CatalogQuery();
            return Paging.Paginate(boLoc.Apply(All()), boLoc.Page, kichThuocTrang);
        }

        // Trừ số lượng tồn kho khi tạo đơn.
        public static bool TakeStock(IEnumerable<KeyValuePair<int, int>> cacDong)
        {
            return Kho.TakeStock(cacDong);
        }

        // Hoàn số lượng tồn kho khi hủy đơn.
        public static void RestoreStock(IEnumerable<KeyValuePair<int, int>> cacDong)
        {
            Kho.RestoreStock(cacDong);
        }

        // Xóa sản phẩm theo mã.
        public static bool Delete(int maSanPham)
        {
            return Kho.Delete(maSanPham);
        }

        // Kiểm tra và lưu sản phẩm quản trị nhập vào.
        public static SaveProductOutcome Save(ProductItem duLieuVao)
        {
            if (duLieuVao == null) return SaveProductOutcome.InvalidName;

            string ten = (duLieuVao.Name ?? "").Trim();
            string thuongHieu = (duLieuVao.Brand ?? "").Trim();
            string moTa = (duLieuVao.Description ?? "").Trim();
            string anhNho = (duLieuVao.ImageUrl ?? "").Trim();
            string anhLon = (duLieuVao.ImageUrlLarge ?? "").Trim();

            if (ten.Length == 0 || ten.Length > 120)
                return SaveProductOutcome.InvalidName;
            if (thuongHieu.Length == 0 || thuongHieu.Length > 60)
                return SaveProductOutcome.InvalidBrand;
            if (moTa.Length > 2000)
                return SaveProductOutcome.InvalidDescription;
            if (duLieuVao.Price <= 0)
                return SaveProductOutcome.InvalidPrice;
            if (duLieuVao.Stock < 0)
                return SaveProductOutcome.InvalidStock;

            string kichThuocDaChuanHoa;
            if (!NormalizeSizes(duLieuVao.Sizes, out kichThuocDaChuanHoa))
                return SaveProductOutcome.InvalidSizes;
            if (anhNho.Length == 0 || anhNho.Length > 400)
                return SaveProductOutcome.InvalidImage;
            if (anhLon.Length > 400)
                return SaveProductOutcome.InvalidImage;

            ProductItem sanPham = new ProductItem
            {
                Id = duLieuVao.Id,
                Name = ten,
                Brand = thuongHieu,
                Description = moTa,
                Price = duLieuVao.Price,
                OldPrice = duLieuVao.OldPrice.HasValue
                    && duLieuVao.OldPrice.Value > duLieuVao.Price
                    ? duLieuVao.OldPrice
                    : null,
                Stock = duLieuVao.Stock,
                Sizes = kichThuocDaChuanHoa,
                ImageUrl = anhNho,
                ImageUrlLarge = anhLon.Length == 0 ? anhNho : anhLon,
                IsHot = duLieuVao.IsHot,
                Colourways = duLieuVao.Colourways
            };

            return Kho.Save(sanPham);
        }

        // Chuẩn hóa cỡ giày thành chuỗi số tăng dần, không trùng.
        private static bool NormalizeSizes(string chuoiGoc, out string ketQua)
        {
            List<int> cacCo = new List<int>();
            string[] phanTach = (chuoiGoc ?? "").Split(
                new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string phan in phanTach)
            {
                int coGiay;
                if (!int.TryParse(phan, out coGiay) || coGiay < 20 || coGiay > 60)
                {
                    ketQua = "";
                    return false;
                }

                if (!cacCo.Contains(coGiay)) cacCo.Add(coGiay);
            }

            if (cacCo.Count == 0)
            {
                ketQua = "";
                return false;
            }

            cacCo.Sort();
            ketQua = string.Join(",", cacCo.Select(co => co.ToString()).ToArray());
            return true;
        }

        private static bool GiongThuongHieu(ProductItem trai, ProductItem phai)
        {
            return string.Equals(trai.Brand, phai.Brand,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
