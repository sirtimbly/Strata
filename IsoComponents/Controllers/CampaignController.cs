using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsoComponents.Controllers
{
    public class CampaignController : Controller
    {
        // GET: Campaign
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
    }
}