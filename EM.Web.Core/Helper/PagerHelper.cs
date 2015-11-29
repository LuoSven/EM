using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Web.Core
{
     public  class PagerHelper
    {
         public  static List<int> GetPageArray(PagerModel model)
         {
             var list = new List<int>();
             var maxPager = (int)Math.Ceiling((double)model.RecCount / model.PageSize);
             for (int i = 1; i <= maxPager; i++)
             {
                 list.Add(i);
             }
             return list;
         }
    }
}
