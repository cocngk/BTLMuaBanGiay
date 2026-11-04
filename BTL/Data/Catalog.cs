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
        private static List<ProductItem> Items
        {
            get { return ApplicationStore.GetOrCreate(ApplicationStore.ProductsKey, ProductSeed.Items); }
        }

        public static ProductItem Find(int id) { return Items.FirstOrDefault(p => p.Id == id); }

        public static List<ProductItem> All()
        {
            lock (ApplicationStore.Gate) return new List<ProductItem>(Items);
        }

        public static List<ProductItem> Featured(int count)
        {
            return count <= 0 ? new List<ProductItem>() : Items.Take(count).ToList();
        }

        public static List<ProductItem> RelatedTo(int id, int count)
        {
            ProductItem current = Find(id);
            if (current == null || count <= 0) return new List<ProductItem>();
            List<ProductItem> other = Items.Where(p => p.Id != id).ToList();
            return other.Where(p => p.Brand == current.Brand).Concat(other.Where(p => p.Brand != current.Brand)).OrderBy(p => p.Id).Take(count).ToList();
        }

        public static List<string> Brands()
        {
            return Items.Select(p => p.Brand).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        }

        public static PagedResult<ProductItem> Search(CatalogQuery query, int pageSize)
        {
            return Paging.Paginate((query ?? new CatalogQuery()).Apply(Items), query == null ? 1 : query.Page, pageSize);
        }

        public static bool TakeStock(IEnumerable<KeyValuePair<int, int>> input)
        {
            Dictionary<int, int> wanted = Totals(input);
            lock (ApplicationStore.Gate)
            {
                foreach (KeyValuePair<int, int> item in wanted)
                {
                    ProductItem product = Find(item.Key);
                    if (product == null || product.Stock < item.Value) return false;
                }
                foreach (KeyValuePair<int, int> item in wanted) Find(item.Key).Stock -= item.Value;
                return true;
            }
        }

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

            ProductItem clean = new ProductItem
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
                ImageUrlLarge = large.Length == 0 ? image : large
            };

            lock (ApplicationStore.Gate)
            {
                if (input.Id <= 0)
                {
                    clean.Id = NextId();
                    Items.Add(clean);
                    return SaveProductOutcome.Created;
                }

                ProductItem target = Find(input.Id);
                if (target == null) return SaveProductOutcome.NotFound;
                bool changed = target.ImageUrl != clean.ImageUrl || target.ImageUrlLarge != clean.ImageUrlLarge;
                target.Name = clean.Name;
                target.Brand = clean.Brand;
                target.Description = clean.Description;
                target.Price = clean.Price;
                target.OldPrice = clean.OldPrice;
                target.Stock = clean.Stock;
                target.Sizes = clean.Sizes;
                target.ImageUrl = clean.ImageUrl;
                target.ImageUrlLarge = clean.ImageUrlLarge;
                if (changed) target.Colourways = null;
                return SaveProductOutcome.Updated;
            }
        }

        public static bool Delete(int id)
        {
            lock (ApplicationStore.Gate)
            {
                ProductItem item = Find(id);
                return item != null && Items.Remove(item);
            }
        }

        public static void RestoreStock(IEnumerable<KeyValuePair<int, int>> input)
        {
            Dictionary<int, int> wanted = Totals(input);
            lock (ApplicationStore.Gate)
            {
                foreach (KeyValuePair<int, int> item in wanted)
                {
                    ProductItem product = Find(item.Key);
                    if (product != null) product.Stock += item.Value;
                }
            }
        }

        private static Dictionary<int, int> Totals(IEnumerable<KeyValuePair<int, int>> input)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            if (input == null) return result;
            foreach (KeyValuePair<int, int> item in input)
            {
                if (item.Value <= 0) continue;
                result[item.Key] = result.ContainsKey(item.Key) ? result[item.Key] + item.Value : item.Value;
            }
            return result;
        }

        private static int NextId()
        {
            return Items.Count == 0 ? 1 : Items.Max(p => p.Id) + 1;
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
