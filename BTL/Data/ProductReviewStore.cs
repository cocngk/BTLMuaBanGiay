using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    public static class ProductReviewStore
    {
        private const string ReviewsKey = "ProductReviews";

        private static List<ProductReview> Reviews
        {
            get
            {
                return ApplicationStore.GetOrCreate(
                    ReviewsKey,
                    () => new List<ProductReview>());
            }
        }

        // Lấy đánh giá theo sản phẩm
        public static List<ProductReview> GetByProduct(int productId)
        {
            lock (ApplicationStore.Gate)
            {
                return Reviews
                    .Where(r => r.ProductId == productId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
            }
        }

        // Tìm đánh giá của một khách hàng
        public static ProductReview Find(int productId, string userKey)
        {
            lock (ApplicationStore.Gate)
            {
                return Reviews.FirstOrDefault(r =>
                    r.ProductId == productId &&
                    string.Equals(r.UserKey, userKey,
                        StringComparison.OrdinalIgnoreCase));
            }
        }

        // Thêm mới hoặc sửa đánh giá
        public static void Save(ProductReview review)
        {
            lock (ApplicationStore.Gate)
            {
                ProductReview old = Find(
                    review.ProductId, review.UserKey);

                if (old == null)
                {
                    review.CreatedAt = DateTime.Now;
                    Reviews.Add(review);
                }
                else
                {
                    old.Rating = review.Rating;
                    old.Comment = review.Comment;
                    old.CreatedAt = DateTime.Now;
                    old.CustomerName = review.CustomerName;
                }
            }
        }

        // Tính điểm trung bình
        public static double GetAverage(int productId)
        {
            lock (ApplicationStore.Gate)
            {
                var list = Reviews
                    .Where(r => r.ProductId == productId)
                    .ToList();

                return list.Count == 0
                    ? 0
                    : list.Average(r => r.Rating);
            }
        }

        // Đếm số lượt đánh giá
        public static int Count(int productId)
        {
            lock (ApplicationStore.Gate)
            {
                return Reviews.Count(r => r.ProductId == productId);
            }
        }
    }
}
