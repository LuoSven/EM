using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Repositories;

namespace Topuc22Top.Data.ResultModel
{
    public class JobSimpleResult
    {
        public int PositionId { get; set; }
        public string Position { get; set; }

        public int EtpId { get; set; }

        public string EtpName { get; set; }

        public int SalaryMin { get; set; }

        public int SalaryMax { get; set; }

        public int CityId { get; set; }

        public DateTime? CreateDate { get; set; }



        public int PositionType { get; set; }

        public int? InternSalaryType { get; set; }
        public DateTime DeployTime { get; set; }
    }
}
