using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    /// <summary>
    /// 公司 账户（联系方式）信息 设置
    /// </summary>
    public class EtpAccountInfoResult
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string ContactMan { get; set; }
        public string ContactAreaCode { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactExt { get; set; }

        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }

        public string AccountName { get; set; }
        public int? CertificationStatus { get; set; }
    }
}
