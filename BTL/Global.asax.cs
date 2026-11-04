using System;
using System.Web;
using ShoeStore.Data;

namespace ShoeStore
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ApplicationStore.Initialize(Application);
        }
    }
}
