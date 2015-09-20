using EM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace EM.Web.Core
{
     public   class ViewHelp
    {
         public static List<ActionPermission> GetAllActionByAssembly()
         {
             var result = new List<ActionPermission>();

             var types = Assembly.Load("EM.Web.Controllers").GetTypes();

             foreach (var type in types)
             {
                 if (type.BaseType.Name == "BaseController")//如果是Controller
                 {
                     var members = type.GetMethods();
                     foreach (var member in members)
                     {
                         if (member.ReturnType.Name == "ActionResult")//如果是Action
                         {

                             var ap = new ActionPermission();

                             ap.ActionName = member.Name;
                             ap.ControllerName = member.DeclaringType.Name.Substring(0, member.DeclaringType.Name.Length - 10); // 去掉“Controller”后缀

                             object[] attrs = member.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                             if (attrs.Length > 0)
                                 ap.Description = (attrs[0] as System.ComponentModel.DescriptionAttribute).Description;

                             result.Add(ap);
                         }

                     }
                 }
             }
             return result;
         }
    }
}
