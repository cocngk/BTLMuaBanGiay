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
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!IsPostBack) BindCart();
        }

        // Thực hiện việc nạp giỏ hàng.
        private void BindCart()
        {
            Cart gioHang = CartStore.Load();
            bool rong = gioHang.IsEmpty;
            pnlEmptyCart.Visible = rong;
            pnlCartHasItems.Visible = !rong;
            if (rong) return;
            rptCart.DataSource = CartService.Lines(gioHang);
            rptCart.DataBind();
            litSubTotal.Text = Money.Format(gioHang.SubTotal());
            litTotal.Text = Money.Format(gioHang.SubTotal());
        }

        // Thực hiện việc thử khóa.
        private bool TryKey(string khoa, out int ma, out string kichThuoc)
        {
            ma = 0;
            kichThuoc = "";
            string[] cacPhan = (khoa ?? "").Split('|');
            if (cacPhan.Length != 2 || !int.TryParse(cacPhan[0], out ma))
                return false;

            kichThuoc = cacPhan[1].Trim();
            return ma > 0 && kichThuoc.Length > 0;
        }

        // Xử lý lệnh trên một dòng dữ liệu.
        protected void rptCart_ItemCommand(object nguon, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remove") return;
            int ma; string kichThuoc;
            if (!TryKey(Convert.ToString(e.CommandArgument), out ma, out kichThuoc)) return;
            FlushQuantities();
            Cart gioHang = CartStore.Load();
            if (gioHang.Remove(ma, kichThuoc))
            {
                CartStore.Save(gioHang);
                BindCart();
                Toast.Info(this, "Đã xóa sản phẩm khỏi giỏ hàng!");
            }
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnUpdateCart_Click(object nguonSuKien, EventArgs e)
        {
            FlushQuantities();
            BindCart();
        }

        // Thực hiện việc flush quantities.
        private void FlushQuantities()
        {
            Cart gioHang = CartStore.Load();
            if (gioHang.IsEmpty) return;
            if (rptCart.Items.Count == 0) { rptCart.DataSource = gioHang.Lines.ToList(); rptCart.DataBind(); }
            bool changed = false;
            foreach (RepeaterItem phanTu in rptCart.Items)
            {
                HiddenField khoa = phanTu.FindControl("hfKey") as HiddenField;
                TextBox vanBan = phanTu.FindControl("txtQuantity") as TextBox;
                if (khoa == null || vanBan == null) continue;
                int ma; string kichThuoc;
                if (!TryKey(khoa.Value, out ma, out kichThuoc)) continue;
                string posted = Request.Form[vanBan.UniqueID];
                int soLuong;
                if (posted == null || !int.TryParse(posted, out soLuong))
                    continue;
                if (CartService.SetQuantity(gioHang, ma, kichThuoc, soLuong)) changed = true;
            }
            if (changed) CartStore.Save(gioHang);
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnGoToCheckout_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiến hành thanh toán!")) return;
            FlushQuantities();
            Toast.Navigate(this, "ThanhToan.aspx");
        }
    }
}
