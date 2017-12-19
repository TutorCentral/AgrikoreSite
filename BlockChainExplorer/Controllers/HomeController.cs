using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlockChainExplorer.Controllers
{
    /** [Authorize] **/
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Agrikore Blockchain";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }

        public ActionResult Network()
        {
            ViewBag.Message = "The MULA Network";

            return View();
        }

        public ActionResult IMESimulation()
        {
            ViewBag.Message = "Initial MULA Exchange Simulation Page";

            return View();
        }

        public ActionResult BlockchainInfo()
        {
            ViewBag.Message = "MULA Blockchain Information";

            return View();
        }

        public ActionResult IMEAdmin()
        {
            ViewBag.Message = "Initial MULA Exchange Administration";

            return View();
        }

        public ActionResult TokenExchange()
        {
            ViewBag.Message = "MULA/ETH Exchange";

            return View();
        }

        public ActionResult WhitePaper()
        {
            ViewBag.Message = "The Agrikore White Paper";

            return View();
        }

        public ActionResult Team()
        {
            ViewBag.Message = "The Agrikore Team";

            return View();
        }

        public ActionResult BitcoinBlockExplorer()
        {
            ViewBag.Message = "Bitcoin Block EXplorer";

            return View();
        }

        public ActionResult AboutBitcoinBlockExplorer()
        {
            ViewBag.Message = "About Bitcoin Block EXplorer";

            return View();
        }

    }
}