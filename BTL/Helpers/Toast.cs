using System;
using System.Web;
using System.Web.UI;

namespace ShoeStore.Helpers
{
    /// <summary>
    /// Cửa duy nhất để hiện toast. Cần ScriptManager của Master; thiếu thì tự lùi
    /// về cách đăng ký script cổ điển để trang không vỡ.
    /// </summary>
    public static class Toast
    {
        // Nhận Control chứ không phải Page: MasterPage không phải Page, nhưng cả hai
        // đều đi qua ScriptManager của Master nên cần một đầu vào chung.
        private static void Register(Control control, string key, string script)
        {
            if (ResolveScriptManager(control) != null)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(control, control.GetType(), key, script, true);
            }
            else if (control is Page)
            {
                ((Page)control).ClientScript.RegisterStartupScript(control.GetType(), key, script, true);
            }
        }

        /// <summary>
        /// ScriptManager nằm trong Site.Master chứ không nằm trên từng trang.
        /// ScriptManager.GetCurrent chỉ nhận Page, nên từ MasterPage phải xác định
        /// trang con trước; nếu không tìm thấy thì quét form tìm ScriptManager.
        /// </summary>
        private static System.Web.UI.ScriptManager ResolveScriptManager(Control control)
        {
            // Control.Page leo lên Control cha để trả về trang chứa nó: gọi từ
            // MasterPage sẽ ra đúng trang con, gọi từ Page thì ra chính nó.
            Page page = control as Page;
            if (page == null && control != null)
            {
                page = control.Page;
            }
            if (page != null)
            {
                System.Web.UI.ScriptManager manager = System.Web.UI.ScriptManager.GetCurrent(page);
                if (manager != null)
                {
                    return manager;
                }
            }

            Control form = page != null ? page.FindControl("form1") : null;
            if (form != null)
            {
                foreach (Control child in form.Controls)
                {
                    System.Web.UI.ScriptManager manager = child as System.Web.UI.ScriptManager;
                    if (manager != null)
                    {
                        return manager;
                    }
                }
            }

            return null;
        }

        /// <summary>Key chỉ là chi tiết của RegisterStartupScript — caller không cần biết.</summary>
        private static string NewKey()
        {
            return "toast_" + Guid.NewGuid().ToString("N");
        }

        public static void Show(Control page, string message, string type)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            Register(page, NewKey(), "showToast('" + safe + "', '" + type + "');");
        }

        public static void Success(Control page, string message)
        {
            Show(page, message, "success");
        }

        public static void Info(Control page, string message)
        {
            Show(page, message, "info");
        }

        /// <summary>
        /// Chuyển trang bằng script thay vì Response.Redirect.
        /// Bắt buộc khi nút nằm trong UpdatePanel: async postback là XHR,
        /// Response.Redirect bị XHR đi theo nên trình duyệt không đổi trang.
        /// </summary>
        public static void Navigate(Control page, string url)
        {
            string safeUrl = HttpUtility.JavaScriptStringEncode(url);
            Register(page, NewKey(), "window.location.href='" + safeUrl + "';");
        }

        public static void ShowAndRedirect(Control page, string message, string type, string url)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            string safeUrl = HttpUtility.JavaScriptStringEncode(url);
            string script = "if (typeof showToast === 'function') {" +
                "showToast('" + safeMessage + "', '" + type + "');" +
                "}" +
                "setTimeout(function() {" +
                "window.location.href='" + safeUrl + "';" +
                "}, 1200);";
            Register(page, NewKey(), script);
        }

        public static void RequireLogin(Control page, string message)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            Register(page, NewKey(), "showToast('" + safe + "', 'info'); openLoginModal();");
        }
    }
}
