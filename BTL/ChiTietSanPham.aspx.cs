using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            List<AnhSanPham> gallery = sanPham.Gallery ?? new List<AnhSanPham>();
            if (gallery.Count == 0)
            {
                gallery.Add(new AnhSanPham
                {
                    ThumbnailUrl = sanPham.ImageUrl,
                    ImageUrl = LayAnhLon(sanPham.ImageUrl)
                });
            }

            AnhSanPham anhDau = gallery[0];
            string anhLon = string.IsNullOrEmpty(anhDau.ImageUrl)
                ? LayAnhLon(anhDau.ThumbnailUrl)
                : anhDau.ImageUrl;
            imgAnh.ImageUrl = ResolveUrl(anhLon);
            imgAnh.AlternateText = "Ảnh " + sanPham.Name;
            rptAnh.DataSource = gallery.Select((anh, chiSo) => new
            {
                ThumbnailUrl = ResolveUrl(anh.ThumbnailUrl),
                ImageUrl = ResolveUrl(string.IsNullOrEmpty(anh.ImageUrl)
                    ? LayAnhLon(anh.ThumbnailUrl)
                    : anh.ImageUrl),
                AltText = "Ảnh " + sanPham.Name + " " + (chiSo + 1)
            }).ToList();
            rptAnh.DataBind();
            litDuongDan.Text = Server.HtmlEncode(sanPham.Name);
            litTen.Text = Server.HtmlEncode(sanPham.Name);
            litHang.Text = Server.HtmlEncode(sanPham.Brand);
            litMa.Text = "SNK-" + sanPham.Id.ToString("D4");
            litHangThongTin.Text = Server.HtmlEncode(sanPham.Brand);
            litMaThongTin.Text = "SNK-" + sanPham.Id.ToString("D4");
            litGia.Text = string.Format("{0:N0} đ", sanPham.Price);
            litMoTa.Text = Server.HtmlEncode(sanPham.Description);
            litMoTaDayDu.Text = Server.HtmlEncode(sanPham.Description);
            litTonKho.Text = sanPham.Stock <= 0
                ? "<span class=\"ton-kho het-hang\"><span class=\"cham-ton\"></span>Hết hàng</span>"
                : sanPham.Stock <= 5
                    ? "<span class=\"ton-kho sap-het\"><span class=\"cham-ton\"></span>Sắp hết · còn " + sanPham.Stock + " đôi</span>"
                    : "<span class=\"ton-kho con-hang\"><span class=\"cham-ton\"></span>Còn " + sanPham.Stock + " đôi</span>";

            // Danh sách size lưu trong một chuỗi, cách nhau bằng dấu phẩy
            string[] kichThuoc = sanPham.Sizes.Split(',')
                .Select(size => size.Trim())
                .Where(size => size.Length > 0)
                .ToArray();

            if (kichThuoc.Length == 0) kichThuoc = new[] { "38" };
            litSizeThongTin.Text = Server.HtmlEncode(string.Join(", ", kichThuoc));
            litTonKhoThongTin.Text = sanPham.Stock.ToString() + " đôi";
            hfSize.Value = kichThuoc[0];
            rptSize.DataSource = kichThuoc;
            rptSize.DataBind();
            txtSoLuong.Attributes["max"] = Math.Max(1, sanPham.Stock).ToString();
            btnThem.Enabled = sanPham.Stock > 0;

            if (sanPham.Stock == 0)
            {
                btnThem.Text = "Sản phẩm hết hàng";
                btnMuaNgay.Enabled = false;
            }

            CapNhatYeuThich(sanPham.Id);
            LoadDanhGia(sanPham.Id);
            var sanPhamLienQuan = DuLieu.SanPhams
                .Where(sp => sp.Id != sanPham.Id && sp.Brand == sanPham.Brand)
                .Take(4)
                .ToList();
            pnlLienQuan.Visible = sanPhamLienQuan.Count > 0;
            rptLienQuan.DataSource = sanPhamLienQuan;
            rptLienQuan.DataBind();
        }

        // Dùng ảnh lớn nếu có; ảnh do quản trị viên thêm có thể chỉ có một kích thước.
        private string LayAnhLon(string duongDan)
        {
            if (string.IsNullOrEmpty(duongDan)) return string.Empty;
            if (Uri.IsWellFormedUriString(duongDan, UriKind.Absolute)) return duongDan;

            string anhLon = duongDan.Replace("-sm.jpg", "-lg.jpg");
            string duongDanVatLy = Server.MapPath("~/" + anhLon.TrimStart('/', '~'));
            return File.Exists(duongDanVatLy) ? anhLon : duongDan;
        }

        // Bấm Thêm vào giỏ hàng.
        protected void btnThem_Click(object sender, EventArgs e)
        {
            ThemVaoGio(false);
        }

        protected void btnMuaNgay_Click(object sender, EventArgs e)
        {
            ThemVaoGio(true);
        }

        private void ThemVaoGio(bool muaNgay)
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
            string[] kichThuocHopLe = sanPham.Sizes.Split(',')
                .Select(size => size.Trim())
                .Where(size => size.Length > 0)
                .ToArray();
            string sizeDaChon = hfSize.Value;
            hang.Size = kichThuocHopLe.Contains(sizeDaChon)
                ? sizeDaChon
                : (kichThuocHopLe.Length > 0 ? kichThuocHopLe[0] : "38");
            hang.SoLuong = soLuong;

            if (muaNgay)
            {
                Gio.DatMuaNgay(hang);
                Response.Redirect("ThanhToan.aspx");
                return;
            }

            Gio.Them(hang);

            litKetQua.Text = "<span class=\"thong-bao tot\">Đã thêm "
                + Server.HtmlEncode(hang.Ten) + " (size " + Server.HtmlEncode(hang.Size)
                + ") vào giỏ. <a href=\"GioHang.aspx\">Xem giỏ hàng</a></span>";
        }

        protected void btnYeuThich_Click(object sender, EventArgs e)
        {
            string tenDangNhap = Session["Username"] as string;
            if (tenDangNhap == null)
            {
                litKetQua.Text = "<span class=\"thong-bao\">Vui lòng đăng nhập để lưu sản phẩm yêu thích.</span>";
                return;
            }

            string khoaYeuThich = "YeuThich_" + tenDangNhap;
            List<int> danhSach = Session[khoaYeuThich] as List<int>;
            if (danhSach == null)
            {
                danhSach = new List<int>();
                Session[khoaYeuThich] = danhSach;
            }

            if (danhSach.Contains(MaSanPham)) danhSach.Remove(MaSanPham);
            else danhSach.Add(MaSanPham);
            CapNhatYeuThich(MaSanPham);
        }

        private void CapNhatYeuThich(int maSanPham)
        {
            string tenDangNhap = Session["Username"] as string;
            List<int> danhSach = tenDangNhap == null
                ? null
                : Session["YeuThich_" + tenDangNhap] as List<int>;
            bool daYeuThich = danhSach != null && danhSach.Contains(maSanPham);
            btnYeuThich.Text = daYeuThich ? "♥ Đã yêu thích" : "♡ Yêu thích";
            btnYeuThich.CssClass = daYeuThich ? "nut-yeu-thich dang-yeu-thich" : "nut-yeu-thich";
        }

        private void LoadDanhGia(int maSanPham)
        {
            List<DanhGiaSanPham> danhSach = DuLieu.DanhGiaTheoSanPham(maSanPham);
            double trungBinh = danhSach.Count == 0 ? 0 : danhSach.Average(dg => dg.Rating);
            litDiemTrungBinh.Text = trungBinh.ToString("0.0");
            litSaoTrungBinh.Text = new string('★', (int)Math.Round(trungBinh))
                + new string('☆', 5 - (int)Math.Round(trungBinh));
            litSoDanhGia.Text = danhSach.Count + " đánh giá";
            rptDanhGia.DataSource = danhSach;
            rptDanhGia.DataBind();
            pnlChuaCoDanhGia.Visible = danhSach.Count == 0;

            string tenDangNhap = Session["Username"] as string;
            bool daMua = !string.IsNullOrEmpty(tenDangNhap) && DuLieu.DonHangs.Any(don =>
                don.Username == tenDangNhap && don.Lines.Any(dong => dong.ProductId == maSanPham));
            pnlFormDanhGia.Visible = daMua;
            pnlCanMuaDeDanhGia.Visible = !daMua;

            if (daMua)
            {
                DanhGiaSanPham daDanhGia = danhSach.FirstOrDefault(dg => dg.Username == tenDangNhap);
                if (daDanhGia != null)
                {
                    ddlSao.SelectedValue = daDanhGia.Rating.ToString();
                    txtDanhGia.Text = daDanhGia.Comment;
                }
            }
        }

        protected void btnGuiDanhGia_Click(object sender, EventArgs e)
        {
            string tenDangNhap = Session["Username"] as string;
            int rating;
            string nhanXet = txtDanhGia.Text.Trim();
            bool daMua = !string.IsNullOrEmpty(tenDangNhap) && DuLieu.DonHangs.Any(don =>
                don.Username == tenDangNhap && don.Lines.Any(dong => dong.ProductId == MaSanPham));

            if (!daMua)
            {
                litThongBaoDanhGia.Text = "<span class=\"loi-danh-gia\">Bạn cần đăng nhập và có đơn hàng chứa sản phẩm này để đánh giá.</span>";
                return;
            }
            if (!int.TryParse(ddlSao.SelectedValue, out rating) || rating < 1 || rating > 5)
            {
                litThongBaoDanhGia.Text = "<span class=\"loi-danh-gia\">Vui lòng chọn mức đánh giá hợp lệ.</span>";
                return;
            }
            if (nhanXet.Length < 5 || nhanXet.Length > 1000)
            {
                litThongBaoDanhGia.Text = "<span class=\"loi-danh-gia\">Nhận xét cần từ 5 đến 1000 ký tự.</span>";
                return;
            }

            string tenHienThi = Convert.ToString(Session["FullName"]);
            DuLieu.LuuDanhGia(new DanhGiaSanPham
            {
                ProductId = MaSanPham,
                Username = tenDangNhap,
                CustomerName = string.IsNullOrWhiteSpace(tenHienThi) ? tenDangNhap : tenHienThi,
                Rating = rating,
                Comment = nhanXet,
                CreatedAt = DateTime.Now
            });
            LoadDanhGia(MaSanPham);
            litThongBaoDanhGia.Text = "<span class=\"thanh-cong-danh-gia\">Cảm ơn bạn, đánh giá đã được lưu.</span>";
        }
    }
}
