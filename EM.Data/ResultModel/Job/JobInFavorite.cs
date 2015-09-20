using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class JobInFavorite
    {
        public int ID { get; set; }

        public int JobID { get; set; }
        public string JobName { get; set; }

        public int EtpID { get; set; }
        public string EtpName { get; set; }

        public DateTime CreateDate { get; set; }

        public int FavoriteStatus { get; set; }

        public string City { get; set; }

        //----Green 2014-7-28 职位是否过期字段
        public int PositionStatus { get; set; }
        public DateTime DeadLine { get; set; }
        //---------------------------------

        public int SalaryMin { get; set; }

        public int SalaryMax { get; set; }

        public string Industry { get; set; }

        public int? InternSalaryType { get; set; }
    }
}
