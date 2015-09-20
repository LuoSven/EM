using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class EnterpriseAccount
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        //public bool IsLockedOut { get; set; }
        //public bool IsDenied { get; set; }
        public bool IsApproved { get; set; }

        //public int AccountStatus { get; set; }

        public int AccountId { get; set; }
        public string AccountName { get;set;}
        public string Abbr { get;set;}
    }
}
