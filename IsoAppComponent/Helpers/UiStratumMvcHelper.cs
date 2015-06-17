using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UiStratum.Helpers;
using UiStratum.UiStratumTypes;


namespace UiStratum.Helpers
{
    public static class MvcHelpers
    {
        public static IHtmlString Stratum<T>(this HtmlHelper helper, string componentName, object data = null, string myId = "", bool withScripts = false, string rootPath = "") where T : UiStratumType
        {
            return StratumHelpers.Stratum<T>(componentName, rootPath, myId, data, withScripts);
        }

        /// <summary>
        /// Render All Scripts for the Stratum registered so far on the page.
        /// </summary>
        /// <param name="helper"></param>
        /// <returns>Script include</returns>
        public static IHtmlString StrataScripts(this HtmlHelper helper)
        {
            return StratumHelpers.StrataScripts();
        }
        /// <summary>
        /// Render the scripts for only one component (includes it's dependencies)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="componentName"></param>
        /// <returns></returns>
        public static IHtmlString StratumScripts(this HtmlHelper helper, string componentName, object modelData = null, string myId = "", string rootPath = "")
        {
            return StratumHelpers.StratumScripts(componentName, rootPath, myId, modelData);
        }
    }
}
