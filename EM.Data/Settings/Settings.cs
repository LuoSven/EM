using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Data
{
    public static class Settings
    {
        public const string Key_AssessmentQuestions = "#AssessmentQuestionsKey#";
        public const string Key_DictItemList = "#DictItemListKey#";
        public const string ParaKey_DictItemTypeList = "#DictItemTypeListPrefix_{0}#";
        //经分层的
        public const string ParaKey_LeveledDictItemTypeList = "#LeveledDictItemTypeListPrefix_{0}#";
        //2014-6-30 Green 有宣讲会、招聘会的城市进行缓存
        public const string Key_CampusTalkCity = "#CampusTalkCityKey#";
        public const string Key_JobFair = "#JobFairCityKey#";

        //相关专业关键词缓存
        public const string ParaKey_RelatedMajorList = "#RelatedMajorListPrefix_{0}#";

        //高校名称ID缓存
        public const string ParaKey_SchoolNameList = "#SchoolNameListPrefix_{0}#";

        public const string ParaKey_CompanyPreviewResult = "#CompanyPreviewResultPrefix_{0}#";

        public const string ParaKey_CompanyInfoResult = "#CompanyInfoResultPrefix_{0}#";

        public const string ParaKey_ITJobs = "#ItJobsKey_{0}#";

        //城市新闻CityId缓存
        public const string ParaKey_CityNews = "#CityNewsKey_{0}#";
        //城市站首页CityAreaInfo
        public const string ParaKey_CityAreaInfo = "#AreaInfoKey_{0}_{1}#";

        //文章热门标签缓存
        public const string ParaKey_HotArticleTagList = "#HotArticleTagListPrefix_{0}#";

        public const string ParaKey_HotArticleList = "#HotArticleListPrefix_{0}#";

        public const string ParaKey_TagName = "#TagNamePrefix_{0}#";

        public const string ParaKey_ArticleTag = "#ArticleTagNamePrefix_{0}#";


        public const string ParaKey_FunctionHotSearch = "#FunctionHotSearch_{0}#";


        public const string ParaKey_SeoPositionKey = "#SeoPositionKeyPrefix_{0}#";

        public const string ParaKey_IndustryRelatedCompanyList = "#IndustryRelatedCompanyListPrefix_{0}#";

        public const string ParaKey_SEOPositionKeyList = "#SeoPositionKeyListPrefix_{0}#";

        public const string ParaKey_SeoCompanyNewsCount = "#SeoCompanyNewsCountPrefix_{0}_{1}_{2}#";

        public const string ParaKey_SeoCompanyNews = "#SeoCompanyNewsPrefix_{0}_{1}_{2}#";

        public static string ParaKey_StatSummary = "#StatSummary#";


        public const string ParaKey_OtherPositionsForJobDetailAsync = "#OtherPositionsForJobDetailAsync{0}_{1}_{2}#";
        public const string ParaKey_OtherPositionsForJobDetail = "#OtherPositionsForJobDetail{0}_{1}_{2}#";


        public const string ParaKey_EdmTemplate = "#EdmTemplateHtml{0}#";

        public const string ParaKey_WeChatAccessToken = "#WeChatAccessToken#";

        
    }
}
