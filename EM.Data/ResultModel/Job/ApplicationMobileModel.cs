using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class ApplicationMobileModel
    {
        //时间 名称 应聘 职位 状态
        public int ApplyId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Position { get; set; }

        public int ApplyStatus { get; set; }

        public DateTime ApplyTime { get; set; }

        public int PositionId { get; set; }
    }
}
