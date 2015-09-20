using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ResultModel
{
    public class LastAdviceTime
    {

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public TB_Advice_Record LastResumeAdvice { get; set; }

        public TB_Advice_Record LastApplyAdvice { get; set; }

        public TB_Advice_Record LastInterviewAdvice { get; set; }
    }
}
