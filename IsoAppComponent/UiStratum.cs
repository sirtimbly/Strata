using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        private string _backboneModelName;


        public string ContainerId
        {
            get
            {
                return _containerId;
            }
        }

        private List<string> _dependentScriptPaths = new List<string>();
        //private List<string> _dependentTemplatePaths = new List<string>();
        private Dictionary<string, string> _dependentTemplatePaths = new Dictionary<string, string>();
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
        public List<string> DependentStylePaths
        {
            get
            {
                return _dependentStylePaths;
            }
        }
        public Dictionary<string, string> DependentTemplatePaths
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

            var page = HttpContext.Current.Request.Path;
            if (page != null && page.StartsWith("/"))
            {
                _virtualBundlePath = "~/bundles" + page.Substring(1);
            }
            //_virtualBundlePath = "~/bundles/components/" + _componentName;

            if (String.IsNullOrWhiteSpace(baseId))
            {
                string rand = Guid.NewGuid().ToString().Substring(0, 5);
                _containerId = componentName + rand + _t.ContainerSuffix;
            }
            else
            {
                _containerId = baseId;// + _t.ContainerSuffix;
            }
            _templateId = componentName + _t.TemplateSuffix;

            _modelData = modelData;

            try
            {
                string componentFile = HttpContext.Current.Server.MapPath(_t.RootPath + _componentName + "/" + _t.DepsFile);
                _dependencies = JsonConvert.DeserializeObject<ComponentDependencies>(File.ReadAllText(componentFile));
                _backboneModelName = _dependencies.BackboneModelName;
                _templateFilePath = ConstructFilePath(_componentName, _t.TemplateExtension);

            }
            catch (Exception ex)
            {

                throw new Exception(String.Format("Something went wrong finding the component '{0}'. Did you give me the right component name? \n {1}", _componentName, ex.InnerException));
            }

            if (_dependencies != null)
            {


                ExtractDependencies();
                UpdateTemplatesCache(_dependentTemplatePaths.Values.ToArray());
            }



        }

        public string RenderAll()
        {
            return RenderComponentHtml(true, true, true);
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
            return RenderAllTemplates();
            //return RenderComponentTemplate(_templateId, _templateFilePath);
        }

        public string RenderStyles()
        {
            //todo: add ability to render styles inline to save network requests
            StringBuilder sb = new StringBuilder();
            foreach (var item in _dependentStylePaths)
            {
                string mappedPath = item.Replace("~/", "/");
                sb.AppendFormat(@"<link href=""{0}"" rel=""stylesheet""/>", mappedPath);
                sb.AppendLine();
            }
            return String.Format(sb.ToString());
        }

        public string RenderComponentHtml(bool includeTemplates, bool includeScriptTag, bool includeViewInit)
        {
            StringBuilder renderedHtml = new StringBuilder();


            StringBuilder templateBuilder = new StringBuilder();

            //add the sub-templates to the top;

            if (DependentTemplatePaths.Any())
            {
                foreach (var dep in DependentTemplatePaths)
                {
                    if (dep.Key != _componentName)
                    {
                        templateBuilder.AppendFormat(_t.InternalTemplateFormat, dep.Key, File.ReadAllText(dep.Value));
                    }
                }
            }

            string path = ConstructFilePath(_componentName, _t.TemplateExtension);
            templateBuilder.Append(File.ReadAllText(path));

            string template = templateBuilder.ToString();


            renderedHtml.AppendFormat(_t.FormatableStartContainer, _containerId);
            if (_modelData != null)
            {
                //var html = Render.FileToString(path, _modelData);
                var html = Render.StringToString(template, _modelData);
                renderedHtml.Append(html);
            }
            renderedHtml.AppendLine(_t.EndTag);

            //renderedHtml.Append(RenderComponentTemplate());

            if (includeViewInit && _modelData != null)
            {
                renderedHtml.Append(RenderBackboneViewModel());
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

        public string RenderComponentTemplate(string templateId, string filePath)
        {
            StringBuilder renderedHtml = new StringBuilder();

            renderedHtml.AppendFormat(_t.FormatableStartScript, _t.TemplateType, templateId);
            var vanillaText = File.ReadAllText(filePath);
            Regex includeSubMatcher = new Regex(@"\{\{>[a-zA-Z0-9\-_]+\}\}");//this gets rid of the sub-template tags for our output js template
            renderedHtml.Append(includeSubMatcher.Replace(vanillaText, ""));
            renderedHtml.AppendLine(_t.EndScript);
            return renderedHtml.ToString();
        }

        public string RenderBackboneViewModel()
        {
            if (String.IsNullOrWhiteSpace(_backboneModelName))
            {
                string inferModel = _modelData.GetType().ToString();
                if (String.IsNullOrWhiteSpace(inferModel))
                {
                    return "";
                }
                else
                {
                    _backboneModelName = inferModel;
                }

            }

            try
            {
                string bbModelName = _backboneModelName;
                StringBuilder renderedHtml = new StringBuilder();
                string path = HttpContext.Current.Server.MapPath(_t.RootPath + _componentName + "/" + _t.InitFileName);
                object data = new
                {
                    containerId = _containerId,
                    templateId = _templateId,
                    componentName = _componentName,
                    modelName = bbModelName,
                    modelData = JsonConvert.SerializeObject(_modelData, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver() //TODO: make this configurable
                    })
                };

                renderedHtml.AppendFormat(_t.FormatableStartScript, _t.JsType, "");
                renderedHtml.Append(Render.FileToString(path, data));
                renderedHtml.AppendLine(_t.EndScript);

                return renderedHtml.ToString();
            }
            catch (Exception)
            {
                return "";
            }

        }

        public string RenderBundleScripts(bool refresh = false)
        {
            string scriptTag = "<script src=\"{0}\" type=\"text/javascript\"></script>";


            //	return "@Scripts.Render(" + _virtualBundlePath + ");";
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in _dependentScriptPaths)
                {
                    string mappedPath = item.Replace("~/", "/");
                    sb.AppendFormat(scriptTag, HttpUtility.HtmlAttributeEncode(mappedPath));
                }
                return sb.ToString();
            }

            return String.Format(scriptTag, HttpUtility.HtmlAttributeEncode(BundleTable.Bundles.ResolveBundleUrl(_virtualBundlePath)));

        }

        public string[] ListBundleScripts(bool refresh = false)
        {
            List<string> result = new List<string>();




            StringBuilder sb = new StringBuilder();
            foreach (var item in _dependentScriptPaths)
            {
                result.Add(HttpUtility.HtmlAttributeEncode(item));
            }


            return result.ToArray<string>();

        }

        public string RenderAllTemplates(bool refresh = false, bool remote = false)
        {
            string result = "";
            if (refresh)
            {
                UpdateTemplatesCache(_dependentTemplatePaths.Values.ToArray());
            }

            if (HttpContext.Current.Cache.Get(_componentName) != null)
            {
                result = HttpContext.Current.Cache.Get(_componentName).ToString();
            }


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

            Dictionary<string, string> flattenedTemplates = new Dictionary<string, string>();

            if (_dependencies.Scripts.Any())
            {
                //Local script dependencies first
                _dependentScriptPaths = _dependencies.Scripts.ToList<string>();
            }


            //TODO: determine why the ~/ path is not being resolved in debug mode, current workaround is: include view.js file in component.json file 
            //then add the [component].view.js file as an unresolved path
            //_dependentScriptPaths.Add(_t.RootPath + _componentName + "/" + _componentName + _t.ViewSuffix);
            flattenedTemplates.Add(_componentName, _templateFilePath);

            _dependentStylePaths.AddRange(_dependencies.Styles);



            if (!_dependencies.Components.Any())
            {
                return;
            }
            //now the nested dependencies
            foreach (string name in _dependencies.Components)
            {
                var nestedComponent = new UiStratum(name);
                _dependentScriptPaths.AddRange(nestedComponent.DependentScriptPaths);
                _dependentStylePaths.AddRange(nestedComponent.DependentStylePaths);
                flattenedTemplates.Add(name, nestedComponent.TemplatePath);
                foreach (var item in nestedComponent.DependentTemplatePaths)
                {
                    if (!flattenedTemplates.ContainsKey(item.Key))
                    {
                        flattenedTemplates.Add(item.Key, item.Value);
                    }
                }

            }
            _dependentScriptPaths = _dependentScriptPaths.Distinct().ToList();
            //flattenedTemplates = flattenedTemplates.Distinct().ToList();



            _dependentTemplatePaths = flattenedTemplates;

        }



        private void UpdateTemplatesCache(string[] filePaths)
        {
            if (!filePaths.Any())
            {
                return;
            }
            StringBuilder allTemplates = new StringBuilder();
            foreach (KeyValuePair<string, string> dep in _dependentTemplatePaths)
            {
                allTemplates.Append(RenderComponentTemplate(dep.Key + "_template", dep.Value));
            }

            var dataObject = allTemplates.ToString();
            var absoluteExpiration = DateTime.UtcNow.Add(new TimeSpan(24, 0, 0));
            HttpContext.Current.Cache.Insert(_componentName, dataObject, null, absoluteExpiration, TimeSpan.Zero);
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
        public string BackboneModelName { get; set; }

    }


}
