using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Deep Catalog module: nơi duy nhất trả lời câu hỏi về sản phẩm.
    /// Trang không còn giữ danh sách rồi tự lọc — mỗi nhu cầu là một method.
    /// Implementation hiện là seed trong bộ nhớ; khi có DB, thay phần thân
    /// của module này mà không đụng trang nào.
    /// </summary>
    public static class Catalog
    {
        // Seed dựng một lần, không phải mỗi lần có người hỏi.
        private static readonly List<ProductItem> Items = ProductSeed.Items();

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
    }
}
