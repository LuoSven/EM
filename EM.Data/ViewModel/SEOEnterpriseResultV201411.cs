using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    public class SEOEnterpriseResultV201411
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public List<Tuple<int, string>> Positions { get; set; }
        public string UpdateDate { get; set; }

    }
}
