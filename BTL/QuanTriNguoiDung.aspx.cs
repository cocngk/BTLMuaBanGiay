using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class QuanTriNguoiDung : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!KiemTraQuyen()) return;
            if (!IsPostBack) BindUsers();
        }

        private bool KiemTraQuyen()
        {
            if (Session["Username"] != null && Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"])) return true;
            Response.Redirect(Session["Username"] == null ? "DangNhap.aspx?return=QuanTriNguoiDung.aspx" : "TrangChu.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        private void BindUsers()
        {
            string q = txtSearch.Text.Trim();
            gvUsers.DataSource = DuLieu.NguoiDungs.Where(u => string.IsNullOrEmpty(q)
                || Has(u.FullName, q) || Has(u.Username, q) || Has(u.Email, q) || Has(u.Phone, q))
                .OrderByDescending(u => u.IsAdmin).ThenBy(u => u.Username).ToList();
            gvUsers.DataBind();
        }

        private static bool Has(string value, string query) => !string.IsNullOrEmpty(value) && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (KiemTraQuyen()) BindUsers();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!KiemTraQuyen() || e.CommandName != "ToggleAdmin") return;
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            NguoiDung user = DuLieu.NguoiDungs.FirstOrDefault(u => u.Id == id);
            if (user == null) { BindUsers(); return; }
            string current = Convert.ToString(Session["Username"]);
            if (string.Equals(user.Username, current, StringComparison.OrdinalIgnoreCase))
                litMessage.Text = "<span class=\"thong-bao\">Không thể tự thay đổi quyền của chính mình.</span>";
            else if (user.IsAdmin && DuLieu.NguoiDungs.Count(u => u.IsAdmin) <= 1)
                litMessage.Text = "<span class=\"thong-bao\">Không thể thu quyền quản trị viên cuối cùng.</span>";
            else
            {
                user.IsAdmin = !user.IsAdmin;
                litMessage.Text = "<span class=\"thong-bao tot\">Đã cập nhật quyền người dùng.</span>";
            }
            BindUsers();
        }
    }
}
