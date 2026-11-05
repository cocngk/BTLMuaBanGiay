using System;
using System.Web;

namespace ShoeStore
{
    // Site.Master: phần đầu trang, menu, ô tìm kiếm và chân trang.
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Số món trong giỏ, cập nhật ở mọi trang
            litSoGio.Text = Gio.SoMon().ToString();

            // Hai biến Application đếm lượt truy cập, khai báo trong Global.asax
            litLuotTruyCap.Text = Convert.ToString(Application["LuotTruyCap"]);
            litDangOnline.Text = Convert.ToString(Application["DangOnline"]);

            // Trạng thái đăng nhập lấy từ Session
            string tenDangNhap = Session["Username"] as string;

            if (tenDangNhap == null)
            {
                litTaiKhoan.Text = "<a href=\"DangNhap.aspx\">Đăng nhập</a> | <a href=\"DangKy.aspx\">Đăng ký</a>";
                btnThoat.Visible = false;
                phnQuanTri.Visible = false;
            }
            else
            {
                litTaiKhoan.Text = "Xin chào, <b>" + Server.HtmlEncode(tenDangNhap) + "</b>";
                btnThoat.Visible = true;
                phnQuanTri.Visible = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            }
        }

        // Bấm nút Tìm: chuyển sang trang sản phẩm kèm từ khóa.
        protected void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();
            Response.Redirect("SanPham.aspx?search=" + Server.UrlEncode(tuKhoa));
        }

        // Bấm Thoát: xóa Session và xóa cookie nhớ đăng nhập.
        protected void btnThoat_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            HttpCookie cookie = Request.Cookies["nhodangnhap"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            Response.Redirect("TrangChu.aspx");
        }
    }
}
