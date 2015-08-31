using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using UiStratum;
using UiStratum.UiStratumTypes;

namespace UiStratum.Helpers
{
    public static class StratumHelpers
    {
        /// <summary>
        /// Render and register a Stratum UI component in the page
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="componentName">name of the component (should correlate the folder in /Components/)</param>
        /// <param name="data"></param>
        /// <param name="withScripts"></param>
        /// <returns></returns>
        public static IHtmlString Stratum<T>(string componentName, string rootPath = "", string myId = "", object data = null, bool withScripts = false, bool withStyles = true, bool withTemplates = true, bool withViewInit = false) where T : UiStratumType
        {

            UiStratumType newType = (T)Activator.CreateInstance(typeof(T));
            UiStratum component = new UiStratum(componentName, rootPath, myId, data, newType);
            List<string> loaded = new List<string>();
            string pageKey = "stratum:" + HttpContext.Current.Request.Path;

            if (HttpContext.Current.Application[pageKey] != null)
            {
                loaded = HttpContext.Current.Application[pageKey] as List<string>;
            }
            loaded.Add(componentName);
            HttpContext.Current.Application[pageKey] = loaded;
            string output = "";
            if (withStyles)
            {
                output += component.RenderStyles();
            }
            output += component.RenderComponentHtml(withTemplates, withScripts, withViewInit);
            return new HtmlString(output);
        }

        /// <summary>
        /// Render All Scripts for the Stratum registered so far on the page.
        /// </summary>
        /// <param name="helper"></param>
        /// <returns>Script include</returns>
        public static IHtmlString StrataScripts()
        {
            List<string> loaded = new List<string>();
            string[] allScripts = new string[0];
            string pageKey = "stratum:" + HttpContext.Current.Request.Path;

            //var page = helper.ViewDataContainer as WebPageExecutingBase;
            string virtualPath = HttpContext.Current.Request.Path;
            if (!String.IsNullOrWhiteSpace(virtualPath) && virtualPath.StartsWith("~/"))
            {
                virtualPath = virtualPath.Substring(1);
            }
            if (HttpContext.Current.Application[pageKey] != null)
            {
                loaded = HttpContext.Current.Application[pageKey] as List<string>;
                foreach (string componentName in loaded)
                {
                    UiStratum component = new UiStratum(componentName, "", "", null, null);
                    //virtualPath = ConstructScriptBundle(helper, virtualPath, component.ListBundleScripts());
                    allScripts = allScripts.Concat(component.ListBundleScripts()).ToArray();
                }
            }
            return RenderBundle<ScriptBundle>(virtualPath, allScripts) as HtmlString;

        }

        /// <summary>
        /// Renders a specific set of scripts as a bundle for one specific component
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static IHtmlString StratumScripts(string componentName, string rootPath = "", string myId = "", object modelData = null, string bundleName = null)
        {
            UiStratum component = new UiStratum(componentName, rootPath, myId, modelData, new UiStratumTypes.BackboneMustache());
            StringBuilder scripts = new StringBuilder();
            scripts.Append(RenderBundle<ScriptBundle>(bundleName, component.ListBundleScripts()).ToString());
            scripts.Append(component.RenderBackboneViewModel());
            return new HtmlString(scripts.ToString());
        }

        /// <summary>
        /// Renders a specific set of scripts as a bundle for one specific component
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static IHtmlString StratumStyles(string componentName, string rootPath = "", string bundleName = null)
        {
            UiStratum component = new UiStratum(componentName, rootPath, "", null, new UiStratumTypes.BackboneMustache());

            return new HtmlString(component.RenderStyles());
        }

        /// <summary>
        /// Renders templates
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static IHtmlString StratumTemplates(string componentName, string rootPath = "")
        {
            UiStratum component = new UiStratum(componentName, rootPath, "", null, new UiStratumTypes.BackboneMustache());

            return new HtmlString(component.RenderTemplates());
        }


        /// <summary>
        /// Create and Render a custom bundle based on the file paths you pass in.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="additionalPaths">string array of file paths</param>
        /// <returns></returns>
        public static IHtmlString RenderBundle<T>(string bundleName, params string[] additionalPaths) where T : Bundle
        {
            string formatter = @"<script src=""{0}""></script>"; //default script tag

            additionalPaths = additionalPaths.Distinct().ToArray<string>();

            if (typeof(T) == typeof(StyleBundle))
            {
                formatter = @"<link href=""{0}"" rel=""stylesheet""/>";
            }

            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                //only use bundles when app is not in Debug
                string virtualPath = ConstructBundle<T>(bundleName, additionalPaths);
                return new HtmlString(String.Format(formatter, HttpUtility.HtmlAttributeEncode(BundleTable.Bundles.ResolveBundleUrl(virtualPath))));
            }

            StringBuilder sb = new StringBuilder();
            foreach (string path in additionalPaths)
            {
                sb.AppendFormat(formatter, HttpUtility.HtmlAttributeEncode(path.Substring(1)));
            }
            return new HtmlString(sb.ToString());

        }

        private static string ConstructBundle<T>(string bundleName, string[] additionalPaths) where T : Bundle
        {
            string virtualPath = "~/bundles/" + bundleName;

            if (BundleTable.Bundles.GetBundleFor(virtualPath) == null)
            {
                //It doesn't exist yet, need to create it
                object[] args = new object[] { virtualPath };
                T newBundle = (T)Activator.CreateInstance(typeof(T), args);
                newBundle = newBundle.Add(additionalPaths) as T;
                BundleTable.Bundles.Add(newBundle);
            }
            return virtualPath;
        }
    }

}