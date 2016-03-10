using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public class ExpenseAccountListDTO
    {
        public int Id { get; set; }
        public string EANumber { get; set; }
        /// <summary>
        /// 当前查询的公司Id
        /// </summary>
        public int? SearchCompanyId  { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public decimal Money { get; set; }
        public decimal SumMoney { get; set; }
        public string Remark { get; set; }
        public System.DateTime OccurDate { get; set; }
        public System.DateTime ApplyDate { get; set; }
        public string CateName { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string Creater { get; set; }
        public int ApproveStatus { get; set; }

        public bool? IsPublic { get; set; }
        
        /// <summary>
        /// 不计入费用
        /// </summary>
        public bool? IsNotAccount { get; set; }
    }
}
