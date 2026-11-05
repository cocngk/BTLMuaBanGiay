using System;
using System.Collections.Generic;

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
            NguoiDung nguoiDung = DuLieu.TimNguoiDung(TenDangNhap);

            if (nguoiDung == null) return;

            litTenDangNhap.Text = Server.HtmlEncode(nguoiDung.Username);
            txtHoTen.Text = nguoiDung.FullName;
            txtEmail.Text = nguoiDung.Email;
            txtDienThoai.Text = nguoiDung.Phone;

            List<DonHang> danhSach = DuLieu.DonHangCua(TenDangNhap);

            rptDonHang.DataSource = danhSach;
            rptDonHang.DataBind();

            litChuaCoDon.Text = danhSach.Count == 0
                ? "<p class=\"nhan\">Bạn chưa có đơn hàng nào.</p>"
                : "";
        }

        // Lưu họ tên, email, điện thoại.
        protected void btnLuu_Click(object sender, EventArgs e)
        {
            NguoiDung nguoiDung = DuLieu.TimNguoiDung(TenDangNhap);

            if (nguoiDung == null) return;

            nguoiDung.FullName = txtHoTen.Text.Trim();
            nguoiDung.Email = txtEmail.Text.Trim();
            nguoiDung.Phone = txtDienThoai.Text.Trim();

            Session["FullName"] = nguoiDung.FullName;

            HienThongTin();
            litThongBao.Text = "<span class=\"thong-bao tot\">Đã lưu thay đổi.</span>";
        }

        // Đổi mật khẩu: phải nhập đúng mật khẩu hiện tại.
        protected void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            NguoiDung nguoiDung = DuLieu.TimNguoiDung(TenDangNhap);

            if (nguoiDung == null) return;

            if (nguoiDung.Password != txtMatKhauCu.Text)
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

            nguoiDung.Password = txtMatKhauMoi.Text;

            txtMatKhauCu.Text = "";
            txtMatKhauMoi.Text = "";
            txtNhapLai.Text = "";

            litThongBaoMatKhau.Text = "<span class=\"thong-bao tot\">Đã đổi mật khẩu.</span>";
        }
    }
}
