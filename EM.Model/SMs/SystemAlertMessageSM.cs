using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.SMs
{
    public class SystemAlertMessageSM
    {
        public int? Sender { get; set; }
        public int? Receiver { get; set; }
        public int? MessageType { get; set; }
        public string Message { get; set; }

        public int? AlertedStstus { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public int? DateTimeType { get; set; }

     
    }
}
