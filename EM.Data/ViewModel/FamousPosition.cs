using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    public class FamousPosition
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string RecruitCount { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public string Citys { get; set; }

        public string PositionName { get; set; }
        public int PositionId { get; set; }

        //public Dictionary<string, int> CityGroup { get; set; }

        public string CityGroup { get; set; }
    }
}
