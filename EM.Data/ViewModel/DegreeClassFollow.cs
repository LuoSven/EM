using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    public class DegreeClassFollow
    {
        public int? DegreeID { get; set; }
        public int? ClassID { get; set; }
        public string DegreeClassName { get; set; }

        public bool IsFollowed { get; set; }
    }
}
