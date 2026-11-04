using System;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TrangChu : ShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptProducts.DataSource = Catalog.Featured(4);
                rptProducts.DataBind();
            }
        }

        protected void btnHeroDiscover_Click(object sender, EventArgs e) { Redirect("SanPham.aspx"); }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "QuickAdd" || !RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!")) return;
            ProductItem product = Catalog.Find(Convert.ToInt32(e.CommandArgument));
            if (product == null) return;
            string size = product.FirstSize();
            CartStore.AddProduct(product, size, 1);
            Toast.Success(this, "Đã thêm " + product.Name + " (Size " + size + ") vào giỏ hàng!");
        }
    }
}
