using System;
using System.Linq;
using System.Web.UI;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class ThanhToan : ShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireLogin("Vui lòng đăng nhập để tiếp tục thanh toán!"))
            {
                // Không đăng nhập thì không hiện form thanh toán; toast + form
                // đăng nhập lo phần còn lại.
                pnlEmpty.Visible = false;
                pnlCheckout.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                LoadOrderSummary();
            }
        }

        private void LoadOrderSummary()
        {
            Cart cart = CartStore.Load();

            if (cart.IsEmpty)
            {
                ShowEmpty();
                return;
            }

            pnlEmpty.Visible = false;
            pnlCheckout.Visible = true;

            // Cart khong cai IEnumerable nen khong bind truc tiep, phai lay Lines
            // giong trang GioHang, neu khong se nem ArgumentException.
            rptSummary.DataSource = cart.Lines.ToList();
            rptSummary.DataBind();

            litSubTotal.Text = Money.Format(cart.SubTotal());
            litTotal.Text = Money.Format(cart.SubTotal());
        }

        private void ShowEmpty()
        {
            pnlEmpty.Visible = true;
            pnlCheckout.Visible = false;
        }

        protected void btnConfirmCheckout_Click(object sender, EventArgs e)
        {
            // Sai định dạng thì lỗi đã hiện ngay dưới từng ô, ở đây chỉ chặn lại.
            if (!Page.IsValid) return;

            Cart cart = CartStore.Load();
            if (cart.IsEmpty)
            {
                ShowEmpty();
                return;
            }

            Order order = new Order
            {
                Code = Order.NewCode(),
                CreatedAt = DateTime.Now,
                CustomerName = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                PaymentMethod = rblPaymentMethod.SelectedValue,
                Lines = OrderLine.FromCart(cart.Lines)
            };

            new SessionOrderStore(Session).Save(order);
            CartStore.Clear();

            Toast.ShowAndRedirect(this, "CheckoutSuccess_" + Guid.NewGuid().ToString("N"),
                "Đặt hàng thành công! Mã đơn " + order.Code + ". Chúng tôi sẽ gọi lại để xác nhận.",
                "success",
                "TrangChu.aspx");
        }
    }
}