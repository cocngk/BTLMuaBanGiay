using System;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class GioHang : ShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindCart();
        }

        private void BindCart()
        {
            Cart cart = CartStore.Load();
            bool empty = cart.IsEmpty;
            pnlEmptyCart.Visible = empty;
            pnlCartHasItems.Visible = !empty;
            if (empty) return;
            rptCart.DataSource = CartService.Lines(cart);
            rptCart.DataBind();
            litSubTotal.Text = Money.Format(cart.SubTotal());
            litTotal.Text = Money.Format(cart.SubTotal());
        }

        private bool TryKey(string key, out int id, out string size)
        {
            id = 0; size = "";
            string[] parts = (key ?? "").Split('|');
            return parts.Length == 2 && int.TryParse(parts[0], out id) && (size = parts[1]) != null;
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remove") return;
            int id; string size;
            if (!TryKey(Convert.ToString(e.CommandArgument), out id, out size)) return;
            FlushQuantities();
            Cart cart = CartStore.Load();
            if (cart.Remove(id, size))
            {
                CartStore.Save(cart);
                BindCart();
                Toast.Info(this, "Đã xóa sản phẩm khỏi giỏ hàng!");
            }
        }

        protected void btnUpdateCart_Click(object sender, EventArgs e) { FlushQuantities(); BindCart(); }

        private void FlushQuantities()
        {
            Cart cart = CartStore.Load();
            if (cart.IsEmpty) return;
            if (rptCart.Items.Count == 0) { rptCart.DataSource = cart.Lines.ToList(); rptCart.DataBind(); }
            bool changed = false;
            foreach (RepeaterItem item in rptCart.Items)
            {
                HiddenField key = item.FindControl("hfKey") as HiddenField;
                TextBox text = item.FindControl("txtQuantity") as TextBox;
                if (key == null || text == null) continue;
                int id; string size;
                if (!TryKey(key.Value, out id, out size)) continue;
                string posted = Request.Form[text.UniqueID];
                int qty;
                if (posted == null || !int.TryParse(posted, out qty)) continue;
                if (CartService.SetQuantity(cart, id, size, qty)) changed = true;
            }
            if (changed) CartStore.Save(cart);
        }

        protected void btnGoToCheckout_Click(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiến hành thanh toán!")) return;
            FlushQuantities();
            Toast.Navigate(this, "ThanhToan.aspx");
        }
    }
}
