using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard1.Controllers
{
    public class MyErrorController : Controller
    {
        public ActionResult Index()
        {
            Exception e = new Exception("无效的路径（无效的Action或Controller）");
            HandleErrorInfo eInfo = new HandleErrorInfo(e, "Unknown", "Unknown");
            return View("Error", eInfo);
        }
    }
}