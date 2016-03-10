using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
    /// <summary>
    /// 额度使用情况明细表查询对象
    /// </summary>
    public class CompanyLimitDetailSM
    {
        public int CateId { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public int CompanyId { get; set; }

    }
}
