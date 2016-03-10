using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.DTOs
{
   public class UserLoginRecordDTO
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public  DateTime LoginTime { get; set; }
        public string LoginInfo { get; set; }
        public bool IsLogin { get; set; }
        public string ErrorInfo { get; set; }
    }
}
