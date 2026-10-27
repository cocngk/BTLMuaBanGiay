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
    /// <summary>
    /// Một dòng của bảng đơn hàng, đã tính sẵn những thứ template cần: chuỗi ngày,
    /// chuỗi tiền, lớp CSS của trạng thái, câu tóm tắt số món. Bind thẳng Order thì
    /// template phải gọi method (Total(), TotalQuantity()) mà Eval không làm được.
    /// </summary>
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

    /// <summary>
    /// Danh sách đơn cho quản trị viên: lọc theo trạng thái, tìm theo mã / khách, và
    /// đổi trạng thái từng đơn. Luật trả hàng về kho nằm ở OrderAdminService — trang
    /// chỉ gọi và hiển thị kết quả, không tự quyết định tồn kho.
    /// </summary>
    public partial class QuanTriDonHang : ShopPage
    {
        private const int PageSize = 10;

        private readonly OrderAdminService service = new OrderAdminService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            pnlAdmin.Visible = true;

            if (!IsPostBack)
            {
                BindStatusFilter();
                CurrentPage = 1;
                BindList();
            }
        }

        private int CurrentPage
        {
            get
            {
                object value = ViewState["Page"];
                return value == null ? 1 : (int)value;
            }
            set
            {
                ViewState["Page"] = value;
            }
        }

        private void BindStatusFilter()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("Tất cả trạng thái", ""));
            foreach (string status in Order.AllStatuses())
            {
                ddlStatus.Items.Add(new ListItem(status, status));
            }
            ddlStatus.SelectedIndex = 0;
        }

        private void BindList()
        {
            string status = ddlStatus.SelectedValue;
            string keyword = txtSearch.Text.Trim();

            List<Order> all = service.All();
            List<Order> filtered = service.ByStatus(status);

            if (keyword.Length > 0)
            {
                filtered = filtered.Where(o =>
                    Contains(o.Code, keyword) ||
                    Contains(o.CustomerName, keyword) ||
                    Contains(o.Phone, keyword)).ToList();
            }

            PagedResult<Order> page = Paging.Paginate(filtered, CurrentPage, PageSize);
            CurrentPage = page.Page;

            List<AdminOrderRow> rows = page.Items.Select(ToRow).ToList();

            rptOrders.DataSource = rows;
            rptOrders.DataBind();

            bool empty = rows.Count == 0;
            pnlTable.Visible = !empty;
            pnlEmpty.Visible = empty;
            pnlPager.Visible = page.TotalPages > 1;

            litEmpty.Text = all.Count == 0
                ? "Chưa có đơn hàng nào. Đơn sẽ xuất hiện ở đây ngay sau khi khách đặt."
                : "Không có đơn nào khớp bộ lọc hiện tại.";

            litCount.Text = filtered.Count == all.Count
                ? all.Count + " đơn"
                : filtered.Count + " / " + all.Count + " đơn";

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

            return new AdminOrderRow
            {
                Code = order.Code,
                CreatedAtText = order.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Address = order.Address,
                Quantity = order.TotalQuantity(),
                TotalText = Money.Format(order.Total()),
                Status = order.Status,
                StatusClass = StatusClassOf(order.Status),
                ItemSummary = lines.Count + " món · xem chi tiết",
                Lines = lines
            };
        }

        private static string StatusClassOf(string status)
        {
            if (string.Equals(status, Order.DeliveredStatus, StringComparison.Ordinal))
            {
                return "status-pill is-delivered";
            }
            if (string.Equals(status, Order.CancelledStatus, StringComparison.Ordinal))
            {
                return "status-pill is-cancelled";
            }
            if (string.Equals(status, Order.ShippingStatus, StringComparison.Ordinal))
            {
                return "status-pill is-shipping";
            }
            return "status-pill is-pending";
        }

        private static bool Contains(string value, string keyword)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            CurrentPage = 1;
            BindList();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            CurrentPage = Math.Max(1, CurrentPage - 1);
            BindList();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            CurrentPage = CurrentPage + 1;
            BindList();
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            DropDownList ddl = e.Item.FindControl("ddlRowStatus") as DropDownList;
            Repeater lines = e.Item.FindControl("rptLines") as Repeater;
            AdminOrderRow row = e.Item.DataItem as AdminOrderRow;

            if (row == null)
            {
                return;
            }

            // Gán nguồn cho repeater con ngay tại đây thay vì DataSource="<%# Eval(...) %>"
            // trong markup: cách này không phụ thuộc vào thứ tự chạy giữa data-binding
            // của cha và DataBind() của con.
            if (lines != null)
            {
                lines.DataSource = row.Lines;
                lines.DataBind();
            }

            if (ddl == null)
            {
                return;
            }

            ddl.DataSource = Order.AllStatuses();
            ddl.DataBind();

            ListItem current = ddl.Items.FindByValue(row.Status);
            if (current != null)
            {
                ddl.ClearSelection();
                current.Selected = true;
            }
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            if (e.CommandName != "ChangeStatus")
            {
                return;
            }

            string code = Convert.ToString(e.CommandArgument);
            DropDownList ddl = e.Item.FindControl("ddlRowStatus") as DropDownList;

            if (ddl == null)
            {
                return;
            }

            StatusChangeResult result = service.ChangeStatus(code, ddl.SelectedValue);

            if (result.Ok)
            {
                if (result.Outcome == StatusChangeOutcome.Updated)
                {
                    Toast.Success(this, result.Message);
                }
                else
                {
                    Toast.Info(this, result.Message);
                }
            }
            else
            {
                Toast.Info(this, result.Message);
            }

            BindList();
        }
    }
}
