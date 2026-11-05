using System;

namespace ShoeStore
{
    // Trang chủ: hiện 8 sản phẩm nổi bật.
    public partial class TrangChu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptSanPham.DataSource = DuLieu.SanPhamNoiBat(8);
                rptSanPham.DataBind();
            }
        }
    }
}
