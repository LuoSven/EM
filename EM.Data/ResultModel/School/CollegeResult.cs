using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class CollegeResult
    {
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }

        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolPinyin { get; set; }

        public string Details { get; set; }
        public int StuCount { get; set; }
        public int EtpCount { get; set; }

        public int CollegeStatus { get; set; }

        public string SEOKeywords { get; set; }
        public string SEODescriptions { get; set; }

        public int SystemPushJob { get; set; }
        public int corpPushJob { get; set; }
    }
}
