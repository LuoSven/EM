

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
        public string ControlName { get; set; }
        public string ActionName { get; set; }

        public void Log(object entity,string Remark="")
        {
            AppLogger.Log(entity, ActionName, ControlName, Remark);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AuthorizeCore(filterContext);
        }
        //权限判断业务逻辑
        protected virtual bool AuthorizeCore(ActionExecutingContext filterContext)
        {
            string[] NormalAction={"delete","edit","add"};
            if (filterContext.HttpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            var path = filterContext.HttpContext.Request.Path.ToLower();
            if (path == "/" || path == "/Account/Login".ToLower() || path == "/Account/Logout".ToLower())
                return true;


            if (ViewHelp.GetUserId() == 0)
                filterContext.RequestContext.HttpContext.Response.Redirect("/account/login?returnUrl=" + filterContext.HttpContext.Request.CurrentExecutionFilePath);

             ControlName = filterContext.RouteData.Values["controller"].ToString().ToLower();
             ActionName = filterContext.RouteData.Values["action"].ToString().ToLower();
             ViewBag.ControlName = ControlName;
             ViewBag.ActionName = ActionName;
             if (Request.IsAjaxRequest() && !NormalAction.Contains(ActionName))
                    return true;



             if (!ViewHelp.HasRight(ControlName, ActionName) && !path.Contains("home"))
            {
                if (Request.IsAjaxRequest())
                {

                    filterContext.RequestContext.HttpContext.Response.Write("{\"code\":0,\"message\":\"无当前作业权限，请联系系统管理员!\"}");
                    filterContext.RequestContext.HttpContext.Response.ContentType = "application/Json";
                    filterContext.RequestContext.HttpContext.Response.End();
                    filterContext.Result = Json(new { code = 0, message = "无当前作业权限，请联系系统管理员!" }, JsonRequestBehavior.AllowGet);
                }
                filterContext.RequestContext.HttpContext.Response.Redirect("/error/noright");

            }
             var actionList = ViewHelp.GetActions(ControlName);
            var actionOb = "{";
            foreach (var item in actionList)
            {
                actionOb += item+":1,";
            }
            actionOb += "}";
            ViewBag.actionOb = actionOb;
            return true;
        }

        protected override void OnException(ExceptionContext filterContext)
        {

            // 标记异常已处理
            filterContext.ExceptionHandled = true;
            // 跳转到错误页
            filterContext.Result = View("Error",filterContext.Exception);
       
        }

    }
}
