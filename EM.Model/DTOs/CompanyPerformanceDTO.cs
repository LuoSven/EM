using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public class CompanyPerformanceDTO
    {

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public decimal SalesPerformance { get; set; }
        public System.DateTime UploadDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string Creater { get; set; }
        public string Modifier { get; set; }

        public string CompanyName { get; set; }
    }
}
