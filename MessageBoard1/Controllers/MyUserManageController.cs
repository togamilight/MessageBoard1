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
    public class MyUserManageController : Controller
    {
        public ActionResult GetAllUserList() {
            DataAccess dataAcc = new DataAccess();
            List<MyUser> users = dataAcc.GetAllUserList();
            return ShowUserList(users,  "所有用户");           
            //ViewData["Message"] = "所有用户";
            //return View("ShowUserList", users);
        }

        [NonAction]
        public ActionResult ShowUserList(List<MyUser> users, string message = "") {
            ViewData["Message"] = message;
            return View("ShowUserList", users);
        }


        public ActionResult DeleteUser(int id) {
            DataAccess dataAcc = new DataAccess();
            dataAcc.DeleteUser(id);
            return GetAllUserList();
        }

        [HttpPost]
        public ActionResult ChangeUserInfo(MyUser user) {
            DataAccess dataAcc = new DataAccess();
            dataAcc.ChangeUserInfoByAdmin(user);
            return GetAllUserList();
        }

        [HttpPost]
        public ActionResult SearchUsers(string KeyWord) {
            if(KeyWord == "" || KeyWord == null) {
                return GetAllUserList();
            }
            DataAccess dataAcc = new DataAccess();
            List<MyUser> users = dataAcc.SearchUserList(KeyWord);
            string message;
            if(users.Count == 0) {
                message = "找不到相关用户！";
            }else {
                message = "搜索结果";
            }
            return ShowUserList(users, message);
        }
    }
}