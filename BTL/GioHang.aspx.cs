using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class GioHang : System.Web.UI.Page
    {
        private SessionCartStore Store => new SessionCartStore(Session);

        private int LookupStock(int id)
        {
            ProductItem product = ProductStore.GetAll().FirstOrDefault(p => p.Id == id);
            return product != null ? product.Stock : int.MaxValue;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCartData();
            }
        }

        private void LoadCartData()
        {
            Cart cart = Store.Load();

            if (cart.IsEmpty)
            {
                pnlEmptyCart.Visible = true;
                pnlCartHasItems.Visible = false;
            }
            else
            {
                pnlEmptyCart.Visible = false;
                pnlCartHasItems.Visible = true;

                rptCart.DataSource = cart.Lines.ToList();
                rptCart.DataBind();

                litSubTotal.Text = string.Format("{0:N0}đ", cart.SubTotal());
                litTotal.Text = string.Format("{0:N0}đ", cart.SubTotal());
            }
        }

        private bool TryParseKey(string key, out int id, out string size)
        {
            id = 0;
            size = "";
            if (string.IsNullOrEmpty(key)) return false;
            string[] parts = key.Split('|');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out id)) return false;
            size = parts[1];
            return true;
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int id;
                string size;
                if (TryParseKey(Convert.ToString(e.CommandArgument), out id, out size))
                {
                    Cart cart = Store.Load();
                    if (cart.Remove(id, size))
                    {
                        Store.Save(cart);
                        LoadCartData();
                        Toast.Info(this, "ToastDelete", "Đã xóa sản phẩm khỏi giỏ hàng!");
                    }
                }
            }
        }

        protected void btnUpdateCart_Click(object sender, EventArgs e)
        {
            Cart cart = Store.Load();
            foreach (RepeaterItem item in rptCart.Items)
            {
                HiddenField hfKey = (HiddenField)item.FindControl("hfKey");
                TextBox txtQty = (TextBox)item.FindControl("txtQuantity");
                if (hfKey == null || txtQty == null) continue;

                int id;
                string size;
                if (!TryParseKey(hfKey.Value, out id, out size)) continue;

                int qty;
                if (!int.TryParse(txtQty.Text, out qty)) continue;
                cart.SetQuantity(id, size, qty, LookupStock(id));
            }
            Store.Save(cart);
            LoadCartData();
            // Tu dong luu: khong hien toast, so tien da cap nhat tu dong o client.
        }

        protected void btnGoToCheckout_Click(object sender, EventArgs e)
        {
            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this, "RequireLoginCheckoutToast",
                    "Vui lòng đăng nhập để tiến hành thanh toán!");
                return;
            }

            Response.Redirect("ThanhToan.aspx");
        }
    }
}
