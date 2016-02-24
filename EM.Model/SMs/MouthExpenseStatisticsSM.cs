using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
    /// <summary>
    /// 月度费用统计查询对象
    /// </summary>
    public class MonthExpenseStatisticsSM
    {
        public int RoleType { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public string CompanyIds { get; set; }

    }
}
