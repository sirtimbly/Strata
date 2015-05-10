using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiStratum.UiStratumTypes
{
	public class UiStratumType {
		//TODO: all the necessary variable names
		public string TemplateFileExtension = ".mustache";
		public string RootPath = "~/Components/";
		public string TemplateExtension = ".mustache";
		public string FormatableStartContainer = "<div id=\"{0}\">";
		public string EndTag = "</div>";
		public string FormatableStartScript = "<script type=\"{0}\" id=\"{1}\">";
		public string EndScript = "</script>";
		public string InitFileName = "/initializer-template.js";
		public string JsType = "text/javascript";
		public string TemplateType = "text/template";
		public string TemplateSuffix = "_template";
		public string ContainerSuffix = "_container";
		public string ViewSuffix = ".view.js";
		public string DepsFile = "component.json";
	}

	public class BackboneMustacheStratumType : UiStratumType
	{
		

	}

	static class Templates
	{
		public static string Mustache = "mustache";
	}

	static class JSFrameworks
	{
		public static string Backbone = "backbone";
	}
}
