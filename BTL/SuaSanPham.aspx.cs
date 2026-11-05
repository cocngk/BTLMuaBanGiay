using System;

namespace ShoeStore
{
    // Thêm mới hoặc sửa một sản phẩm. Có ?id=... là sửa, không có là thêm.
    public partial class SuaSanPham : System.Web.UI.Page
    {
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
            if (Session["Username"] == null)
            {
                Response.Redirect("DangNhap.aspx?return=SuaSanPham.aspx");
                return;
            }

            if (Session["IsAdmin"] == null || !Convert.ToBoolean(Session["IsAdmin"]))
            {
                Response.Redirect("TrangChu.aspx");
                return;
            }

            if (IsPostBack) return;

            if (MaSanPham > 0) NapSanPham();
        }

        // Điền sẵn thông tin sản phẩm đang sửa.
        private void NapSanPham()
        {
            SanPhamItem sanPham = DuLieu.TimSanPham(MaSanPham);

            if (sanPham == null)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Không tìm thấy sản phẩm.</span>";
                return;
            }

            litTieuDe.Text = "Sửa sản phẩm";
            txtTen.Text = sanPham.Name;
            txtHang.Text = sanPham.Brand;
            txtGia.Text = sanPham.Price.ToString();
            txtTon.Text = sanPham.Stock.ToString();
            txtSize.Text = sanPham.Sizes;
            txtAnh.Text = sanPham.ImageUrl;
            txtMoTa.Text = sanPham.Description;
            chkNoiBat.Checked = sanPham.IsHot;
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            string ten = txtTen.Text.Trim();
            string hang = txtHang.Text.Trim();

            if (ten == "" || hang == "")
            {
                litThongBao.Text = "<span class=\"thong-bao\">Vui lòng nhập tên sản phẩm và hãng.</span>";
                return;
            }

            decimal gia;
            if (!decimal.TryParse(txtGia.Text, out gia) || gia <= 0)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Giá phải là số lớn hơn 0.</span>";
                return;
            }

            int ton;
            if (!int.TryParse(txtTon.Text, out ton) || ton < 0)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Số lượng trong kho phải là số không âm.</span>";
                return;
            }

            string size = txtSize.Text.Trim();
            if (size == "")
            {
                litThongBao.Text = "<span class=\"thong-bao\">Vui lòng nhập ít nhất một size.</span>";
                return;
            }

            // Sửa sản phẩm đang có
            SanPhamItem sanPham = MaSanPham > 0 ? DuLieu.TimSanPham(MaSanPham) : null;

            if (sanPham == null)
            {
                // Không có thì thêm mới
                sanPham = new SanPhamItem();
                GanGiaTri(sanPham, ten, hang, gia, ton, size);
                DuLieu.ThemSanPham(sanPham);
            }
            else
            {
                GanGiaTri(sanPham, ten, hang, gia, ton, size);
            }

            Response.Redirect("QuanTriSanPham.aspx");
        }

        // Chép dữ liệu từ form vào sản phẩm.
        private void GanGiaTri(SanPhamItem sanPham, string ten, string hang,
            decimal gia, int ton, string size)
        {
            sanPham.Name = ten;
            sanPham.Brand = hang;
            sanPham.Price = gia;
            sanPham.Stock = ton;
            sanPham.Sizes = size;
            sanPham.ImageUrl = txtAnh.Text.Trim();
            sanPham.Description = txtMoTa.Text.Trim();
            sanPham.IsHot = chkNoiBat.Checked;
        }
    }
}
