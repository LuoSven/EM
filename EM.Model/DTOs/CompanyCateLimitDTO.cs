using EM.Common;
using EM.Utils;
using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace EM.Model.DTOs
{

    public class CompanyCateLimitDTO
    {
        public decimal TotalCost { get; set; }
        public decimal ExpectTotalCost { get; set; }
        public decimal TotalLimit { get; set; }

        public decimal TotalRest
        {
            get
            {
                return TotalLimit - TotalCost;
            }
        }

        public string CateName { get;set; }
        public List<CompanyCateLimitDateDTO> DateDetails { get; set; }
        public List<CompanyCateLimitDateDTO> ExpectDateDetails { get; set; }
        
    }
}
