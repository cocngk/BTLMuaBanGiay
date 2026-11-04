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
            get { int id; return int.TryParse(Request.QueryString["id"], out id) && id > 0 ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin()) return;
            string raw = Request.QueryString["id"];
            int id;
            if (!string.IsNullOrEmpty(raw) && (!int.TryParse(raw, out id) || id <= 0))
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

        private void LoadForm()
        {
            ProductItem product = ProductId == 0 ? null : Catalog.Find(ProductId);
            if (product == null && ProductId > 0)
            {
                pnlAdmin.Visible = false;
                Toast.ShowAndRedirect(this, "Không tìm thấy sản phẩm cần sửa. Có thể nó vừa bị xoá.", "info", "QuanTriSanPham.aspx");
                return;
            }
            if (product == null)
            {
                litTitle.Text = "Thêm sản phẩm";
                litSub.Text = "Sản phẩm mới sẽ hiện ngay ở trang Sản phẩm của cửa hàng.";
                txtStock.Text = "10";
                btnDelete.Visible = false;
                UpdatePreview();
                return;
            }
            litTitle.Text = "Sửa sản phẩm";
            litSub.Text = "Mã #" + product.Id + " · " + product.Name;
            txtName.Text = product.Name;
            txtBrand.Text = product.Brand;
            txtDescription.Text = product.Description;
            txtPrice.Text = product.Price.ToString("0", CultureInfo.InvariantCulture);
            txtOldPrice.Text = product.OldPrice.HasValue ? product.OldPrice.Value.ToString("0", CultureInfo.InvariantCulture) : "";
            txtStock.Text = product.Stock.ToString(CultureInfo.InvariantCulture);
            txtSizes.Text = product.Sizes;
            txtImageUrl.Text = product.ImageUrl;
            txtImageUrlLarge.Text = product.ImageUrlLarge;
            btnDelete.Visible = true;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            string url = txtImageUrlLarge.Text.Trim();
            if (url.Length == 0) url = txtImageUrl.Text.Trim();
            imgPreview.Visible = url.Length > 0;
            if (imgPreview.Visible) imgPreview.ImageUrl = ResolveUrl(url);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin()) return;
            decimal price;
            if (!ReadMoney(txtPrice.Text, out price)) { ShowError("Giá bán phải là số, ví dụ 2500000 hoặc 2.500.000."); return; }
            decimal old;
            decimal? oldPrice = string.IsNullOrWhiteSpace(txtOldPrice.Text) ? (decimal?)null : (ReadMoney(txtOldPrice.Text, out old) ? old : (decimal?)null);
            if (!string.IsNullOrWhiteSpace(txtOldPrice.Text) && !oldPrice.HasValue) { ShowError("Giá gạch phải là số. Để trống nếu sản phẩm không giảm giá."); return; }
            int stock;
            if (!int.TryParse(txtStock.Text.Trim(), out stock)) { ShowError("Tồn kho phải là số nguyên, ví dụ 10."); return; }
            SaveProductOutcome result = Catalog.Save(new ProductItem { Id = ProductId, Name = txtName.Text, Brand = txtBrand.Text, Description = txtDescription.Text, Price = price, OldPrice = oldPrice, Stock = stock, Sizes = txtSizes.Text, ImageUrl = txtImageUrl.Text, ImageUrlLarge = txtImageUrlLarge.Text });
            if (result == SaveProductOutcome.Created) { Toast.ShowAndRedirect(this, "Đã thêm sản phẩm mới.", "success", "QuanTriSanPham.aspx"); return; }
            if (result == SaveProductOutcome.Updated) { Toast.ShowAndRedirect(this, "Đã lưu thay đổi.", "success", "QuanTriSanPham.aspx"); return; }
            ShowError(Message(result));
            UpdatePreview();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin() || ProductId <= 0) return;
            ProductItem product = Catalog.Find(ProductId);
            if (product == null) { Toast.ShowAndRedirect(this, "Sản phẩm này đã bị xoá trước đó.", "info", "QuanTriSanPham.aspx"); return; }
            if (Catalog.Delete(ProductId)) Toast.ShowAndRedirect(this, "Đã xoá sản phẩm " + product.Name + ".", "success", "QuanTriSanPham.aspx");
            else ShowError("Không xoá được sản phẩm này. Vui lòng thử lại.");
        }

        private static bool ReadMoney(string raw, out decimal value)
        {
            string clean = (raw ?? "").Trim().Replace(".", "").Replace(",", "").Replace(" ", "").Replace("đ", "").Replace("₫", "");
            return decimal.TryParse(clean, NumberStyles.None, CultureInfo.InvariantCulture, out value);
        }

        private static string Message(SaveProductOutcome result)
        {
            switch (result)
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

        private void ShowError(string message) { litFormError.Text = message; pnlFormError.Visible = true; }
    }
}
