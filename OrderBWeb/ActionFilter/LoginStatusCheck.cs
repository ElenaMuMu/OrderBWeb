using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderBWeb.ActionFilter
{
    public class LoginStatusCheck :  ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var loginstatus = System.Web.HttpContext.Current.Session["UserId"];

            if (loginstatus == null)
            {
                filterContext.Result = new RedirectResult("~/Views/Order/Login.cshtml");
            }
        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LoginStatusCheck());
        }
    }
}