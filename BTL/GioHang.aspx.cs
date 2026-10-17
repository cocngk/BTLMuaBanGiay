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

                rptCart.DataSource = cart.Lines
                    .Select(l => new
                    {
                        l.Id,
                        l.Name,
                        l.Price,
                        l.Size,
                        l.ImageUrl,
                        l.Quantity,
                        // Luôn >= 1: nếu size khác đã chiếm hết tồn kho thì max="0"
                        // sẽ làm trình duyệt từ chối mọi giá trị hợp lệ.
                        Max = Math.Max(1, cart.RemainingToBuy(l.Id, LookupStock(l.Id), l.Size))
                    }).ToList();
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
                    // Ô số lượng đang debounce 800ms; bấm xoá trong lúc đó sẽ nuốt
                    // thao tác lưu. Đọc luôn giá trị đang được gõ trong form rồi mới xoá.
                    FlushPendingQuantities();

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
            FlushPendingQuantities();
            LoadCartData();
            // Tu dong luu: khong hien toast, so tien da cap nhat tu dong o client.
        }

        /// <summary>
        /// Ghi lại số lượng của mọi dòng đang hiện trong form.
        /// Mọi postback của trang đều mang theo giá trị các ô TextBox, nên gọi hàm này
        /// ở đầu mỗi handler là cách duy nhất để không mất thay đổi khi người dùng
        /// bấm nút khác trước khi debounce kịp chạy.
        /// </summary>
        private void FlushPendingQuantities()
        {
            Cart cart = Store.Load();
            if (cart.IsEmpty) return;

            if (rptCart.Items.Count == 0)
            {
                rptCart.DataSource = cart.Lines.ToList();
                rptCart.DataBind();
            }

            bool changed = false;
            foreach (RepeaterItem item in rptCart.Items)
            {
                HiddenField hfKey = item.FindControl("hfKey") as HiddenField;
                TextBox txtQty = item.FindControl("txtQuantity") as TextBox;
                if (hfKey == null || txtQty == null) continue;

                int id;
                string size;
                if (!TryParseKey(hfKey.Value, out id, out size)) continue;

                // Ô trong Repeater không được Web Forms nạp postback data (item sinh sau
                // ProcessPostData) nên phải đọc thẳng Request.Form theo UniqueID.
                string posted = Request.Form[txtQty.UniqueID];
                if (posted == null) continue;

                int qty;
                if (!int.TryParse(posted, out qty)) qty = 1;

                CartItem line = cart.Lines.FirstOrDefault(c => c.Id == id && c.Size == size);
                if (line != null && line.Quantity == qty) continue;

                cart.SetQuantity(id, size, qty, LookupStock(id));
                changed = true;
            }

            if (changed) Store.Save(cart);
        }

        protected void btnGoToCheckout_Click(object sender, EventArgs e)
        {
            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this, "RequireLoginCheckoutToast",
                    "Vui lòng đăng nhập để tiến hành thanh toán!");
                return;
            }

            // Nút nằm trong UpdatePanel nên postback là async: Response.Redirect bị
            // XHR đi theo và trình duyệt không đổi trang. Phải chuyển bằng script.
            FlushPendingQuantities();
            Toast.Navigate(this, "GoCheckout", "ThanhToan.aspx");
        }
    }
}
