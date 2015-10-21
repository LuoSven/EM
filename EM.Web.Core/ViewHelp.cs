using EM.Data.Repositories;
using EM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using EM.Data.Infrastructure;
using EM.Common;
using EM.Utils;
using EM.Model.VMs;
using EM.Model.Entities;
using System.Configuration;

namespace EM.Web.Core
{
     public   class ViewHelp
    {
         private static readonly IUserRightRepo userRightRepo = new UserRightRepo(new DatabaseFactory());
         public static List<EM_System_Program> GetAllActionByAssembly()
         {
             var result = new List<EM_System_Program>();

             var types = Assembly.LoadFile(ConfigurationManager.AppSettings["WebBinPath"]+ "EM.Web.dll").GetTypes();

             foreach (var type in types)
             {
                 if (type.BaseType.Name == "BaseController")//如果是Controller
                 {
                     ControlType SysType = (ControlType)type.GetCustomAttribute(typeof(ControlType));
                     var ControlDescription = "";
                     object[] Controlattrs = type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                     if (Controlattrs.Length > 0)
                         ControlDescription = (Controlattrs[0] as System.ComponentModel.DescriptionAttribute).Description;
                     if(SysType!=null)
                     {
                         var members = type.GetMethods();
                         foreach (var member in members)
                         {
                             if (member.ReturnType.Name.Contains("ActionResult") || member.ReturnType.Name.Contains("Task"))//如果是Action
                             {
                                ActionType rightType= (ActionType)member.GetCustomAttribute(typeof(ActionType));
                                 
                                 if(rightType!=null)
                                 {
                                     var ap = new EM_System_Program();
                                     ap.ParentAction = rightType.ParentAction;
                                     ap.RightType = (int)rightType.RightType;
                                     ap.SystemType = (int)SysType.SystemType;
                                     ap.ActionName = member.Name.ToLower();
                                     ap.ControllerName = member.DeclaringType.Name.Substring(0, member.DeclaringType.Name.Length - 10).ToLower(); // 去掉“Controller”后缀
                                     ap.ControllerDescription = ControlDescription;
                                     object[] attrs = member.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                                     if (attrs.Length > 0)
                                         ap.ActionDescription = (attrs[0] as System.ComponentModel.DescriptionAttribute).Description;

                                     result.Add(ap);
                                 }

                             }

                         }
                     }
                     
                 }
             }
             return result;
         }
         private static AccountVM GetAccountInfoFromCookie()
         {
             var accountCookie = CookieHelper.GetCookie(StaticKey.CookieAccountKey);
             return new AccountVM(accountCookie);

         }
         public static int GetUserId()
         {
             return GetAccountInfoFromCookie().UserId;
         }

         public static string GetUserName()
         {
             return GetAccountInfoFromCookie().UserName;
         }

         public static string GetUserMoBile()
         {
             return GetAccountInfoFromCookie().Mobile;
         }

         public static bool HasRight(string ControllerName, string ActionName)
         {
             var actions = userRightRepo.GetActions(GetUserId(), ControllerName);
             return actions.Contains(ActionName);
         }
         
         public static List<string> GetActions(string ControllerName)
         {
             return userRightRepo.GetActions(GetUserId(), ControllerName);
         }
    }
}
