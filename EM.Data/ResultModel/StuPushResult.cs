using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.Repositories;

namespace Topuc22Top.Data.ResultModel
{
    public class StuPushResult
    {
        IDictItemRepo dictItemRepo = new DictItemRepo(new DatabaseFactory(), new InMemoryCache());
        public int PositionId { get; set; }

        public int EnterpriseId { get; set; }
        public string Postion { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool? Gender { get; set; }

        public int? City { get; set; }

        public string CityName
        {
            get
            {
                string name = string.Empty;
                if(City.HasValue)
                {
                    name = dictItemRepo.GetCityName(City.Value);
                }
                return name;
            }
        }

        public int? Degree { get; set; }

        public string Major { get; set; }

        public string School { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public int? ObjectivePosType { get; set; }

        public string ObjectiveSalary { get; set; }

        public string ObjectiveCity { get; set; }

        public string ObjectiveIndustry { get; set; }

        public string ObjectiveScale { get; set; }

        public int MatchDegree { get; set; }

        public DateTime CreateDate { get; set; }

        public string ObjectiveScaleName
        {
            get
            {
                int scale = 0;
                Int32.TryParse(ObjectiveScale, out scale);
                return dictItemRepo.GetCompanyScale(scale);
            }
        }

        public object ObjectiveCityName
        {
            get
            {
                var ciityNamesTemp = dictItemRepo.GetCityNames(ObjectiveCity,",");
                if (string.IsNullOrEmpty(ciityNamesTemp))
                    return null;
                return ciityNamesTemp;
            }
        }

        public object ObjectiveIndustryName
        {
            get
            {
                return dictItemRepo.GetIndustryName(ObjectiveIndustry,",");
            }
        }

        public int PushId { get; set; }

        public string DegreeName 
        {
            get
            {
                return dictItemRepo.GetDegreeName(Degree);
            }
        }

        public int EtpMark { get; set; }

        public string VideoUrl { get; set; }

        public string ObjectiveSalaryName
        {
            get
            {
                return dictItemRepo.GetObjectiveSalaryName(ObjectiveSalary);
            }
        }

        public int JobCityId { get; set; }

        public string JobCityName
        {
            get
            {
                if (JobCityId <= 0) return "";
                var name = dictItemRepo.GetCityName(JobCityId);
                return name;
            }
        }

    }
}
