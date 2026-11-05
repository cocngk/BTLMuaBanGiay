using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
            rptHang.DataSource = KetNoi.Bang("SELECT DISTINCT Brand FROM Products ORDER BY Brand");
            rptHang.DataBind();

            string tuKhoa = Request.QueryString["search"];
            string hang = Request.QueryString["brand"];

            string sql = "SELECT * FROM Products WHERE 1 = 1";
            List<SqlParameter> thamSo = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                sql = sql + " AND (Name LIKE @tuKhoa OR Brand LIKE @tuKhoa)";
                thamSo.Add(KetNoi.P("@tuKhoa", "%" + tuKhoa + "%"));
            }

            if (!string.IsNullOrEmpty(hang))
            {
                sql = sql + " AND Brand = @hang";
                thamSo.Add(KetNoi.P("@hang", hang));
            }

            sql = sql + " ORDER BY Id";

            DataTable bang = KetNoi.Bang(sql, thamSo.ToArray());

            rptSanPham.DataSource = bang;
            rptSanPham.DataBind();

            litKetQua.Text = "Tìm thấy " + bang.Rows.Count + " sản phẩm"
                + (string.IsNullOrEmpty(tuKhoa) ? "" : " cho \"" + Server.HtmlEncode(tuKhoa) + "\"")
                + (string.IsNullOrEmpty(hang) ? "" : " của hãng " + Server.HtmlEncode(hang));

            litRong.Text = bang.Rows.Count == 0
                ? "Không có sản phẩm nào phù hợp. <a href=\"SanPham.aspx\">Xem tất cả</a>"
                : "";
        }
    }
}
