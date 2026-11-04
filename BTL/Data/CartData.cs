using System;
using System.Collections.Generic;
using System.Linq;
using ShoeStore.Models;
using System.Web.SessionState;
using ShoeStore.Helpers;


namespace ShoeStore.Data
{
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

    public static class CartService
    {
        public static void AddOrMerge(Cart cart, ProductItem product, string size, int qty)
        {
            if (cart != null && product != null) cart.AddOrMerge(product.Id, product.Name, product.Price, size, qty, product.ImageUrl, product.Stock);
        }

        public static bool SetQuantity(Cart cart, int id, string size, int qty)
        {
            if (cart == null) return false;
            CartItem line = cart.Lines.FirstOrDefault(x => x.Id == id && x.Size == size);
            if (line == null) return false;
            int old = line.Quantity;
            cart.SetQuantity(id, size, qty, StockOf(id));
            return old != line.Quantity;
        }

        public static int MaxBuyable(Cart cart, int id)
        {
            int value = cart.RemainingToBuy(id, StockOf(id), null);
            return value > 0 ? value : 1;
        }

        public static List<CartLineView> Lines(Cart cart)
        {
            return cart.Lines.Select(x => new CartLineView
            {
                Id = x.Id, Name = x.Name, Price = x.Price, Size = x.Size,
                ImageUrl = x.ImageUrl, Quantity = x.Quantity,
                Max = Math.Max(1, cart.RemainingToBuy(x.Id, StockOf(x.Id), x.Size))
            }).ToList();
        }

        private static int StockOf(int id)
        {
            ProductItem product = Catalog.Find(id);
            return product == null ? int.MaxValue : product.Stock;
        }
    }
}



namespace ShoeStore.Data
{
    public sealed class CartStore
    {
        private const string SessionKey = "Cart";
        private readonly HttpSessionState session;

        public CartStore(HttpSessionState session) { this.session = session; }

        private string Owner
        {
            get
            {
                string value = AuthHelper.UserKeyOf(session);
                return value.Length == 0 ? null : value;
            }
        }

        public Cart Load() { return new Cart(Read()); }
        public void Save(Cart cart) { SaveLines(cart == null ? null : cart.Lines.ToList()); }
        public void Clear() { SaveLines(null); }
        public void ClearGuest() { session.Remove(SessionKey); }

        public void AddProduct(ProductItem product, string size, int qty)
        {
            Cart cart = Load();
            CartService.AddOrMerge(cart, product, size, qty);
            Save(cart);
        }

        public void MergeGuestIntoAccount()
        {
            if (Owner == null) return;
            List<CartItem> guest = session[SessionKey] as List<CartItem>;
            session.Remove(SessionKey);
            if (guest == null) return;
            Cart cart = Load();
            foreach (CartItem item in guest)
            {
                ProductItem product = Catalog.Find(item.Id);
                if (product != null) CartService.AddOrMerge(cart, product, item.Size, item.Quantity);
            }
            Save(cart);
        }

        private List<CartItem> Read()
        {
            if (Owner == null)
            {
                List<CartItem> guest = session[SessionKey] as List<CartItem>;
                return guest == null ? null : new List<CartItem>(guest);
            }

            lock (ApplicationStore.Gate)
            {
                List<CartItem> saved;
                return Carts().TryGetValue(Owner, out saved) && saved != null ? new List<CartItem>(saved) : null;
            }
        }

        private void SaveLines(List<CartItem> lines)
        {
            bool empty = lines == null || lines.Count == 0;
            if (Owner == null)
            {
                session[SessionKey] = empty ? null : new List<CartItem>(lines);
                return;
            }

            lock (ApplicationStore.Gate)
            {
                if (empty) Carts().Remove(Owner);
                else Carts()[Owner] = new List<CartItem>(lines);
            }
        }

        private static Dictionary<string, List<CartItem>> Carts()
        {
            return ApplicationStore.GetOrCreate(ApplicationStore.CartsKey, () => new Dictionary<string, List<CartItem>>(System.StringComparer.OrdinalIgnoreCase));
        }
    }
}



namespace ShoeStore.Data
{
    public sealed class WishlistStore
    {
        private const string SessionKey = "Wishlist";
        private readonly HttpSessionState session;

        public WishlistStore(HttpSessionState session) { this.session = session; }

        private string Owner
        {
            get
            {
                string value = AuthHelper.UserKeyOf(session);
                return value.Length == 0 ? null : value;
            }
        }

        public bool Contains(int id) { return Read().Contains(id); }

        public bool Toggle(int id)
        {
            HashSet<int> items = Read();
            bool liked = !items.Remove(id);
            if (liked) items.Add(id);
            Save(items);
            return liked;
        }

        public void Clear() { Save(new HashSet<int>()); }
        public void ClearGuest() { session.Remove(SessionKey); }

        public void MergeGuestIntoAccount()
        {
            if (Owner == null) return;
            HashSet<int> guest = session[SessionKey] as HashSet<int>;
            session.Remove(SessionKey);
            if (guest == null) return;
            HashSet<int> items = Read();
            items.UnionWith(guest);
            Save(items);
        }

        private HashSet<int> Read()
        {
            if (Owner == null)
            {
                HashSet<int> guest = session[SessionKey] as HashSet<int>;
                return guest == null ? new HashSet<int>() : new HashSet<int>(guest);
            }

            lock (ApplicationStore.Gate)
            {
                HashSet<int> saved;
                return Lists().TryGetValue(Owner, out saved) && saved != null ? new HashSet<int>(saved) : new HashSet<int>();
            }
        }

        private void Save(HashSet<int> items)
        {
            bool empty = items == null || items.Count == 0;
            if (Owner == null)
            {
                session[SessionKey] = empty ? null : new HashSet<int>(items);
                return;
            }

            lock (ApplicationStore.Gate)
            {
                if (empty) Lists().Remove(Owner);
                else Lists()[Owner] = new HashSet<int>(items);
            }
        }

        private static Dictionary<string, HashSet<int>> Lists()
        {
            return ApplicationStore.GetOrCreate(ApplicationStore.WishlistsKey, () => new Dictionary<string, HashSet<int>>(System.StringComparer.OrdinalIgnoreCase));
        }
    }
}

