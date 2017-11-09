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
            MyUser user = dataAcc.GetUserInfo((string)Session["AccountName"]);
            ViewData["Message"] = message;
            return View("ChangeUserInfo", user);
        }

        public ActionResult ChangeInfo(MyUser user) {
            user.Username = (string)Session["AccountName"];
            var dataAcc = new DataAccess();
            dataAcc.ChangeUserInfo(user);
            return RedirectToAction("ChangeUserInfo", new { message = "修改用户信息成功！" });
        }

        public ActionResult ChangePassword(string OldPassword, string NewPassword) {
            string username = (string)Session["AccountName"];
            MyUser user = new MyUser() { Username = username, Password = OldPassword };
            //确认原密码正确性
            var dataAcc = new DataAccess();
            if (dataAcc.CheckUser(user)) {
                user.Password = NewPassword;
                dataAcc.ChangeUserPassword(user);
                return RedirectToAction("ChangeUserInfo", new { message = "修改密码成功！" });
            }
            else {
                return RedirectToAction("ChangeUserInfo", new { message = "原密码错误，修改密码失败" });
            }
        }

        public ActionResult WriteMessage() {
            return View("WriteMessage");
        }

        public ActionResult DoWriteMessage(Message msg) {
            msg.Username = (string)Session["AccountName"];
            DataAccess dataAcc = new DataAccess();
            dataAcc.SaveMessage(msg);
            return RedirectToAction("GetHistoryMessage");
        }

        public ActionResult GetHistoryMessage() {
            string username = (string)Session["AccountName"];
            DataAccess dataAcc = new DataAccess();
            List<Message> msgs = dataAcc.GetUserMsgTitleList(username);
            return View("GetHistoryMessage", msgs);
        }

        public ActionResult GetMessage(int MsgId) {
            DataAccess dataAcc = new DataAccess();
            Message msg = dataAcc.GetMessage(MsgId);
            if (msg.IsPublic) {
                return View("LookMessage");
            }
            return View("EditMessage");
        }

        public ActionResult DeleteMessage(int MsgId) {
            DataAccess dataAcc = new DataAccess();
            dataAcc.DeleteMessage(MsgId);
            return RedirectToAction("GetHistoryMessage");
        }
    }
}