using System;
using System.Web;
using ShoeStore.Data;

namespace ShoeStore
{
    /// <summary>
    /// Khởi động ứng dụng: nạp sẵn trạng thái cấp Application.
    /// Trước đây danh mục và tài khoản là biến static của lớp nên không có chỗ nào
    /// nhìn thấy vòng đời của chúng; nay mọi thứ nằm trong Application và được
    /// khởi tạo đúng một lần, ngay tại đây.
    /// </summary>
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Idempotent: các kho này cũng tự nạp lazy nếu chưa có, nên gọi lại vô hại.
            ApplicationStore.Initialize(Application);
        }
    }
}
