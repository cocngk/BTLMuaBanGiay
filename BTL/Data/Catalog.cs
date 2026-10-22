using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
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
    }
}
