namespace ShoeStore.Models
{
    /// <summary>
    /// Một màu của sản phẩm: tên màu + ảnh nhỏ cho thẻ chọn + ảnh lớn cho khung chính.
    ///
    /// Đặt tên "màu" chứ không phải "góc nhìn" là có chủ ý: ảnh là ảnh chụp tự
    /// nhiên, mỗi đôi chỉ có một góc duy nhất, không có bộ 3 góc như ảnh sản
    /// phẩm chụp studio. Gắn nhãn màu cho ảnh phụ thì hai ảnh gần giống nhau
    /// trông như người xem đang chọn màu, thay vì như thiếu ảnh.
    /// </summary>
    public class ProductColourway
    {
        public string Name { get; set; }
        public string Small { get; set; }
        public string Large { get; set; }
    }
}