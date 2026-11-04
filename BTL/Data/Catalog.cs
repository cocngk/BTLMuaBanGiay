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
        private static readonly SqlCatalogStore store = new SqlCatalogStore();
        public static ProductItem Find(int id) { return store.Find(id); }
        public static List<ProductItem> All() { return store.All(); }
        public static List<ProductItem> Featured(int count) { return count <= 0 ? new List<ProductItem>() : All().Take(count).ToList(); }

        public static List<ProductItem> RelatedTo(int id, int count)
        {
            ProductItem current = Find(id);
            if (current == null || count <= 0) return new List<ProductItem>();
            List<ProductItem> other = All().Where(x => x.Id != id).ToList();
            return other.Where(x => x.Brand == current.Brand).Concat(other.Where(x => x.Brand != current.Brand)).OrderBy(x => x.Id).Take(count).ToList();
        }

        public static List<string> Brands() { return All().Select(x => x.Brand).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList(); }
        public static PagedResult<ProductItem> Search(CatalogQuery query, int pageSize) { CatalogQuery value = query ?? new CatalogQuery(); return Paging.Paginate(value.Apply(All()), value.Page, pageSize); }
        public static bool TakeStock(IEnumerable<KeyValuePair<int, int>> items) { return store.TakeStock(items); }
        public static void RestoreStock(IEnumerable<KeyValuePair<int, int>> items) { store.RestoreStock(items); }
        public static bool Delete(int id) { return store.Delete(id); }

        public static SaveProductOutcome Save(ProductItem input)
        {
            if (input == null) return SaveProductOutcome.InvalidName;
            string name = (input.Name ?? "").Trim();
            string brand = (input.Brand ?? "").Trim();
            string description = (input.Description ?? "").Trim();
            string image = (input.ImageUrl ?? "").Trim();
            string large = (input.ImageUrlLarge ?? "").Trim();
            if (name.Length == 0 || name.Length > 120) return SaveProductOutcome.InvalidName;
            if (brand.Length == 0 || brand.Length > 60) return SaveProductOutcome.InvalidBrand;
            if (description.Length > 2000) return SaveProductOutcome.InvalidDescription;
            if (input.Price <= 0) return SaveProductOutcome.InvalidPrice;
            if (input.Stock < 0) return SaveProductOutcome.InvalidStock;
            string sizes;
            if (!NormalizeSizes(input.Sizes, out sizes)) return SaveProductOutcome.InvalidSizes;
            if (image.Length == 0 || image.Length > 400) return SaveProductOutcome.InvalidImage;
            return store.Save(new ProductItem
            {
                Id = input.Id,
                Name = name,
                Brand = brand,
                Description = description,
                Price = input.Price,
                OldPrice = input.OldPrice.HasValue && input.OldPrice.Value > input.Price ? input.OldPrice : null,
                Stock = input.Stock,
                Sizes = sizes,
                ImageUrl = image,
                ImageUrlLarge = large.Length == 0 ? image : large,
                IsHot = input.IsHot,
                Colourways = input.Colourways
            });
        }

        private static bool NormalizeSizes(string raw, out string value)
        {
            List<int> sizes = new List<int>();
            foreach (string part in (raw ?? "").Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int size;
                if (!int.TryParse(part, out size) || size < 20 || size > 60) { value = ""; return false; }
                if (!sizes.Contains(size)) sizes.Add(size);
            }
            if (sizes.Count == 0) { value = ""; return false; }
            sizes.Sort();
            value = string.Join(",", sizes.Select(x => x.ToString()).ToArray());
            return true;
        }
    }
}
