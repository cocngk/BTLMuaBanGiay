
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class ThanhToan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                string script = @"
                    showToast('Vui lòng đăng nhập để tiếp tục thanh toán!', 'error');
                    setTimeout(function() {
                        window.location.href = 'DangNhap.aspx';
                    }, 1800);
                ";

                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "RequireLogin",
                    script,
                    true
                );

                return;
            }

            if (!IsPostBack)
            {
                LoadOrderSummary();
            }
        }

        private void LoadOrderSummary()
        {
            Cart cart = new SessionCartStore(Session).Load();

            if (cart.IsEmpty)
            {
                pnlEmpty.Visible = true;
                pnlCheckout.Visible = false;
            }
            else
            {
                pnlEmpty.Visible = false;
                pnlCheckout.Visible = true;

                rptSummary.DataSource = cart;
                rptSummary.DataBind();

                decimal total = cart.SubTotal();

                litSubTotal.Text = string.Format("{0:N0}đ", total);
                litTotal.Text = string.Format("{0:N0}đ", total);
            }
        }

        protected void btnConfirmCheckout_Click(object sender, EventArgs e)
        {
            OrderInput order = new OrderInput
            {
                CustomerName = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                PaymentMethod = rblPaymentMethod.SelectedValue
            };

            string error = OrderValidator.Validate(order);
            if (error != null)
            {
                Toast.Show(this, "Toast_" + Guid.NewGuid().ToString("N"), error, "error");
                return;
            }

            Cart cart = new SessionCartStore(Session).Load();

            if (cart.IsEmpty)
            {
                Toast.Show(this, "Toast_" + Guid.NewGuid().ToString("N"),
                    "Giỏ hàng đang trống. Vui lòng chọn sản phẩm trước!", "error");
                return;
            }

            new SessionCartStore(Session).Clear();

            string methodText = rblPaymentMethod.SelectedItem != null
                ? rblPaymentMethod.SelectedItem.Text
                : "Thanh toán khi nhận hàng (COD)";
            string script = @"
                showToast('Đặt hàng thành công! " + methodText.Replace("'", "\\'") + @"', 'success');
                setTimeout(function() {
                    window.location.href = 'TrangChu.aspx';
                }, 2000);
            ";

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "CheckoutSuccess",
                script,
                true
            );
        }
    }
}
