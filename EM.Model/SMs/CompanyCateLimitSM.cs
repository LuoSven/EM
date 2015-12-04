using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
    public class CompanyCateLimitSM
    {
        public int CateId { get; set; }
        public int CompanyId { get; set; }
        public DateTime SDate { get; set; }
        public DateTime EDate { get; set; }

    }
}
