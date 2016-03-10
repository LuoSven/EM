using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Common
{
    public  class DateHepler
    {

        public static List<KeyValuePair<string, string>> GetMonthListByBeforeCount(int BeforeMonthCount=5, string DateForamt = "yyyy/MM")
        {
            var MonthList = new List<KeyValuePair<string, string>>();
            //默认选择上下5个月
            var BeginMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-BeforeMonthCount);
            for (int i = 0; i < BeforeMonthCount * 2; i++)
            {
                var Month = BeginMonth.AddMonths(i);
                MonthList.Add(new KeyValuePair<string, string>(Month.ToString(DateForamt), Month.GetMonthName()));
            }
            return MonthList;
        }
    }
}
