using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class SchoolResult
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }       
        public string City { get; set; }
        public string Majors { get; set; }
        public int CityId { get; set; }
        public int SchoolLevel { get; set; }
    }
}
