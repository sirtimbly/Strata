using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;
using UiStratum;

namespace IsoComponents.Helpers
{
	public static class StratumHelpers
	{
		public static MvcHtmlString Stratum(this HtmlHelper helper, string componentName, object data = null, bool withScripts = false)
		{
			UiStratum.UiStratum component = new UiStratum.UiStratum(componentName, "", "", data, new UiStratum.UiStratumTypes.BackboneMustacheStratumType());
			return MvcHtmlString.Create(component.RenderComponentHtml(true, withScripts, true));
		}

		public static MvcHtmlString StratumScripts(this HtmlHelper helper, string componentName, string bundleName = null)
		{
			UiStratum.UiStratum component = new UiStratum.UiStratum(componentName, "", "", null, new UiStratum.UiStratumTypes.BackboneMustacheStratumType());
			return new MvcHtmlString(RenderScripts(helper, bundleName, component.ListBundleScripts()).ToString());
		}

		public static IHtmlString RenderStyles(this HtmlHelper helper, string bundleName = null, params string[] additionalPaths)
		{
			var virtualPath = "~/bundles/" + bundleName;

			if (bundleName == null)
			{
				var page = helper.ViewDataContainer as WebPageExecutingBase;
				if (page != null && page.VirtualPath.StartsWith("~/"))
				{ 
					virtualPath = "~/bundles" + page.VirtualPath.Substring(1);
				}
			}
			
			if (BundleTable.Bundles.GetBundleFor(virtualPath) == null)
			{
				BundleTable.Bundles.Add(new StyleBundle(virtualPath).Include(additionalPaths));
			}

			return MvcHtmlString.Create(@"<link href=""" + HttpUtility.HtmlAttributeEncode(BundleTable.Bundles.ResolveBundleUrl(virtualPath)) + @""" rel=""stylesheet""/>");

		}

		public static IHtmlString RenderScripts(this HtmlHelper helper, string bundleName = null, params string[] additionalPaths)
		{
			var virtualPath = "~/bundles/" + bundleName;

			if (bundleName == null)
			{
				var page = helper.ViewDataContainer as WebPageExecutingBase;
				if (page != null && page.VirtualPath.StartsWith("~/"))
				{
					virtualPath = "~/bundles" + page.VirtualPath.Substring(1);
				}
			}

			if (BundleTable.Bundles.GetBundleFor(virtualPath) == null)
			{
				BundleTable.Bundles.Add(new ScriptBundle(virtualPath).Include(additionalPaths));
			}
			return MvcHtmlString.Create(@"<script src=""" + HttpUtility.HtmlAttributeEncode(BundleTable.Bundles.ResolveBundleUrl(virtualPath)) + @"""></script>");
			  
		}
	}
}