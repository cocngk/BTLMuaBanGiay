using System;
using System.Linq;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;
using ShoeStore.Services;

namespace ShoeStore
{
    public partial class ThanhToan : ShopPage
    {
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiếp tục thanh toán!"))
            {
                pnlEmpty.Visible = false;
                pnlCheckout.Visible = false;
                return;
            }
            if (!IsPostBack) BindSummary();
        }

        // Thực hiện việc nạp summary.
        private void BindSummary()
        {
            Cart gioHang = CartStore.Load();
            pnlEmpty.Visible = gioHang.IsEmpty;
            pnlCheckout.Visible = !gioHang.IsEmpty;
            if (gioHang.IsEmpty) return;
            rptSummary.DataSource = gioHang.Lines.ToList();
            rptSummary.DataBind();
            litSubTotal.Text = Money.Format(gioHang.SubTotal());
            litTotal.Text = Money.Format(gioHang.SubTotal());
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnConfirmCheckout_Click(object nguonSuKien, EventArgs e)
        {
            if (!Page.IsValid) return;

            OrderResult ketQua = new OrderIntake(CartStore, new ApplicationOrderStore()).Place(new OrderInput
            {
                Username = AuthHelper.CurrentUserKey(),
                CustomerName = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                PaymentMethod = rblPaymentMethod.SelectedValue
            });
            if (ketQua.IsEmptyCart)
            {
                BindSummary();
                return;
            }
            if (!ketQua.IsPlaced)
            {
                Toast.Info(this, ketQua.Error);
                return;
            }

            Toast.ShowAndRedirect(this, "Đặt hàng thành công! Mã đơn " + ketQua.Order.Code + ". Chúng tôi sẽ gọi lại để xác nhận.", "success", "DonHang.aspx?code=" + Server.UrlEncode(ketQua.Order.Code));
        }
    }
}
