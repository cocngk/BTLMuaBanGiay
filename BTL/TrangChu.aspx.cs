using System;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TrangChu : System.Web.UI.Page
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
            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this, "HomeHeroRequireLogin",
                    "Vui lòng đăng nhập hoặc đăng ký tài khoản để khám phá cửa hàng!");
                return;
            }

            Response.Redirect("SanPham.aspx");
        }

        // BẤM NÚT DẤU CỘNG Ở CÁC SẢN PHẨM TRANG CHỦ
        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "QuickAdd")
            {
                // 1. KIỂM TRA ĐĂNG NHẬP
                if (!AuthHelper.IsLoggedIn())
                {
                    Toast.RequireLogin(this, "HomeRequireLoginToast",
                        "Vui lòng đăng nhập hoặc đăng ký tài khoản để mua hàng!");
                    return;
                }

                // 2. THÊM SẢN PHẨM VÀO GIỎ HÀNG (size đầu tiên có sẵn, key gồm Id + Size)
                int productId = Convert.ToInt32(e.CommandArgument);
                ProductItem selected = ProductStore.GetAll().FirstOrDefault(p => p.Id == productId);

                if (selected != null)
                {
                    string size = (selected.Sizes ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()).FirstOrDefault() ?? "40";
                    new SessionCartStore(Session).AddOrMerge(selected.Id, selected.Name, selected.Price, size, 1, selected.ImageUrl, selected.Stock);
                    Toast.Success(this, "HomeAddToCartSuccessToast",
                        "Đã thêm " + selected.Name + " (Size " + size + ") vào giỏ hàng!");
                }
            }
        }
    }
}
