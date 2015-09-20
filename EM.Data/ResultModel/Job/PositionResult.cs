using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    /// <summary>
    /// 企业职位管理使用
    /// </summary>
    public class PositionResult
    {
        public int PositionId { get; set; }
        public string Position { get; set; }
        public DateTime DeployTime { get; set; }
        public int CityId { get; set; }
        public string RecruitCount { get; set; }
        public int PositionStatus { get; set; }
        public int AppliedCount { get; set; }
        public DateTime Deadline { get; set; }
        public string FunctionIds { get; set; }
        //2014-8-20添加
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public string DegreeIds { get; set; }
        public int PositionType { get; set; }
        public DateTime? PushDate { get; set; }
        public int UnreadApplyCount { get; set; }

        public int PushCount { get; set; }

        public int? InternSalaryType { get; set; }
    }
}
