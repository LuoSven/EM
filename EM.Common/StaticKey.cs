using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Common
{
     public  class StaticKey
    {

        public static  string Split {
         get{
             return SplitChar.ToString();
         }
        }
        public const char SplitChar = '‖';

         public const string CookieAccountKey = "asd";

         public static int[] AdminRoleIds
         {
             get
             {
                 int[] roleIds=  { 1 };
                 return roleIds;
             }
         }
 
    }
}
