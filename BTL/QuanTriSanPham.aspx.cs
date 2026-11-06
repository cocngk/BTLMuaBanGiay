using System;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    // Trang quản trị: chỉ tài khoản IsAdmin = true mới vào được.
    public partial class QuanTriSanPham : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("DangNhap.aspx?return=QuanTriSanPham.aspx");
                return;
            }

            if (Session["IsAdmin"] == null || !Convert.ToBoolean(Session["IsAdmin"]))
            {
                Response.Redirect("TrangChu.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["xoa"] == "1")
                    litThongBao.Text = "<span class=\"thong-bao tot\">Đã xóa sản phẩm.</span>";

                HienDanhSach();
            }
        }

        private void HienDanhSach()
        {
            gvSanPham.DataSource = DuLieu.SanPhams;
            gvSanPham.DataBind();
        }

        // Bấm nút Xóa trên một dòng.
        protected void gvSanPham_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Session["Username"] == null || Session["IsAdmin"] == null || !Convert.ToBoolean(Session["IsAdmin"]))
            {
                Response.Redirect("DangNhap.aspx?return=QuanTriSanPham.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            if (e.CommandName != "Xoa") return;

            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            DuLieu.XoaSanPham(id);

            Response.Redirect("QuanTriSanPham.aspx?xoa=1", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
