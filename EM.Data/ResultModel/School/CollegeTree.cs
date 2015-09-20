using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class CollegeTree
    {
        public CollegeTree()
        {
            CollegeMajors = new List<CollegeTreeMajor>();
        }

        public int CollegeId { get; set; }

        public string CollegeName { get; set; }

        public IList<CollegeTreeMajor> CollegeMajors { get; set; }

    }

    public class CollegeTreeMajor
    {
        public int MajorId { get; set; }

        public string MajorName { get; set; }
    }
}
