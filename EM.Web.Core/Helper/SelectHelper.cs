using EM.Common;
using EM.Data.Infrastructure;
using EM.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EM.Web.Core.Helper
{
     public static class SelectHelper
    {

         private static readonly IUserAccountRepo userAccountRepo = new UserAccountRepo(new DatabaseFactory());
         public static SelectList GetSelectList(SelectType selectType)
         {
             switch (selectType)
             {
                 case SelectType.User:
                     var userList = userAccountRepo.GetSelectList();
                     return new SelectList(userList, "Key", "Value");
                 default:
                     return null;
             }
         }

         public static SelectList GetUserList()
         {
             return GetSelectList(SelectType.User);
         }
    }
}
