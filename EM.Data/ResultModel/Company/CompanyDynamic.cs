using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class CompanyDynamic
    {
        public DateTime Time { get; set; }
        public string DynamicType { get; set; }
        public object DynamicItem { get; set; }

        public int EnterpriseId { get; set; }

        public string EnterpriseName { get; set; }

    }
}
