using System;
using System.Web;

namespace ShoeStore
{
    // Global.asax dùng để khởi tạo biến Application và đếm lượt truy cập.
    public class Global : HttpApplication
    {
        // Chạy một lần khi ứng dụng bắt đầu.
        protected void Application_Start(object nguonSuKien, EventArgs e)
        {
            Application["LuotTruyCap"] = 0;
            Application["DangOnline"] = 0;
        }

        // Chạy mỗi khi có một phiên làm việc mới.
        protected void Session_Start(object nguonSuKien, EventArgs e)
        {
            Application.Lock();
            Application["LuotTruyCap"] = (int)Application["LuotTruyCap"] + 1;
            Application["DangOnline"] = (int)Application["DangOnline"] + 1;
            Application.UnLock();
        }

        // Chạy khi phiên làm việc kết thúc.
        protected void Session_End(object nguonSuKien, EventArgs e)
        {
            Application.Lock();
            Application["DangOnline"] = (int)Application["DangOnline"] - 1;
            Application.UnLock();
        }
    }
}
