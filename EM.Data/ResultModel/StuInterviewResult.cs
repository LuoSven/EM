using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class StuInterviewResult
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public int ApplyId { get; set; }
        public int UserId { get; set; }
        public string InterviewInvitation { get; set; }
        public System.DateTime InterviewTime { get; set; }
        public string InterviewPlace { get; set; }
        public string ContactMan { get; set; }
        public string ContactTelephone { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreateTime { get; set; }
    }
}
