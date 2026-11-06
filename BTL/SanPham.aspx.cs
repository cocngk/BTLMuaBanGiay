using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace ShoeStore
{
    public partial class SanPham : System.Web.UI.Page
    {
        private const int KichThuocTrang = 6;
        private int TrangHienTai
        {
            get
            {
                int trang;
                return int.TryParse(Request.QueryString["page"], out trang) && trang > 0 ? trang : 1;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cblBrands.Items.Clear();
            foreach (string hang in DuLieu.DanhSachHang())
                cblBrands.Items.Add(new ListItem(hang, hang));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadControlsFromQuery();

            BindProducts(IsPostBack ? TrangHienTai : ReadPage());
        }

        private int ReadPage()
        {
            int page;
            return int.TryParse(Request.QueryString["page"], out page) && page > 0 ? page : 1;
        }

        private void LoadControlsFromQuery()
        {
            txtStoreSearch.Text = Request.QueryString["search"] ?? string.Empty;
            string[] brands = (Request.QueryString["brand"] ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (ListItem item in cblBrands.Items)
                item.Selected = brands.Any(brand => string.Equals(brand, item.Value, StringComparison.OrdinalIgnoreCase));

            string price = Request.QueryString["price"];
            if (rblPriceRange.Items.FindByValue(price) != null)
                rblPriceRange.SelectedValue = price;
            else
                rblPriceRange.SelectedValue = "All";

            string size = Request.QueryString["size"];
            hfSelectedSize.Value = new[] { "38", "39", "40", "41", "42", "43" }.Contains(size) ? size : string.Empty;

            string sort = Request.QueryString["sort"];
            if (ddlSort.Items.FindByValue(sort) != null)
                ddlSort.SelectedValue = sort;
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect(BuildUrl(1), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(BuildUrl(1), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected bool IsProductLiked(int productId)
        {
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username)) return false;
            List<int> liked = Session["YeuThich_" + username] as List<int>;
            return liked != null && liked.Contains(productId);
        }

        protected void rptSanPham_ItemCommand(object source, RepeaterCommandEventArgs e)
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
                    litActionMessage.Text = "<p class=\"shop-action-message\">Đã bỏ sản phẩm khỏi danh sách yêu thích.</p>";
                }
                else
                {
                    liked.Add(productId);
                    litActionMessage.Text = "<p class=\"shop-action-message\">Đã thêm sản phẩm vào danh sách yêu thích.</p>";
                }
                BindProducts(TrangHienTai);
                return;
            }

            if (product.Stock <= 0)
            {
                litActionMessage.Text = "<p class=\"shop-action-message error\">Sản phẩm này hiện đã hết hàng.</p>";
                return;
            }

            string size = (product.Sizes ?? string.Empty).Split(',')
                .Select(value => value.Trim()).FirstOrDefault(value => value.Length > 0) ?? "38";
            MatHang hangMua = new MatHang
            {
                MaSanPham = product.Id,
                Ten = product.Name,
                Gia = product.Price,
                Size = size,
                SoLuong = 1,
                Anh = product.ImageUrl
            };

            if (e.CommandName == "BuyNow")
            {
                Gio.DatMuaNgay(hangMua);
                Response.Redirect("ThanhToan.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Gio.Them(hangMua);

            litActionMessage.Text = "<p class=\"shop-action-message\">Đã thêm "
                + Server.HtmlEncode(product.Name) + " vào giỏ hàng.</p>";
        }

        private string BuildUrl(int page)
        {
            var values = new List<string>();
            string search = txtStoreSearch.Text.Trim();
            if (search.Length > 0) values.Add("search=" + HttpUtility.UrlEncode(search));

            string brands = string.Join(",", cblBrands.Items.Cast<ListItem>()
                .Where(item => item.Selected).Select(item => item.Value));
            if (brands.Length > 0) values.Add("brand=" + HttpUtility.UrlEncode(brands));
            if (rblPriceRange.SelectedValue != "All") values.Add("price=" + HttpUtility.UrlEncode(rblPriceRange.SelectedValue));
            if (!string.IsNullOrEmpty(hfSelectedSize.Value)) values.Add("size=" + HttpUtility.UrlEncode(hfSelectedSize.Value));
            if (ddlSort.SelectedValue != "Default") values.Add("sort=" + HttpUtility.UrlEncode(ddlSort.SelectedValue));
            if (page > 1) values.Add("page=" + page);
            return "SanPham.aspx" + (values.Count == 0 ? string.Empty : "?" + string.Join("&", values));
        }

        private void BindProducts(int requestedPage)
        {
            IEnumerable<SanPhamItem> products = DuLieu.SanPhams;
            string search = txtStoreSearch.Text.Trim();
            if (search.Length > 0)
                products = products.Where(product => Contains(product.Name, search)
                    || Contains(product.Brand, search) || Contains(product.Description, search));

            string[] brands = cblBrands.Items.Cast<ListItem>()
                .Where(item => item.Selected).Select(item => item.Value).ToArray();
            if (brands.Length > 0)
                products = products.Where(product => brands.Contains(product.Brand, StringComparer.OrdinalIgnoreCase));

            switch (rblPriceRange.SelectedValue)
            {
                case "Under2M": products = products.Where(product => product.Price < 2000000m); break;
                case "2Mto4M": products = products.Where(product => product.Price >= 2000000m && product.Price <= 4000000m); break;
                case "Above4M": products = products.Where(product => product.Price > 4000000m); break;
            }

            if (!string.IsNullOrEmpty(hfSelectedSize.Value))
                products = products.Where(product => (product.Sizes ?? string.Empty).Split(',')
                    .Any(size => size.Trim() == hfSelectedSize.Value));

            switch (ddlSort.SelectedValue)
            {
                case "PriceAsc": products = products.OrderBy(product => product.Price).ThenBy(product => product.Name); break;
                case "PriceDesc": products = products.OrderByDescending(product => product.Price).ThenBy(product => product.Name); break;
                case "NameAsc": products = products.OrderBy(product => product.Name); break;
                default: products = products.OrderBy(product => product.Id); break;
            }

            List<SanPhamItem> all = products.ToList();
            int totalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)KichThuocTrang));
            int page = Math.Max(1, Math.Min(requestedPage, totalPages));
            List<SanPhamItem> current = all.Skip((page - 1) * KichThuocTrang).Take(KichThuocTrang).ToList();

            rptSanPham.DataSource = current;
            rptSanPham.DataBind();
            pnlNoResults.Visible = all.Count == 0;
            pnlPagination.Visible = all.Count > 0 && totalPages > 1;
            litKetQua.Text = all.Count + " sản phẩm";

            if (totalPages <= 1 || all.Count == 0) return;
            rptPages.DataSource = Enumerable.Range(1, totalPages).Select(number => new
            {
                Number = number,
                Url = BuildUrl(number),
                Css = number == page ? "page-link-custom active" : "page-link-custom"
            }).ToList();
            rptPages.DataBind();

            bool hasPrevious = page > 1;
            lnkPrevPage.NavigateUrl = BuildUrl(hasPrevious ? page - 1 : page);
            lnkPrevPage.CssClass = hasPrevious ? "page-link-custom" : "page-link-custom disabled";
            lnkPrevPage.Attributes["aria-disabled"] = hasPrevious ? "false" : "true";
            bool hasNext = page < totalPages;
            lnkNextPage.NavigateUrl = BuildUrl(hasNext ? page + 1 : page);
            lnkNextPage.CssClass = hasNext ? "page-link-custom" : "page-link-custom disabled";
            lnkNextPage.Attributes["aria-disabled"] = hasNext ? "false" : "true";
        }

        private static bool Contains(string value, string search)
        {
            return !string.IsNullOrEmpty(value)
                && value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
