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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiếp tục thanh toán!"))
            {
                pnlEmpty.Visible = false;
                pnlCheckout.Visible = false;
                return;
            }
            if (!IsPostBack) BindSummary();
        }

        private void BindSummary()
        {
            Cart cart = CartStore.Load();
            pnlEmpty.Visible = cart.IsEmpty;
            pnlCheckout.Visible = !cart.IsEmpty;
            if (cart.IsEmpty) return;
            rptSummary.DataSource = cart.Lines.ToList();
            rptSummary.DataBind();
            litSubTotal.Text = Money.Format(cart.SubTotal());
            litTotal.Text = Money.Format(cart.SubTotal());
        }

        protected void btnConfirmCheckout_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            OrderResult result = new OrderIntake(CartStore, new ApplicationOrderStore()).Place(new OrderInput
            {
                Username = AuthHelper.CurrentUserKey(),
                CustomerName = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                PaymentMethod = rblPaymentMethod.SelectedValue
            });
            if (result.IsEmptyCart) { BindSummary(); return; }
            if (!result.IsPlaced) { Toast.Info(this, result.Error); return; }
            Toast.ShowAndRedirect(this, "Đặt hàng thành công! Mã đơn " + result.Order.Code + ". Chúng tôi sẽ gọi lại để xác nhận.", "success", "DonHang.aspx?code=" + Server.UrlEncode(result.Order.Code));
        }
    }
}
