using System;
using System.Collections.Generic;
using System.Web.SessionState;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Seam duy nhất cho giỏ hàng — không ai chạm Session["Cart"] hay Application["Carts"]
    /// trực tiếp. Chỗ lưu phụ thuộc người đang xem:
    /// - Đã đăng nhập: Application, khóa theo tên đăng nhập (UserKey) → đăng xuất, đóng
    ///   trình duyệt hay đổi máy rồi đăng nhập lại vẫn còn giỏ.
    /// - Khách vãng lai: Session — chưa có tài khoản thì không có chỗ nào bền hơn để gắn.
    /// Giỏ của tài khoản A không bao giờ hiện cho tài khoản B vì khóa là UserKey, không
    /// phải phiên trình duyệt.
    /// </summary>
    public sealed class CartStore
    {
        private const string SessionKey = "Cart";

        private readonly HttpSessionState session;

        public CartStore(HttpSessionState session)
        {
            this.session = session;
        }

        /// <summary>Tên đăng nhập của chủ giỏ; null khi là khách vãng lai.</summary>
        private string Owner
        {
            get
            {
                string key = AuthHelper.UserKeyOf(session);
                return key.Length > 0 ? key : null;
            }
        }

        public Cart Load()
        {
            return new Cart(Lines());
        }

        public void Save(Cart cart)
        {
            SaveLines(cart != null && cart.Lines != null ? new List<CartItem>(cart.Lines) : null);
        }

        /// <summary>Xoá giỏ đang dùng (của tài khoản nếu đã đăng nhập). Dùng sau khi đặt hàng xong.</summary>
        public void Clear()
        {
            SaveLines(null);
        }

        /// <summary>
        /// Chỉ xoá bản trong session. Gọi lúc đăng xuất: dữ liệu của tài khoản nằm ở
        /// Application nên phải giữ, chỉ bỏ phần của phiên trình duyệt này đi.
        /// </summary>
        public void ClearGuest()
        {
            session.Remove(SessionKey);
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

        /// <summary>
        /// Gọi ngay sau khi đăng nhập: gộp giỏ khách vãng lai (Session) vào giỏ tài khoản
        /// rồi xoá bản trong session. Nhờ vậy khách thêm hàng trước khi đăng nhập không
        /// mất hàng, mà giỏ của người đăng nhập trước cũng không bị lẫn sang.
        /// </summary>
        public void MergeGuestIntoAccount()
        {
            if (Owner == null)
            {
                return;
            }

            List<CartItem> guest = session[SessionKey] as List<CartItem>;
            session.Remove(SessionKey);
            if (guest == null || guest.Count == 0)
            {
                return;
            }

            Cart cart = Load();
            foreach (CartItem line in guest)
            {
                ProductItem product = Catalog.Find(line.Id);
                if (product == null)
                {
                    continue; // sản phẩm đã bị bỏ khỏi catalog
                }
                CartService.AddOrMerge(cart, product, line.Size, line.Quantity);
            }
            Save(cart);
        }

        /// <summary>
        /// Bản sao của các dòng đang lưu; null khi giỏ rỗng. Luôn trả bản sao: trả thẳng
        /// list trong Application thì người gọi sửa Cart là sửa dữ liệu chung mà không
        /// đi qua khóa.
        /// </summary>
        private List<CartItem> Lines()
        {
            string owner = Owner;
            if (owner == null)
            {
                List<CartItem> guest = session[SessionKey] as List<CartItem>;
                return guest != null && guest.Count > 0 ? new List<CartItem>(guest) : null;
            }

            lock (ApplicationStore.Gate)
            {
                List<CartItem> saved;
                return Carts().TryGetValue(owner, out saved) && saved != null && saved.Count > 0
                    ? new List<CartItem>(saved)
                    : null;
            }
        }

        private void SaveLines(List<CartItem> lines)
        {
            bool empty = lines == null || lines.Count == 0;

            string owner = Owner;
            if (owner == null)
            {
                session[SessionKey] = empty ? null : new List<CartItem>(lines);
                return;
            }

            lock (ApplicationStore.Gate)
            {
                Dictionary<string, List<CartItem>> carts = Carts();
                if (empty)
                {
                    // Giỏ rỗng thì bỏ luôn khóa: đặt hàng xong là Application sạch,
                    // không phình theo số tài khoản từng ghé.
                    carts.Remove(owner);
                }
                else
                {
                    carts[owner] = new List<CartItem>(lines);
                }
            }
        }

        private static Dictionary<string, List<CartItem>> Carts()
        {
            return ApplicationStore.GetOrCreate(
                ApplicationStore.CartsKey,
                () => new Dictionary<string, List<CartItem>>(StringComparer.OrdinalIgnoreCase));
        }
    }
}
