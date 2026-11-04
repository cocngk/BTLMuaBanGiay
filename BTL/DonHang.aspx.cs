using System;
using System.Collections.Generic;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class DonHang : ShopPage
    {
        private readonly ApplicationOrderStore orders = new ApplicationOrderStore();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            if (!string.IsNullOrEmpty(Request.QueryString["code"])) ShowOrder(Request.QueryString["code"]);
            LoadMyOrders();
        }

        protected void btnLookup_Click(object sender, EventArgs e) { ShowOrder(txtCode.Text); }

        private void ShowOrder(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) { Toast.Info(this, "Vui lòng nhập mã đơn cần tra cứu."); return; }
            Order order = orders.FindByCode(code);
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

        private void LoadMyOrders()
        {
            if (!AuthHelper.IsLoggedIn()) { pnlLoginHint.Visible = true; return; }
            List<Order> list = orders.ByUser(AuthHelper.CurrentUserKey());
            if (list.Count == 0) { pnlNoOrders.Visible = true; return; }
            litOrderCount.Text = list.Count.ToString();
            rptOrders.DataSource = list;
            rptOrders.DataBind();
            pnlMyOrders.Visible = true;
        }
    }
}
