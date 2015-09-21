using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Common.Json
{
    public class Original
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? pid { get; set; }
        public Original item { get; set; }
    }

    public class OriginalGet
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? pid { get; set; }
        public string item { get; set; }
    }
}
