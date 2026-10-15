using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ShoeStore.Helpers
{
    /// <summary>
    /// Single place for cart session access + cart mutations
    /// (was copy-pasted in TrangChu / SanPham / ChiTietSanPham).
    /// Cart lines are keyed by (product Id, Size).
    /// </summary>
    public static class CartHelper
    {
        public static bool IsLoggedIn()
        {
            return HttpContext.Current.Session["Username"] != null;
        }

        public static List<GioHang.CartItem> GetCart()
        {
            return HttpContext.Current.Session["Cart"] as List<GioHang.CartItem>
                ?? new List<GioHang.CartItem>();
        }

        public static void SaveCart(List<GioHang.CartItem> cart)
        {
            HttpContext.Current.Session["Cart"] = cart;
        }

        /// <summary>
        /// Merge item into the existing cart (never wipes it). Quantity is
        /// clamped to available stock.
        /// </summary>
        public static void AddOrMerge(int id, string name, decimal price, string size, int qty, string imageUrl, int stock)
        {
            List<GioHang.CartItem> cart = GetCart();
            int max = stock > 0 ? stock : 1;
            if (qty < 1) qty = 1;
            if (qty > max) qty = max;

            GioHang.CartItem existing = cart.FirstOrDefault(c => c.Id == id && c.Size == size);
            if (existing != null)
            {
                existing.Quantity = Math.Min(existing.Quantity + qty, max);
            }
            else
            {
                cart.Add(new GioHang.CartItem
                {
                    Id = id,
                    Name = name,
                    Price = price,
                    Quantity = qty,
                    Size = size,
                    ImageUrl = imageUrl
                });
            }

            SaveCart(cart);
        }

        public static void RequireLoginToast(Page page, string key, string message)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(page, page.GetType(), key,
                "showToast('" + safe + "', 'info'); openLoginModal();", true);
        }

        public static void SuccessToast(Page page, string key, string message)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(page, page.GetType(), key,
                "showToast('" + safe + "', 'success');", true);
        }
    }
}
