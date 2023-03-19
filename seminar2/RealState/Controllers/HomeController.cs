using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealState.Controllers
{
    public class HomeController : Controller
    {

        //Home
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult States()
        {
            return View();
        }
    }
}