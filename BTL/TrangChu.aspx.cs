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
            rptProducts.DataSource = ProductStore.GetAll().Take(4).ToList();
            rptProducts.DataBind();
        }

        // BẤM NÚT "KHÁM PHÁ NGAY" TRÊN HERO BANNER
        protected void btnHeroDiscover_Click(object sender, EventArgs e)
        {
            // Xem danh mục không cần đăng nhập — chỉ mua mới cần. Trước đây nút này
            // chặn đăng nhập trong khi trang Sản phẩm vẫn cho xem thoải mái.
            Redirect("SanPham.aspx");
        }

        // BẤM NÚT DẤU CỘNG Ở CÁC SẢN PHẨM TRANG CHỦ
        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "QuickAdd")
            {
                // 1. KIỂM TRA ĐĂNG NHẬP
                if (!RequireLogin("Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!"))
                {
                    return;
                }

                // 2. THÊM SẢN PHẨM VÀO GIỎ HÀNG (size đầu tiên có sẵn, key gồm Id + Size)
                int productId = Convert.ToInt32(e.CommandArgument);
                ProductItem selected = ProductStore.GetAll().FirstOrDefault(p => p.Id == productId);

                if (selected != null)
                {
                    string size = selected.FirstSize();
                    CartStore.AddOrMerge(selected.Id, selected.Name, selected.Price, size, 1, selected.ImageUrl, selected.Stock);
                    Toast.Success(this, "HomeAddToCartSuccessToast",
                        "Đã thêm " + selected.Name + " (Size " + size + ") vào giỏ hàng!");
                }
            }
        }
    }
}
