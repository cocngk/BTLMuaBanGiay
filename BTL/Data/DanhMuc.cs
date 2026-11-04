using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    public enum SaveProductOutcome
    {
        Created, Updated, NotFound, InvalidName, InvalidBrand, InvalidPrice,
        InvalidStock, InvalidSizes, InvalidImage, InvalidDescription
    }

    public static class Catalog
    {
        private static readonly SqlCatalogStore kho = new SqlCatalogStore();
        // Thực hiện việc tìm.
        public static ProductItem Find(int ma) { return kho.Find(ma); }
        // Thực hiện việc toàn bộ.
        public static List<ProductItem> All() { return kho.All(); }
        // Thực hiện việc featured.
        public static List<ProductItem> Featured(int soLuong) { return soLuong <= 0 ? new List<ProductItem>() : All().Take(soLuong).ToList(); }

        // Thực hiện việc related chuyển đổi sang.
        public static List<ProductItem> RelatedTo(int ma, int soLuong)
        {
            ProductItem hienTai = Find(ma);
            if (hienTai == null || soLuong <= 0) return new List<ProductItem>();
            List<ProductItem> other = All().Where(x => x.Id != ma).ToList();
            return other.Where(x => x.Brand == hienTai.Brand).Concat(other.Where(x => x.Brand != hienTai.Brand)).OrderBy(x => x.Id).Take(soLuong).ToList();
        }

        // Lấy danh sách thương hiệu.
        public static List<string> Brands() { return All().Select(x => x.Brand).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList(); }
        // Thực hiện việc tìm kiếm.
        public static PagedResult<ProductItem> Search(CatalogQuery truyVan, int kichThuocTrang) { CatalogQuery giaTri = truyVan ?? new CatalogQuery(); return Paging.Paginate(giaTri.Apply(All()), giaTri.Page, kichThuocTrang); }
        // Thực hiện việc trừ stock.
        public static bool TakeStock(IEnumerable<KeyValuePair<int, int>> phanTu) { return kho.TakeStock(phanTu); }
        // Thực hiện việc hoàn lại stock.
        public static void RestoreStock(IEnumerable<KeyValuePair<int, int>> phanTu) { kho.RestoreStock(phanTu); }
        // Thực hiện việc xóa.
        public static bool Delete(int ma) { return kho.Delete(ma); }

        // Thực hiện việc lưu.
        public static SaveProductOutcome Save(ProductItem duLieuVao)
        {
            if (duLieuVao == null) return SaveProductOutcome.InvalidName;
            string ten = (duLieuVao.Name ?? "").Trim();
            string thuongHieu = (duLieuVao.Brand ?? "").Trim();
            string moTa = (duLieuVao.Description ?? "").Trim();
            string anh = (duLieuVao.ImageUrl ?? "").Trim();
            string anhLon = (duLieuVao.ImageUrlLarge ?? "").Trim();
            if (ten.Length == 0 || ten.Length > 120) return SaveProductOutcome.InvalidName;
            if (thuongHieu.Length == 0 || thuongHieu.Length > 60) return SaveProductOutcome.InvalidBrand;
            if (moTa.Length > 2000) return SaveProductOutcome.InvalidDescription;
            if (duLieuVao.Price <= 0) return SaveProductOutcome.InvalidPrice;
            if (duLieuVao.Stock < 0) return SaveProductOutcome.InvalidStock;
            string kichThuoc;
            if (!NormalizeSizes(duLieuVao.Sizes, out kichThuoc)) return SaveProductOutcome.InvalidSizes;
            if (anh.Length == 0 || anh.Length > 400) return SaveProductOutcome.InvalidImage;
            return kho.Save(new ProductItem
            {
                Id = duLieuVao.Id,
                Name = ten,
                Brand = thuongHieu,
                Description = moTa,
                Price = duLieuVao.Price,
                OldPrice = duLieuVao.OldPrice.HasValue && duLieuVao.OldPrice.Value > duLieuVao.Price ? duLieuVao.OldPrice : null,
                Stock = duLieuVao.Stock,
                Sizes = kichThuoc,
                ImageUrl = anh,
                ImageUrlLarge = anhLon.Length == 0 ? anh : anhLon,
                IsHot = duLieuVao.IsHot,
                Colourways = duLieuVao.Colourways
            });
        }

        // Thực hiện việc chuẩn hóa kích thước.
        private static bool NormalizeSizes(string chuoiGoc, out string giaTri)
        {
            List<int> danhSachKichThuoc = new List<int>();
            foreach (string phan in (chuoiGoc ?? "").Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int kichThuocSo;
                if (!int.TryParse(phan, out kichThuocSo) || kichThuocSo < 20 || kichThuocSo > 60) { giaTri = ""; return false; }
                if (!danhSachKichThuoc.Contains(kichThuocSo)) danhSachKichThuoc.Add(kichThuocSo);
            }
            if (danhSachKichThuoc.Count == 0) { giaTri = ""; return false; }
            danhSachKichThuoc.Sort();
            giaTri = string.Join(",", danhSachKichThuoc.Select(x => x.ToString()).ToArray());
            return true;
        }
    }
}
