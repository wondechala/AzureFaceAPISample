//|---------------------------------------------------------------|
//|                     REAL TIME MESSAGE BROKER                  |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                     REAL TIME MESSAGE BROKER                  |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealTimeServiceBroker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "RESTful SignalR service";

            return View();
        }
    }
}
