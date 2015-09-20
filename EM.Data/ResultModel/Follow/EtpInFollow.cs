using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topuc22Top.Common;


namespace Topuc22Top.Data.ResultModel
{
    /// <summary>
    /// 学生关注者基本信息（一般是企业）
    /// </summary>
    public class EtpInFollow
    {
        public int EtpID { get; set; }

        public string EtpName { get; set; }

        public string Industry { get; set; }

        public int PosCnt { get; set; }


        public DateTime FollowDate { get; set; }
    }
}
