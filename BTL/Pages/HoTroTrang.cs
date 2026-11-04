using System;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;
using ShoeStore.Helpers;

namespace ShoeStore
{
    // Đọc và ghi các control bộ lọc của trang sản phẩm.
    internal sealed class CatalogFilterControls
    {
        private readonly TextBox oTimKiem;
        private readonly CheckBoxList oThuongHieu;
        private readonly RadioButtonList oKhoangGia;
        private readonly HiddenField oKichThuoc;
        private readonly DropDownList oSapXep;

        public CatalogFilterControls(TextBox timKiem, CheckBoxList thuongHieu,
            RadioButtonList khoangGia, HiddenField kichThuoc,
            DropDownList sapXep)
        {
            oTimKiem = timKiem;
            oThuongHieu = thuongHieu;
            oKhoangGia = khoangGia;
            oKichThuoc = kichThuoc;
            oSapXep = sapXep;
        }

        // Đọc bộ lọc hiện tại từ giao diện.
        public CatalogQuery Read(int trang)
        {
            return new CatalogQuery
            {
                Keyword = oTimKiem == null ? "" : oTimKiem.Text.Trim(),
                Brands = oThuongHieu == null
                    ? new System.Collections.Generic.List<string>()
                    : oThuongHieu.Items.Cast<ListItem>()
                        .Where(item => item.Selected)
                        .Select(item => item.Value)
                        .ToList(),
                PriceRange = oKhoangGia == null
                    ? PriceBuckets.All
                    : oKhoangGia.SelectedValue,
                Size = oKichThuoc == null ? "" : oKichThuoc.Value,
                Sort = oSapXep == null ? "Default" : oSapXep.SelectedValue,
                Page = trang
            };
        }

        // Ghi bộ lọc vào giao diện sau khi đọc query string.
        public void Write(CatalogQuery truyVan)
        {
            if (truyVan == null) return;

            if (oTimKiem != null) oTimKiem.Text = truyVan.Keyword;
            if (oThuongHieu != null)
            {
                foreach (ListItem item in oThuongHieu.Items)
                {
                    item.Selected = truyVan.Brands.Contains(
                        item.Value, StringComparer.OrdinalIgnoreCase);
                }
            }

            if (oKhoangGia != null
                && oKhoangGia.Items.FindByValue(truyVan.PriceRange) != null)
            {
                oKhoangGia.SelectedValue = truyVan.PriceRange;
            }

            if (oKichThuoc != null) oKichThuoc.Value = truyVan.Size;
            if (oSapXep != null
                && oSapXep.Items.FindByValue(truyVan.Sort) != null)
            {
                oSapXep.SelectedValue = truyVan.Sort;
            }
        }
    }

    // Lớp cơ sở dùng chung cho các trang WebForms của cửa hàng.
    public class ShopPage : System.Web.UI.Page
    {
        protected CartStore CartStore
        {
            get { return new CartStore(Session); }
        }

        protected WishlistStore DanhSachYeuThich
        {
            get { return new WishlistStore(Session); }
        }

        // Chuyển trang và kết thúc request hiện tại.
        protected void Redirect(string duongDan)
        {
            Response.Redirect(duongDan, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // Kiểm tra người dùng đã đăng nhập chưa.
        protected bool RequireLogin(string thongBao)
        {
            if (AuthHelper.IsLoggedIn()) return true;
            Toast.RequireLogin(this, thongBao);
            return false;
        }

        // Kiểm tra tài khoản có quyền quản trị không.
        protected bool RequireAdmin()
        {
            if (AuthHelper.IsAdmin()) return true;

            if (!AuthHelper.IsLoggedIn())
            {
                Toast.RequireLogin(this,
                    "Vui lòng đăng nhập bằng tài khoản quản trị để vào khu này.");
            }
            else
            {
                Toast.ShowAndRedirect(this,
                    "Tài khoản của bạn không có quyền vào khu quản trị.",
                    "info", "TrangChu.aspx");
            }

            return false;
        }
    }
}
