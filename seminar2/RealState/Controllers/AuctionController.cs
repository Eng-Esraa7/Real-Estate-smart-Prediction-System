using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealState.Controllers
{
    public class AuctionController : Controller
    {
        // GET: Auction
        public ActionResult Auction()
        {
            return View();
        }
        public ActionResult MainAuction()
        {
            return View();
        }
        public ActionResult Item()
        {
            return View();
        }
    }
}