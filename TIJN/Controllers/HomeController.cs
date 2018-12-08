using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TIJN.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var userId = System.Web.HttpContext.Current.Session["UserId"];
            if (userId != null)
            {
                return Redirect("~/Users/Details/" + userId);
            }
            else
            {
                return Redirect("~/Users/Login");
            }
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