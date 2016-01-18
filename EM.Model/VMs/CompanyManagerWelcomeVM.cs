using EM.Common;
using EM.Utils;
using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using EM.Model.DTOs;

namespace EM.Model.VMs
{

    public class CompanyManagerWelcomeVM 
    {
        public List<CompanyCateLimitDTO> CompanyCateLimits { get; set; }

        public CompanyPerformanceSumDTO Performance { get; set; }
    }
}
