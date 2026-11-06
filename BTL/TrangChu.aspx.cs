using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class TrangChu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindProducts();
        }

        private void BindProducts()
        {
            rptProducts.DataSource = DuLieu.SanPhamNoiBat(4);
            rptProducts.DataBind();
        }

        protected bool IsProductLiked(int productId)
        {
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username)) return false;
            List<int> liked = Session["YeuThich_" + username] as List<int>;
            return liked != null && liked.Contains(productId);
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int productId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out productId)) return;
            SanPhamItem product = DuLieu.TimSanPham(productId);
            if (product == null) return;

            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                Response.Redirect("DangNhap.aspx?return=" + Server.UrlEncode(Request.RawUrl), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (e.CommandName == "Wishlist")
            {
                string key = "YeuThich_" + username;
                List<int> liked = Session[key] as List<int>;
                if (liked == null) Session[key] = liked = new List<int>();
                if (liked.Contains(productId))
                {
                    liked.Remove(productId);
                    litThongBao.Text = "<p class=\"home-action-message\">Đã bỏ sản phẩm khỏi danh sách yêu thích.</p>";
                }
                else
                {
                    liked.Add(productId);
                    litThongBao.Text = "<p class=\"home-action-message\">Đã thêm sản phẩm vào danh sách yêu thích.</p>";
                }
                BindProducts();
                return;
            }

            if (e.CommandName != "QuickAdd") return;
            if (product.Stock <= 0)
            {
                litThongBao.Text = "<p class=\"home-action-message error\">Sản phẩm này hiện đã hết hàng.</p>";
                return;
            }

            string size = (product.Sizes ?? string.Empty).Split(',')
                .Select(value => value.Trim()).FirstOrDefault(value => value.Length > 0) ?? "38";
            Gio.Them(new MatHang
            {
                MaSanPham = product.Id,
                Ten = product.Name,
                Gia = product.Price,
                Size = size,
                SoLuong = 1,
                Anh = product.ImageUrl
            });
            litThongBao.Text = "<p class=\"home-action-message\">Đã thêm "
                + Server.HtmlEncode(product.Name) + " (size " + Server.HtmlEncode(size) + ") vào giỏ hàng.</p>";
        }
    }
}
