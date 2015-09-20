using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ResultModel
{
    public class EtpActivity
    {
        public int EtpId { get; set; }

        public string EtpName { get; set; }

        public int ActivityType { get; set; }

        public DateTime CreateDate { get; set; }

        public TB_Position_Element Position { get; set; }
        public TB_Enterprise_Life Life { get; set; }
    }
}
