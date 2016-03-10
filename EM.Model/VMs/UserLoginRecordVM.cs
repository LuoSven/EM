using EM.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
    public class UserLoginRecordVM : UserLoginRecordDTO
    {
        public string LoginEmail { get; set; }
        public string Password { get; set; }
        public string LoginIp { get; set; }
        public string LoginSystem { get; set; }
        public string LoginBrower { get; set; }
    }
}
