using System;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;

namespace ShoeStore
{
    internal sealed class CatalogFilterControls
    {
        private readonly TextBox tuKhoa; private readonly CheckBoxList thuongHieu; private readonly RadioButtonList gia; private readonly HiddenField kichThuoc; private readonly DropDownList sapXep;
        // Thực hiện việc danh mục lọc controls.
        public CatalogFilterControls(TextBox tuKhoa, CheckBoxList thuongHieu, RadioButtonList gia, HiddenField kichThuoc, DropDownList sapXep) { this.tuKhoa = tuKhoa; this.thuongHieu = thuongHieu; this.gia = gia; this.kichThuoc = kichThuoc; this.sapXep = sapXep; }
        // Thực hiện việc đọc.
        public CatalogQuery Read(int trang) { return new CatalogQuery { Keyword = tuKhoa.Text.Trim(), Brands = thuongHieu.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList(), PriceRange = gia.SelectedValue, Size = kichThuoc.Value, Sort = sapXep.SelectedValue, Page = trang }; }
        // Thực hiện việc ghi.
        public void Write(CatalogQuery truyVan)
        {
            tuKhoa.Text = truyVan.Keyword;
            foreach (ListItem phanTu in thuongHieu.Items) phanTu.Selected = truyVan.Brands.Contains(phanTu.Value, StringComparer.OrdinalIgnoreCase);
            if (gia.Items.FindByValue(truyVan.PriceRange) != null) gia.SelectedValue = truyVan.PriceRange;
            kichThuoc.Value = truyVan.Size;
            if (sapXep.Items.FindByValue(truyVan.Sort) != null) sapXep.SelectedValue = truyVan.Sort;
        }
    }

    public class ShopPage : System.Web.UI.Page
    {
        protected CartStore CartStore { get { return new CartStore(Session); } }
        protected WishlistStore DanhSachYeuThich { get { return new WishlistStore(Session); } }
        // Thực hiện việc chuyển trang.
        protected void Redirect(string duongDan) { Response.Redirect(duongDan, false); Context.ApplicationInstance.CompleteRequest(); }
        // Thực hiện việc kiểm tra yêu cầu login.
        protected bool RequireLogin(string thongBao) { if (Helpers.AuthHelper.IsLoggedIn()) return true; Helpers.Toast.RequireLogin(this, thongBao); return false; }
        // Thực hiện việc kiểm tra yêu cầu quản trị.
        protected bool RequireAdmin()
        {
            if (Helpers.AuthHelper.IsAdmin()) return true;
            if (!Helpers.AuthHelper.IsLoggedIn()) Helpers.Toast.RequireLogin(this, "Vui lòng đăng nhập bằng tài khoản quản trị để vào khu này.");
            else Helpers.Toast.ShowAndRedirect(this, "Tài khoản của bạn không có quyền vào khu quản trị.", "info", "TrangChu.aspx");
            return false;
        }
    }
}
