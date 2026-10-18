namespace ShoeStore.Models
{
    /// <summary>
    /// Một góc nhìn của sản phẩm trong thư viện ảnh: ảnh nhỏ cho thumbnail,
    /// ảnh lớn cho khung chính. Trước đây dải thumbnail chỉ là 3 nét cắt của
    /// cùng một tấm ảnh nên bấm vào không thấy gì mới — mỗi góc nay là ảnh riêng.
    /// </summary>
    public class ProductImage
    {
        public string Small { get; set; }
        public string Large { get; set; }
    }
}