using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Kết quả ghi danh mục. Kho chỉ nói lý do, trang tự quyết câu chữ — cùng kiểu
    /// với AddUserOutcome, để thông báo không nằm rải trong tầng dữ liệu.
    /// </summary>
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

    /// <summary>
    /// Deep Catalog module: nơi duy nhất trả lời câu hỏi về sản phẩm.
    /// Trang không còn giữ danh sách rồi tự lọc — mỗi nhu cầu là một method.
    /// Danh mục nằm trong Application (cấp ứng dụng) chứ không phải biến static:
    /// tồn kho bị trừ sau khi đặt hàng nên phải sống ngoài phạm vi một lớp.
    /// Khi có DB, thay phần thân của module này mà không đụng trang nào.
    /// </summary>
    public static class Catalog
    {
        // Nạp một lần vào Application, không phải mỗi lần có người hỏi.
        private static List<ProductItem> Items
        {
            get
            {
                return ApplicationStore.GetOrCreate(
                    ApplicationStore.ProductsKey,
                    ProductSeed.Items);
            }
        }

        /// <summary>Tìm theo id. Null khi không có — phía gọi tự quyết định "không tìm thấy".</summary>
        public static ProductItem Find(int id)
        {
            return Items.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Toàn bộ danh mục cho khu quản trị. Trả bản sao của list nhưng vẫn là tham
        /// chiếu sống tới từng sản phẩm: nơi gọi chỉ được đọc, mọi thay đổi phải đi
        /// qua <see cref="Save"/> — nếu không sẽ ghi mà không giữ khóa chung với
        /// TakeStock, và sửa dở dang vẫn dính dù người dùng bấm Huỷ.
        /// </summary>
        public static List<ProductItem> All()
        {
            lock (ApplicationStore.Gate)
            {
                return new List<ProductItem>(Items);
            }
        }

        /// <summary>N sản phẩm đầu tiên cho khối nổi bật ở trang chủ.</summary>
        public static List<ProductItem> Featured(int count)
        {
            if (count <= 0)
            {
                return new List<ProductItem>();
            }
            return Items.Take(count).ToList();
        }

        /// <summary>
        /// Sản phẩm liên quan: ưu tiên cùng thương hiệu, thiếu thì lấp từ phần còn lại.
        /// </summary>
        public static List<ProductItem> RelatedTo(int productId, int count)
        {
            if (count <= 0)
            {
                return new List<ProductItem>();
            }

            ProductItem product = Find(productId);
            if (product == null)
            {
                return new List<ProductItem>();
            }

            List<ProductItem> others = Items.Where(p => p.Id != product.Id).ToList();

            return others.Where(p => p.Brand == product.Brand)
                .Concat(others.Where(p => p.Brand != product.Brand))
                .OrderBy(p => p.Id)
                .Take(count)
                .ToList();
        }

        /// <summary>Thương hiệu có trong dữ liệu, dùng cho bộ lọc — không ghi cứng ở markup.</summary>
        public static List<string> Brands()
        {
            return Items.Select(p => p.Brand)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(b => b)
                .ToList();
        }

        /// <summary>Một cửa cho danh sách có lọc + phân trang.</summary>
        public static PagedResult<ProductItem> Search(CatalogQuery query, int pageSize)
        {
            if (query == null)
            {
                query = new CatalogQuery();
            }
            return Paging.Paginate(query.Apply(Items), query.Page, pageSize);
        }

        /// <summary>
        /// Trừ tồn kho cho cả đơn trong một lượt: đủ hàng mọi dòng thì mới trừ,
        /// thiếu một dòng là không trừ gì. Số lượng được cộng dồn theo sản phẩm
        /// trước khi so tồn kho — cùng một đôi chọn hai size khác nhau vẫn phải
        /// so với tổng tồn, không phải so từng dòng riêng lẻ.
        /// </summary>
        public static bool TakeStock(IEnumerable<KeyValuePair<int, int>> items)
        {
            Dictionary<int, int> wanted = new Dictionary<int, int>();
            if (items != null)
            {
                foreach (KeyValuePair<int, int> item in items)
                {
                    if (item.Value <= 0)
                    {
                        continue;
                    }

                    int current;
                    wanted[item.Key] = wanted.TryGetValue(item.Key, out current)
                        ? current + item.Value
                        : item.Value;
                }
            }

            if (wanted.Count == 0)
            {
                return true;
            }

            lock (ApplicationStore.Gate)
            {
                foreach (KeyValuePair<int, int> pair in wanted)
                {
                    ProductItem product = Find(pair.Key);
                    if (product == null || product.Stock < pair.Value)
                    {
                        return false;
                    }
                }

                foreach (KeyValuePair<int, int> pair in wanted)
                {
                    Find(pair.Key).Stock -= pair.Value;
                }

                return true;
            }
        }

        // ===== Ghi danh mục (khu quản trị) =====
        // Các method dưới đây là cửa ghi duy nhất. Trang sửa dựng một ProductItem
        // mới từ form rồi đưa vào Save; Catalog tự sao đúng những trường được phép,
        // nên form không thể ghi đè Colourways / IsHot / Id chỉ vì chúng có mặt
        // trên đối tượng. Mọi thao tác chạy trong ApplicationStore.Gate để không
        // đua với TakeStock lúc khách đặt hàng.

        /// <summary>
        /// Thêm mới (Id &lt;= 0) hoặc cập nhật (Id &gt; 0). Trả lý do khi từ chối để
        /// trang hiển thị đúng chỗ sai, thay vì một câu "dữ liệu không hợp lệ" chung.
        /// </summary>
        public static SaveProductOutcome Save(ProductItem input)
        {
            if (input == null)
            {
                return SaveProductOutcome.InvalidName;
            }

            string name = (input.Name ?? "").Trim();
            string brand = (input.Brand ?? "").Trim();
            string description = (input.Description ?? "").Trim();
            string image = (input.ImageUrl ?? "").Trim();
            string imageLarge = (input.ImageUrlLarge ?? "").Trim();

            if (name.Length == 0 || name.Length > 120)
            {
                return SaveProductOutcome.InvalidName;
            }

            if (brand.Length == 0 || brand.Length > 60)
            {
                return SaveProductOutcome.InvalidBrand;
            }

            if (description.Length > 2000)
            {
                return SaveProductOutcome.InvalidDescription;
            }

            if (input.Price <= 0)
            {
                return SaveProductOutcome.InvalidPrice;
            }

            if (input.Stock < 0)
            {
                return SaveProductOutcome.InvalidStock;
            }

            string sizes;
            if (!TryNormalizeSizes(input.Sizes, out sizes))
            {
                return SaveProductOutcome.InvalidSizes;
            }

            if (image.Length == 0 || image.Length > 400)
            {
                return SaveProductOutcome.InvalidImage;
            }

            // Giá gạch chỉ có nghĩa khi cao hơn giá bán; thấp hơn hoặc bằng thì bỏ,
            // để trang chi tiết không hiện "giảm giá" âm.
            decimal? oldPrice = input.OldPrice.HasValue && input.OldPrice.Value > input.Price
                ? input.OldPrice
                : null;

            ProductItem clean = new ProductItem
            {
                Id = input.Id,
                Name = name,
                Brand = brand,
                Description = description,
                Price = input.Price,
                OldPrice = oldPrice,
                Stock = input.Stock,
                Sizes = sizes,
                ImageUrl = image,
                ImageUrlLarge = imageLarge.Length == 0 ? image : imageLarge
            };

            lock (ApplicationStore.Gate)
            {
                if (input.Id <= 0)
                {
                    clean.Id = NextIdLocked();
                    Items.Add(clean);
                    return SaveProductOutcome.Created;
                }

                ProductItem target = Find(input.Id);
                if (target == null)
                {
                    return SaveProductOutcome.NotFound;
                }

                // Đổi ảnh thì bộ sưu tập màu cũ trỏ vào ảnh cũ — giữ lại sẽ thành
                // gallery sai (mở màu nào cũng ra ảnh của sản phẩm khác). Bỏ hẳn
                // để trang chi tiết hiện đúng một ảnh mới.
                bool imageChanged =
                    !string.Equals(target.ImageUrl, clean.ImageUrl, StringComparison.Ordinal) ||
                    !string.Equals(target.ImageUrlLarge, clean.ImageUrlLarge, StringComparison.Ordinal);

                target.Name = clean.Name;
                target.Brand = clean.Brand;
                target.Description = clean.Description;
                target.Price = clean.Price;
                target.OldPrice = clean.OldPrice;
                target.Stock = clean.Stock;
                target.Sizes = clean.Sizes;
                target.ImageUrl = clean.ImageUrl;
                target.ImageUrlLarge = clean.ImageUrlLarge;

                if (imageChanged)
                {
                    target.Colourways = null;
                }

                return SaveProductOutcome.Updated;
            }
        }

        /// <summary>
        /// Xoá hẳn. Đơn hàng cũ không ảnh hưởng vì OrderLine đã chụp lại tên và giá;
        /// giỏ của ai còn món này thì lúc thanh toán TakeStock trả false và trang báo
        /// "không còn bán" — đúng luồng đã có cho món hết hàng.
        /// </summary>
        public static bool Delete(int id)
        {
            lock (ApplicationStore.Gate)
            {
                ProductItem product = Find(id);
                return product != null && Items.Remove(product);
            }
        }

        /// <summary>
        /// Cộng lại tồn kho — dùng khi huỷ đơn. Cộng dồn theo sản phẩm giống TakeStock
        /// để cùng một đôi mua hai size khác nhau vẫn về đúng tổng.
        /// </summary>
        public static void RestoreStock(IEnumerable<KeyValuePair<int, int>> items)
        {
            if (items == null)
            {
                return;
            }

            Dictionary<int, int> wanted = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> item in items)
            {
                if (item.Value <= 0)
                {
                    continue;
                }

                int current;
                wanted[item.Key] = wanted.TryGetValue(item.Key, out current)
                    ? current + item.Value
                    : item.Value;
            }

            if (wanted.Count == 0)
            {
                return;
            }

            lock (ApplicationStore.Gate)
            {
                foreach (KeyValuePair<int, int> pair in wanted)
                {
                    // Sản phẩm có thể đã bị xoá trong lúc đơn đang chờ: bỏ qua dòng đó,
                    // không dựng lại một sản phẩm "ma" chỉ để cộng tồn kho.
                    ProductItem product = Find(pair.Key);
                    if (product != null)
                    {
                        product.Stock += pair.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Id kế tiếp = lớn nhất + 1. Không dùng Count + 1: xoá một sản phẩm rồi thêm
        /// mới sẽ trùng id với sản phẩm cuối, và Find sẽ luôn trả về sản phẩm đầu tiên.
        /// </summary>
        private static int NextIdLocked()
        {
            int max = 0;
            foreach (ProductItem item in Items)
            {
                if (item.Id > max)
                {
                    max = item.Id;
                }
            }
            return max + 1;
        }

        /// <summary>
        /// Chuẩn hoá "38, 39;40 41" thành "38,39,40,41": bỏ khoảng trắng, bỏ trùng,
        /// sắp tăng dần. Size phải là số trong khoảng đi giày thật (20–60) — chuỗi
        /// tự do sẽ làm bộ lọc size ở trang Sản phẩm không bao giờ khớp.
        /// </summary>
        private static bool TryNormalizeSizes(string raw, out string normalized)
        {
            normalized = "";
            string[] parts = (raw ?? "").Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<int> sizes = new List<int>();
            foreach (string part in parts)
            {
                int size;
                if (!int.TryParse(part.Trim(), out size) || size < 20 || size > 60)
                {
                    return false;
                }

                if (!sizes.Contains(size))
                {
                    sizes.Add(size);
                }
            }

            if (sizes.Count == 0)
            {
                return false;
            }

            sizes.Sort();
            normalized = string.Join(",", sizes.Select(s => s.ToString()).ToArray());
            return true;
        }
    }
}
