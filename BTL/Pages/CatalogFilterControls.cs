using System;
using System.Linq;
using System.Web.UI.WebControls;
using ShoeStore.Data;

namespace ShoeStore
{
    /// <summary>
    /// Page adapter của bộ lọc: nơi duy nhất biết trang Sản phẩm dùng control nào.
    /// CatalogQuery ở Data không còn import WebForms, nên luật lọc test được
    /// mà không cần dựng Page.
    /// </summary>
    internal sealed class CatalogFilterControls
    {
        private readonly TextBox keyword;
        private readonly CheckBoxList brands;
        private readonly RadioButtonList price;
        private readonly HiddenField size;
        private readonly DropDownList sort;

        public CatalogFilterControls(TextBox keyword, CheckBoxList brands, RadioButtonList price,
            HiddenField size, DropDownList sort)
        {
            this.keyword = keyword;
            this.brands = brands;
            this.price = price;
            this.size = size;
            this.sort = sort;
        }

        /// <summary>Đọc control thành query. Page chỉ truyền số trang, không truyền state.</summary>
        public CatalogQuery Read(int page)
        {
            return new CatalogQuery
            {
                Keyword = keyword.Text.Trim(),
                Brands = brands.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value)
                    .ToList(),
                PriceRange = price.SelectedValue,
                Size = size.Value,
                Sort = sort.SelectedValue,
                Page = page
            };
        }

        /// <summary>Ghi query xuống control khi mở trang bằng URL.</summary>
        public void Write(CatalogQuery query)
        {
            keyword.Text = query.Keyword;
            foreach (ListItem item in brands.Items)
            {
                item.Selected = query.Brands.Contains(item.Value, StringComparer.OrdinalIgnoreCase);
            }
            if (price.Items.FindByValue(query.PriceRange) != null)
            {
                price.SelectedValue = query.PriceRange;
            }
            size.Value = query.Size;
            if (sort.Items.FindByValue(query.Sort) != null)
            {
                sort.SelectedValue = query.Sort;
            }
        }
    }
}
