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
        protected void btnHeroDiscover_Click(object nguonSuKien, EventArgs e) { Redirect("SanPham.aspx"); }

        // Xử lý lệnh trên một dòng dữ liệu.
        protected void rptProducts_ItemCommand(object nguon, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "QuickAdd" || !RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!")) return;
            ProductItem sanPham = Catalog.Find(Convert.ToInt32(e.CommandArgument));
            if (sanPham == null) return;
            string kichThuoc = sanPham.FirstSize();
            CartStore.AddProduct(sanPham, kichThuoc, 1);
            Toast.Success(this, "Đã thêm " + sanPham.Name + " (Size " + kichThuoc + ") vào giỏ hàng!");
        }
    }
}
