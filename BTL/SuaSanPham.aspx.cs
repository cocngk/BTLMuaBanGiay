using System;
using System.Data;

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
            DataTable bang = KetNoi.Bang(
                "SELECT * FROM Products WHERE Id = @id", KetNoi.P("@id", MaSanPham));

            if (bang.Rows.Count == 0)
            {
                litThongBao.Text = "<span class=\"thong-bao\">Không tìm thấy sản phẩm.</span>";
                return;
            }

            DataRow sanPham = bang.Rows[0];

            litTieuDe.Text = "Sửa sản phẩm";
            txtTen.Text = Convert.ToString(sanPham["Name"]);
            txtHang.Text = Convert.ToString(sanPham["Brand"]);
            txtGia.Text = Convert.ToString(sanPham["Price"]);
            txtTon.Text = Convert.ToString(sanPham["Stock"]);
            txtSize.Text = Convert.ToString(sanPham["Sizes"]);
            txtAnh.Text = Convert.ToString(sanPham["ImageUrl"]);
            txtMoTa.Text = Convert.ToString(sanPham["Description"]);
            chkNoiBat.Checked = Convert.ToBoolean(sanPham["IsHot"]);
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

            if (MaSanPham > 0)
            {
                KetNoi.ThucThi(
                    "UPDATE Products SET Name = @ten, Brand = @hang, Price = @gia, Stock = @ton, "
                    + "Sizes = @size, ImageUrl = @anh, Description = @moTa, IsHot = @noiBat "
                    + "WHERE Id = @id",
                    KetNoi.P("@ten", ten),
                    KetNoi.P("@hang", hang),
                    KetNoi.P("@gia", gia),
                    KetNoi.P("@ton", ton),
                    KetNoi.P("@size", size),
                    KetNoi.P("@anh", txtAnh.Text.Trim()),
                    KetNoi.P("@moTa", txtMoTa.Text.Trim()),
                    KetNoi.P("@noiBat", chkNoiBat.Checked),
                    KetNoi.P("@id", MaSanPham));
            }
            else
            {
                KetNoi.ThucThi(
                    "INSERT INTO Products (Name, Brand, Price, Stock, Sizes, ImageUrl, Description, IsHot) "
                    + "VALUES (@ten, @hang, @gia, @ton, @size, @anh, @moTa, @noiBat)",
                    KetNoi.P("@ten", ten),
                    KetNoi.P("@hang", hang),
                    KetNoi.P("@gia", gia),
                    KetNoi.P("@ton", ton),
                    KetNoi.P("@size", size),
                    KetNoi.P("@anh", txtAnh.Text.Trim()),
                    KetNoi.P("@moTa", txtMoTa.Text.Trim()),
                    KetNoi.P("@noiBat", chkNoiBat.Checked));
            }

            Response.Redirect("QuanTriSanPham.aspx");
        }
    }
}
