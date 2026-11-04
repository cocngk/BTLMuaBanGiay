using System;

namespace ShoeStore
{
    public partial class HopTimKiem : System.Web.UI.UserControl
    {
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!IsPostBack)
                txtQuery.Text = Request.QueryString["search"] ?? "";
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnGo_Click(object nguonSuKien, EventArgs e)
        {
            string giaTri = txtQuery.Text.Trim();
            string duongDan = "SanPham.aspx";
            if (giaTri.Length > 0)
                duongDan += "?search=" + Server.UrlEncode(giaTri);
            Response.Redirect(duongDan);
        }
    }
}
