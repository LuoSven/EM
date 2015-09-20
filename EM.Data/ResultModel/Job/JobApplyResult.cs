using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class JobApplyResult
    {
        public int PositionId { get; set; }
        public string Position { get; set; }
        public int EnterpriserId { get; set; }
        public string Enterprise { get; set; }
        public int ApplyCount { get; set; }


        public int City { get; set; }

        public int SalaryMin { get; set; }

        public int SalaryMax { get; set; }
    }
}
