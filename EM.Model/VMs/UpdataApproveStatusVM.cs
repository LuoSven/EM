using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
    public class UpdataApproveStatusVM
    {
        public int Id { get; set; }

        public int ApproveStatus { get; set; }
        public string Message { get; set; }
        public string Note { get; set; }
         
    }
}
