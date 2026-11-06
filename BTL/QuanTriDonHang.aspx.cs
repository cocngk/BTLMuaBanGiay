using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class QuanTriDonHang : System.Web.UI.Page
    {
        private static readonly string[] TrangThais = { "Chờ xác nhận", "Đang giao", "Đã giao", "Đã hủy" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!KiemTraQuyen()) return;
            if (!IsPostBack) BindOrders();
        }

        private bool KiemTraQuyen()
        {
            if (Session["Username"] != null && Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"])) return true;
            Response.Redirect(Session["Username"] == null ? "DangNhap.aspx?return=QuanTriDonHang.aspx" : "TrangChu.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        private void BindOrders()
        {
            string q = txtSearch.Text.Trim();
            gvOrders.DataSource = DuLieu.DonHangs.Where(o => string.IsNullOrEmpty(q)
                || Has(o.Code, q) || Has(o.CustomerName, q) || Has(o.Phone, q)).OrderByDescending(o => o.CreatedAt).ToList();
            gvOrders.DataBind();
        }

        private static bool Has(string value, string query) => !string.IsNullOrEmpty(value) && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (KiemTraQuyen()) BindOrders();
        }

        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!KiemTraQuyen() || e.CommandName != "SaveStatus") return;
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            DropDownList ddl = row.FindControl("ddlStatus") as DropDownList;
            DonHang order = DuLieu.DonHangs.FirstOrDefault(o => o.Id == id);
            if (order != null && ddl != null && TrangThais.Contains(ddl.SelectedValue))
            {
                order.Status = ddl.SelectedValue;
                litMessage.Text = "<span class=\"thong-bao tot\">Đã cập nhật trạng thái đơn " + Server.HtmlEncode(order.Code) + ".</span>";
            }
            BindOrders();
        }
    }
}
