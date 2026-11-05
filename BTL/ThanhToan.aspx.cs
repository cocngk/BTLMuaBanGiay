using System;
using System.Collections.Generic;

namespace ShoeStore
{
    // Trang đặt hàng: ghi đơn vào danh sách DonHangs, trừ tồn kho, xóa giỏ.
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
            DonHang donHang = DuLieu.TimDonHang(maDon);

            if (donHang == null || donHang.Username != TenDangNhap) return;

            pnlDatHang.Visible = false;
            pnlXong.Visible = true;

            litMaDon.Text = Server.HtmlEncode(donHang.Code);
            litTongXong.Text = string.Format("{0:N0} đ", donHang.Total);
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

            DonHang donHang = new DonHang();
            donHang.Code = "DH" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            donHang.CreatedAt = DateTime.Now;
            donHang.Username = TenDangNhap;
            donHang.CustomerName = txtHoTen.Text.Trim();
            donHang.Phone = txtDienThoai.Text.Trim();
            donHang.Address = txtDiaChi.Text.Trim();
            donHang.PaymentMethod = rblThanhToan.SelectedValue;
            donHang.Total = Gio.TongTien();

            // Ghi từng dòng và trừ tồn kho
            foreach (MatHang hang in danhSach)
            {
                DongDon dong = new DongDon();
                dong.ProductId = hang.MaSanPham;
                dong.ProductName = hang.Ten;
                dong.Size = hang.Size;
                dong.Price = hang.Gia;
                dong.Quantity = hang.SoLuong;
                donHang.Lines.Add(dong);

                SanPhamItem sanPham = DuLieu.TimSanPham(hang.MaSanPham);
                if (sanPham != null && sanPham.Stock >= hang.SoLuong)
                    sanPham.Stock = sanPham.Stock - hang.SoLuong;
            }

            DuLieu.ThemDonHang(donHang);
            Gio.XoaHet();

            Response.Redirect("ThanhToan.aspx?ma=" + Server.UrlEncode(donHang.Code));
        }
    }
}
