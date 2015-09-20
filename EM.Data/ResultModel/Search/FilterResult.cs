using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{

    public class FilterResult
    {
        public string key { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public string value { get; set; }
        public bool isCheck { get; set; }
    }

    public class AllFilterResult
    {
        public IList<FilterResult> Salary { get; set; }
        public IList<FilterResult> Function { get; set; }
        public IList<FilterResult> Scale { get; set; }
        public IList<FilterResult> Mode { get; set; }
        public IList<FilterResult> Industry { get; set; }
    }
}
