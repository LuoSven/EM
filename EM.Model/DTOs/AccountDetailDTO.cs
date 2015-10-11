using EM.Common;
using EM.Utils;
using EM.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace EM.Model.DTOs
{

    public class AccountDetailDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LoginEmail { get; set; }
        public string Mobile { get; set; }
        public int Status { get; set; }
        public DateTime ModifyTime { get; set; }
        public string RoleName { get; set; }

        public DateTime LastLoginTime { get; set; }
        
    }
}
