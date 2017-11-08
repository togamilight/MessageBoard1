using MessageBoard1.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard1.Controllers
{
    [AdminFilter]
    public class MyBackController : Controller
    {
        // GET: MyBack
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}