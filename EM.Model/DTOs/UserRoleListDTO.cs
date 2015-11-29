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

    public class UserRoleListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CompanyIds { get; set; }
        public int RoleType { get; set; }
        
    }
}
