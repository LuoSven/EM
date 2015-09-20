using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ResultModel
{
    /// <summary>
    /// 公司、职位按条件返回的结果类
    /// CustomerLevel!=99 and Status not in(31,99)
    /// </summary>
    public class CompanyPositionResult
    {
        /// <summary>
        /// 公司信息
        /// </summary>
        public TB_Enterprise Enterprise { get; set; }
        /// <summary>
        /// 职位信息
        /// </summary>
        public TB_Position_Element Position { get; set; }
    }
}
