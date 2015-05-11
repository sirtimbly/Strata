using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UiStratum;

namespace IsoComponents.Helpers
{
	public static class StratumHelpers
	{
		public static MvcHtmlString Stratum(this HtmlHelper helper, string name, object data = null, bool withScripts = false)
		{
			UiStratum.UiStratum component = new UiStratum.UiStratum(name, "", "", data, new UiStratum.UiStratumTypes.BackboneMustacheStratumType());
			return MvcHtmlString.Create(component.RenderComponentHtml(true, withScripts, true));
		}

		public static MvcHtmlString StratumScripts(this HtmlHelper helper, string name)
		{
			UiStratum.UiStratum component = new UiStratum.UiStratum(name, "", "", null, new UiStratum.UiStratumTypes.BackboneMustacheStratumType());
			return MvcHtmlString.Create(component.RenderBundleScripts(false));
		}
	}
}