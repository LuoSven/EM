using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel.School
{
    public class SchoolRegionGroup
    {
        public SchoolRegionGroup()
        {
            Schools = new List<SchoolHotInfo>();
        }

        public string Region { get; set; }
        public IList<SchoolHotInfo> Schools { get; set; }

    }

    public class SchoolHotInfo
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public long HotDegree { get; set; }
        public string City { get; set; }

    }
}
