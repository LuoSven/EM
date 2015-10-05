
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Security.Principal;
using EM.Common;

namespace EM.Web.Core
{
    /// <summary>
    /// 权限拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            var path = filterContext.HttpContext.Request.Path.ToLower();
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



            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            if (!ViewHelp.HasRight(controllerName, actionName) && !path.Contains("home"))
                filterContext.RequestContext.HttpContext.Response.Redirect("/error/noright");
            return true;
        }
    }
}