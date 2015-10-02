
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
    /// 表示Action的权限类型
    /// </summary>
     [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ActionType : System.Attribute
    {
         public ActionType(RightType ActionType, string ParentAction="")
         {
             this.ActionType = ActionType;
             this.ParentAction = ParentAction;
         }

         public string ParentAction {get;set;}
         public RightType ActionType { get; set; }
            
    }
}