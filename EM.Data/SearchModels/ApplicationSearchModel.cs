using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;

namespace Topuc22Top.Data.SearchModels
{
    public class ApplicationSearchModel
    {
        public int? currentstatus { get; set; }
        public string degree { get; set; }
        public string posId { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string school { get; set; }
        public string dtime { get; set; }
        public string keyword { get; set; }
        public string major { get; set; }
        //暂时禁用智能过滤选项
        public string IsFilterOut { get; set; }

        //2015-05-06 add 推荐简历的状态筛选
        public int? StuPushStatus { get; set; }

        //20150615收藏的职位
        public string IsFavorites { get; set; }
        /// <summary>
        /// 回收箱
        /// </summary>
        public string IsRecycleBin { get; set; }

        /// <summary>
        /// 推荐简历
        /// </summary>
        public string IsPush { get; set; }


        //20150730add用于绑定高级筛选已选择的值
        public string cityName { get; set; }
        public string degreeName { get; set; }
        public string dtimeName { get; set; }

        public bool HasValue {
            get {
                return !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(degree) || !string.IsNullOrEmpty(school) || !string.IsNullOrEmpty(major) || !string.IsNullOrEmpty(dtime) || !string.IsNullOrEmpty(keyword);
            }
        }

    }
}
