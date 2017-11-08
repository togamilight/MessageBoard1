using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MessageBoard1.Filter {
    public class AdminFilter : ActionFilterAttribute{
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var accountStatus = filterContext.HttpContext.Session["AccountStatus"];
            if (accountStatus == null || (MyAccountStatus)accountStatus != MyAccountStatus.Admin) {
                //当账户不为Admin时，跳转到Admin登录界面
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "MyAdmin", action = "Login", message = "请先登录" }));
            }
            //base.OnActionExecuting(filterContext);
        }
    }
}