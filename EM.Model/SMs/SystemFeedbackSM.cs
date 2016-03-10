using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
    public class SystemFeedbackSM
    {
        public int? UserId { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public int? IsRepert { get; set; }
    }
}
