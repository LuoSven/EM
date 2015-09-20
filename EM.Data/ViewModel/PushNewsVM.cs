using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;

namespace Topuc22Top.Data.ViewModel
{
    public class PushNewsVM
    {

        //职位用的属性
        public int PositionId { get; set; }
        public int EnterpriseId { get; set; }
        public string Position { get; set; }
        public int PositionType { get; set; }
        public int CityId { get; set; }
        public DateTime NewsTime { get; set; }
        public string EtpName { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }

        //标识来源
        public string positionFrom { get; set; }
        //标识类型
        public PushNewsType NewsType { get; set; }

        //职位以外的对象id
        public int TargetId { get; set; }
        //文章用的图片
        public string Pic { get; set; }

        //同专业人数
        public int Count { get; set; }

        //BBS和文章的标题、摘要
        public string Title { get; set; }
        public string Summary { get; set; }

    }
}
