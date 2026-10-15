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
        public static void Show(Page page, string key, string message, string type)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            string script = "showToast('" + safe + "', '" + type + "');";
            if (System.Web.UI.ScriptManager.GetCurrent(page) != null)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(page, page.GetType(), key, script, true);
            }
            else
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), key, script, true);
            }
        }

        public static void Success(Page page, string key, string message)
        {
            Show(page, key, message, "success");
        }

        public static void Info(Page page, string key, string message)
        {
            Show(page, key, message, "info");
        }

        public static void RequireLogin(Page page, string key, string message)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            string script = "showToast('" + safe + "', 'info'); openLoginModal();";
            if (System.Web.UI.ScriptManager.GetCurrent(page) != null)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(page, page.GetType(), key, script, true);
            }
            else
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), key, script, true);
            }
        }
    }
}
