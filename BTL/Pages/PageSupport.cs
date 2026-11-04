using System;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;

namespace ShoeStore
{
    internal sealed class CatalogFilterControls
    {
        private readonly TextBox keyword; private readonly CheckBoxList brands; private readonly RadioButtonList price; private readonly HiddenField size; private readonly DropDownList sort;
        public CatalogFilterControls(TextBox keyword, CheckBoxList brands, RadioButtonList price, HiddenField size, DropDownList sort) { this.keyword = keyword; this.brands = brands; this.price = price; this.size = size; this.sort = sort; }
        public CatalogQuery Read(int page) { return new CatalogQuery { Keyword = keyword.Text.Trim(), Brands = brands.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList(), PriceRange = price.SelectedValue, Size = size.Value, Sort = sort.SelectedValue, Page = page }; }
        public void Write(CatalogQuery query)
        {
            keyword.Text = query.Keyword;
            foreach (ListItem item in brands.Items) item.Selected = query.Brands.Contains(item.Value, StringComparer.OrdinalIgnoreCase);
            if (price.Items.FindByValue(query.PriceRange) != null) price.SelectedValue = query.PriceRange;
            size.Value = query.Size;
            if (sort.Items.FindByValue(query.Sort) != null) sort.SelectedValue = query.Sort;
        }
    }

    public class ShopPage : System.Web.UI.Page
    {
        protected CartStore CartStore { get { return new CartStore(Session); } }
        protected WishlistStore Wishlist { get { return new WishlistStore(Session); } }
        protected void Redirect(string url) { Response.Redirect(url, false); Context.ApplicationInstance.CompleteRequest(); }
        protected bool RequireLogin(string message) { if (Helpers.AuthHelper.IsLoggedIn()) return true; Helpers.Toast.RequireLogin(this, message); return false; }
        protected bool RequireAdmin()
        {
            if (Helpers.AuthHelper.IsAdmin()) return true;
            if (!Helpers.AuthHelper.IsLoggedIn()) Helpers.Toast.RequireLogin(this, "Vui lòng đăng nhập bằng tài khoản quản trị để vào khu này.");
            else Helpers.Toast.ShowAndRedirect(this, "Tài khoản của bạn không có quyền vào khu quản trị.", "info", "TrangChu.aspx");
            return false;
        }
    }
}
