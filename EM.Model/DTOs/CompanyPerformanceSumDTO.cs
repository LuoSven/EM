using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public class CompanyPerformanceSumDTO
    {
        public DateTime EndDate { get; set; }
        public string EndDateName { get {
            return EndDate.ToShortDateString();
        } }
        public decimal FinishPerformance { get; set; }
        public decimal TotalPerformance { get; set; }
    }
}
