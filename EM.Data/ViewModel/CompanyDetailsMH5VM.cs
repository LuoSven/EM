using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ViewModel
{
    /// <summary>
    /// 微主页VM
    /// </summary>
    public class CompanyDetailsMH5VM
    {
        public TB_Enterprise Basic { get; set; }
        //----------2014-7-16 公司亮点
        public IList<string> EnterpriseTags { get; set; }
        //员工成长案例
        public IList<TB_Enterprise_StaffGrowth> Staffs { get; set; }
        //在招职位
        public IList<ZtcPositionInfoVM> Positions { get; set; }
        //public IList<TB_Position_Element> Positions { get; set; }
        //小步爱打听
        public string ConsultantComment { get;set; }

        public string IndustryName { get; set; }

        public string CityName { get; set; }

        public IList<Photo> Photos { get; set; }

        public string MusicLink { get; set; }

        public int? BGId { get; set; }

    }
}
