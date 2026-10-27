using System;
using System.Globalization;
using System.Web.UI;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    /// <summary>
    /// Form thêm / sửa sản phẩm. Một trang cho cả hai việc: có <c>?id=</c> là sửa,
    /// không có là thêm mới — hai chế độ dùng chung đúng một bộ ô nhập và một bộ
    /// kiểm tra, nên không thể lệch luật giữa "thêm" và "sửa".
    /// Trang chỉ gửi giá trị lên Catalog.Save; mọi kiểm tra và ghi nằm trong đó.
    /// </summary>
    public partial class SuaSanPham : ShopPage
    {
        /// <summary>Id cần sửa; 0 nghĩa là thêm mới. Query string còn nguyên qua postback.</summary>
        private int ProductId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) && id > 0 ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            pnlAdmin.Visible = true;

            // Gợi ý hãng phải có cả sau postback, nếu không danh sách gợi ý biến mất
            // ngay khi người dùng bấm Lưu và phải sửa lại một trường.
            BindBrandSuggestions();
            txtBrand.Attributes["list"] = "dlBrands";

            if (!IsPostBack)
            {
                LoadForm();
            }
        }

        private void BindBrandSuggestions()
        {
            rptBrands.DataSource = Catalog.Brands();
            rptBrands.DataBind();
        }

        private void LoadForm()
        {
            int id = ProductId;

            if (id == 0)
            {
                litTitle.Text = "Thêm sản phẩm";
                litSub.Text = "Sản phẩm mới sẽ hiện ngay ở trang Sản phẩm của cửa hàng.";
                txtStock.Text = "10";
                btnDelete.Visible = false;
                UpdatePreview();
                return;
            }

            ProductItem product = Catalog.Find(id);
            if (product == null)
            {
                pnlAdmin.Visible = false;
                Toast.ShowAndRedirect(this, "Không tìm thấy sản phẩm cần sửa. Có thể nó vừa bị xoá.",
                    "info", "QuanTriSanPham.aspx");
                return;
            }

            litTitle.Text = "Sửa sản phẩm";
            litSub.Text = "Mã #" + product.Id + " · " + product.Name;

            // Chép giá trị sang ô nhập, không giữ tham chiếu: Find trả về đúng đối tượng
            // đang sống trong Application, gán thẳng vào form là sửa dở dang cũng dính
            // dù người dùng bấm Huỷ.
            txtName.Text = product.Name;
            txtBrand.Text = product.Brand;
            txtDescription.Text = product.Description;
            txtPrice.Text = product.Price.ToString("0", CultureInfo.InvariantCulture);
            txtOldPrice.Text = product.OldPrice.HasValue
                ? product.OldPrice.Value.ToString("0", CultureInfo.InvariantCulture)
                : "";
            txtStock.Text = product.Stock.ToString(CultureInfo.InvariantCulture);
            txtSizes.Text = product.Sizes;
            txtImageUrl.Text = product.ImageUrl;
            txtImageUrlLarge.Text = product.ImageUrlLarge;

            btnDelete.Visible = true;
            UpdatePreview();
        }

        /// <summary>Ảnh xem trước bám theo ô đường dẫn để thấy ngay mình gõ đúng hay sai.</summary>
        private void UpdatePreview()
        {
            string url = txtImageUrlLarge.Text.Trim();
            if (url.Length == 0)
            {
                url = txtImageUrl.Text.Trim();
            }

            if (url.Length == 0)
            {
                imgPreview.Visible = false;
                return;
            }

            imgPreview.Visible = true;
            imgPreview.ImageUrl = ResolveUrl(url);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            decimal price;
            if (!TryReadMoney(txtPrice.Text, out price))
            {
                ShowFormError("Giá bán phải là số, ví dụ 2500000 hoặc 2.500.000.");
                return;
            }

            decimal? oldPrice = null;
            string rawOldPrice = txtOldPrice.Text.Trim();
            if (rawOldPrice.Length > 0)
            {
                decimal parsed;
                if (!TryReadMoney(rawOldPrice, out parsed))
                {
                    ShowFormError("Giá gạch phải là số. Để trống nếu sản phẩm không giảm giá.");
                    return;
                }
                oldPrice = parsed;
            }

            int stock;
            if (!int.TryParse(txtStock.Text.Trim(), out stock))
            {
                ShowFormError("Tồn kho phải là số nguyên, ví dụ 10.");
                return;
            }

            ProductItem input = new ProductItem
            {
                Id = ProductId,
                Name = txtName.Text,
                Brand = txtBrand.Text,
                Description = txtDescription.Text,
                Price = price,
                OldPrice = oldPrice,
                Stock = stock,
                Sizes = txtSizes.Text,
                ImageUrl = txtImageUrl.Text,
                ImageUrlLarge = txtImageUrlLarge.Text
            };

            SaveProductOutcome outcome = Catalog.Save(input);
            switch (outcome)
            {
                case SaveProductOutcome.Created:
                    Toast.ShowAndRedirect(this, "Đã thêm sản phẩm mới.", "success", "QuanTriSanPham.aspx");
                    return;

                case SaveProductOutcome.Updated:
                    Toast.ShowAndRedirect(this, "Đã lưu thay đổi.", "success", "QuanTriSanPham.aspx");
                    return;

                default:
                    ShowFormError(MessageFor(outcome));
                    UpdatePreview();
                    return;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!RequireAdmin())
            {
                return;
            }

            int id = ProductId;
            if (id <= 0)
            {
                return;
            }

            ProductItem product = Catalog.Find(id);
            if (product == null)
            {
                Toast.ShowAndRedirect(this, "Sản phẩm này đã bị xoá trước đó.", "info", "QuanTriSanPham.aspx");
                return;
            }

            if (Catalog.Delete(id))
            {
                Toast.ShowAndRedirect(this, "Đã xoá sản phẩm " + product.Name + ".", "success", "QuanTriSanPham.aspx");
            }
            else
            {
                ShowFormError("Không xoá được sản phẩm này. Vui lòng thử lại.");
            }
        }

        /// <summary>
        /// Đọc tiền theo kiểu người Việt gõ: "2.500.000", "2 500 000" hay "2500000"
        /// đều ra 2500000. Bỏ hết dấu phân cách thay vì đoán culture — giá ở đây là
        /// số nguyên đồng, không có phần lẻ để nhầm.
        /// </summary>
        private static bool TryReadMoney(string raw, out decimal value)
        {
            string clean = (raw ?? "").Trim()
                .Replace(".", "")
                .Replace(",", "")
                .Replace(" ", "")
                .Replace("đ", "")
                .Replace("₫", "");

            return decimal.TryParse(clean, NumberStyles.None, CultureInfo.InvariantCulture, out value);
        }

        private static string MessageFor(SaveProductOutcome outcome)
        {
            switch (outcome)
            {
                case SaveProductOutcome.NotFound:
                    return "Sản phẩm không còn tồn tại — có thể vừa bị xoá ở cửa sổ khác. Hãy quay lại danh sách.";
                case SaveProductOutcome.InvalidName:
                    return "Tên sản phẩm không được để trống và tối đa 120 ký tự.";
                case SaveProductOutcome.InvalidBrand:
                    return "Thương hiệu không được để trống và tối đa 60 ký tự.";
                case SaveProductOutcome.InvalidDescription:
                    return "Mô tả tối đa 2000 ký tự.";
                case SaveProductOutcome.InvalidPrice:
                    return "Giá bán phải lớn hơn 0.";
                case SaveProductOutcome.InvalidStock:
                    return "Tồn kho không được âm.";
                case SaveProductOutcome.InvalidSizes:
                    return "Size chưa hợp lệ: nhập các số từ 20 đến 60, cách nhau bằng dấu phẩy (ví dụ 38,39,40).";
                case SaveProductOutcome.InvalidImage:
                    return "Cần có đường dẫn ảnh nhỏ, tối đa 400 ký tự.";
                default:
                    return "Không lưu được sản phẩm. Vui lòng kiểm tra lại các ô nhập.";
            }
        }

        private void ShowFormError(string message)
        {
            litFormError.Text = message;
            pnlFormError.Visible = true;
        }
    }
}
