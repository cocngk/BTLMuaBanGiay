using System;
using System.Web;
using ShoeStore.Data;

namespace ShoeStore
{
    public class Global : HttpApplication
    {
        // Thực hiện việc ứng dụng khởi động.
        protected void Application_Start(object nguonSuKien, EventArgs e)
        {
            SqlDatabase.SeedIfEmpty();
        }
    }
}
