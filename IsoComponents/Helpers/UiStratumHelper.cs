using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UiStratum;

namespace IsoComponents.Helpers
{
	public static class UiStratumHelpers
	{
		public static string RenderComponent(this HtmlHelper helper, string name, object data = null)
		{
			UiStratum.UiStratum component = new UiStratum.UiStratum(name, "", "", data, new UiStratum.UiStratumTypes.BackboneMustacheStratumType());
			return component.RenderComponentHtml(true, true, true);
		}
	}
}