using System;

namespace ShoeStore
{
    // Trang chi tiết một sản phẩm. Mã sản phẩm lấy từ QueryString: ChiTietSanPham.aspx?id=3
    public partial class ChiTietSanPham : System.Web.UI.Page
    {
        // Đọc mã sản phẩm trên đường dẫn, sai thì trả về 0.
        private int MaSanPham
        {
            get
            {
                int ma;
                return int.TryParse(Request.QueryString["id"], out ma) ? ma : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            SanPhamItem sanPham = DuLieu.TimSanPham(MaSanPham);

            if (sanPham == null)
            {
                pnlSanPham.Visible = false;
                litThongBao.Text = "<p class=\"thong-bao\">Không tìm thấy sản phẩm.</p>"
                    + "<p><a href=\"SanPham.aspx\">Xem tất cả sản phẩm</a></p>";
                return;
            }

            imgAnh.ImageUrl = sanPham.ImageUrl;
            litTen.Text = Server.HtmlEncode(sanPham.Name);
            litHang.Text = Server.HtmlEncode(sanPham.Brand);
            litGia.Text = string.Format("{0:N0} đ", sanPham.Price);
            litMoTa.Text = Server.HtmlEncode(sanPham.Description);
            litTonKho.Text = sanPham.Stock.ToString();

            // Danh sách size lưu trong một chuỗi, cách nhau bằng dấu phẩy
            foreach (string size in sanPham.Sizes.Split(','))
                ddlSize.Items.Add(size.Trim());
        }

        // Bấm Thêm vào giỏ hàng.
        protected void btnThem_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("DangNhap.aspx?return=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            SanPhamItem sanPham = DuLieu.TimSanPham(MaSanPham);

            if (sanPham == null)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Không tìm thấy sản phẩm.</span>";
                return;
            }

            int soLuong;
            if (!int.TryParse(txtSoLuong.Text, out soLuong) || soLuong < 1)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Số lượng không hợp lệ.</span>";
                return;
            }

            if (sanPham.Stock <= 0)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Sản phẩm này đã hết hàng.</span>";
                return;
            }

            if (soLuong > sanPham.Stock)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Chỉ còn " + sanPham.Stock + " đôi.</span>";
                return;
            }

            MatHang hang = new MatHang();
            hang.MaSanPham = sanPham.Id;
            hang.Ten = sanPham.Name;
            hang.Gia = sanPham.Price;
            hang.Anh = sanPham.ImageUrl;
            hang.Size = ddlSize.SelectedValue;
            hang.SoLuong = soLuong;

            Gio.Them(hang);

            litKetQua.Text = "<span class=\"thong-bao tot\">Đã thêm "
                + Server.HtmlEncode(hang.Ten) + " (size " + Server.HtmlEncode(hang.Size)
                + ") vào giỏ. <a href=\"GioHang.aspx\">Xem giỏ hàng</a></span>";
        }
    }
}
