using System;
using System.Collections.Generic;
using System.Data;

namespace ShoeStore
{
    // Trang đặt hàng: ghi đơn vào bảng Orders và OrderLines, trừ tồn kho, xóa giỏ.
    public partial class ThanhToan : System.Web.UI.Page
    {
        private string TenDangNhap
        {
            get { return Convert.ToString(Session["Username"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("DangNhap.aspx?return=ThanhToan.aspx");
                return;
            }

            if (IsPostBack) return;

            // Đặt hàng xong thì hiện mã đơn
            string maDon = Request.QueryString["ma"];
            if (!string.IsNullOrEmpty(maDon))
            {
                HienKetQua(maDon);
                return;
            }

            List<MatHang> danhSach = Gio.Doc();

            if (danhSach.Count == 0)
            {
                pnlDatHang.Visible = false;
                pnlTrong.Visible = true;
                return;
            }

            rptTomTat.DataSource = danhSach;
            rptTomTat.DataBind();

            litTong.Text = string.Format("{0:N0} đ", Gio.TongTien());
            txtHoTen.Text = Convert.ToString(Session["FullName"]);
        }

        // Hiện thông tin đơn vừa đặt.
        private void HienKetQua(string maDon)
        {
            DataTable bang = KetNoi.Bang(
                "SELECT * FROM Orders WHERE Code = @ma AND Username = @ten",
                KetNoi.P("@ma", maDon),
                KetNoi.P("@ten", TenDangNhap));

            if (bang.Rows.Count == 0) return;

            pnlDatHang.Visible = false;
            pnlXong.Visible = true;

            litMaDon.Text = Server.HtmlEncode(Convert.ToString(bang.Rows[0]["Code"]));
            litTongXong.Text = string.Format("{0:N0} đ", bang.Rows[0]["Total"]);
        }

        protected void btnDatHang_Click(object sender, EventArgs e)
        {
            if (txtHoTen.Text.Trim() == "" || txtDienThoai.Text.Trim() == "" || txtDiaChi.Text.Trim() == "")
            {
                litThongBao.Text = "<span class=\"thong-bao\">Vui lòng nhập đủ họ tên, điện thoại và địa chỉ.</span>";
                return;
            }

            List<MatHang> danhSach = Gio.Doc();

            if (danhSach.Count == 0)
            {
                Response.Redirect("ThanhToan.aspx");
                return;
            }

            string maDon = "DH" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // Ghi đơn hàng
            KetNoi.ThucThi(
                "INSERT INTO Orders (Code, Username, CustomerName, Phone, Address, PaymentMethod, Total) "
                + "VALUES (@ma, @ten, @hoTen, @dienThoai, @diaChi, @thanhToan, @tong)",
                KetNoi.P("@ma", maDon),
                KetNoi.P("@ten", TenDangNhap),
                KetNoi.P("@hoTen", txtHoTen.Text.Trim()),
                KetNoi.P("@dienThoai", txtDienThoai.Text.Trim()),
                KetNoi.P("@diaChi", txtDiaChi.Text.Trim()),
                KetNoi.P("@thanhToan", rblThanhToan.SelectedValue),
                KetNoi.P("@tong", Gio.TongTien()));

            int maDonId = Convert.ToInt32(KetNoi.Mot(
                "SELECT Id FROM Orders WHERE Code = @ma", KetNoi.P("@ma", maDon)));

            // Ghi từng dòng và trừ tồn kho
            foreach (MatHang hang in danhSach)
            {
                KetNoi.ThucThi(
                    "INSERT INTO OrderLines (OrderId, ProductId, ProductName, Size, Price, Quantity) "
                    + "VALUES (@don, @sanPham, @ten, @size, @gia, @soLuong)",
                    KetNoi.P("@don", maDonId),
                    KetNoi.P("@sanPham", hang.MaSanPham),
                    KetNoi.P("@ten", hang.Ten),
                    KetNoi.P("@size", hang.Size),
                    KetNoi.P("@gia", hang.Gia),
                    KetNoi.P("@soLuong", hang.SoLuong));

                KetNoi.ThucThi(
                    "UPDATE Products SET Stock = Stock - @soLuong WHERE Id = @id AND Stock >= @soLuong",
                    KetNoi.P("@soLuong", hang.SoLuong),
                    KetNoi.P("@id", hang.MaSanPham));
            }

            Gio.XoaHet();

            Response.Redirect("ThanhToan.aspx?ma=" + Server.UrlEncode(maDon));
        }
    }
}
