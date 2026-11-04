using System;

namespace ShoeStore
{
    public partial class SearchBox : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) txtQuery.Text = Request.QueryString["search"] ?? "";
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            string value = txtQuery.Text.Trim();
            Response.Redirect("SanPham.aspx" + (value.Length == 0 ? "" : "?search=" + Server.UrlEncode(value)));
        }
    }
}
