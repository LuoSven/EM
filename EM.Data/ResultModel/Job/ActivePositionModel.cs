using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Repositories;

namespace Topuc22Top.Data.ResultModel
{
    public class ActivePositionModel
    {

        public int PositionId { get; set; }

        public int EnterpriseId { get; set; }

        public string Position { get; set; }

        public int SalaryMin { get; set; }

        public int SalaryMax { get; set; }

        public DateTime DeployTime { get; set; }

        public int PositionType { get; set; }

        public int CityId { get; set; }

        public string RecruitCount { get; set; }

        public int ApplyCnt { get; set; }

        public string DegreeIds { get; set; }




        public int? InternSalaryType { get; set; }
    }
}
