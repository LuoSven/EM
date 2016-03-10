using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public class ExpenseAccountExcelDTO
    {
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime OccurDate { get; set; }
        /// <summary>
        /// 报销单号
        /// </summary>
        public string EANumber { get; set; }
        /// <summary>
        /// 报销人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 费用类别
        /// </summary>
        public string CateName { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }    
    }
}
