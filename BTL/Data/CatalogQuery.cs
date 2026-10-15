using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ShoeStore.Models;

namespace ShoeStore.Data
{
    /// <summary>
    /// Price bucket vocabulary, shared by markup (radio values) and filtering.
    /// Add a bucket here and both sides follow.
    /// </summary>
    public static class PriceBuckets
    {
        public const string All = "All";
        public const string Under2M = "Under2M";
        public const string From2MTo4M = "2Mto4M";
        public const string Above4M = "Above4M";

        public static bool Matches(string bucket, decimal price)
        {
            switch (bucket)
            {
                case Under2M: return price < 2000000;
                case From2MTo4M: return price >= 2000000 && price <= 4000000;
                case Above4M: return price > 4000000;
                default: return true;
            }
        }
    }

    /// <summary>
    /// Facet metadata derived from data (brands) or fixed vocabulary (sizes).
    /// </summary>
    public static class ProductFacets
    {
        public static readonly string[] Sizes = { "38", "39", "40", "41", "42", "43" };

        public static List<string> Brands()
        {
            return ProductStore.GetAll()
                .Select(p => p.Brand)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(b => b)
                .ToList();
        }
    }

    /// <summary>
    /// Single filter interface: parse (query) -> apply (controls) -> build URL.
    /// Replaces the triple-tracked query/controls/ViewState contract.
    /// </summary>
    public class CatalogQuery
    {
        public string Keyword = "";
        public List<string> Brands = new List<string>();
        public string PriceRange = PriceBuckets.All;
        public string Size = "";
        public string Sort = "Default";
        public int Page = 1;

        public static CatalogQuery FromRequest(NameValueCollection query)
        {
            CatalogQuery f = new CatalogQuery();
            f.Keyword = query["search"] ?? "";
            string brands = query["brand"] ?? "";
            f.Brands = brands.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(b => b.Trim()).ToList();
            f.PriceRange = string.IsNullOrEmpty(query["price"]) ? PriceBuckets.All : query["price"];
            f.Size = query["size"] ?? "";
            f.Sort = string.IsNullOrEmpty(query["sort"]) ? "Default" : query["sort"];
            int page;
            f.Page = (int.TryParse(query["page"], out page) && page > 0) ? page : 1;
            return f;
        }

        public static CatalogQuery FromControls(TextBox keyword, CheckBoxList brands, RadioButtonList price, HiddenField size, DropDownList sort, int page)
        {
            return new CatalogQuery
            {
                Keyword = keyword.Text.Trim(),
                Brands = brands.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList(),
                PriceRange = price.SelectedValue,
                Size = size.Value,
                Sort = sort.SelectedValue,
                Page = page
            };
        }

        public void ApplyToControls(TextBox keyword, CheckBoxList brands, RadioButtonList price, HiddenField size, DropDownList sort)
        {
            keyword.Text = Keyword;
            foreach (ListItem item in brands.Items)
            {
                item.Selected = Brands.Contains(item.Value, StringComparer.OrdinalIgnoreCase);
            }
            if (price.Items.FindByValue(PriceRange) != null) price.SelectedValue = PriceRange;
            size.Value = Size;
            if (sort.Items.FindByValue(Sort) != null) sort.SelectedValue = Sort;
        }

        public string ToUrl(string page = "SanPham.aspx")
        {
            List<string> parts = new List<string>();
            if (!string.IsNullOrEmpty(Keyword)) parts.Add("search=" + HttpUtility.UrlEncode(Keyword));
            if (Brands.Count > 0) parts.Add("brand=" + HttpUtility.UrlEncode(string.Join(",", Brands)));
            if (!string.IsNullOrEmpty(PriceRange) && PriceRange != PriceBuckets.All) parts.Add("price=" + HttpUtility.UrlEncode(PriceRange));
            if (!string.IsNullOrEmpty(Size)) parts.Add("size=" + HttpUtility.UrlEncode(Size));
            if (!string.IsNullOrEmpty(Sort) && Sort != "Default") parts.Add("sort=" + HttpUtility.UrlEncode(Sort));
            if (Page > 1) parts.Add("page=" + Page);
            return page + (parts.Count > 0 ? "?" + string.Join("&", parts) : "");
        }

        public IEnumerable<ProductItem> Apply(IEnumerable<ProductItem> source)
        {
            IEnumerable<ProductItem> q = source;
            if (!string.IsNullOrEmpty(Keyword))
            {
                q = q.Where(p => p.Name.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 p.Brand.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 p.Description.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (Brands.Count > 0)
            {
                q = q.Where(p => Brands.Contains(p.Brand, StringComparer.OrdinalIgnoreCase));
            }
            q = q.Where(p => PriceBuckets.Matches(PriceRange, p.Price));
            if (!string.IsNullOrEmpty(Size))
            {
                q = q.Where(p => p.SizeList().Contains(Size));
            }
            switch (Sort)
            {
                case "PriceAsc": return q.OrderBy(p => p.Price);
                case "PriceDesc": return q.OrderByDescending(p => p.Price);
                case "NameAsc": return q.OrderBy(p => p.Name);
                default: return q;
            }
        }
    }

    /// <summary>Shared paging (SanPham today, TinTuc when it binds data).</summary>
    public class PagedResult<T>
    {
        public List<T> Items;
        public int TotalPages;
        public int Page;
    }

    public static class Paging
    {
        public static PagedResult<T> Paginate<T>(IEnumerable<T> source, int page, int pageSize)
        {
            List<T> list = source.ToList();
            int totalPages = (int)Math.Ceiling((double)list.Count / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;
            return new PagedResult<T>
            {
                Items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalPages = totalPages,
                Page = page
            };
        }
    }
}
