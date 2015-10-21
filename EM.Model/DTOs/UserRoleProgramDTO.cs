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

    public class UserRoleProgramDTO
    {
        public int Id { get; set; }
        public string ActionDescription { get; set; }
        public string RightType { get; set; }
        public string ControllerDescription { get; set; }
        public int SystemType { get; set; }
        public bool PerMit { get; set; }
        
    }
}
