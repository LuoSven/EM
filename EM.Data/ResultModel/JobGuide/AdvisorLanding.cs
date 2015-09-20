using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class AdvisorLanding
    {
        public int AdvisorId { get; set; }
        public string Name { get; set; }
        public string HelloWord { get; set; }
        public int StudentCount { get; set; }
        public int Vote { get; set; }
        public string Industrystr { get; set; }
        /// <summary>
        /// 用tuple传递，item1是company的id，item2是company的名字
        /// </summary>
        public List<Tuple<int, string>> RecomCompany { get; set; }
    }

    public class AdvisorInfo
    {
        public int AdvisorId { get; set; }
        public string Name { get; set; }
        public string HelloWord { get; set; }
        public int StudentCount { get; set; }
        public int Vote { get; set; }
        public string Industrystr { get; set; }
        /// <summary>
        /// 用tuple传递，item1是company的id，item2是company的名字
        /// </summary>
        public List<Tuple<int, string>> RecomCompany { get; set; }
    }

    public class AdvisorRecomCompany
    {
        public int AdvisorId { get; set; }

        public string RecomReason { get; set; }

        public string City { get; set; }

        public string Mode { get; set; }

        public string Scale { get; set; }

        public int PosCount { get; set; }

        public List<PositonAndId> PosList { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
    }
    public class PositonAndId
    {
        public int PositionId { get; set; }

        public string Position { get; set; }
    }


}
