using System;
using System.Collections.Generic;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class DonHang : ShopPage
    {
        private readonly ApplicationOrderStore danhSachDonHang = new ApplicationOrderStore();

        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (IsPostBack) return;
            string maTruyVan = Request.QueryString["code"];
            if (!string.IsNullOrEmpty(maTruyVan)) ShowOrder(maTruyVan);
            LoadMyOrders();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnLookup_Click(object nguonSuKien, EventArgs e)
        {
            ShowOrder(txtCode.Text);
        }

        // Thực hiện việc hiển thị đơn hàng.
        private void ShowOrder(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma))
            {
                Toast.Info(this, "Vui lòng nhập mã đơn cần tra cứu.");
                return;
            }

            Order donHang = danhSachDonHang.FindByCode(ma);
            if (donHang == null)
            {
                pnlLookup.Visible = false;
                Toast.Info(this, "Không tìm thấy đơn hàng có mã " + ma.Trim() + ". Vui lòng kiểm tra lại.");
                return;
            }
            litLookupCode.Text = donHang.Code;
            litLookupStatus.Text = donHang.Status;
            litLookupDate.Text = donHang.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            litLookupTotal.Text = Money.Format(donHang.Total());
            rptLookupLines.DataSource = donHang.Lines ?? new List<OrderLine>();
            rptLookupLines.DataBind();
            pnlLookup.Visible = true;
        }

        // Thực hiện việc tải my đơn hàng.
        private void LoadMyOrders()
        {
            if (!AuthHelper.IsLoggedIn())
            {
                pnlLoginHint.Visible = true;
                return;
            }

            List<Order> danhSach = danhSachDonHang.ByUser(AuthHelper.CurrentUserKey());
            if (danhSach.Count == 0)
            {
                pnlNoOrders.Visible = true;
                return;
            }
            litOrderCount.Text = danhSach.Count.ToString();
            rptOrders.DataSource = danhSach;
            rptOrders.DataBind();
            pnlMyOrders.Visible = true;
        }
    }
}
