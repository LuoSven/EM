using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class PositionDynamicInfo
    {
        public int PositionId { get; set; }
        public string Position { get; set; }
        public int EnterpriseId { get; set; }
        public int PositionType { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public int CityId { get; set; }
        public int DegreeId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
