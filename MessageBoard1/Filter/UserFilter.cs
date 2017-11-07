using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MessageBoard1.Filter {
    public class UserFilter : ActionFilterAttribute{
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var accountStatus = filterContext.HttpContext.Session["AccountStatus"];
            if (accountStatus == null || (MyAccountStatus)accountStatus != MyAccountStatus.User) {
                //当账户不为User时，跳转到User登录界面
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "MyUser", action = "Login", message = "请先登录" }));
            }
            //base.OnActionExecuting(filterContext);
        }
    }
}