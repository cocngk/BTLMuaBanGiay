using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;
using ShoeStore.Services;

namespace ShoeStore
{
    public sealed class AdminOrderRow
    {
        public string Code { get; set; }
        public string CreatedAtText { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Quantity { get; set; }
        public string TotalText { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
        public string ItemSummary { get; set; }
        public List<OrderLine> Lines { get; set; }
    }

    public partial class QuanTriDonHang : ShopPage
    {
        private const int KichThuocTrang = 10;
        private readonly OrderAdminService dichVu = new OrderAdminService();
        private int TrangHienTai
        {
            get { return ViewState["Page"] == null ? 1 : (int)ViewState["Page"]; }
            set { ViewState["Page"] = value; }
        }

        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            pnlAdmin.Visible = true;
            if (!IsPostBack) { BindStatus(); TrangHienTai = 1; BindList(); }
        }

        // Thực hiện việc nạp trạng thái.
        private void BindStatus()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("Tất cả trạng thái", ""));
            foreach (string trangThai in Order.AllStatuses())
                ddlStatus.Items.Add(new ListItem(trangThai, trangThai));
        }

        // Thực hiện việc nạp danh sách.
        private void BindList()
        {
            List<Order> all = dichVu.All();
            List<Order> filtered = OrderAdminService.FilterByStatus(all, ddlStatus.SelectedValue);
            string tuKhoa = txtSearch.Text.Trim();
            if (tuKhoa.Length > 0)
            {
                filtered = filtered.Where(x =>
                    Contains(x.Code, tuKhoa)
                    || Contains(x.CustomerName, tuKhoa)
                    || Contains(x.Phone, tuKhoa))
                    .ToList();
            }
            PagedResult<Order> trang = Paging.Paginate(filtered, TrangHienTai, KichThuocTrang);
            TrangHienTai = trang.Page;
            rptOrders.DataSource = trang.Items.Select(ToRow).ToList();
            rptOrders.DataBind();
            bool rong = trang.Items.Count == 0;
            pnlTable.Visible = !rong;
            pnlEmpty.Visible = rong;
            pnlPager.Visible = trang.TotalPages > 1;
            litEmpty.Text = all.Count == 0 ? "Chưa có đơn hàng nào. Đơn sẽ xuất hiện ở đây ngay sau khi khách đặt." : "Không có đơn nào khớp bộ lọc hiện tại.";
            litCount.Text = filtered.Count == all.Count ? all.Count + " đơn" : filtered.Count + " / " + all.Count + " đơn";
            litPage.Text = trang.Page.ToString();
            litTotalPages.Text = trang.TotalPages.ToString();
            btnPrev.Enabled = trang.Page > 1;
            btnNext.Enabled = trang.Page < trang.TotalPages;
            btnPrev.CssClass = trang.Page > 1 ? "btn-row" : "btn-row is-disabled";
            btnNext.CssClass = trang.Page < trang.TotalPages ? "btn-row" : "btn-row is-disabled";
        }

        // Thực hiện việc chuyển đổi sang row.
        private static AdminOrderRow ToRow(Order donHang)
        {
            List<OrderLine> cacDong = donHang.Lines ?? new List<OrderLine>();
            return new AdminOrderRow
            {
                Code = donHang.Code,
                CreatedAtText = donHang.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                CustomerName = donHang.CustomerName,
                Phone = donHang.Phone,
                Address = donHang.Address,
                Quantity = donHang.TotalQuantity(),
                TotalText = Money.Format(donHang.Total()),
                Status = donHang.Status,
                StatusClass = StatusClass(donHang.Status),
                ItemSummary = cacDong.Count + " món · xem chi tiết",
                Lines = cacDong
            };
        }

        // Thực hiện việc trạng thái lớp.
        private static string StatusClass(string trangThai)
        {
            if (trangThai == Order.DeliveredStatus) return "status-pill is-delivered";
            if (trangThai == Order.CancelledStatus) return "status-pill is-cancelled";
            if (trangThai == Order.ShippingStatus) return "status-pill is-shipping";
            return "status-pill is-pending";
        }

        // Thực hiện việc kiểm tra chứa.
        private static bool Contains(string giaTri, string tuKhoa)
        {
            return !string.IsNullOrEmpty(giaTri)
                && giaTri.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        // Xử lý khi người dùng bấm nút.
        protected void btnFilter_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            TrangHienTai = 1;
            BindList();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnPrev_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            TrangHienTai = Math.Max(1, TrangHienTai - 1);
            BindList();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnNext_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            TrangHienTai++;
            BindList();
        }

        // Xử lý khi một dòng dữ liệu được liên kết.
        protected void rptOrders_ItemDataBound(object nguonSuKien, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            AdminOrderRow row = e.Item.DataItem as AdminOrderRow;
            DropDownList trangThai = e.Item.FindControl("ddlRowStatus") as DropDownList;
            Repeater cacDong = e.Item.FindControl("rptLines") as Repeater;
            if (row == null) return;
            if (cacDong != null)
            {
                cacDong.DataSource = row.Lines;
                cacDong.DataBind();
            }
            if (trangThai != null)
            {
                trangThai.DataSource = Order.AllStatuses();
                trangThai.DataBind();
                if (trangThai.Items.FindByValue(row.Status) != null) trangThai.SelectedValue = row.Status;
            }
        }

        // Xử lý lệnh trên một dòng dữ liệu.
        protected void rptOrders_ItemCommand(object nguon, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin() || e.CommandName != "ChangeStatus") return;
            DropDownList trangThai = e.Item.FindControl("ddlRowStatus") as DropDownList;
            if (trangThai == null) return;
            StatusChangeResult ketQua = dichVu.ChangeStatus(Convert.ToString(e.CommandArgument), trangThai.SelectedValue);
            if (ketQua.Ok && ketQua.Outcome == StatusChangeOutcome.Updated)
                Toast.Success(this, ketQua.Message);
            else
                Toast.Info(this, ketQua.Message);
            BindList();
        }
    }
}
