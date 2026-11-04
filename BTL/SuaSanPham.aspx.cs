using System;
using System.Globalization;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class SuaSanPham : ShopPage
    {
        private int ProductId
        {
            get { int ma; return int.TryParse(Request.QueryString["id"], out ma) && ma > 0 ? ma : 0; }
        }

        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            string chuoiGoc = Request.QueryString["id"];
            int ma;
            if (!string.IsNullOrEmpty(chuoiGoc) && (!int.TryParse(chuoiGoc, out ma) || ma <= 0))
            {
                Toast.ShowAndRedirect(this, "Đường dẫn sửa sản phẩm không hợp lệ. Hãy chọn lại từ danh sách.", "info", "QuanTriSanPham.aspx");
                return;
            }
            pnlAdmin.Visible = true;
            rptBrands.DataSource = Catalog.Brands();
            rptBrands.DataBind();
            txtBrand.Attributes["list"] = "dlBrands";
            if (!IsPostBack) LoadForm();
        }

        // Thực hiện việc tải form.
        private void LoadForm()
        {
            ProductItem sanPham = ProductId == 0 ? null : Catalog.Find(ProductId);
            if (sanPham == null && ProductId > 0)
            {
                pnlAdmin.Visible = false;
                Toast.ShowAndRedirect(this, "Không tìm thấy sản phẩm cần sửa. Có thể nó vừa bị xoá.", "info", "QuanTriSanPham.aspx");
                return;
            }
            if (sanPham == null)
            {
                litTitle.Text = "Thêm sản phẩm";
                litSub.Text = "Sản phẩm mới sẽ hiện ngay ở trang Sản phẩm của cửa hàng.";
                txtStock.Text = "10";
                btnDelete.Visible = false;
                UpdatePreview();
                return;
            }
            litTitle.Text = "Sửa sản phẩm";
            litSub.Text = "Mã #" + sanPham.Id + " · " + sanPham.Name;
            txtName.Text = sanPham.Name;
            txtBrand.Text = sanPham.Brand;
            txtDescription.Text = sanPham.Description;
            txtPrice.Text = sanPham.Price.ToString("0", CultureInfo.InvariantCulture);
            txtOldPrice.Text = sanPham.OldPrice.HasValue ? sanPham.OldPrice.Value.ToString("0", CultureInfo.InvariantCulture) : "";
            txtStock.Text = sanPham.Stock.ToString(CultureInfo.InvariantCulture);
            txtSizes.Text = sanPham.Sizes;
            txtImageUrl.Text = sanPham.ImageUrl;
            txtImageUrlLarge.Text = sanPham.ImageUrlLarge;
            btnDelete.Visible = true;
            UpdatePreview();
        }

        // Thực hiện việc cập nhật preview.
        private void UpdatePreview()
        {
            string duongDan = txtImageUrlLarge.Text.Trim();
            if (duongDan.Length == 0) duongDan = txtImageUrl.Text.Trim();
            imgPreview.Visible = duongDan.Length > 0;
            if (imgPreview.Visible) imgPreview.ImageUrl = ResolveUrl(duongDan);
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnSave_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin()) return;
            decimal gia;
            if (!ReadMoney(txtPrice.Text, out gia)) { ShowError("Giá bán phải là số, ví dụ 2500000 hoặc 2.500.000."); return; }
            decimal cu;
            decimal? oldPrice = string.IsNullOrWhiteSpace(txtOldPrice.Text) ? (decimal?)null : (ReadMoney(txtOldPrice.Text, out cu) ? cu : (decimal?)null);
            if (!string.IsNullOrWhiteSpace(txtOldPrice.Text) && !oldPrice.HasValue) { ShowError("Giá gạch phải là số. Để trống nếu sản phẩm không giảm giá."); return; }
            int stock;
            if (!int.TryParse(txtStock.Text.Trim(), out stock)) { ShowError("Tồn kho phải là số nguyên, ví dụ 10."); return; }
            SaveProductOutcome ketQua = Catalog.Save(new ProductItem { Id = ProductId, Name = txtName.Text, Brand = txtBrand.Text, Description = txtDescription.Text, Price = gia, OldPrice = oldPrice, Stock = stock, Sizes = txtSizes.Text, ImageUrl = txtImageUrl.Text, ImageUrlLarge = txtImageUrlLarge.Text });
            if (ketQua == SaveProductOutcome.Created) { Toast.ShowAndRedirect(this, "Đã thêm sản phẩm mới.", "success", "QuanTriSanPham.aspx"); return; }
            if (ketQua == SaveProductOutcome.Updated) { Toast.ShowAndRedirect(this, "Đã lưu thay đổi.", "success", "QuanTriSanPham.aspx"); return; }
            ShowError(Message(ketQua));
            UpdatePreview();
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnDelete_Click(object nguonSuKien, EventArgs e)
        {
            if (!RequireAdmin() || ProductId <= 0) return;
            ProductItem sanPham = Catalog.Find(ProductId);
            if (sanPham == null) { Toast.ShowAndRedirect(this, "Sản phẩm này đã bị xoá trước đó.", "info", "QuanTriSanPham.aspx"); return; }
            if (Catalog.Delete(ProductId)) Toast.ShowAndRedirect(this, "Đã xoá sản phẩm " + sanPham.Name + ".", "success", "QuanTriSanPham.aspx");
            else ShowError("Không xoá được sản phẩm này. Vui lòng thử lại.");
        }

        // Thực hiện việc đọc tiền.
        private static bool ReadMoney(string chuoiGoc, out decimal giaTri)
        {
            string clean = (chuoiGoc ?? "").Trim().Replace(".", "").Replace(",", "").Replace(" ", "").Replace("đ", "").Replace("₫", "");
            return decimal.TryParse(clean, NumberStyles.None, CultureInfo.InvariantCulture, out giaTri);
        }

        // Thực hiện việc message.
        private static string Message(SaveProductOutcome ketQua)
        {
            switch (ketQua)
            {
                case SaveProductOutcome.NotFound: return "Sản phẩm không còn tồn tại — hãy quay lại danh sách.";
                case SaveProductOutcome.InvalidName: return "Tên sản phẩm không được để trống và tối đa 120 ký tự.";
                case SaveProductOutcome.InvalidBrand: return "Thương hiệu không được để trống và tối đa 60 ký tự.";
                case SaveProductOutcome.InvalidDescription: return "Mô tả tối đa 2000 ký tự.";
                case SaveProductOutcome.InvalidPrice: return "Giá bán phải lớn hơn 0.";
                case SaveProductOutcome.InvalidStock: return "Tồn kho không được âm.";
                case SaveProductOutcome.InvalidSizes: return "Size chưa hợp lệ: nhập các số từ 20 đến 60, cách nhau bằng dấu phẩy.";
                case SaveProductOutcome.InvalidImage: return "Cần có đường dẫn ảnh nhỏ, tối đa 400 ký tự.";
                default: return "Không lưu được sản phẩm. Vui lòng kiểm tra lại các ô nhập.";
            }
        }

        // Thực hiện việc hiển thị error.
        private void ShowError(string thongBao) { litFormError.Text = thongBao; pnlFormError.Visible = true; }
    }
}
