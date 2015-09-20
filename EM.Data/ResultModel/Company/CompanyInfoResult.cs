using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class CompanyInfoResult
    {
        public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }

        public string Industry { get; set; }

        public int? Scale { get; set; }

        public int? Mode { get; set; }

        public string City { get; set; }

        public string WebSite { get; set; }
        public string DescText { get; set; }

        //联系人
        public int? ContactId { get; set; }
        public string ContactMan { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }

        //地址
        public string Address { get; set; }
        public string PostCode { get; set; }


        //联系人电话
        public string ContactAreaCode { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactExt { get; set; }

        public string ContactFullTelephone
        {
            get
            {
                return (string.IsNullOrEmpty(ContactAreaCode) ? "" : ContactAreaCode + "-") + 
                    ContactTelephone + (string.IsNullOrEmpty(ContactExt) ? "" : "-" + ContactExt);
            }
        }

        public int? ProcessStatus { get; set; }

        public string Tags { get; set; }
        public int? CertificationStatus { get; set; }
    }
}
