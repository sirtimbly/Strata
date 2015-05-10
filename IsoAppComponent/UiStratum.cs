using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using Newtonsoft.Json;
using Nustache.Core;
using UiStratum.UiStratumTypes;

namespace UiStratum
{
	

	/// <summary>
	/// Summary description for UiComponent
	/// </summary>
	public class UiStratum : IUiStratum
	{
		private UiStratumType _t = new UiStratumType();

		
		private string _componentName;
		private string _baseId;
		private string _containerId;
		private string _templateId;
		private object _modelData;
		private string _virtualBundlePath;
		private string _templateFilePath;
		private ComponentDependencies _dependencies;
		

		public string ContainerId
		{
			get
			{
				return _containerId;
			}
		}

		private List<string> _dependentScriptPaths = new List<string>();
		private List<string> _dependentTemplatePaths = new List<string>();
		private List<string> _dependentStylePaths = new List<string>();


		public string TemplatePath
		{
			get
			{
				return _templateFilePath;
			}
		}

		public List<string> DependentScriptPaths
		{
			get
			{
				return _dependentScriptPaths;
			}
		}
		public List<string> DependentTemplatePaths
		{
			get
			{
				return _dependentTemplatePaths;
			}
		}

		public readonly object TemplateCacheDelegate;



		public UiStratum(string componentName, string rootPath = "", string baseId = "", object modelData = null, UiStratumType type = null)
		{
			_componentName = componentName;

			if (type != null)
			{
				_t = type;
			}
			if (!String.IsNullOrWhiteSpace(rootPath))
			{
				_t.RootPath = rootPath;
			}

			//var page = HttpContext.Current.Request.Path;
			//if (page != null && page.StartsWith("~/"))
			//{
			//	_virtualBundlePath = "~/bundles" + page.Substring(1);
			//}
			_virtualBundlePath = "~/bundles/components/" + _componentName;

			if (String.IsNullOrWhiteSpace(baseId))
			{
				string rand = Guid.NewGuid().ToString().Substring(0, 5);
				_containerId = componentName + rand + _t.ContainerSuffix;
			}
			else
			{
				_containerId = baseId + _t.ContainerSuffix;
			}
			_templateId = componentName + _t.TemplateSuffix;

			_modelData = modelData;

			string componentFile = HttpContext.Current.Server.MapPath(_t.RootPath + _componentName + "/" + _t.DepsFile);
			_dependencies = JsonConvert.DeserializeObject<ComponentDependencies>(File.ReadAllText(componentFile));
			_templateFilePath = ConstructFilePath(_componentName, _t.TemplateExtension);

			ExtractDependencies();
			UpdateScriptsBundle(_dependentScriptPaths.ToArray());
			UpdateTemplatesCache(_dependentTemplatePaths.ToArray());

		}

		public string RenderAll()
		{
			return RenderComponentHtml(true, true, true, _modelData.GetType().ToString());
		}

		public string RenderHtml()
		{
			return RenderComponentHtml(false, false, false);
		}

		public string RenderScripts()
		{
			return RenderBundleScripts();
		}

		public string RenderTemplates()
		{
			return RenderComponentTemplate();
		}

		public string RenderStyles()
		{
			throw new NotImplementedException();
		}

		public string RenderComponentHtml(bool includeTemplates, bool includeScriptTag, bool includeViewInit, string _bbModelName = "")
		{
			StringBuilder renderedHtml = new StringBuilder();

			string path = ConstructFilePath(_componentName, _t.TemplateExtension);


			renderedHtml.AppendFormat(_t.FormatableStartContainer, _containerId);
			if (_modelData != null)
			{
				var html = Render.FileToString(path, _modelData);
				renderedHtml.Append(html);
			}
			renderedHtml.AppendLine(_t.EndTag);

			//renderedHtml.Append(RenderComponentTemplate());

			if (includeViewInit && _modelData != null && !String.IsNullOrWhiteSpace(_bbModelName))
			{
				renderedHtml.Append(RenderBackboneViewModel(_bbModelName));
			}

			if (includeTemplates)
			{
				renderedHtml.Append(RenderAllTemplates());
			}
			if (includeScriptTag)
			{
				renderedHtml.Append(RenderBundleScripts());
			}



			return renderedHtml.ToString();
		}

		public string RenderComponentTemplate(string filePath = "")
		{
			StringBuilder renderedHtml = new StringBuilder();
			if (String.IsNullOrWhiteSpace(filePath))
			{
				filePath = _templateFilePath;
			}
			renderedHtml.AppendFormat(_t.FormatableStartScript, _t.TemplateType, _templateId);
			renderedHtml.Append(File.ReadAllText(filePath));
			renderedHtml.AppendLine(_t.EndScript);
			return renderedHtml.ToString();
		}

		public string RenderBackboneViewModel(string bbModelName)
		{
			StringBuilder renderedHtml = new StringBuilder();
			string path = HttpContext.Current.Server.MapPath(_t.RootPath + _componentName + "/" + _t.InitFileName);
			object data = new
			{
				containerId = _containerId,
				templateId = _templateId,
				componentName = _componentName,
				modelName = bbModelName,
				modelData = JsonConvert.SerializeObject(_modelData)
			};

			renderedHtml.AppendFormat(_t.FormatableStartScript, _t.JsType, "");
			renderedHtml.Append(Render.FileToString(path, data));
			renderedHtml.AppendLine(_t.EndScript);

			return renderedHtml.ToString();
		}

		public string RenderBundleScripts(bool refresh = false)
		{
			if (refresh)
				UpdateScriptsBundle(_dependentScriptPaths.ToArray());

			return "<script src=\"" + HttpUtility.HtmlAttributeEncode(BundleTable.Bundles.ResolveBundleUrl(_virtualBundlePath)) + "\"></script>";

		}

		public string RenderAllTemplates(bool refresh = false, bool remote = false)
		{
			string result = "";
			if (refresh || String.IsNullOrWhiteSpace(HttpContext.Current.Cache.Get(_componentName).ToString()))
			{
				UpdateTemplatesCache(_dependentTemplatePaths.ToArray());
			}


			result = HttpContext.Current.Cache.Get(_componentName).ToString();


			if (remote)
			{
				//TODO: implement a file writer and remote html file reference
			}

			return result;
		}

		/// <summary>
		/// ExtractDependecies will traverse the dependency tree and gather all js files needed to make this component function, updating the local property
		/// </summary>
		/// <returns></returns>
		private void ExtractDependencies()
		{

			//Local script dependencies first
			List<string> flattenedScripts = _dependencies.Scripts.ToList<string>();
			List<string> flattenedTemplates = new List<string>();


			//then the .view.js file as an unresolved path
			flattenedScripts.Add(_t.RootPath + _componentName + "/" + _componentName + _t.ViewSuffix);
			flattenedTemplates.Add(_templateFilePath);

			//now the nested dependencies
			foreach (string name in _dependencies.Components)
			{
				var nestedComponent = new UiStratum(name);
				flattenedScripts.AddRange(nestedComponent.DependentScriptPaths);
				flattenedTemplates.AddRange(nestedComponent.DependentTemplatePaths);
			} 
			flattenedScripts = flattenedScripts.Distinct().ToList();
			flattenedTemplates = flattenedTemplates.Distinct().ToList();

			_dependentScriptPaths.AddRange(flattenedScripts);
			_dependentTemplatePaths.AddRange(flattenedTemplates);

		}

		private void UpdateScriptsBundle(string[] additionalPaths)
		{
			Bundle bundle = BundleTable.Bundles.GetBundleFor(_virtualBundlePath);
			if (bundle == null)
			{
				//var defaultPath = page + ".js";

				BundleTable.Bundles.Add(new ScriptBundle(_virtualBundlePath).Include(additionalPaths));
			}
			else
			{
				//TODO: find out if bundles take care of multiple instances of the same file
				BundleTable.Bundles.Remove(bundle);
				bundle = bundle.Include(additionalPaths);
				BundleTable.Bundles.Add(bundle);
			}
		}

		private void UpdateTemplatesCache(string[] filePaths)
		{
			StringBuilder allTemplates = new StringBuilder();
			foreach (string filePath in _dependentTemplatePaths)
			{
				allTemplates.Append(RenderComponentTemplate(filePath));
			}
			
			var dataObject = allTemplates.ToString();
			var absoluteExpiration = DateTime.UtcNow.Add(new TimeSpan(24, 0, 0));
			HttpContext.Current.Cache.Insert(_componentName, dataObject, null, absoluteExpiration, new TimeSpan(1,0,0));
			//_context.CustomCache.AddCache(_componentName, cacheComponent);
		}

		private string ConstructFilePath(string componentName, string extension)
		{
			return HttpContext.Current.Server.MapPath(_t.RootPath + componentName + "/" + componentName + extension);
		}
	}
	public class ComponentDependencies
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string[] Components { get; set; }
		public string[] Scripts { get; set; }
		public string[] Styles { get; set; }

	}

	
}
