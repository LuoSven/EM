using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
    public class CompanyLimitSM
    {
        public int CateId { get; set; }
        public int SeasonType { get; set; }
        public int CompanyId { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }

    }
}
