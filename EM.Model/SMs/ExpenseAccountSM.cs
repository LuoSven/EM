using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
     public   class ExpenseAccountSM
    {
        public string EANumber { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public int? CateId { get; set; }
        public int DateType { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public string Creater { get; set; }
        public string Modifier { get; set; }
        public string CompanyIds { get; set; }
        public int? ApproveStatus { get; set; }

    }
}
