using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Model.VMs
{
    public class KeyValueVM
    {
        public bool Selected { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public List<KeyValueVM> Items { get; set; }
    }
}
