using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ViewModel
{
    public class CompanyDetailsMobileVM
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string IndustryName { get; set; }
        public string IndustryIDs { get; set; }
        public string ModeName { get; set; }
        public string ScaleName { get; set; }
        public string CityName { get; set; }
        public string CityIDs { get; set; }
        public string WebSite { get; set; }
        public string Address { get; set; }
        public string Abbr { get; set; }
        public string CompanyDesc { get; set; }
        public string SourceName { get; set; }


        //欢迎词
        public string Slogan { get; set; }
        //员工成长案例


        /// <summary>
        /// 是否为激活企业
        /// </summary>
        public bool HasAccount { get; set; }
        /// <summary>
        /// 是否开启对话
        /// </summary>
        public bool HasQA { get; set; }
        /// <summary>
        /// 宣讲会
        /// </summary>


        //----Green 2014-7-15 判断是否上传Banner
        public bool HasBanner { get; set; }

        //-------------------------------------

    }

    public class CompanySEOBasicInfoMobileVM
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string IndustryName { get; set; }
        public string IndustryIDs { get; set; }
        public string ModeName { get; set; }
        public string ScaleName { get; set; }
        public string CityName { get; set; }
        public string CityIDs { get; set; }
        public string WebSite { get; set; }
        public string Address { get; set; }
        public string Abbr { get; set; }
        public string CompanyDesc { get; set; }
    }

    #region

    //基本信息

    //企业主页
    public class CompanySEOPageIndexMobileVM
    {
        public CompanySEOBasicInfoMobileVM BasicInfo { get; set; }

        public List<SEO_CompanyQA> QAList { get; set; }

    }
    //企业新闻
    public class CompanySEOPageNewsMobileVM
    {
        public CompanySEOBasicInfoMobileVM BasicInfo { get; set; }

        public List<SEOCompanyNews> NewsList { get; set; }

    }
    //联系方式
    public class CompanySEOPageLinkMobileVM
    {
        public CompanySEOBasicInfoMobileVM BasicInfo { get; set; }
        public SEOCompanyContact Contact { get; set; }
    }
    //职位列表
    public class CompanySEOPageJobsMobileVM
    {
        public CompanySEOBasicInfoMobileVM BasicInfo { get; set; }
        public IList<ActivePositionModel> JobList { get; set; }
    }
    //企业薪酬
    public class CompanySEOPageSalaryMobileVM
    {
        public CompanySEOBasicInfoMobileVM BasicInfo { get; set; }

        public List<SEO_CompanyQA> QAList { get; set; }

    }
    //企业QA列表
    public class CompanySEOPageQAIndexMobileVM
    {
        public CompanySEOBasicInfoMobileVM BasicInfo { get; set; }

        public List<SEO_CompanyQA> QAList { get; set; }

    }
    //企业QA详情
    public class CompanySEOPageQADetailMobileVM
    {
        public CompanySEOBasicInfoMobileVM BasicInfo { get; set; }

        public SEO_CompanyQA QA { get; set; }

    }
    #endregion
}
