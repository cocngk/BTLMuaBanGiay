using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;

namespace ShoeStore
{
    public sealed class AdminUserRow
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanToggle { get; set; }
        public string RoleLabel { get; set; }
        public string ToggleLabel { get; set; }
        public string LockedReason { get; set; }
    }

    public partial class QuanTriNguoiDung : ShopPage
    {
        private const int PageSize = 10;
        private int CurrentPage { get { return ViewState["Page"] == null ? 1 : (int)ViewState["Page"]; } set { ViewState["Page"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin()) return;
            pnlAdmin.Visible = true;
            if (!IsPostBack) { CurrentPage = 1; BindList(); }
        }

        private void BindList()
        {
            string keyword = txtSearch.Text.Trim();
            List<UserAccount> accounts = AuthHelper.AllAccounts();
            int admins = accounts.Count(x => x.IsAdmin);
            string me = AuthHelper.CurrentUserKey();
            List<AdminUserRow> rows = accounts.Where(x => keyword.Length == 0 || Matches(x, keyword)).Select(x =>
                new AdminUserRow
                {
                    FullName = string.IsNullOrEmpty(x.FullName) ? x.Username : x.FullName,
                    Username = x.Username,
                    Email = x.Email,
                    Phone = string.IsNullOrEmpty(x.Phone) ? "—" : x.Phone,
                    IsAdmin = x.IsAdmin,
                    CanToggle = !string.Equals(x.Username, me, StringComparison.OrdinalIgnoreCase) && !(x.IsAdmin && admins <= 1),
                    RoleLabel = x.IsAdmin ? "Quản trị viên" : "Khách hàng",
                    ToggleLabel = x.IsAdmin ? "Thu quyền" : "Cấp quyền",
                    LockedReason = string.Equals(x.Username, me, StringComparison.OrdinalIgnoreCase) ? "Tài khoản của bạn" : (x.IsAdmin && admins <= 1 ? "Quản trị viên cuối cùng" : "")
                }).OrderByDescending(x => x.IsAdmin).ThenBy(x => x.Username, StringComparer.OrdinalIgnoreCase).ToList();
            PagedResult<AdminUserRow> page = Paging.Paginate(rows, CurrentPage, PageSize);
            CurrentPage = page.Page;
            rptUsers.DataSource = page.Items;
            rptUsers.DataBind();
            pnlTable.Visible = page.Items.Count > 0;
            pnlEmpty.Visible = page.Items.Count == 0;
            pnlPager.Visible = page.TotalPages > 1;
            litPage.Text = page.Page.ToString();
            litTotalPages.Text = page.TotalPages.ToString();
            btnPrev.Enabled = page.Page > 1;
            btnNext.Enabled = page.Page < page.TotalPages;
            btnPrev.CssClass = page.Page > 1 ? "btn-row" : "btn-row is-disabled";
            btnNext.CssClass = page.Page < page.TotalPages ? "btn-row" : "btn-row is-disabled";
            litEmpty.Text = keyword.Length == 0 ? "Chưa có tài khoản nào." : "Không có tài khoản nào khớp với \"" + keyword + "\".";
            litCount.Text = keyword.Length == 0 ? accounts.Count + " tài khoản · " + admins + " quản trị viên" : rows.Count + " / " + accounts.Count + " tài khoản";
        }

        private static bool Matches(UserAccount x, string keyword) { return Contains(x.FullName, keyword) || Contains(x.Username, keyword) || Contains(x.Email, keyword) || Contains(x.Phone, keyword); }
        private static bool Contains(string value, string keyword) { return !string.IsNullOrEmpty(value) && value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0; }
        protected void btnSearch_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage = 1; BindList(); } }
        protected void btnPrev_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage = Math.Max(1, CurrentPage - 1); BindList(); } }
        protected void btnNext_Click(object sender, EventArgs e) { if (RequireAdmin()) { CurrentPage++; BindList(); } }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin() || e.CommandName != "Toggle") return;
            string username = Convert.ToString(e.CommandArgument);
            UserAccount account = AuthHelper.FindByUsername(username);
            if (account == null) { Toast.Info(this, "Không tìm thấy tài khoản này."); BindList(); return; }
            if (string.Equals(username, AuthHelper.CurrentUserKey(), StringComparison.OrdinalIgnoreCase)) { Toast.Info(this, "Không thể tự đổi quyền của chính mình."); BindList(); return; }
            bool grant = !account.IsAdmin;
            RoleChangeOutcome result = AuthHelper.TrySetAdmin(username, grant);
            if (result == RoleChangeOutcome.Changed) Toast.Success(this, grant ? "Đã cấp quyền quản trị cho " + account.DisplayName + "." : "Đã thu quyền quản trị của " + account.DisplayName + ".");
            else if (result == RoleChangeOutcome.LastAdmin) Toast.Info(this, "Không thể thu quyền của quản trị viên cuối cùng.");
            else Toast.Info(this, result == RoleChangeOutcome.Unchanged ? "Quyền của tài khoản này không thay đổi." : "Không tìm thấy tài khoản này.");
            BindList();
        }
    }
}
