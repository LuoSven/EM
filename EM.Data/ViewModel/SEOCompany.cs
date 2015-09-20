using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    public class SEOCompany
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string IndustryIds { get; set; }
        public string IndustryNames { get; set; }
        public string FirstInduId { get; set; }
        public string FirstInduName { get; set; }
        public string FirstInduPY { get; set; }
        public string Abbr { get; set; }
        public int FollowCount { get; set; }
    }

    public class SEOCompanyIntro
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string EnterpriseDesc { get; set; }
    }

    public class SEOCompanySalary
    {
        //public int PositionId { get; set; }
        public string Position { get; set; }
        public int SalaryAvg { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public DateTime CreateTime { get; set; }
        //-----Green 2014-7-15 关联Positionkey
        public int? PositionKeyId { get; set; }
        public string PositionKeyPY { get; set; }
    }

    public class SEOCompanyExper
    {
        public string Position { get; set; }
        public string Question { get; set; }
        public string ExperDesc { get; set; }
        public DateTime PublishTime { get; set; }
    }

    public class SEOCompanyNews
    {
        public string Title { get; set; }
        public string NewsUrl { get; set; }
        public DateTime? PublishTime { get; set; }

        public string Source { get; set; }
    }

    public class SEOCompanyContact
    {
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    [Serializable]
    public class SEOIndustryRelatedCompany
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public List<SEOJobBasicInfo> JobBasicInfoList { get; set; }
    }

    [Serializable]
    public class SEOJobBasicInfo
    {
        public int PositionId { get; set; }
        public string Position { get; set; }
        public DateTime DeployDate { get; set; }
    }

    [Serializable]
    public class SEOJobListBasicInfo
    {
        public int PositionId { get; set; }
        public string Position { get; set; }
        public string Desc { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public DateTime DeployDate { get; set; }
    }
}
