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

    public class ExpenseAccountDetailListDTO
    {
        public int Id { get; set; }
        public string OccurDateName
        {
            get
            {
                return OccurDate.ToShortDateString();
            }
        }
        public System.DateTime OccurDate { get; set; }
        public string Remark { get; set; }
        public string CateName { get; set; }
        public decimal Money { get; set; }
        public string CompanyName { get; set; }

        public string LimitInfo { get; set; }
    }
}
