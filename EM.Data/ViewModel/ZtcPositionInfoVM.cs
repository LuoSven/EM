using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    public class ZtcPositionInfoVM
    {
        public int PositionId { get; set; }
        public int CityId { get; set; }
        public string PositionName { get; set; }
        public string RecruitCount { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public DateTime DeployTime { get; set; }

        public DateTime DeadLine { get; set; }

        public int PositionType { get; set; }
        //新增,Company（Index）页面使用
        public string CityName { get; set; }
        public string SalaryRange { get; set; }
    }
}
