using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMTop.Common
{
    public class OurUrl
    {
        private string _loc;
        private DateTime _lastMod;
        private ChangeFreq _changeFreq;
        private float _priority;

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

        public ChangeFreq ChangeFreq
        {
            get { return _changeFreq; }
            set { _changeFreq = value; }
        }

        public float Priority
        {
            get { return _priority; }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(
                        "priority", value, "priority 只能在0.0-1.0之间。");
                }

                _priority = value;
            }
        }

        public OurUrl() { }


        public OurUrl(
            string loc,
            DateTime lastMod,
            ChangeFreq changeFreq,
            float priority)
        {
            _loc = loc;
            _lastMod = lastMod;
            _changeFreq = changeFreq;
            _priority = priority;
        }
    }
}
