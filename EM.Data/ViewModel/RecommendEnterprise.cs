using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.ResultModel;

namespace Topuc22Top.Data.ViewModel
{
    public class RecommendEnterprise
    {

        public int EnterpriseId { get; set; }

        public string EnterpriseName { get; set; }

        public string ShortDesc { get; set; }

        public int ItemId { get; set; }

        public IList<PositionShortInfo> PositionList { get; set; }
    }
}
