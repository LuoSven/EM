using EM.Model.DTOs;
using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
    public class CompanyLimitVM : CompanyLimitDTO
    {
        public string SeasonTypeName { get; set; }
    }
}
