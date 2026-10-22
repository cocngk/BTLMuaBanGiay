using System;
using System.Linq;
using System.Web.UI;
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

            // Đơn lưu ở cấp ứng dụng: đóng trình duyệt hay đăng nhập máy khác vẫn tra được.
            OrderResult result = new OrderIntake(CartStore, new ApplicationOrderStore())
                .Place(new OrderInput
                {
                    Username = AuthHelper.CurrentUserKey(),
                    CustomerName = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    PaymentMethod = rblPaymentMethod.SelectedValue
                });

            if (result.IsEmptyCart)
            {
                ShowEmpty();
                return;
            }

            if (!result.IsPlaced)
            {
                Toast.Info(this, result.Error);
                return;
            }

            // Sang thẳng trang đơn hàng để khách thấy đơn vừa tạo, không phải tự đi tìm.
            Toast.ShowAndRedirect(this,
                "Đặt hàng thành công! Mã đơn " + result.Order.Code + ". Chúng tôi sẽ gọi lại để xác nhận.",
                "success",
                "DonHang.aspx?code=" + Server.UrlEncode(result.Order.Code));
        }
    }
}