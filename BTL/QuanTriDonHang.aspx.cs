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
        private const int PageSize = 10;
        private readonly OrderAdminService service = new OrderAdminService();
        private int CurrentPage { get { return ViewState["Page"] == null ? 1 : (int)ViewState["Page"]; } set { ViewState["Page"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin()) return;
            pnlAdmin.Visible = true;
            if (!IsPostBack) { BindStatus(); CurrentPage = 1; BindList(); }
        }

        private void BindStatus()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("Tất cả trạng thái", ""));
            foreach (string status in Order.AllStatuses()) ddlStatus.Items.Add(new ListItem(status, status));
        }

        private void BindList()
        {
            List<Order> all = service.All();
            List<Order> filtered = OrderAdminService.FilterByStatus(all, ddlStatus.SelectedValue);
            string keyword = txtSearch.Text.Trim();
            if (keyword.Length > 0) filtered = filtered.Where(x => Contains(x.Code, keyword) || Contains(x.CustomerName, keyword) || Contains(x.Phone, keyword)).ToList();
            PagedResult<Order> page = Paging.Paginate(filtered, CurrentPage, PageSize);
            CurrentPage = page.Page;
            rptOrders.DataSource = page.Items.Select(ToRow).ToList();
            rptOrders.DataBind();
            bool empty = page.Items.Count == 0;
            pnlTable.Visible = !empty;
            pnlEmpty.Visible = empty;
            pnlPager.Visible = page.TotalPages > 1;
            litEmpty.Text = all.Count == 0 ? "Chưa có đơn hàng nào. Đơn sẽ xuất hiện ở đây ngay sau khi khách đặt." : "Không có đơn nào khớp bộ lọc hiện tại.";
            litCount.Text = filtered.Count == all.Count ? all.Count + " đơn" : filtered.Count + " / " + all.Count + " đơn";
            litPage.Text = page.Page.ToString();
            litTotalPages.Text = page.TotalPages.ToString();
            btnPrev.Enabled = page.Page > 1;
            btnNext.Enabled = page.Page < page.TotalPages;
            btnPrev.CssClass = page.Page > 1 ? "btn-row" : "btn-row is-disabled";
            btnNext.CssClass = page.Page < page.TotalPages ? "btn-row" : "btn-row is-disabled";
        }

        private static AdminOrderRow ToRow(Order order)
        {
            List<OrderLine> lines = order.Lines ?? new List<OrderLine>();
            return new AdminOrderRow { Code = order.Code, CreatedAtText = order.CreatedAt.ToString("dd/MM/yyyy HH:mm"), CustomerName = order.CustomerName, Phone = order.Phone, Address = order.Address, Quantity = order.TotalQuantity(), TotalText = Money.Format(order.Total()), Status = order.Status, StatusClass = StatusClass(order.Status), ItemSummary = lines.Count + " món · xem chi tiết", Lines = lines };
        }

        private static string StatusClass(string status)
        {
            if (status == Order.DeliveredStatus) return "status-pill is-delivered";
            if (status == Order.CancelledStatus) return "status-pill is-cancelled";
            if (status == Order.ShippingStatus) return "status-pill is-shipping";
            return "status-pill is-pending";
        }

        private static bool Contains(string value, string keyword) { return !string.IsNullOrEmpty(value) && value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0; }
        protected void btnFilter_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage = 1; BindList(); } }
        protected void btnPrev_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage = Math.Max(1, CurrentPage - 1); BindList(); } }
        protected void btnNext_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage++; BindList(); } }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            AdminOrderRow row = e.Item.DataItem as AdminOrderRow;
            DropDownList status = e.Item.FindControl("ddlRowStatus") as DropDownList;
            Repeater lines = e.Item.FindControl("rptLines") as Repeater;
            if (row == null) return;
            if (lines != null) { lines.DataSource = row.Lines; lines.DataBind(); }
            if (status != null)
            {
                status.DataSource = Order.AllStatuses();
                status.DataBind();
                if (status.Items.FindByValue(row.Status) != null) status.SelectedValue = row.Status;
            }
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin() || e.CommandName != "ChangeStatus") return;
            DropDownList status = e.Item.FindControl("ddlRowStatus") as DropDownList;
            if (status == null) return;
            StatusChangeResult result = service.ChangeStatus(Convert.ToString(e.CommandArgument), status.SelectedValue);
            if (result.Ok && result.Outcome == StatusChangeOutcome.Updated) Toast.Success(this, result.Message);
            else Toast.Info(this, result.Message);
            BindList();
        }
    }
}
