using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;

namespace ShoeStore
{
    /// <summary>
    /// Một dòng của bảng người dùng. Là lớp riêng thay vì bind thẳng UserAccount vì
    /// template cần biết thêm những thứ chỉ trang mới tính được: ai đang đăng nhập,
    /// ai được phép đổi quyền, và vì sao bị khoá.
    /// </summary>
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

    /// <summary>
    /// Danh sách tài khoản cho quản trị viên: xem hồ sơ và cấp/thu quyền quản trị.
    /// Không có thao tác xoá — xoá tài khoản sẽ bỏ lại đơn hàng, giỏ và danh sách
    /// yêu thích không còn chủ, mà kho chỉ nằm trong bộ nhớ nên không có gì để dọn.
    /// </summary>
    public partial class QuanTriNguoiDung : ShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            pnlAdmin.Visible = true;

            if (!IsPostBack)
            {
                BindList();
            }
        }

        private void BindList()
        {
            string keyword = txtSearch.Text.Trim();
            string me = AuthHelper.CurrentUserKey();

            List<UserAccount> accounts = AuthHelper.AllAccounts();
            int adminCount = accounts.Count(a => a.IsAdmin);

            List<AdminUserRow> rows = new List<AdminUserRow>();

            foreach (UserAccount account in accounts)
            {
                if (keyword.Length > 0 && !Matches(account, keyword))
                {
                    continue;
                }

                bool isSelf = string.Equals(account.Username, me, StringComparison.OrdinalIgnoreCase);

                // Hai chốt: không tự bấm vào quyền của mình (tránh tự khoá cửa), và
                // không thu quyền của admin cuối cùng (mất người vào khu quản trị).
                bool isLastAdmin = account.IsAdmin && adminCount <= 1;
                bool canToggle = !isSelf && !isLastAdmin;

                rows.Add(new AdminUserRow
                {
                    FullName = string.IsNullOrEmpty(account.FullName) ? account.Username : account.FullName,
                    Username = account.Username,
                    Email = account.Email,
                    Phone = string.IsNullOrEmpty(account.Phone) ? "—" : account.Phone,
                    IsAdmin = account.IsAdmin,
                    CanToggle = canToggle,
                    RoleLabel = account.IsAdmin ? "Quản trị viên" : "Khách hàng",
                    ToggleLabel = account.IsAdmin ? "Thu quyền" : "Cấp quyền",
                    LockedReason = isSelf
                        ? "Tài khoản của bạn"
                        : (isLastAdmin ? "Quản trị viên cuối cùng" : "")
                });
            }

            // Quản trị viên lên trước để thấy ngay ai đang có quyền, phần còn lại theo tên.
            rows = rows
                .OrderByDescending(r => r.IsAdmin)
                .ThenBy(r => r.Username, StringComparer.OrdinalIgnoreCase)
                .ToList();

            rptUsers.DataSource = rows;
            rptUsers.DataBind();

            bool empty = rows.Count == 0;
            pnlTable.Visible = !empty;
            pnlEmpty.Visible = empty;
            litEmpty.Text = keyword.Length == 0
                ? "Chưa có tài khoản nào."
                : "Không có tài khoản nào khớp với \"" + keyword + "\".";

            litCount.Text = keyword.Length == 0
                ? accounts.Count + " tài khoản · " + adminCount + " quản trị viên"
                : rows.Count + " / " + accounts.Count + " tài khoản";
        }

        private static bool Matches(UserAccount account, string keyword)
        {
            return Contains(account.FullName, keyword)
                || Contains(account.Username, keyword)
                || Contains(account.Email, keyword)
                || Contains(account.Phone, keyword);
        }

        private static bool Contains(string value, string keyword)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            BindList();
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            if (e.CommandName != "Toggle")
            {
                return;
            }

            string username = Convert.ToString(e.CommandArgument);

            UserAccount target = AuthHelper.FindByUsername(username);
            if (target == null)
            {
                Toast.Info(this, "Không tìm thấy tài khoản này. Danh sách có thể vừa thay đổi.");
                BindList();
                return;
            }

            if (string.Equals(username, AuthHelper.CurrentUserKey(), StringComparison.OrdinalIgnoreCase))
            {
                Toast.Info(this, "Không thể tự đổi quyền của chính mình. Nhờ quản trị viên khác làm việc đó.");
                BindList();
                return;
            }

            bool grant = !target.IsAdmin;
            string display = string.IsNullOrEmpty(target.FullName) ? target.Username : target.FullName;

            switch (AuthHelper.TrySetAdmin(username, grant))
            {
                case RoleChangeOutcome.Changed:
                    Toast.Success(this, grant
                        ? "Đã cấp quyền quản trị cho " + display + "."
                        : "Đã thu quyền quản trị của " + display + ".");
                    break;

                case RoleChangeOutcome.LastAdmin:
                    Toast.Info(this, "Không thể thu quyền của quản trị viên cuối cùng — sẽ không còn ai vào được khu quản trị.");
                    break;

                case RoleChangeOutcome.Unchanged:
                    Toast.Info(this, "Quyền của tài khoản này không thay đổi.");
                    break;

                default:
                    Toast.Info(this, "Không tìm thấy tài khoản này.");
                    break;
            }

            BindList();
        }
    }
}
