using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class InternJobInfo
    {

        public int PositionId { get; set; }

        public DateTime DeployTime { get; set; }

        public string Position { get; set; }

        public int EnterpriseId { get; set; }

        public string EtpName { get; set; }

        public int CityId { get; set; }

        public int PositionType { get; set; }

        public int SalaryMin { get; set; }

        public int SalaryMax { get; set; }

        public string Industry { get; set; }


    }
}
