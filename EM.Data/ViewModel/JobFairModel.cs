using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ViewModel
{
    public class JobFairModel
    {
        public IList<JobFairCityResult> CityList { get; set; }
        public IList<CollegeResult> SchoolList { get; set; }
        public IList<TB_JobFair> FairList { get; set; }
        public IList<DateTime> TimeList { get; set; }
        public string CityName { get; set; }
        public string SchoolName { get; set; }
        public string DTime { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int RecCount { get; set; }
    }
}
