using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlockChainExplorer.Controllers
{
    public class IMESimulationController : Controller
    {
        // GET: IMESimulation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IMESimulation()
        {
            ViewBag.Message = "Initial MULA Exchange";

            return View();
        }
    }
}