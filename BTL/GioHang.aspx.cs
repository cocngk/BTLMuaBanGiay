using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    // Giỏ hàng: xem, sửa số lượng, xóa dòng và sang trang đặt hàng.
    public partial class GioHang : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("DangNhap.aspx?return=GioHang.aspx");
                return;
            }

            if (!IsPostBack) HienGio();
        }

        // Đổ giỏ hàng ra màn hình.
        private void HienGio()
        {
            List<MatHang> danhSach = Gio.Doc();

            rptGio.DataSource = danhSach;
            rptGio.DataBind();

            pnlGio.Visible = danhSach.Count > 0;
            pnlTrong.Visible = danhSach.Count == 0;

            litTong.Text = string.Format("{0:N0} đ", Gio.TongTien());
        }

        // Xóa một dòng hoặc lưu số lượng mới của một dòng.
        protected void rptGio_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string[] tach = Convert.ToString(e.CommandArgument).Split('|');
            int maSanPham = Convert.ToInt32(tach[0]);
            string size = tach[1];

            if (e.CommandName == "Xoa")
            {
                Gio.Xoa(maSanPham, size);
            }
            else if (e.CommandName == "Sua")
            {
                TextBox oSoLuong = (TextBox)e.Item.FindControl("txtSoLuong");

                int soLuong;
                if (int.TryParse(oSoLuong.Text, out soLuong) && soLuong > 0)
                    Gio.DoiSoLuong(maSanPham, size, soLuong);
                else
                    litThongBao.Text = "<span class=\"thong-bao\">Số lượng phải là số lớn hơn 0.</span>";
            }

            HienGio();
        }

        // Bấm Đặt hàng: sang trang thanh toán.
        protected void btnDatHang_Click(object sender, EventArgs e)
        {
            Response.Redirect("ThanhToan.aspx");
        }
    }
}
