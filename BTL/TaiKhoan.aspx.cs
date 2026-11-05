using System;
using System.Data;

namespace ShoeStore
{
    // Trang tài khoản: xem và sửa thông tin, đổi mật khẩu, xem đơn hàng của mình.
    public partial class TaiKhoan : System.Web.UI.Page
    {
        private string TenDangNhap
        {
            get { return Convert.ToString(Session["Username"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("DangNhap.aspx?return=TaiKhoan.aspx");
                return;
            }

            if (!IsPostBack) HienThongTin();
        }

        // Điền thông tin tài khoản và danh sách đơn hàng.
        private void HienThongTin()
        {
            DataTable bang = KetNoi.Bang(
                "SELECT * FROM Users WHERE Username = @ten", KetNoi.P("@ten", TenDangNhap));

            if (bang.Rows.Count == 0) return;

            DataRow nguoiDung = bang.Rows[0];

            litTenDangNhap.Text = Server.HtmlEncode(Convert.ToString(nguoiDung["Username"]));
            txtHoTen.Text = Convert.ToString(nguoiDung["FullName"]);
            txtEmail.Text = Convert.ToString(nguoiDung["Email"]);
            txtDienThoai.Text = Convert.ToString(nguoiDung["Phone"]);

            DataTable donHang = KetNoi.Bang(
                "SELECT * FROM Orders WHERE Username = @ten ORDER BY Id DESC",
                KetNoi.P("@ten", TenDangNhap));

            rptDonHang.DataSource = donHang;
            rptDonHang.DataBind();

            litChuaCoDon.Text = donHang.Rows.Count == 0
                ? "<p class=\"nhan\">Bạn chưa có đơn hàng nào.</p>"
                : "";
        }

        // Lưu họ tên, email, điện thoại.
        protected void btnLuu_Click(object sender, EventArgs e)
        {
            KetNoi.ThucThi(
                "UPDATE Users SET FullName = @hoTen, Email = @email, Phone = @dienThoai "
                + "WHERE Username = @ten",
                KetNoi.P("@hoTen", txtHoTen.Text.Trim()),
                KetNoi.P("@email", txtEmail.Text.Trim()),
                KetNoi.P("@dienThoai", txtDienThoai.Text.Trim()),
                KetNoi.P("@ten", TenDangNhap));

            HienThongTin();
            litThongBao.Text = "<span class=\"thong-bao tot\">Đã lưu thay đổi.</span>";
        }

        // Đổi mật khẩu: phải nhập đúng mật khẩu hiện tại.
        protected void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            DataTable bang = KetNoi.Bang(
                "SELECT PasswordHash FROM Users WHERE Username = @ten",
                KetNoi.P("@ten", TenDangNhap));

            if (bang.Rows.Count == 0) return;

            string maBamCu = Convert.ToString(bang.Rows[0]["PasswordHash"]);

            if (MatKhau.Bam(txtMatKhauCu.Text) != maBamCu)
            {
                litThongBaoMatKhau.Text = "<span class=\"thong-bao\">Mật khẩu hiện tại không đúng.</span>";
                return;
            }

            if (txtMatKhauMoi.Text.Length < 6)
            {
                litThongBaoMatKhau.Text = "<span class=\"thong-bao\">Mật khẩu mới phải có ít nhất 6 ký tự.</span>";
                return;
            }

            if (txtMatKhauMoi.Text != txtNhapLai.Text)
            {
                litThongBaoMatKhau.Text = "<span class=\"thong-bao\">Hai lần nhập mật khẩu mới không giống nhau.</span>";
                return;
            }

            KetNoi.ThucThi(
                "UPDATE Users SET PasswordHash = @matKhau WHERE Username = @ten",
                KetNoi.P("@matKhau", MatKhau.Bam(txtMatKhauMoi.Text)),
                KetNoi.P("@ten", TenDangNhap));

            txtMatKhauCu.Text = "";
            txtMatKhauMoi.Text = "";
            txtNhapLai.Text = "";

            litThongBaoMatKhau.Text = "<span class=\"thong-bao tot\">Đã đổi mật khẩu.</span>";
        }
    }
}
