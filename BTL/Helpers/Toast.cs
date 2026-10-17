using System.Web;
using System.Web.UI;

namespace ShoeStore.Helpers
{
    /// <summary>
    /// Single toast adapter (was copy-pasted per page with two dialects).
    /// Requires the Master ScriptManager; falls back safely when absent.
    /// </summary>
    public static class Toast
    {
        private static void Register(Page page, string key, string script)
        {
            if (System.Web.UI.ScriptManager.GetCurrent(page) != null)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(page, page.GetType(), key, script, true);
            }
            else
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), key, script, true);
            }
        }

        public static void Show(Page page, string key, string message, string type)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            Register(page, key, "showToast('" + safe + "', '" + type + "');");
        }

        public static void Success(Page page, string key, string message)
        {
            Show(page, key, message, "success");
        }

        public static void Info(Page page, string key, string message)
        {
            Show(page, key, message, "info");
        }

        /// <summary>
        /// Chuyển trang bằng script thay vì Response.Redirect.
        /// Bắt buộc khi nút nằm trong UpdatePanel: async postback là XHR,
        /// Response.Redirect bị XHR đi theo nên trình duyệt không đổi trang.
        /// </summary>
        public static void Navigate(Page page, string key, string url)
        {
            string safeUrl = HttpUtility.JavaScriptStringEncode(url);
            Register(page, key, "window.location.href='" + safeUrl + "';");
        }

        public static void ShowAndRedirect(Page page, string key, string message, string type, string url)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            string safeUrl = HttpUtility.JavaScriptStringEncode(url);
            string script = "if (typeof showToast === 'function') {" +
                "showToast('" + safeMessage + "', '" + type + "');" +
                "}" +
                "setTimeout(function() {" +
                "window.location.href='" + safeUrl + "';" +
                "}, 1200);";
            Register(page, key, script);
        }

        public static void RequireLogin(Page page, string key, string message)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            Register(page, key, "showToast('" + safe + "', 'info'); openLoginModal();");
        }
    }
}
