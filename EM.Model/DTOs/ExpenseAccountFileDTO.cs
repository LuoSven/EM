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

    public class ExpenseAccountFileDTO
    {
        public string EANumber { get; set; }
        public string CompanyName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Remark { get; set; }
        public string UpLoader { get; set; }
    }
}
