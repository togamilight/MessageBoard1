using MessageBoard1.DataAccessLayer;
using MessageBoard1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard1.Controllers
{
    public class MyUserManageController : Controller
    {
        public ActionResult GetAllUserList() {
            DataAccess dataAcc = new DataAccess();
            List<MyUser> users = dataAcc.GetAllUserList();
            return RedirectToAction("ShowUserList", new { users = users, message = "所有用户" });
        }

        public ActionResult ShowUserList(List<MyUser> users, string message = "") {
            ViewData["Message"] = message;
            return View("ShowUserList", users);
        }
    }
}