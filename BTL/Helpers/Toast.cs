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
