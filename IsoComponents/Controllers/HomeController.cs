using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IsoComponents.Models;

namespace IsoComponents.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			//just to mock up data that might come from a db
			return View(new Banner {
				Title = "My Test Banner",
				Description = "lorem ipsum blah blah blah something other and other here. this should come from a database",
				Link = "http://google.com/",
				CallToAction = "Go For The Gold!"
			});
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}