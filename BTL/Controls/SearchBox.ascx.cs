using System;

namespace ShoeStore
{
    /// <summary>
    /// Ô tìm kiếm header tự trị: tự giữ từ khóa, tự chuyển trang.
    /// </summary>
    public partial class SearchBox : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["search"] != null)
            {
                txtQuery.Text = Request.QueryString["search"];
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            string keyword = txtQuery.Text.Trim();

            if (!string.IsNullOrEmpty(keyword))
            {
                Response.Redirect("SanPham.aspx?search=" + Server.UrlEncode(keyword));
            }
            else
            {
                Response.Redirect("SanPham.aspx");
            }
        }
    }
}
