using System;
using System.Linq;
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
                LoadHomeProducts();
            }
        }

        private void LoadHomeProducts()
        {
            rptProducts.DataSource = Catalog.Featured(4);
            rptProducts.DataBind();
        }

        // BẤM NÚT "KHÁM PHÁ NGAY" TRÊN HERO BANNER
        protected void btnHeroDiscover_Click(object sender, EventArgs e)
        {
            // Xem danh mục không cần đăng nhập — chỉ mua mới cần. Trước đây nút này
            // chặn đăng nhập trong khi trang Sản phẩm vẫn cho xem thoải mái.
            Redirect("SanPham.aspx");
        }

        // Nút dấu cộng ở thẻ sản phẩm trên trang chủ: thêm nhanh size đầu tiên.
        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "QuickAdd")
            {
                // Chưa đăng nhập thì mời đăng nhập, không thêm gì vào giỏ.
                if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!"))
                {
                    return;
                }

                // Thêm size đầu tiên còn hàng; dòng giỏ được khóa theo Id + Size.
                int productId = Convert.ToInt32(e.CommandArgument);
                ProductItem selected = Catalog.Find(productId);

                if (selected != null)
                {
                    string size = selected.FirstSize();
                    CartStore.AddProduct(selected, size, 1);
                    Toast.Success(this,
                        "Đã thêm " + selected.Name + " (Size " + size + ") vào giỏ hàng!");
                }
            }
        }
    }
}
