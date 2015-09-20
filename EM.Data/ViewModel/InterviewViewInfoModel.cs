using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    /// <summary>
    /// 通知面试 弹层 数据初始化
    /// </summary>
    public class InterviewViewInfoModel
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public int ApplyId { get; set; }
        public int UserId { get; set; }
        public string InterviewInvitation { get; set; }
        public System.DateTime InterviewTime { get; set; }
        public int InterviewHour { get; set; }
        public int InterivewMinute { get; set; }
        public string InterviewPlace { get; set; }
        public string ContactMan { get; set; }
        public string ContactTelephone { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string PositionName { get; set; }
        public int PositionID { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public IList<string> AddressList { get; set; }

        public int ApplyStatus { get; set;}
    }
}
