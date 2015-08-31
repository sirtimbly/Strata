using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiStratum.UiStratumTypes;

namespace UiStratum
{
    interface IUiStratum
    {


        string RenderAll();
        string RenderHtml();
        string RenderScripts();
        string RenderStyles();
        string RenderTemplates();

    }
}
