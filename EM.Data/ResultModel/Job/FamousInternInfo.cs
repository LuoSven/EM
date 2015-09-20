using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class FamousInternInfo
    {
        public int EnterpriseId { get; set; }

        public string EtpName { get; set; }

        public List<FamousInternJobRow> InternJobs { get; set; }


        public int? IsFamous { get; set; }
    }
}
