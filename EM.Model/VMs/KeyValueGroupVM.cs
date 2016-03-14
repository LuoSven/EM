using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EM.Model.VMs
{
    public class KeyValueGroupVM
    {
        public KeyValueGroupVM(List<KeyValueVM> list,object value)
        {
            List = new Dictionary<string, IEnumerable<SelectListItem>>();
            foreach (var item in list)
            {
                List.Add(item.Key,new SelectList(item.Items, "Key", "Value", value));
            }
        }
        public Dictionary<string, IEnumerable<SelectListItem>> List { get; set; }
    }
}
