using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
  public   class CompanyCateLimitVM
    {

        public int CompanyId { get; set; }
        public string  CompanyName { get; set; }
        public List<CompanyCateLimitDTO> CompanyCateLimits { get; set; }
    }
}
