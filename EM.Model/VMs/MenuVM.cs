using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
     public class MenuVM
    {
         public string ProgramId { get; set; }
         public string Name { get; set; }

         public string Url { get; set; }

         public List<MenuVM> Items { get; set; } 
         
    }
}
