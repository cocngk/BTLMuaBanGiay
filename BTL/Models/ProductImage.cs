namespace ShoeStore.Models
{
    /// <summary>
    /// Một ảnh của sản phẩm. Chỉ giữ kích thước lớn: ảnh nhỏ từng dùng cho dải
    /// thumbnail ở trang chi tiết, dải đó đã bỏ (xem BindSecondaryImage trong
    /// ChiTietSanPham.aspx.cs) nên 22 file ảnh nhỏ đi kèm cũng thành rác.
    /// </summary>
    public class ProductImage
    {
        public string Large { get; set; }
    }
}