using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class JobPushResult
    {
        public int JobId { get; set; }
        public string JobName { get; set; }

        public int EtpId { get; set; }

        public string EtpName { get; set; }

        public DateTime DeployTime { get; set; }

        public int SchoolId { get; set; }

        public string SchoolName
        {
            get
            {
                return string.Empty;
            }
        }

        public string MajorCode { get; set; }

        public string MajorName
        {
            get
            {
                return string.Empty;
            }
        }

        public int StuCount { get; set; }

        public int PushId { get; set; }
    }
}
