using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ResultModel
{
    public class AdviceRecord
    {
        public int AdviceId { get; set; }
        public int AdvisorId { get; set; }
        public string AdvisorName { get; set; }
        public string AdviceBody { get; set; }
        public DateTime CreateTime { get; set; }
        public int Vote { get; set; }

        public IList<TB_Advice_Reply> ReplyList { get; set; }

        public string AdviceType { get; set; }

        public int UserId { get; set; }
    }
}
