using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Topuc22Top.Data.ResultModel
{
   public  class EnterpriseResult
    {
       public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public string IndustryId { get; set; }
        public int  ModeId { get; set; }
        public string CityId { get; set; }
       //新增相似职位使用的
        public int? IsFamous { get; set; }
    }
}
