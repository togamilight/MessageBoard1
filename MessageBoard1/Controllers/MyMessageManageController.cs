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
    public class MyMessageManageController : Controller
    {
        public ActionResult GetUserMsgTitleList(string Username) {
            DataAccess dataAcc = new DataAccess();
            List<Message> msgs = dataAcc.GetUserMsgTitleList(Username);
            return ShowMsgTitleList(msgs, Username+"的所有留言");
        }

        public ActionResult GetAllMsgTitleList() {
            DataAccess dataAcc = new DataAccess();
            List<Message> msgs = dataAcc.GetAllMsgTitleList();
            return ShowMsgTitleList(msgs, "所有留言");
        }

        [NonAction]
        public ActionResult ShowMsgTitleList(List<Message> msgs, string message = "") {
            ViewData["Message"] = message;
            return View("ShowMsgTitleList", msgs);
        }

        public ActionResult DeleteMessage(int MsgId) {
            DataAccess dataAcc = new DataAccess();
            dataAcc.DeleteMessage(MsgId);
            return RedirectToAction("GetAllMsgTitleList");
        }

        [HttpPost]
        public ActionResult SwitchMsgState(int MsgId) {
            DataAccess dataAcc = new DataAccess();
            dataAcc.SwitchMsgState(MsgId);
            return Json(new { success = true});
        }

        public ActionResult GetMessage(int MsgId, string message = "") {
            DataAccess dataAcc = new DataAccess();
            Message msg = dataAcc.GetMessage(MsgId);
            ViewData["Message"] = message;
            return View("EditMessage", msg);
        }

        [HttpPost]
        public ActionResult SearchMsgs(string KeyWord) {
            if (KeyWord == "" || KeyWord == null) {
                return GetAllMsgTitleList();
            }
            DataAccess dataAcc = new DataAccess();
            List<Message> msgs = dataAcc.SearchMsgTitleList(KeyWord);
            string message;
            if (msgs.Count == 0) {
                message = "找不到相关留言！";
            }
            else {
                message = "搜索结果";
            }
            return ShowMsgTitleList(msgs, message);
        }

        [HttpPost]
        public ActionResult ChangeMessage(Message msg) {
            DataAccess dataAcc = new DataAccess();
            dataAcc.ChangeMessage(msg);
            return RedirectToAction("GetMessage", new { msgId = msg.Id, message = "修改成功" });
        }

        [HttpPost]
        public ActionResult SaveReply(Reply reply) {
            reply.AdminName = (string)Session["AccountName"];
            DataAccess dataAcc = new DataAccess();
            dataAcc.SaveReply(reply);
            return RedirectToAction("GetMessage", new { msgId = reply.MessageId, message = "回复成功" });
        }
    }
}