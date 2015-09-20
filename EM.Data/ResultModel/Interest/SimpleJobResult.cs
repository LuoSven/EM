using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Topuc22Top.Data.ResultModel
{
    public class SimpleJobResult
    {
        public int PositionId { get; set; }
        public int EnterpriseId { get; set; }
        public string Position { get; set; }
        public int PositionType { get; set; }
        public int CityId { get; set; }
        public DateTime DeployTime { get; set; }
        public string EtpName { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public string Tags { get; set; }
        public string positionFrom { get; set; }

        public DateTime LastApplyTime { get; set; }
        public string Function { get; set; }
        public string Industry { get; set; }
    }
}
