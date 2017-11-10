using System.Web;
using System.Web.Mvc;

namespace MessageBoard1 {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());    //绑定异常过滤器
        }
    }
}
