

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EM.Web.Core.Base
{
    [AuthorizeFilterAttribute]
    public class BaseController:Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AuthorizeCore(filterContext);
        }
        //权限判断业务逻辑
        protected virtual bool AuthorizeCore(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            var path = filterContext.HttpContext.Request.Path.ToLower();
            if (path == "/" || path == "/Account/Login".ToLower() || path == "/Account/Logout".ToLower())
                return true;

            if (ViewHelp.GetUserId() == 0)
                filterContext.RequestContext.HttpContext.Response.Redirect("/account/login?returnUrl=" + filterContext.HttpContext.Request.CurrentExecutionFilePath);



            var controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
            var actionName = filterContext.RouteData.Values["action"].ToString().ToLower();
            if (!ViewHelp.HasRight(controllerName, actionName) && !path.Contains("home"))
                filterContext.RequestContext.HttpContext.Response.Redirect("/error/noright");
            var actionList = ViewHelp.GetActions(controllerName);
            var actionOb = "{";
            foreach (var item in actionList)
            {
                actionOb += item+":1,";
            }
            actionOb += "}";
            ViewBag.actionOb = actionOb;
            return true;
        }

    }
}
