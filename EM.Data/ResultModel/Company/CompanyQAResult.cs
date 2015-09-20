using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ResultModel
{
    public class CompanyQAResult
    {
        public TB_Enterprise_QA QA{get;set;}
        public string EnterpriseName { get; set; }
        public int? EtpCertificateStatus { get; set; }
        public string StuName { get; set; }

    }
}
