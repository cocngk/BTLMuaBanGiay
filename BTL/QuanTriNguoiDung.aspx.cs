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
        private const int KichThuocTrang = 10;
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
            if (!IsPostBack) { TrangHienTai = 1; BindList(); }
        }

        // Thực hiện việc nạp danh sách.
        private void BindList()
        {
            string tuKhoa = txtSearch.Text.Trim();
            List<UserAccount> accounts = AuthHelper.AllAccounts();
            int admins = accounts.Count(x => x.IsAdmin);
            string me = AuthHelper.CurrentUserKey();
            List<AdminUserRow> cacDong = accounts
                .Where(x => tuKhoa.Length == 0 || Matches(x, tuKhoa))
                .Select(x => new AdminUserRow
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
                })
                .OrderByDescending(x => x.IsAdmin)
                .ThenBy(x => x.Username, StringComparer.OrdinalIgnoreCase)
                .ToList();
            PagedResult<AdminUserRow> trang = Paging.Paginate(cacDong, TrangHienTai, KichThuocTrang);
            TrangHienTai = trang.Page;
            rptUsers.DataSource = trang.Items;
            rptUsers.DataBind();
            pnlTable.Visible = trang.Items.Count > 0;
            pnlEmpty.Visible = trang.Items.Count == 0;
            pnlPager.Visible = trang.TotalPages > 1;
            litPage.Text = trang.Page.ToString();
            litTotalPages.Text = trang.TotalPages.ToString();
            btnPrev.Enabled = trang.Page > 1;
            btnNext.Enabled = trang.Page < trang.TotalPages;
            btnPrev.CssClass = trang.Page > 1 ? "btn-row" : "btn-row is-disabled";
            btnNext.CssClass = trang.Page < trang.TotalPages ? "btn-row" : "btn-row is-disabled";
            litEmpty.Text = tuKhoa.Length == 0 ? "Chưa có tài khoản nào." : "Không có tài khoản nào khớp với \"" + tuKhoa + "\".";
            litCount.Text = tuKhoa.Length == 0 ? accounts.Count + " tài khoản · " + admins + " quản trị viên" : cacDong.Count + " / " + accounts.Count + " tài khoản";
        }

        // Thực hiện việc đối chiếu.
        private static bool Matches(UserAccount x, string tuKhoa)
        {
            return Contains(x.FullName, tuKhoa)
                || Contains(x.Username, tuKhoa)
                || Contains(x.Email, tuKhoa)
                || Contains(x.Phone, tuKhoa);
        }
        // Thực hiện việc kiểm tra chứa.
        private static bool Contains(string giaTri, string tuKhoa)
        {
            return !string.IsNullOrEmpty(giaTri)
                && giaTri.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        // Xử lý khi người dùng bấm nút.
        protected void btnSearch_Click(object nguonSuKien, EventArgs e)
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

        // Xử lý lệnh trên một dòng dữ liệu.
        protected void rptUsers_ItemCommand(object nguon, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin() || e.CommandName != "Toggle") return;
            string tenDangNhap = Convert.ToString(e.CommandArgument);
            UserAccount taiKhoan = AuthHelper.FindByUsername(tenDangNhap);
            if (taiKhoan == null) { Toast.Info(this, "Không tìm thấy tài khoản này."); BindList(); return; }
            if (string.Equals(tenDangNhap, AuthHelper.CurrentUserKey(), StringComparison.OrdinalIgnoreCase)) { Toast.Info(this, "Không thể tự đổi quyền của chính mình."); BindList(); return; }
            bool grant = !taiKhoan.IsAdmin;
            RoleChangeOutcome ketQua = AuthHelper.TrySetAdmin(tenDangNhap, grant);
            if (ketQua == RoleChangeOutcome.Changed)
            {
                Toast.Success(this, grant
                    ? "Đã cấp quyền quản trị cho " + taiKhoan.DisplayName + "."
                    : "Đã thu quyền quản trị của " + taiKhoan.DisplayName + ".");
            }
            else if (ketQua == RoleChangeOutcome.LastAdmin) Toast.Info(this, "Không thể thu quyền của quản trị viên cuối cùng.");
            else Toast.Info(this, ketQua == RoleChangeOutcome.Unchanged ? "Quyền của tài khoản này không thay đổi." : "Không tìm thấy tài khoản này.");
            BindList();
        }
    }
}
