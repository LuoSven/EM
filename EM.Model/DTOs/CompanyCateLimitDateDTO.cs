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

    public class CompanyCateLimitDateDTO
    {
        public decimal Money { get; set; }
        public DateTime OccurDate { get; set; }

        public string OccurDateName
        {
            get
            {
                return OccurDate.ToShortDateString();

            }
        }
        
    }
}
