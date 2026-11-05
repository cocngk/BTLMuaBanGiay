using System;
using System.Collections.Generic;

namespace ShoeStore
{
    // Danh sách sản phẩm, có tìm theo tên và lọc theo hãng.
    // Từ khóa và hãng nằm trên đường dẫn: SanPham.aspx?search=...&brand=...
    public partial class SanPham : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Danh sách hãng để làm bộ lọc
            rptHang.DataSource = DuLieu.DanhSachHang();
            rptHang.DataBind();

            string tuKhoa = Request.QueryString["search"];
            string hang = Request.QueryString["brand"];

            List<SanPhamItem> danhSach = DuLieu.LocSanPham(tuKhoa, hang);

            rptSanPham.DataSource = danhSach;
            rptSanPham.DataBind();

            litKetQua.Text = "Tìm thấy " + danhSach.Count + " sản phẩm"
                + (string.IsNullOrEmpty(tuKhoa) ? "" : " cho \"" + Server.HtmlEncode(tuKhoa) + "\"")
                + (string.IsNullOrEmpty(hang) ? "" : " của hãng " + Server.HtmlEncode(hang));

            litRong.Text = danhSach.Count == 0
                ? "Không có sản phẩm nào phù hợp. <a href=\"SanPham.aspx\">Xem tất cả</a>"
                : "";
        }
    }
}
