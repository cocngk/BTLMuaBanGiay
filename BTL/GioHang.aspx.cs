using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class GioHang : System.Web.UI.Page
    {
        [Serializable]
        public class CartItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Size { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string ImageUrl { get; set; }
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
            List<CartItem> cart = Session["Cart"] as List<CartItem>;

            if (cart == null || cart.Count == 0)
            {
                pnlEmptyCart.Visible = true;
                pnlCartHasItems.Visible = false;
            }
            else
            {
                pnlEmptyCart.Visible = false;
                pnlCartHasItems.Visible = true;

                rptCart.DataSource = cart;
                rptCart.DataBind();

                decimal total = cart.Sum(item => item.Price * item.Quantity);
                litSubTotal.Text = string.Format("{0:N0}đ", total);
                litTotal.Text = string.Format("{0:N0}đ", total);
            }
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                List<CartItem> cart = Session["Cart"] as List<CartItem>;

                if (cart != null)
                {
                    cart.RemoveAll(x => x.Id == id);
                    Session["Cart"] = cart;
                    LoadCartData();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastDelete", "showToast('Đã xóa sản phẩm khỏi giỏ hàng!', 'info');", true);
                }
            }
        }

        protected void btnUpdateCart_Click(object sender, EventArgs e)
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                for (int i = 0; i < rptCart.Items.Count; i++)
                {
                    RepeaterItem item = rptCart.Items[i];
                    TextBox txtQty = (TextBox)item.FindControl("txtQuantity");

                    if (txtQty != null && int.TryParse(txtQty.Text, out int qty))
                    {
                        cart[i].Quantity = qty > 0 ? qty : 1;
                    }
                }
                Session["Cart"] = cart;
                LoadCartData();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastUpdate", "showToast('Cập nhật giỏ hàng thành công!', 'success');", true);
            }
        }

        protected void btnGoToCheckout_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "RequireLoginCheckoutToast",
                    "showToast('Vui lòng đăng nhập để tiến hành thanh toán!', 'info'); openLoginModal();", true);
                return;
            }

            Response.Redirect("ThanhToan.aspx");
        }
    }
}