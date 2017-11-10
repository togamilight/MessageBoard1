using MessageBoard1.DataAccessLayer;
using MessageBoard1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard1.Controllers
{
    public class MyUserController : Controller
    {
        public ActionResult GetUserHeader() {
            MyUser user = new MyUser();
            if(Session["AccountStatus"] != null && (MyAccountStatus)Session["AccountStatus"] == MyAccountStatus.User) {
                DataAccess dataAcc = new DataAccess();
                string userName = (string)Session["AccountName"];
                user = dataAcc.GetUserInfo(userName);
            }

            return PartialView("UserHeader", user);
        }
        //注册页面
        public ActionResult SignUp() {
            //已经登录时不能注册
            if(Session["AccountStatus"] != null && (MyAccountStatus)Session["AccountStatus"] == MyAccountStatus.User) {
                return RedirectToAction("Index", "MyHome");
            }
            return View("SignUp");
        }
        
        [HttpPost]
        public ActionResult CheckUsername(string Username) {
            var dataAcc = new DataAccess();
            bool result = dataAcc.CheckUsername(Username);
            return Json(new { isUnique = result });
        }

        [HttpPost]
        public ActionResult DoSignUp(MyUser user) {
            var dataAcc = new DataAccess();
            dataAcc.SaveUser(user);
            return RedirectToAction("Login", new { message = "注册成功，请登录"});
        }

        public ActionResult Login(string message = "") {
            //已经登录时不能登录
            if (Session["AccountStatus"] != null && (MyAccountStatus)Session["AccountStatus"] == MyAccountStatus.User) {
                return RedirectToAction("Index", "MyHome");
            }
            ViewData["Message"] = message;  //设置提示信息
            return View("Login");
        }

        [HttpPost]
        public ActionResult DoLogin(MyUser user) {
            var dataAcc = new DataAccess();
            bool result = dataAcc.CheckUser(user);
            if (!result) {
                return RedirectToAction("Login", new { message = "用户名或密码错误，请重新登录！" });
            }else {
                //登录成功，信息写进Session
                Session["AccountStatus"] = MyAccountStatus.User;
                Session["AccountName"] = user.Username;
                return RedirectToAction("Index", "MyHome");
            }
        }

        public ActionResult Logout() {
            //注销，修改Session中的信息
            Session["AccountStatus"] = MyAccountStatus.None;
            Session["AccountName"] = "";

            return RedirectToAction("Login", new { message = "注销成功" });
        }
    }
}