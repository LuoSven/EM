using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class PositionGroup
    {
        public PositionGroup()
        {
            PositionType = new Dictionary<int, string>();
            City = new Dictionary<int, string>();
            Func = new Dictionary<int, string>();
            Degree = new Dictionary<int, string>();
            PublishTime = new Dictionary<int, string>();

        }

        public Dictionary<int, string> PositionType;
        public Dictionary<int, string> City;
        public Dictionary<int, string> Func;
        public Dictionary<int, string> Degree;
        public Dictionary<int, string> PublishTime;

    }
}
