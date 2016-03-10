using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public class ExpenseAccountMonthCateDTO 
    {
        public decimal? SumMoney { get; set; }
        public string CateName { get; set; }
        public int ECYear { get; set; }
        public int ECMonth { get; set; }
    }
}
