using System;
using System.Collections.Generic;
using System.Web.UI;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    /// <summary>
    /// Trang đơn hàng: đơn nằm ở cấp ứng dụng nên xem lại được sau khi đóng
    /// trình duyệt, hoặc khi đăng nhập ở máy khác. Trang này là lý do tồn tại
    /// của Order.Username — lọc đơn theo đúng chủ sở hữu.
    /// </summary>
    public partial class DonHang : ShopPage
    {
        private readonly ApplicationOrderStore orderStore = new ApplicationOrderStore();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            // Sau khi đặt hàng, trang thanh toán chuyển sang đây kèm ?code=...
            string code = Request.QueryString["code"];
            if (!string.IsNullOrEmpty(code))
            {
                ShowOrder(code);
            }

            LoadMyOrders();
        }

        protected void btnLookup_Click(object sender, EventArgs e)
        {
            ShowOrder(txtCode.Text);
        }

        /// <summary>Hiện một đơn theo mã. Mã không tồn tại thì báo rõ, không im lặng.</summary>
        private void ShowOrder(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                Toast.Info(this, "Vui lòng nhập mã đơn cần tra cứu.");
                return;
            }

            Order order = orderStore.FindByCode(code);

            if (order == null)
            {
                pnlLookup.Visible = false;
                Toast.Info(this, "Không tìm thấy đơn hàng có mã " + code.Trim() + ". Vui lòng kiểm tra lại.");
                return;
            }

            litLookupCode.Text = order.Code;
            litLookupStatus.Text = order.Status;
            litLookupDate.Text = order.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            litLookupTotal.Text = Money.Format(order.Total());

            rptLookupLines.DataSource = order.Lines;
            rptLookupLines.DataBind();

            pnlLookup.Visible = true;
        }

        /// <summary>Danh sách đơn của người đang đăng nhập; khách thì chỉ thấy lời mời đăng nhập.</summary>
        private void LoadMyOrders()
        {
            if (!AuthHelper.IsLoggedIn())
            {
                pnlLoginHint.Visible = true;
                return;
            }

            List<Order> orders = orderStore.ByUser(AuthHelper.CurrentUserKey());

            if (orders.Count == 0)
            {
                pnlNoOrders.Visible = true;
                return;
            }

            litOrderCount.Text = orders.Count.ToString();
            rptOrders.DataSource = orders;
            rptOrders.DataBind();
            pnlMyOrders.Visible = true;
        }
    }
}
