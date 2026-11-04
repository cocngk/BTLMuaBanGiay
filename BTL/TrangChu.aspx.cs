using System;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TrangChu : ShopPage
    {
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptProducts.DataSource = Catalog.Featured(4);
                rptProducts.DataBind();
            }
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnHeroDiscover_Click(object nguonSuKien, EventArgs e)
        {
            Redirect("SanPham.aspx");
        }

        // Xử lý lệnh trên một dòng dữ liệu.
        protected void rptProducts_ItemCommand(object nguon, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "QuickAdd") return;
            if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!")) return;

            int maSanPham;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out maSanPham)) return;
            ProductItem sanPham = Catalog.Find(maSanPham);
            if (sanPham == null) return;
            if (sanPham.Stock <= 0)
            {
                Toast.Info(this, "Sản phẩm này hiện đã hết hàng.");
                return;
            }

            string kichThuoc = sanPham.FirstSize();
            CartStore.AddProduct(sanPham, kichThuoc, 1);
            Toast.Success(this, "Đã thêm " + sanPham.Name + " (Size " + kichThuoc + ") vào giỏ hàng!");
        }
    }
}
