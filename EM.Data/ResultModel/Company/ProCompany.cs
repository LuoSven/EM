using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class ProCompanySecTop
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public int FollowCnts { get; set; }
        public string IndustryId { get; set; }
        public string IndustryStr { get; set; }
        public int? Mode { get; set; }
        public string ModeStr { get; set; }
        public int? Scale { get; set; }
        public string ScaleStr { get; set; }

        public int PositonCnts { get; set; }
    }

    public class ProCompanyNavDisplay
    {


        public bool HasVideo { get; set; }

        public bool HasAlbum { get; set; }

        public bool HasStaff { get; set; }

        public bool HasBusiness { get; set; }

        public bool HasQAs { get; set; }

        //----2014.8.1 Green 添加司生活
        public bool HasLife { get; set; }
    }
}
