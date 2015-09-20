using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class JobApplyInfo
    {
        public string StuName { get; set; }
        public DateTime ApplyDate { get; set; }
        public string EnterpriseName { get; set; }
        public string PositionName { get; set; }
        public string Email { get; set; }
        public string RegisterEmail { get; set; }

        public int ApplyId { get; set; }
    }
}
