using EM.Model.DTOs;
using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
    public class SystemAlertMessageVM : EM_System_AlertMessage 
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public string MessageTypeName { get; set; }
         
    }
}
