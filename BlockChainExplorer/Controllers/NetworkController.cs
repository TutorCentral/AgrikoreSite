using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlockChainExplorer.Controllers
{
    public class NetworkController : Controller
    {
        // GET: Network
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Network()
        {
            ViewBag.Message = "The MULA Network Live View";

            return View();
        }
    }
}
