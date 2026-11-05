using System;
using System.Data;

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

            DataTable bang = KetNoi.Bang(
                "SELECT * FROM Products WHERE Id = @id", KetNoi.P("@id", MaSanPham));

            if (bang.Rows.Count == 0)
            {
                pnlSanPham.Visible = false;
                litThongBao.Text = "<p class=\"thong-bao\">Không tìm thấy sản phẩm.</p>"
                    + "<p><a href=\"SanPham.aspx\">Xem tất cả sản phẩm</a></p>";
                return;
            }

            DataRow sanPham = bang.Rows[0];

            imgAnh.ImageUrl = Convert.ToString(sanPham["ImageUrl"]);
            litTen.Text = Server.HtmlEncode(Convert.ToString(sanPham["Name"]));
            litHang.Text = Server.HtmlEncode(Convert.ToString(sanPham["Brand"]));
            litGia.Text = string.Format("{0:N0} đ", sanPham["Price"]);
            litMoTa.Text = Server.HtmlEncode(Convert.ToString(sanPham["Description"]));
            litTonKho.Text = Convert.ToString(sanPham["Stock"]);

            // Danh sách size lưu trong cột Sizes, cách nhau bằng dấu phẩy
            foreach (string size in Convert.ToString(sanPham["Sizes"]).Split(','))
                ddlSize.Items.Add(size.Trim());

            // Giữ lại thông tin sản phẩm cho lần bấm nút (postback)
            ViewState["ten"] = Convert.ToString(sanPham["Name"]);
            ViewState["gia"] = Convert.ToDecimal(sanPham["Price"]);
            ViewState["anh"] = Convert.ToString(sanPham["ImageUrl"]);
            ViewState["ton"] = Convert.ToInt32(sanPham["Stock"]);
        }

        // Bấm Thêm vào giỏ hàng.
        protected void btnThem_Click(object sender, EventArgs e)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("DangNhap.aspx?return=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            int soLuong;
            if (!int.TryParse(txtSoLuong.Text, out soLuong) || soLuong < 1)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Số lượng không hợp lệ.</span>";
                return;
            }

            int ton = Convert.ToInt32(ViewState["ton"]);

            if (ton <= 0)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Sản phẩm này đã hết hàng.</span>";
                return;
            }

            if (soLuong > ton)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Chỉ còn " + ton + " đôi.</span>";
                return;
            }

            MatHang hang = new MatHang();
            hang.MaSanPham = MaSanPham;
            hang.Ten = Convert.ToString(ViewState["ten"]);
            hang.Gia = Convert.ToDecimal(ViewState["gia"]);
            hang.Anh = Convert.ToString(ViewState["anh"]);
            hang.Size = ddlSize.SelectedValue;
            hang.SoLuong = soLuong;

            Gio.Them(hang);

            litKetQua.Text = "<span class=\"thong-bao tot\">Đã thêm "
                + Server.HtmlEncode(hang.Ten) + " (size " + Server.HtmlEncode(hang.Size)
                + ") vào giỏ. <a href=\"GioHang.aspx\">Xem giỏ hàng</a></span>";
        }
    }
}
