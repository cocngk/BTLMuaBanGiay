
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using static ShoeStore.GioHang;

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
            List<CartItem> cart = Session["Cart"] as List<CartItem>;

            if (cart == null || cart.Count == 0)
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

                decimal total = cart.Sum(item => item.Price * item.Quantity);

                litSubTotal.Text = string.Format("{0:N0}đ", total);
                litTotal.Text = string.Format("{0:N0}đ", total);
            }
        }

        protected void btnConfirmCheckout_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(address))
            {
                ShowToast("Bạn chưa nhập đủ thông tin nhận hàng!", "error");
                return;
            }

            List<CartItem> cart = Session["Cart"] as List<CartItem>;

            if (cart == null || cart.Count == 0)
            {
                ShowToast("Giỏ hàng đang trống. Vui lòng chọn sản phẩm trước!", "error");
                return;
            }

            if (phone.Length < 9 || phone.Length > 11 ||
                !phone.All(char.IsDigit))
            {
                ShowToast("Số điện thoại chưa hợp lệ. Vui lòng kiểm tra lại!", "error");
                return;
            }

            Session["Cart"] = null;

            string script = @"
                showToast('Đặt hàng thành công! Cảm ơn bạn đã mua sắm tại SneakerX.', 'success');
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

        private void ShowToast(string message, string type)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);

            string script = "showToast('" + safeMessage + "', '" + type + "');";

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "Toast_" + Guid.NewGuid().ToString("N"),
                script,
                true
            );
        }
    }
}
