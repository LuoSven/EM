using EM.Data.Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace EM.Web.Core
{
     public class AppLogger
    {
         public static void Log(object enetity,string ActionName,string ControlName,string Remark="")
         {

             var Json=JsonConvert.SerializeObject(enetity);
             DapperHelper.SqlExecute(@"INSERT INTO EM_System_ActionLog
           (TableName
           ,ControlName
           ,ActionName
           ,Json
           ,UserName
           ,Remark
           ,CreateDate)
     VALUES
           (@TableName
           ,@ControlName
           ,@ActionName
           ,@Json
           ,@UserName
           ,@Remark
           ,@CreateDate)", new { TableName=enetity.GetType().Name, Json = Json, ActionName = ActionName, ControlName = ControlName, Remark = Remark, UserName = ViewHelp.GetUserName(), CreateDate = DateTime.Now });
         }
    }
}
