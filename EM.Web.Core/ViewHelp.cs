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
using System.Web.Mvc;
using AutoMapper;

namespace EM.Web.Core
{
     public   class ViewHelp
    {
        private static readonly IUserRightRepo userRightRepo = new UserRightRepo(new DatabaseFactory());
        private static readonly ISystemProgromRepo systemProgromRepo = new SystemProgromRepo(new DatabaseFactory());
        public static List<EM_System_Program> GetAllActionByAssembly(string FilePath)
         {
             var result = new List<EM_System_Program>();
             FilePath = FilePath + "\\EM.Web.dll";
             var types = Assembly.LoadFile(FilePath).GetTypes();

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

         public static AccountVM UserInfo()
         {
             return GetAccountInfoFromCookie();
         }
         public static int GetUserId()
         {
             return GetAccountInfoFromCookie().UserId;
         }
         public static string GetCompanyIds()
         {
             return GetAccountInfoFromCookie().CompanyIds;
         }
         /// <summary>
         /// 根据角色类型,决定明细的公司查看权限:老板和员工角色类型只能看到自己的公司的明细
         /// </summary>
         /// <returns></returns>
         public static string GetDetailCompanyIds()
         {
             var companyIds = GetRoleType() == (int)RoleType.CompanyManager || GetRoleType() == (int)RoleType.Staff ? GetCompanyIds() : string.Empty;
             return companyIds;
         }
         public static bool IsAdmin()
         {

             return StaticKey.AdminRoleIds.Contains(GetRoleId());
         }

         public static int GetRoleId()
         {
             return GetAccountInfoFromCookie().UserRole;
         }
         public static int GetRoleType()
         {
             return GetAccountInfoFromCookie().RoleType;
         }
         public static string GetUserName()
         {
             return GetAccountInfoFromCookie().UserName;
         }

         public static string GetUserMoBile()
         {
             return GetAccountInfoFromCookie().Mobile;
         }

         public static string GetCateIds()
         {
             return GetAccountInfoFromCookie().CateIds;
         }

         public static bool HasRight(string ControllerName, string ActionName)
         {
             var actions = userRightRepo.GetActions(GetUserId(), ControllerName);
             if (!systemProgromRepo.IsNeedRight(ActionName, ControllerName))
                 return true;
             return actions.Contains(ActionName);
         }
         /// <summary>
         /// 获取当前登陆人的当前控制器的actions
         /// </summary>
         /// <param name="ControllerName"></param>
         /// <returns></returns>
         public static List<string> GetActions(string ControllerName)
         {
             return userRightRepo.GetActions(GetUserId(), ControllerName);
         }

         /// <summary>
         /// 获取最近几个月的枚举
         /// </summary>
         /// <returns></returns>
         public static SelectList GetRecentlyMonthList()
         {
             var MonthList = Mapper.Map<List<KeyValuePair<string, string>>, List<KeyValueVM>>(DateHepler.GetMonthListByBeforeCount());
             return  new SelectList(MonthList, "Key", "Value");
         }

         /// <summary>
         /// 绑定查看报销单的Onclick
         /// </summary>
         /// <param name="id"></param>
         /// <param name="EANumber"></param>
         /// <returns></returns>
         public static string BindEAViewClick(int id,string EANumber)
         {
             var format=@"Global.Form.NewIframe('查看报销单_{1}','browseexpenseaccount_{0}','/ExpenseAccount/Browse?id={0}')";
             format= string.Format(format, id, EANumber);
             format = "onclick=" + format;
             return format;
         }

         /// <summary>
         /// 绑定额度使用明细的Onclick
         /// </summary>
         /// <param name="id"></param>
         /// <param name="EANumber"></param>
         /// <returns></returns>
         public static string BindCompanyLimitDetailClick(int CompanyId, int CateId)
         {
             var format = @"Global.Form.NewIframe('额度使用明细','companylimitdetailreport','/report/companylimitdetail?CompanyId={0}&CateId={1}')";
             format = string.Format(format, CompanyId, CateId);
             format = "onclick=" + format;
             return format;
         }
    }
}
