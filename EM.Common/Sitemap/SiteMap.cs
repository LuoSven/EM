using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Common
{
    public class OurSiteMap
    {
        private string _loc;
        private DateTime _lastMod;

        public OurSiteMap() { }

        public OurSiteMap(string loc, DateTime lastMod)
        {
            _loc = loc;
            _lastMod = lastMod;
        }

        public string Loc
        {
            get { return _loc; }
            set { _loc = value; }
        }

        public DateTime LastMod
        {
            get { return _lastMod; }
            set { _lastMod = value; }
        }
    }
}
