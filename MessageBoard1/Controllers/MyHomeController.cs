using MessageBoard1.DataAccessLayer;
using MessageBoard1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageBoard1.Controllers
{
    public class MyHomeController : Controller
    {
        public ActionResult Index() {
            return GetAllPublicMsgTitleList();
        }

        [NonAction]
        public ActionResult GetAllPublicMsgTitleList()
        {
            DataAccess dataAcc = new DataAccess();
            List<Message> msgs = dataAcc.GetAllPublicMsgTitleList();
            return ShowMsgTitleList(msgs, "公开留言");
        }

        [NonAction]
        public ActionResult ShowMsgTitleList(List<Message> msgs, string message = "") {
            ViewData["Message"] = message;
            return View("Index", msgs);
        }

        [HttpPost]
        public ActionResult SearchMsgs(string KeyWord) {
            if (KeyWord == "" || KeyWord == null) {
                return GetAllPublicMsgTitleList();
            }
            DataAccess dataAcc = new DataAccess();
            List<Message> msgs = dataAcc.SearchPublicMsgTitleList(KeyWord);
            string message;
            if (msgs.Count == 0) {
                message = "找不到相关留言！";
            }
            else {
                message = "搜索结果";
            }
            return ShowMsgTitleList(msgs, message);
        }

        public ActionResult GetMessage(int MsgId, string message = "") {
            DataAccess dataAcc = new DataAccess();
            Message msg = dataAcc.GetMessage(MsgId);
            if (msg.IsPublic) {
                ViewData["Message"] = message;
                return View("LookMessage", msg);
            }
            return RedirectToAction("Index");
        }
    }
}