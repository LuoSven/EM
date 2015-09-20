using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    public class StuObjectiveVM
    {
        //数据字段
        public int ObjectiveId { get; set; }
        public int? ObjectiveType { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }

        public string ObjectLocationName { get; set; }
        public string ObjectLocation { get; set; }

        public string ObjectIndustryName { get; set; }
        public string ObjectIndustry { get; set; }

        public string ObjectFunctionName { get; set; }
        public string ObjectFunction { get; set; }

        public string ObjectiveSalaryName { get; set; }
        public string ObjectiveSalary { get; set; }

        public string ObjectModeNames { get; set; }
        public string ObjectMode { get; set; }

        public string ObjectScaleName { get; set; }
        public string ObjectScale { get; set; }

        public string InternShipFunctionName { get; set; }
        public string InternShipFunction { get; set; }

        public string InternShipLocationName { get; set; }
        public string InternShipLocation { get; set; }

        public string InternShipSalaryName { get; set; }
        public string InternShipSalary { get; set; }

        public string InternShipIndustryName { get; set; }
        public string InternShipIndustry { get; set; }

        public string Tags { get; set; }

        public string Keyword { get; set; }
    }
}
