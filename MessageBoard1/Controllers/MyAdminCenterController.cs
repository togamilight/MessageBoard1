using MessageBoard1.DataAccessLayer;
using MessageBoard1.Filter;
using MessageBoard1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard1.Controllers
{
    [AdminFilter]
    public class MyAdminCenterController : Controller
    {
        public ActionResult ChangeAdminInfo(string message = "") {
            ViewData["Message"] = message;
            return View("ChangeAdminInfo");
        }

        public ActionResult ChangePassword(string OldPassword, string NewPassword) {
            string adminName = (string)Session["AccountName"];
            Admin admin = new Admin() { AdminName = adminName, Password = OldPassword };
            //确认原密码正确性
            var dataAcc = new DataAccess();
            if (dataAcc.CheckAdmin(admin)) {
                admin.Password = NewPassword;
                dataAcc.ChangeAdminPassword(admin);
                return RedirectToAction("ChangeAdminInfo", new { message = "修改密码成功！" });
            }
            else {
                return RedirectToAction("ChangeAdminInfo", new { message = "原密码错误，修改密码失败" });
            }
        }

        //注册页面
        public ActionResult SignUp(string message = "") {
            ViewData["Message"] = message;
            return View("SignUp");
        }

        [HttpPost]
        public ActionResult CheckAdminName(string AdminName) {
            var dataAcc = new DataAccess();
            bool result = dataAcc.CheckAdminName(AdminName);
            return Json(new { isUnique = result });
        }

        [HttpPost]
        public ActionResult DoSignUp(Admin admin) {
            var dataAcc = new DataAccess();
            dataAcc.SaveAdmin(admin);
            return RedirectToAction("SignUp", new { message = "注册成功!" });
        }
    }
}