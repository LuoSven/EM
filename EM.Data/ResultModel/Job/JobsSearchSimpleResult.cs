using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.Repositories;
using Topuc22Top.Utils;

namespace Topuc22Top.Data.ResultModel
{

    public class JobsSearchSimpleResult
    {
        public JobsSearchSimpleResult() 
        {
            FacetFields_SalaryRange = new List<Tuple<string, int, string>>() { };
            FacetFields_FunctionIds = new List<Tuple<string, int, string>>() { };
            FacetFields_EtpScale = new List<Tuple<string, int, string>>() { };
            FacetFields_EtpMode = new List<Tuple<string, int, string>>() { };
            FacetFields_IndustryIds = new List<Tuple<string, int, string>>() { };
        }

        /// <summary>
        /// 符合要求的职位总数
        /// </summary>
        public int MatchesCount { get; set; }
        /// <summary>
        /// 分页职位列表
        /// </summary>
        public IList<JobsSearchSimpleResultModel> JobList { get; set; }


        public IList<Tuple<string, int, string>> FacetFields_SalaryRange { get; set; }
        public IList<Tuple<string, int, string>> FacetFields_FunctionIds { get; set; }
        public IList<Tuple<string, int, string>> FacetFields_EtpScale { get; set; }
        public IList<Tuple<string, int, string>> FacetFields_EtpMode { get; set; }
        public IList<Tuple<string, int, string>> FacetFields_IndustryIds { get; set; }
    }

    /// <summary>
    /// search/jobs api返回结果
    /// </summary>
    public class JobsSearchSimpleResultModel
    {
        IDictItemRepo dictItemRepo = new DictItemRepo(new DatabaseFactory(), new InMemoryCache());

        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public int PositionId { get; set; }
        public string Position { get; set; }
        public int CityId { get; set; }

        private string _City;
        public string City
        {
            get
            {
                if (string.IsNullOrEmpty(_City) && CityId != 0)
                {
                   _City = dictItemRepo.GetCityName(CityId);
                }
                return _City;
            }
            set
            {
                _City = value;
            }
        }

        public int SalaryMin { get; set; }

        public int SalaryMax { get; set; }

        public int InternSalaryType { get; set; }

        public string SalaryRange
        {
            get
            {
                return dictItemRepo.GetSalaryRange(SalaryMin, SalaryMax, InternSalaryType);
            }
        }

        public int PositionTypeId { get; set; }
        public string PositionTypeName
        {
            get
            {
                return ((Topuc22Top.Common.PositionType)PositionTypeId).GetEnumDescription();
            }
        }

        public string IndustryIds { get; set; }

        private string _IndustryNames;
        public string IndustryNames
        {
            get
            {
                if (string.IsNullOrEmpty(_IndustryNames) && !string.IsNullOrEmpty(IndustryIds))
                {
                    _IndustryNames = dictItemRepo.GetIndustryName(string.Join(",", IndustryIds), ",");
                    //cut
                    _IndustryNames = StringUtil.CutString(_IndustryNames, 26, "...");
                }
                return _IndustryNames;
            }
            set
            {
                _IndustryNames = value;
            }
        }

        public DateTime DeployTime { get; set; }

        /// <summary>
        /// 在其他城市招聘
        /// </summary>
        public List<Tuple<int, string>> OverOtherCitys { get; set; }

        public long ViewdCount { get; set; }

    }
}
