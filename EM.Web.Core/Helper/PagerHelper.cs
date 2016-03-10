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
             for (int i = model.CurrentPage; i <= maxPager||i<=model.CurrentPage+5; i++)
             {
                 list.Add(i);
             }
             return list;
         }
    }
}
