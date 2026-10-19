using System.Collections.Generic;
using System.Web.SessionState;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Thin persistence adapter: the single seam through which the Cart
    /// module touches Session. All cart reads/writes go through here —
    /// no direct Session["Cart"] access anywhere else.
    /// </summary>
    public sealed class SessionCartStore
    {
        private const string Key = "Cart";
        private readonly HttpSessionState session;

        public SessionCartStore(HttpSessionState session)
        {
            this.session = session;
        }

        public Cart Load()
        {
            return new Cart(session[Key] as List<CartItem>);
        }

        public void Save(Cart cart)
        {
            session[Key] = cart.Lines != null
                ? new List<CartItem>(cart.Lines)
                : null;
        }

        public void Clear()
        {
            session.Remove(Key);
        }

        /// <summary>
        /// Thêm một sản phẩm từ Catalog. Tồn kho do CartService đọc — caller
        /// không còn phải tự đi tìm stock rồi truyền vào.
        /// </summary>
        public void AddProduct(ProductItem product, string size, int qty)
        {
            Cart cart = Load();
            CartService.AddOrMerge(cart, product, size, qty);
            Save(cart);
        }
    }
}
