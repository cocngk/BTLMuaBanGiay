using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>Một dòng giỏ đã kèm trần mua được — dữ liệu để trang Giỏ hàng render.</summary>
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

    /// <summary>
    /// Deep Cart module: nơi duy nhất join giỏ với Catalog để biết tồn kho.
    /// Cart thuần vẫn giữ luật clamp; phần cần stock nằm ở đây, nên trang
    /// không còn tự tìm tồn kho rồi truyền vào Cart.
    /// </summary>
    public static class CartService
    {
        public static void AddOrMerge(Cart cart, ProductItem product, string size, int qty)
        {
            if (cart == null || product == null)
            {
                return;
            }
            cart.AddOrMerge(product.Id, product.Name, product.Price, size, qty, product.ImageUrl, product.Stock);
        }

        /// <summary>Đổi số lượng một dòng. Trả về true nếu số lượng thực sự đổi.</summary>
        public static bool SetQuantity(Cart cart, int id, string size, int qty)
        {
            if (cart == null)
            {
                return false;
            }

            CartItem line = cart.Lines.FirstOrDefault(c => c.Id == id && c.Size == size);
            if (line == null)
            {
                return false;
            }

            int before = line.Quantity;
            cart.SetQuantity(id, size, qty, StockOf(id));
            return line.Quantity != before;
        }

        /// <summary>Số lớn nhất còn mua được của sản phẩm, tính mọi size đang có trong giỏ.</summary>
        public static int MaxBuyable(Cart cart, int productId)
        {
            int remaining = cart.RemainingToBuy(productId, StockOf(productId), null);
            return remaining > 0 ? remaining : 1;
        }

        public static List<CartLineView> Lines(Cart cart)
        {
            return cart.Lines.Select(l => new CartLineView
            {
                Id = l.Id,
                Name = l.Name,
                Price = l.Price,
                Size = l.Size,
                ImageUrl = l.ImageUrl,
                Quantity = l.Quantity,
                // Luôn >= 1: nếu size khác đã chiếm hết tồn kho thì max="0"
                // sẽ làm trình duyệt từ chối mọi giá trị hợp lệ.
                Max = Math.Max(1, cart.RemainingToBuy(l.Id, StockOf(l.Id), l.Size))
            }).ToList();
        }

        /// <summary>Tồn kho của sản phẩm; sản phẩm không còn trong catalog thì không chặn giỏ.</summary>
        private static int StockOf(int productId)
        {
            ProductItem product = Catalog.Find(productId);
            return product != null ? product.Stock : int.MaxValue;
        }
    }
}
