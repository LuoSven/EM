using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public class CompanyLimitDTO
    {

        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CateName { get; set; }
        public int SeasonType { get; set; }
        public decimal LimitSum { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string Modifier { get; set; }
    }
}
