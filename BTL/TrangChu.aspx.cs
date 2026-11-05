using System;
using System.Data;

namespace ShoeStore
{
    // Trang chủ: hiện 8 sản phẩm nổi bật.
    public partial class TrangChu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable bang = KetNoi.Bang(
                    "SELECT TOP 8 * FROM Products WHERE IsHot = 1 ORDER BY Id");

                rptSanPham.DataSource = bang;
                rptSanPham.DataBind();
            }
        }
    }
}
