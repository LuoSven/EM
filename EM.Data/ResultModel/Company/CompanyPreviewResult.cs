using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class CompanyPreviewResult
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string Industry { get; set; }

        public int? Mode { get; set; }
        public int? Scale { get; set; }

        public string Tags { get; set; }
        public int QACnt { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// 一句话简介
        /// </summary>
        public string ShortDesc { get; set; }

        public string Abbr { get; set; }

        public IList<PositionShortInfo> PositionList { get; set; }
    }
}
