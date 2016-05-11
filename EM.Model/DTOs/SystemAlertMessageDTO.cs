using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public class SystemAlertMessageDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public int MessagType { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string CreateTime { get; set; }
         
    }
}
