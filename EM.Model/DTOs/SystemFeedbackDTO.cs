using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
    public  class SystemFeedbackDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<System.DateTime> ReplyDate { get; set; }
        public string ReplyMessage { get; set; }
        public int Creater { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModefyDate { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
    }
}
