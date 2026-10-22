using System;
using System.Collections.Generic;
using System.Web.SessionState;
using ShoeStore.Helpers;

namespace ShoeStore.Data
{
    /// <summary>
    /// Seam duy nhất cho danh sách yêu thích. Cùng luật với CartStore: đã đăng nhập
    /// thì ở Application theo UserKey (đăng xuất rồi đăng nhập lại vẫn còn tim), khách
    /// vãng lai thì ở Session. Khóa theo tài khoản chứ không theo phiên trình duyệt,
    /// nên người đăng nhập sau trên cùng máy không thấy tim của người trước.
    /// </summary>
    public sealed class WishlistStore
    {
        private const string SessionKey = "Wishlist";

        private readonly HttpSessionState session;

        public WishlistStore(HttpSessionState session)
        {
            this.session = session;
        }

        /// <summary>Tên đăng nhập của chủ danh sách; null khi là khách vãng lai.</summary>
        private string Owner
        {
            get
            {
                string key = AuthHelper.UserKeyOf(session);
                return key.Length > 0 ? key : null;
            }
        }

        public bool Contains(int productId)
        {
            return All().Contains(productId);
        }

        /// <summary>Bật/tắt một mục. Trả về true nếu sau khi bật.</summary>
        public bool Toggle(int productId)
        {
            HashSet<int> items = All();
            bool liked = !items.Remove(productId);
            if (liked)
            {
                items.Add(productId);
            }
            Save(items);
            return liked;
        }

        /// <summary>Xoá danh sách đang dùng (của tài khoản nếu đã đăng nhập).</summary>
        public void Clear()
        {
            Save(new HashSet<int>());
        }

        /// <summary>
        /// Chỉ xoá bản trong session. Gọi lúc đăng xuất: tim của tài khoản nằm ở
        /// Application nên phải giữ.
        /// </summary>
        public void ClearGuest()
        {
            session.Remove(SessionKey);
        }

        /// <summary>Gộp tim của khách vãng lai vào tài khoản ngay sau khi đăng nhập.</summary>
        public void MergeGuestIntoAccount()
        {
            if (Owner == null)
            {
                return;
            }

            HashSet<int> guest = session[SessionKey] as HashSet<int>;
            session.Remove(SessionKey);
            if (guest == null || guest.Count == 0)
            {
                return;
            }

            HashSet<int> items = All();
            items.UnionWith(guest);
            Save(items);
        }

        /// <summary>Bản sao — không trả thẳng HashSet đang lưu.</summary>
        private HashSet<int> All()
        {
            string owner = Owner;
            if (owner == null)
            {
                HashSet<int> guest = session[SessionKey] as HashSet<int>;
                return guest != null ? new HashSet<int>(guest) : new HashSet<int>();
            }

            lock (ApplicationStore.Gate)
            {
                HashSet<int> saved;
                return Wishlists().TryGetValue(owner, out saved) && saved != null
                    ? new HashSet<int>(saved)
                    : new HashSet<int>();
            }
        }

        private void Save(HashSet<int> items)
        {
            bool empty = items == null || items.Count == 0;

            string owner = Owner;
            if (owner == null)
            {
                session[SessionKey] = empty ? null : new HashSet<int>(items);
                return;
            }

            lock (ApplicationStore.Gate)
            {
                Dictionary<string, HashSet<int>> lists = Wishlists();
                if (empty)
                {
                    lists.Remove(owner);
                }
                else
                {
                    lists[owner] = new HashSet<int>(items);
                }
            }
        }

        private static Dictionary<string, HashSet<int>> Wishlists()
        {
            return ApplicationStore.GetOrCreate(
                ApplicationStore.WishlistsKey,
                () => new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase));
        }
    }
}
