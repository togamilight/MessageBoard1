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
    [UserFilter]
    public class MyUserCenterController : Controller
    {
        // GET: MyUserCenter
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeUserInfo(string message = "") {
            var dataAcc = new DataAccess();
            MyUser user = dataAcc.GetUserInfo((string)Session["Username"]);
            ViewData["Message"] = message;
            return View("ChangeUserInfo", user);
        }

        public ActionResult ChangeInfo(MyUser user) {
            user.Username = (string)Session["UserName"];
            var dataAcc = new DataAccess();
            dataAcc.ChangeUserInfo(user);
            return RedirectToAction("ChangeUserInfo", new { message = "修改用户信息成功！" });
        }

        public ActionResult ChangePassword(string OldPassword, string NewPassword) {
            string username = (string)Session["UserName"];
            MyUser user = new MyUser() { Username = username, Password = OldPassword };
            //确认原密码正确性
            var dataAcc = new DataAccess();
            if (dataAcc.CheckUser(user)) {
                user.Password = NewPassword;
                dataAcc.ChangePassword(user);
                return RedirectToAction("ChangeUserInfo", new { message = "修改密码成功！" });
            }
            else {
                return RedirectToAction("ChangeUserInfo", new { message = "原密码错误，修改密码失败" });
            }
        }
    }
}