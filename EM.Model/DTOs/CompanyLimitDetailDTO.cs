using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
     public    class CompanyLimitDetailDTO
    {
        public int Id { get; set; }
        public int ExpenseAccountId { get; set; }
        public string EANumber { get; set; }
        public DateTime OccurDate { get; set; }
        public string Remark { get; set; }
        public string CateName { get; set; }
        public decimal Money { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public string Creater { get; set; }
        public string Modifier { get; set; }
        public string CompanyName { get; set; }
    }
}
