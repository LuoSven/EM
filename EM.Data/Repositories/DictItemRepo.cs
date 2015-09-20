using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc.Framework.Cache.CacheProvider;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class DictItemRepo : RepositoryBase<DictItem>, IDictItemRepo
    {
        private readonly ICache cache;
#if Debug
        private readonly int  cacheMinutes=1;
#else
        private readonly int cacheMinutes = 60;
#endif


        public DictItemRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }


        #region 公司性质 CompanyMode

        public IList<DictItem> GetCompanyModeList()
        {
            return GetListByType("CompanyMode");
        }

        public string GetModeName(int id)
        {
            return GetName(id, "CompanyMode");
        }

        public string GetModeNames(string ids, string joinCharacter)
        {
            return GetNames(ids, joinCharacter, "CompanyMode");
        }

        public string GetCompanyMode(int? id)
        {
            if (!id.HasValue || id.Value == 0) return string.Empty;
            return GetName(id.Value, "CompanyMode");
        }

        public IList<DictItem> GetModeList()
        {
            return GetListByType("CompanyMode");
        }

        #endregion

        #region 公司规模 CompanyScale

        public IList<DictItem> GetCompanyScaleList()
        {
            return GetListByType("CompanyScale");
        }

        public string GetScaleName(int id)
        {
            if (id == 0) return string.Empty;
            return GetName(id, "CompanyScale");
        }

        public string GetScaleNames(string ids, string joinCharacter)
        {
            return GetNames(ids, joinCharacter, "CompanyScale");
        }

        public string GetCompanyScale(int? id)
        {
            if (!id.HasValue || id.Value == 0) return string.Empty;
            return GetName(id.Value, "CompanyScale");
        }

        public IList<DictItem> GetObjectiveScaleList()
        {
            return GetListByType("ObjectiveScale");

        }

        public IList<DictItem> GetScaleList()
        {
            return GetListByType("CompanyScale");
        }


        public string GetObjectiveScaleName(int ObjectiveScaleId)
        {
            var ObjectiveScale = (from s in DataContext.DictItem
                                  where s.Type == "ObjectiveScale" && s.ItemId == ObjectiveScaleId
                                  select s.ItemName).FirstOrDefault();

            if (ObjectiveScale != null)
            {
                return ObjectiveScale.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region 行业 Industry

        public string GetIndustryName(string idStr, string joinCharacter)
        {
            return GetNames(idStr, joinCharacter, "Industry");
        }

        public IList<DictItem> GetIndustryList()
        {
            return GetListByType("Industry");
        }

        public IList<Tuple<DictItem, List<DictItem>>> GetLeveledIndustryList() 
        {
            return cache.Get(string.Format(Settings.ParaKey_LeveledDictItemTypeList, "Industry"), () =>
            {
                var list = GetIndustryList();
                var lvedList = list.Where(p => p.ParentItemId == 0)
                    .Select(p => new Tuple<DictItem, List<DictItem>>(
                                p, list.Where(i => i.ParentItemId == p.ItemId).OrderBy(i => i.ItemId).ToList()
                            )
                        ).OrderBy(p => p.Item1.ItemId).ToList();
                return lvedList;
            }, cacheMinutes);
        }

        public string GetIndustryPY(string id)
        {
            if (string.IsNullOrEmpty(id)) return string.Empty;
            int idVal = 0;
            Int32.TryParse(id, out idVal);
            var query = GetListByType("Industry").Where(it => it.ItemId == idVal).Select(it => it.ItemPY);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return string.Empty;
        }

        public string GetIndustryIds(string funcNames)
        {
            if (string.IsNullOrEmpty(funcNames)) return string.Empty;
            string[] list = funcNames.Split(',');
            IList<int> idList = new List<int>();
            foreach (string func in list)
            {
                idList.Add(GetIdByValue(func, "Industry"));
            }
            return string.Join(",", idList);
        }

        public int GetIndustryIDByPinYin(string pyIndustry)
        {
            if (string.IsNullOrEmpty(pyIndustry)) return 0;

            var query = from item in DataContext.DictItem
                        where item.ItemValue == pyIndustry && item.Type == "Industry"
                        select item.ItemId;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return 0;
        }
        #endregion

        #region 搜索用的行业 SearchIndustry
        public string GetSearchIndustryName(string idStr, string joinCharacter)
        {
            return GetNames(idStr, joinCharacter, "SearchIndu");
        }

        public IList<DictItem> GetSearchIndustryList()
        {
            return GetListByType("SearchIndu");
        }
        #endregion

        #region 职能 Function

        public string GetFunctionName(string idStr, string joinCharacter)
        {
            return GetNames(idStr, joinCharacter, "Function");
        }


        public IList<DictItem> GetFunctionList()
        {
            return GetListByType("Function");
        }

        public string GetFunctionIds(string funcNames)
        {
            if (string.IsNullOrEmpty(funcNames)) return string.Empty;
            string[] list = funcNames.Split(',');
            IList<int> idList = new List<int>();
            foreach (string func in list)
            {
                idList.Add(GetIdByValue(func, "Function"));
            }
            return string.Join(",", idList);
        }


        #endregion

        #region 学历 Degree
        public IList<DictItem> GetDegreeList()
        {
            return GetListByType("Degree");
        }

        public string GetDegreeName(int? DegreeId)
        {
            string name = string.Empty;
            if (DegreeId.HasValue)
            {
                if (DegreeId.Value == 0) name = "不限";
                name = GetName(DegreeId.Value, "Degree");
            }
            return name;
        }

        public string GetDegreeName(string strId)
        {
            int id = 0;
            Int32.TryParse(strId, out id);
            if (id != 0)
            {
                return GetDegreeName(id);
            }
            return string.Empty;
        }

        public Dictionary<string, string> GetDegreeFilterList() 
        {
            var dict = new Dictionary<string, string>() { };
            dict.Add("1,2,3,4", "大专以上");
            dict.Add("1,2,3", "本科以上");
            dict.Add("1,2", "硕士以上");
            dict.Add("1", "博士以上");
            dict.Add("-1", "无学历要求"); //无学历要求 和 不限 是有区别的 ， 要查看SolrService是怎样的逻辑
            return dict;
        }

        #endregion

        #region 城市 City

        public string GetCityName(int id)
        {
            return GetName(id, "City");
        }

        public int GetCityId(string cityName)
        {
            if (string.IsNullOrEmpty(cityName)) return 0;

            var query = GetListByType("City").Where(it => it.ItemName == cityName).Select(it => it.ItemId);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return 0;
        }

        public int GetCityIdByPinyin(string cityPinyin)
        {
            if (string.IsNullOrEmpty(cityPinyin)) return 0;

            var query = GetListByType("City").Where(it => it.ItemPY == cityPinyin).Select(it => it.ItemId);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return 0;
        }

        public string GetCityNameByPinYin(string cityPinyin)
        {
            if (string.IsNullOrEmpty(cityPinyin)) return string.Empty;

            var query = GetListByType("City").Where(it => it.ItemPY == cityPinyin).Select(it => it.ItemName);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return string.Empty;
        }

        public string GetCityPinyinByName(string cityName)
        {
            if (string.IsNullOrEmpty(cityName)) return string.Empty;

            var query = GetListByType("City").Where(it => it.ItemName == cityName).Select(it => it.ItemPY);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return string.Empty;
        }

        public string GetCityNames(string idStr, string joinCharacter)
        {
            return GetNames(idStr, joinCharacter, "City");
        }

        public string GetDegreeNames(string idStr, string joinCharacter)
        {
            if (string.IsNullOrEmpty(idStr)) return string.Empty;
            else if (idStr == "0") return "不限";
            return GetNames(idStr, joinCharacter, "Degree");
        }

        public IList<DictItem> GetCityList()
        {
            return GetListByType("City");
        }

        public IList<DictItem> GetCityListByPid(int pid)
        {
            return GetListByType("City").Where(a => a.ParentItemId == pid).ToList();
        }

        public IList<DictItem> GetIndustryListByPid(int pid)
        {
            return GetListByType("Industry").Where(a => a.ParentItemId == pid).ToList();
        }

        public IList<DictItem> GetProvinceList()
        {
            var query = from m in GetListByType("City")
                        where (m.ParentItemId == 0 || m.ParentItemId == 2) && m.ItemId != 2 && m.ItemId <= 2027
                        select m;
            return query.ToList();
        }
        /// <summary>
        /// 按Id获取城市父节点Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetCityPidByid(int id)
        {
            int pid = 0;

            var citylist = GetListByType("City").Where(a => a.ItemId == id).ToList();
            if (citylist.Count > 0)
            {
                if (citylist.First().ParentItemId.HasValue)
                {

                    pid = citylist.First().ParentItemId.Value;
                    if (pid < 1000)
                    {
                        pid = id;
                    }
                }
            }

            return pid;

        }

        #endregion

        #region 热门职位关键词

        public string GetWordByPinyin(string py)
        {
            if (string.IsNullOrEmpty(py)) return string.Empty;

            string[] typelist = new string[] { "HotWord", "HotMajor", "SEO", "Function" };
            var query = from item in DataContext.DictItem
                        where (item.ItemValue == py || item.ItemPY == py) && (typelist.Contains(item.Type))
                        select item.ItemName;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            query = from item in DataContext.DictItem
                    where item.Type == "StandardPosition" && item.ItemPY == py
                    select item.ItemValue;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            query = from item in DataContext.MajorStandard
                    where item.PY == py
                    select item.Major;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return py;
        }

        public IList<DictItem> GetWordList()
        {
            return GetListByType("HotWord");
        }

        #endregion

        #region 首页热门专业
        public IList<DictItem> GetHotMajorList()
        {
            return GetListByType("HotMajor");
        }


        public IList<DictItem> GetAllTagList()
        {
            var query = from item in DataContext.DictItem
                        where (item.Type == "HotMajor" || item.Type == "HotWord" || item.Type == "SEO") && item.ParentItemId != 0
                        select item;
            return query.OrderBy(it => it.ParentItemId).ThenBy(it => it.ItemId).ToList();
        }
        #endregion

        #region 发布日期 Publish
        public IList<DictItem> GetPublishList()
        {
            int[] ids = { 1, 3, 4, 5, 7 };

            var Publish = GetListByType("Publish").Where(it => ids.Contains(it.ItemId));

            return Publish.ToList();
        }


        public int GetDTimeByValue(string dtime)
        {
            if (!string.IsNullOrEmpty(dtime))
            {
                var query = GetListByType("HotWord").Where(it => it.ItemValue == dtime).Select(it => it.ItemId);
                if (query.Any())
                {
                    return query.FirstOrDefault();
                }
            }
            return 0;
        }
        #endregion

        #region 职位类型 PositionType
        public IList<DictItem> GetPositionTypeList()
        {
            return GetListByType("PositionType");
        }

        #endregion

        #region 薪资 Salary


        public IList<DictItem> GetObjectiveSalaryList()
        {
            return GetListByType("ObjectiveSalary").Take(13).ToList();
        }

        public int GetObjectiveSalaryNum(int SalaryId)
        {

            var ObjectiveSalary = (from s in DataContext.DictItem
                                   where s.Type == "ObjectiveSalary" && s.ItemId == SalaryId
                                   select s).First();
            Regex regex = new Regex(@"\d+", RegexOptions.IgnoreCase);
            MatchCollection collection = regex.Matches(ObjectiveSalary.ItemName);
            return int.Parse(collection[0].Groups[0].Value);

        }


        public string GetObjectiveSalaryName(int ObjectiveSalaryId)
        {

            var query = GetListByType("ObjectiveSalary").Where(it => it.ItemId == ObjectiveSalaryId).Select(it => it.ItemName);

            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetObjectiveSalaryName(string strId)
        {
            int id = 0;
            Int32.TryParse(strId, out id);
            if (id != 0)
            {
                return GetObjectiveSalaryName(id);
            }
            return string.Empty;
        }

        public IList<DictItem> GetSalaryList()
        {
            var Salary = GetListByType("Salary").Take(9).ToList();
            Salary[8].ItemName = "10000及以上";
            return Salary;
        }

        public string GetSalaryName(int SalaryId)
        {
            return GetName(SalaryId, "Salary");
        }
        public string GetSalaryName(string SalaryId)
        {
            if (string.IsNullOrEmpty(SalaryId)) return string.Empty;

            int id = 0;
            Int32.TryParse(SalaryId, out id);
            if (id == 0) return string.Empty;

            return GetName(id, "Salary");
        }


        public string GetSalaryRange(int SalaryMin, int SalaryMax, int InternSalaryType = 0)
        {
            string Salary = "";
            if (InternSalaryType == 0)
            {
                if (SalaryMin == -1)
                    Salary = "面议";
                else if (SalaryMin == 0 && SalaryMax == 0)
                    Salary = " ";
                else if (SalaryMin == 0 && SalaryMax > 0)
                    Salary = SalaryMax.ToString() + "及以下";
                else if (SalaryMin > 0 && SalaryMax == 0)
                    Salary = SalaryMin.ToString() + "及以上";
                else
                    Salary = SalaryMin.ToString() + "-" + SalaryMax.ToString();

                if (Salary.Trim() == "")
                {
                    Salary = "面议";
                }
            }
            else
            {
                if (SalaryMin == -1 || SalaryMin == 0)
                {
                    Salary = "面议";
                }
                else
                {
                    string per = "月";
                    switch (InternSalaryType)
                    {
                        case 1: per = "月"; break;
                        case 2: per = "天"; break;
                        case 3: per = "小时"; break;
                        default: break;
                    }
                    string finalSanary = SalaryMin.ToString();
                    if (InternSalaryType == 1)
                    {
                        if (SalaryMax > 0)
                        {
                            finalSanary = string.Format("{0}-{1}", SalaryMin, SalaryMax);
                        }
                        else
                        {
                            finalSanary = string.Format("{0}及以上", SalaryMin);
                        }
                    }
                    Salary = string.Format("{0}/{1}", finalSanary, per);
                }
            }

            return Salary;
        }
        public int GetSalaryIdBySalaryMinMax(int SalaryMin, int SalaryMax)
        {
            string salary = GetSalaryRange(SalaryMin, SalaryMax);

            var query = from s in DataContext.DictItem
                        where s.Type == "Salary" && s.ItemName == salary
                        select s.ItemId;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return 0;

        }
        public int[] GetSalaryMinMax(int SalaryId)
        {
            string salary = GetSalaryName(SalaryId);

            int[] salaryarry = new int[2];

            salaryarry[0] = 0;
            salaryarry[1] = 0;

            Regex regex = new Regex(@"(\d+)", RegexOptions.IgnoreCase);
            MatchCollection collection = regex.Matches(salary);
            if (collection.Count == 2)
            {
                salaryarry[0] = Int32.Parse(collection[0].Groups[1].Value);
                salaryarry[1] = Int32.Parse(collection[1].Groups[1].Value);
            }
            else if (salary.Contains("以上"))
            {
                salaryarry[0] = Int32.Parse(collection[0].Groups[1].Value);
            }
            else if (salary.Contains("以下"))
            {
                salaryarry[1] = Int32.Parse(collection[0].Groups[1].Value);
            }
            else if (salary.Contains("面议"))
            {
                salaryarry[0] = -1;
            }

            return salaryarry;

        }
        #endregion

        #region Certification
        public IList<DictItem> GetCertificationList()
        {
            return GetListByType("Certification");
        }

        public IList<DictItem> GetCertificationList(int pid)
        {
            return GetListByType("Certification").Where(it => it.ParentItemId == pid).ToList();
        }



        public    string  GetCertificationName(int  Id)
        {
            return GetListByType("Certification").Where(x => x.Id == Id).Select(x => x.ItemName).FirstOrDefault();
        }
       public    int GetCertificationId(string Name)
        {
            return GetListByType("Certification").Where(x => x.ItemName == Name).Select(x => x.ItemId).FirstOrDefault();
        }

        public IList<DictItem> GetCertificationList(string certName)
        {
            var certPid = GetListByType("Certification").Where(x => x.ItemName == certName).Select(x => x.ParentItemId).FirstOrDefault();
            if (certPid.HasValue)
            {
                return GetCertificationList(certPid.Value);
            }

            return new List<DictItem>();
        }
        #endregion

        #region 专业 Major
        public string GetMajorNames(string idStr, string joinCharacter)
        {

            if (string.IsNullOrEmpty(idStr)) return string.Empty;

            if (idStr == "0") return "不限";
            string[] idlist = idStr.Split(',');


            var majors = (from a in DataContext.MajorStandard
                          where idlist.Contains(a.Code)
                          select a.Major);

            if (majors == null)
            {
                return "";
            }
            else
            {
                return string.Join(joinCharacter, majors.ToArray());
            }

        }

        /// <summary>根据专业拼音获取专业职位信息
        /// 
        /// </summary>
        /// <param name="majorPinYin"></param>
        /// <returns></returns>
        public MajorPosition GetMajorPositionByPY(string majorPinYin)
        {
            if (!string.IsNullOrEmpty(majorPinYin))
            {
                var query = from a in DataContext.MajorStandard
                            join b in DataContext.MajorPosition on a.MajorId equals b.MajorID
                            where a.PY == majorPinYin
                            select b;
                return query.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public MajorPosition GetMajorPositionByName(string majorName) 
        {
            if (!string.IsNullOrEmpty(majorName))
            {
                var query = from a in DataContext.MajorStandard
                            join b in DataContext.MajorPosition on a.MajorId equals b.MajorID
                            where a.Major == majorName
                            select b;
                return query.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }


        public IList<DictItem> GetRelatedMajors(string major)
        {
            return cache.Get(string.Format(Settings.ParaKey_RelatedMajorList, major), () =>
            {
                IList<DictItem> list = new List<DictItem>();
                var pcodeQuery = from it in DataContext.DictItem
                                 where it.ItemName == major && it.Type == "IndexMajor"
                                 && it.ParentItemId.HasValue && it.ParentItemId != 0
                                 select it.ParentItemId;
                if (pcodeQuery.Any())
                {
                    int pid = pcodeQuery.FirstOrDefault().Value;
                    var query = from it in DataContext.DictItem
                                where it.ParentItemId == pid && it.ItemName != major && it.Type == "IndexMajor"
                                select it;
                    list = query.ToList();
                }
                return list;
            });
        }
        #endregion

        #region 学年制 SchoolTime


        public string GetSchoolTimeName(int SchoolTimeId)
        {
            return GetName(SchoolTimeId, "SchoolTime");
        }
        #endregion



        #region 政治面貌 Politics

        public IList<DictItem> GetPolitics()
        {
            return GetListByType("Politics");
        }


        public string GetPoliticsName(int PoliticsId)
        {
            return GetName(PoliticsId, "Politics");
        }
        #endregion

        #region 性别 Sex


        public string GetSexName(int SexId)
        {
            return GetName(SexId, "Sex");
        }
        #endregion

        #region 竞争技能 ComputeSkill


        public string GetComputeSkillName(int ComputeSkillId)
        {
            return GetName(ComputeSkillId, "ComputeSkill");
        }

        public string GetSkillName(string idStr)
        {
            if (string.IsNullOrEmpty(idStr)) return string.Empty;
            int id = 0;
            if (!Int32.TryParse(idStr, out id)) return string.Empty;
            return GetSkillName(id);
        }

        private string GetSkillName(int id)
        {
            if (id == 0) return string.Empty;

            var query = from item in (DataContext.DictItem.Where(a => a.Type == "ComputeSkill").ToList())
                        where item.ItemId == id
                        select item.ItemName;
            if (query.Any())
            {
                return query.First().ToString();
            }
            return string.Empty;
        }


        #endregion



        #region 语言熟练等级 LanguageLevel


        public string GetLanguageLevelName(int LanguageLevelId)
        {
            return GetName(LanguageLevelId, "LanguageLevel");
        }

        public IList<DictItem> GetLanguageLevelList()
        {
            return GetListByType("LanguageLevel");
        }
        #endregion

        #region 语言类型 LanguageType

        public IList<DictItem> GetLanguageTypeList()
        {
            return GetListByType("LanguageType");
        }

        public string GetLanguageTypeName(int LanguageTypeId)
        {
            return GetName(LanguageTypeId, "LanguageType");
        }

        public string GetLanguageTypeName(string LanguageTypeId)
        {
            int id = 0;
            Int32.TryParse(LanguageTypeId, out id);
            if (id != 0)
            {
                return GetName(id, "LanguageType");
            }
            return string.Empty;
        }

        public IList<string> GetPositionLanguage(string languageCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(languageCode))
                    return null;
                IList<int> intList = ArrayConvertor.Convert(languageCode.Trim().Trim(','));

                var lanList = (from ind in DataContext.DictItem
                               where intList.Contains(ind.ItemId) && ind.Type == "LanguageType"
                               select ind.ItemName).ToList();
                IList<string> language = new List<string>();
                foreach (string list in lanList)
                {
                    language.Add(list);
                }
                return language;
            }
            catch
            {
                return new List<string>();
            }
        }

        public string GetPositionLanguageName(string LanMaster)
        {
            if (string.IsNullOrEmpty(LanMaster) || LanMaster.Trim() == string.Empty || LanMaster.Trim() == "0")
            {
                return string.Empty;
            }
            IList<String> lans = GetPositionLanguage(LanMaster.Trim());
            if (lans.Count == 0)
            {
                return string.Empty;
            }
            else if (lans.Count == 1)
            {
                return lans[0];
            }
            else
            {
                return lans[0] + "+" + lans[1];
            }
        }

        #endregion

        #region 语言证书 LanguageCertification


        public string GetLanguageCertificationName(int LanguageCertificationId)
        {
            return GetName(LanguageCertificationId, "LanguageCertification");
        }

        //原systemitem
        public string GetLanguageName(string idStr)
        {
            if (string.IsNullOrEmpty(idStr)) return string.Empty;
            int id = Int32.Parse(idStr);
            if (id == 0) return string.Empty;

            var query = from item in DataContext.DictItem
                        where item.ItemId == id && item.Type == "LanguageCertification" && item.ParentItemId == 0
                        select item.ItemName;
            if (query.Any())
            {
                return query.First().ToString();
            }
            return string.Empty;
        }

        //原systemitem
        public IList<DictItem> GetLanguageCerts(int langId)
        {

            var query = from s in DataContext.DictItem
                        where s.Type == "LanguageCertification" && s.ParentItemId == langId
                        select s;
            return query.ToList();
        }

        public IList<DictItem> GetLanguageCertificationList(int parentId)
        {
            return GetListByType("LanguageCertification").Where(it => it.ParentItemId == parentId).ToList();
        }

        //原systemitem
        public string GetLanguageCert(string idStr)
        {
            if (string.IsNullOrEmpty(idStr)) return string.Empty;
            int id = int.Parse(idStr);

            var query = from s in DataContext.DictItem
                        where s.Type == "LanguageCertification" && s.ItemId == id
                        select s.ItemName;
            return query.FirstOrDefault().ToString();
        }

        //原systemitem
        public string GetLanguageCertifications(string certStr)
        {
            if (string.IsNullOrEmpty(certStr)) return string.Empty;
            try
            {
                string desc = string.Empty;
                string[] certList = certStr.Split(',');
                foreach (string cert in certList)
                {
                    string certName = GetLanguageCert(cert.Split(':')[0]);
                    desc += certName;
                    string score = cert.Split(':')[1];
                    if (score != "0")
                    {
                        desc += "(" + score + "分);";
                    }
                }
                return desc.Trim(';');
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetLanguageCertificationsV1411(string certStr)
        {
            if (string.IsNullOrEmpty(certStr)) return string.Empty;
            try
            {
                string desc = string.Empty;
                string[] certList = certStr.Split(',');
                foreach (string cert in certList)
                {
                    desc += "<span>";
                    string certName = GetLanguageCert(cert.Split(':')[0]);
                    desc += certName;
                    string score = cert.Split(':')[1];
                    if (score != "0")
                    {
                        desc += "(" + score + "分)";
                    }
                    desc += "</span>";
                }
                return desc;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region 职位类型 PositionType


        public string GetPositionTypeName(int PositionTypeId)
        {
            return PositionTypeId == 1 ? "全职" : "实习";
            
            //return GetName(PositionTypeId, "PositionType");
        }

        //public string GetPositionTypeName(string strId)
        #endregion

        #region DictionaryItemHelper

        public string GetIndustryName(int id)
        {
            if (id == 0) return string.Empty;
            var query = from item in (DataContext.DictItem.Where(a => a.Type == "Industry").ToList())
                        where item.ItemId == id
                        select item;
            if (query.Any()) return query.First().ItemName;
            return string.Empty;
        }



        public string GetFunctionName(int id)
        {
            if (id == 0) return string.Empty;
            var query = from item in (DataContext.DictItem.Where(a => a.Type == "Function").ToList())
                        where item.ItemId == id
                        select item;
            if (query.Any()) return query.First().ItemName;
            return string.Empty;
        }

        public string GetClassRankingName(int DegreeId)
        {
            return GetName(DegreeId, "ClassRanking");
        }

        public string GetClassRankingName(string strId)
        {
            int id = 0;
            Int32.TryParse(strId, out id);
            if (id != 0)
            {
                return GetClassRankingName(id);
            }
            return string.Empty;
        }


        public IList<DictItem> GetClassRankingList()
        {
            return GetListByType("ClassRanking");
        }

        /// <summary>
        /// 扩展功能ID包含自身以及所所有子类ID
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001,1001001,1001002</returns>
        public string ExtendFunctionIds(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            string[] ids = idsStr.Split(',');
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in DataContext.DictItem
                            where item.ParentItemId == id && item.Type == "Function"
                            select item.ItemId;
                if (query.Any())//如果找到任何子元素
                {
                    idsStr += "," + string.Join(",", query.ToList());
                }
            }
            return idsStr;
        }

        /// <summary>
        /// 扩展功能ID包含自身以及所所有子类ID,用空格连接
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001 1001001 1001002</returns>
        public string ExtendFunctionIdsWithBlank(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            string[] ids = idsStr.Split(',');
            idsStr = idsStr.Replace(",", " ");
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in DataContext.DictItem
                            where item.ParentItemId == id && item.Type == "Function"
                            select item.ItemId;
                if (query.Any())//如果找到任何子元素
                {
                    idsStr += " " + string.Join(" ", query.ToList());
                }
            }
            return idsStr;
        }

        /// <summary>
        /// 扩展城市ID包含自身以及所所有子类ID
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001,1001001,1001002</returns>
        public string ExtendCityIds(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;

            string[] ids = idsStr.Split(',');
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in (DataContext.DictItem.Where(a => a.Type == "City").ToList())
                            where item.ParentItemId == id
                            select item.ItemId;
                if (query.Any())//如果找到任何子元素
                {
                    idsStr += "," + string.Join(",", query.ToList());
                }
            }
            return idsStr;
        }

        #endregion


        #region 内部共用方法

        private string GetName(int id, string type)
        {
            var query = GetListByType(type).Where(it => it.ItemId == id).Select(it => it.ItemName);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return string.Empty;
        }

        private int GetIdByValue(string name, string type)
        {
            var query = GetListByType(type).Where(it => it.ItemName == name).Select(it => it.ItemId);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return 0;
        }

        private string GetNames(string idStr, string joinCharacter, string type)
        {
            try
            {
                if (string.IsNullOrEmpty(idStr)) return string.Empty;
                if (idStr == "0") return "不限";
                int[] idlist = ArrayConvertor.Convert(idStr);
                var query = GetListByType(type).Where(it => idlist.Contains(it.ItemId)).Select(it => new {ItemId = it.ItemId, Name = it.ItemName }).ToList();
                var nameList = new List<string>();
                foreach (var itemid in idlist)
                {
                    var name = query.Where(p => p.ItemId == itemid).Select(p => p.Name).FirstOrDefault();
                    if (!string.IsNullOrEmpty(name))
                        nameList.Add(name);
                }
                return string.Join(joinCharacter, nameList);
            }
            catch
            {
                return string.Empty;
            }
        }

        private IList<DictItem> GetListByType(string type)
        {
            return cache.Get(string.Format(Settings.ParaKey_DictItemTypeList, type), () =>
            {
                var query = from s in DataContext.DictItem
                            where s.Type == type
                            select s;
                return query.OrderBy(it => it.ParentItemId).ThenBy(it => it.ItemId).ToList();
            }, cacheMinutes);
        }

        #endregion


        public IList<DictItem> GetIndexMajorWordList()
        {
            return GetListByType("IndexMajor");
        }

        public IList<ProfessionalMajor> GetProfessionalMajorList()
        {
            var query = from m in GetListByType("Professional")
                        where m.ItemId > 1001000
                        select new ProfessionalMajor()
                        {
                            MajorId = m.ItemId,
                            MajorName = m.ItemName,
                            MajorPinyin = m.ItemValue
                        };
            return query.ToList();
        }

        public string GetHotWordNameByPinyin(string pyValue)
        {
            var query = GetListByType("HotWord").Where(it => it.ItemPY.ToLower() == pyValue.ToLower()).Select(it => it.ItemName);
            if (query.Any())
            {
                return query.FirstOrDefault(); ;
            }
            return string.Empty;
        }

        public IList<DictItem> GetStandardPositionList()
        {
            return GetListByType("StandardPosition");
        }

        public string GetIndustryIdsByPY(string pyIndustry)
        {
            if (string.IsNullOrEmpty(pyIndustry)) return string.Empty;
            var query = from item in GetListByType("Industry")
                        where item.ItemPY == pyIndustry
                        select item.ItemId;
            if (query.Any())
            {
                var industryId = query.FirstOrDefault();
                var list = from item in GetListByType("Industry")
                           where (item.ItemId == industryId || item.ParentItemId == industryId)
                           select item.ItemId;
                return string.Join(",", list.ToArray());
            }
            return string.Empty;
        }


        public int GetTagCount()
        {
            var list = from item in DataContext.DictItem
                       where (item.Type == "HotMajor" || item.Type == "HotWord" || item.Type == "SEO") && item.ParentItemId != 0
                       select item;
            return list.Count();
        }

        public IQueryable<DictItem> GetAllTag(int page, int skipNumber)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            var list = (from item in DataContext.DictItem
                        where (item.Type == "HotMajor" || item.Type == "HotWord" || item.Type == "SEO") && item.ParentItemId != 0
                        select item).OrderBy(it => it.ParentItemId).ThenBy(it => it.ItemId).Skip(page * skipNumber).Take(skipNumber);
            return list;
        }


        public int GetCertificationParentID(string certName)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            var query = from m in context.DictItem
                        join n in context.DictItem
                        on m.ItemId equals n.ParentItemId
                        where m.Type == "Certification" && n.Type == "Certification" && n.ItemName == certName
                        select m.ItemId;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            else
            {
                //自定义证书
                return 9;
            }
        }


        public string GetCertificationParentName(string certName)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            var query = from m in context.DictItem
                        join n in context.DictItem
                        on m.ItemId equals n.ParentItemId
                        where m.Type == "Certification" && n.Type == "Certification" && n.ItemName == certName
                        select m.ItemName;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            else
            {
                //自定义证书
                return "自定义证书";
            }
        }


        public bool IsPositionWord(string word)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            var query1 = from item in GetStandardPositionList()
                         where item.ItemName == word
                         select item;
            var query2 = from item in GetHotWordList()
                         where item.ItemName == word
                         select item;
            return query1.Any() || query2.Any();
        }


        public string GetSearchIndustryIds(string idStr)
        {
            if (string.IsNullOrEmpty(idStr)) return string.Empty;
            int[] idList = ArrayConvertor.Convert(idStr);

            var query = from item in GetSearchIndustryList()
                        where idList.Contains(item.ItemId)
                        select item.ItemValue;
            if (query.Any())
            {
                return string.Join(",", query.ToList());
            }

            return string.Empty;
        }


        public string ExtendCityId(string cityId)
        {
            int id = 0;
            Int32.TryParse(cityId, out id);
            if (id != 0)
            {
                var query = from it in this.GetCityList()
                            where it.ItemId == id || it.ParentItemId == id
                            select it.ItemId;
                if (query.Any())
                {
                    return string.Join(",", query.ToArray());
                }
            }
            return string.Empty;
        }

        public IList<DictItem> GetHotWordList()
        {
            var query = from item in DataContext.DictItem
                        where item.Type == "HotWord"
                        select item;
            return query.OrderBy(it => it.ParentItemId).ThenBy(it => it.ItemId).ToList();
        }

    }

    public interface IDictItemRepo : IRepository<DictItem>
    {
        #region 公司性质 CompanyMode

        /// <summary>
        /// 获得所有公司性质的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetCompanyModeList();

        /// <summary>
        /// 获得公司性质的名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetModeName(int id);

        string GetModeNames(string ids, string joinCharacter);


        /// <summary>
        /// 获得公司性质的名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetCompanyMode(int? id);

        /// <summary>
        /// 获得所有公司性质的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetModeList();


        #endregion

        #region 公司规模 CompanyScale

        /// <summary>
        /// 获得所有公司规模的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetCompanyScaleList();

        /// <summary>
        /// 获得公司规模名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetScaleName(int id);

        /// <summary>
        /// 获得公司规模
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetCompanyScale(int? id);


        string GetScaleNames(string ids, string joinCharacter);

        /// <summary>
        /// 获得求职意向的公司规模List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetObjectiveScaleList();

        /// <summary>
        /// 获得公司规模
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetScaleList();

        /// <summary>
        /// 获得职业理想期望的公司规模
        /// </summary>
        /// <param name="ObjectiveScaleId"></param>
        /// <returns></returns>
        string GetObjectiveScaleName(int ObjectiveScaleId);

        #endregion

        #region 行业 Industry
        /// <summary>
        /// 返回行业名称
        /// </summary>
        /// <param name="idStr"></param>
        /// <param name="joinCharacter"></param>
        /// <returns></returns>
        string GetIndustryName(string idStr, string joinCharacter);

        /// <summary>
        /// 获得所有行业的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetIndustryList();

        /// <summary>
        /// 获取分级行业数据
        /// </summary>
        /// <returns></returns>
        IList<Tuple<DictItem, List<DictItem>>> GetLeveledIndustryList();

        /// <summary>
        /// 通过行业名称找id
        /// </summary>
        /// <param name="funcNames"></param>
        /// <returns></returns>
        string GetIndustryIds(string funcNames);

        /// <summary>
        /// 根据行业拼音获得行业id
        /// </summary>
        /// <param name="pyIndustry"></param>
        /// <returns></returns>
        int GetIndustryIDByPinYin(string pyIndustry);


        string GetIndustryPY(string id);
        #endregion

        #region 搜索用的行业 SearchIndustry

        /// <summary>
        /// 获得搜索用的行业
        /// </summary>
        /// <param name="idStr"></param>
        /// <param name="joinCharacter"></param>
        /// <returns></returns>
        string GetSearchIndustryName(string idStr, string joinCharacter);

        /// <summary>
        /// 获得搜索用的行业List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetSearchIndustryList();

        #endregion

        #region 职能 Function

        /// <summary>
        /// 获得职能名称
        /// </summary>
        /// <param name="idStr"></param>
        /// <param name="joinCharacter"></param>
        /// <returns></returns>
        string GetFunctionName(string idStr, string joinCharacter);

        /// <summary>
        /// 获得职能的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetFunctionList();

        /// <summary>
        /// 根据名称获得职能Id
        /// </summary>
        /// <param name="funcNames"></param>
        /// <returns></returns>
        string GetFunctionIds(string funcNames);



        #endregion

        #region 学历 Degree
        /// <summary>
        /// 获得学历的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetDegreeList();

        /// <summary>
        /// 获得学历名称
        /// </summary>
        /// <param name="DegreeId"></param>
        /// <returns></returns>
        string GetDegreeName(int? DegreeId);

        string GetDegreeNames(string idStr, string joinCharacter);

        /// <summary>
        /// 根据学历筛选
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetDegreeFilterList();

        #endregion

        #region 城市 City
        /// <summary>
        /// 获得城市名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetCityName(int id);

        /// <summary>
        /// 获得城市id
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        int GetCityId(string cityName);

        /// <summary>
        /// 通过拼音获得城市id
        /// </summary>
        /// <param name="cityPinyin"></param>
        /// <returns></returns>
        int GetCityIdByPinyin(string cityPinyin);

        /// <summary>
        /// 通过拼音获得城市名称
        /// </summary>
        /// <param name="cityPinyin"></param>
        /// <returns></returns>
        string GetCityNameByPinYin(string cityPinyin);

        /// <summary>
        /// 通过城市名称获得城市拼音
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        string GetCityPinyinByName(string cityName);


        /// <summary>
        /// 获得城市名称
        /// </summary>
        /// <param name="idStr"></param>
        /// <param name="joinCharacter"></param>
        /// <returns></returns>
        string GetCityNames(string idStr, string joinCharacter);

        /// <summary>
        /// 获得城市的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetCityList();
        /// <summary>
        /// 获得某个父节点id下所有的城市
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        IList<DictItem> GetCityListByPid(int pid);
        /// <summary>
        /// 获得省市
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetProvinceList();

        /// <summary>
        /// 按Id获取城市父节点Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetCityPidByid(int id);


        string ExtendCityId(string cityId);


        #endregion

        #region 热门职位关键词

        /// <summary>
        /// 通过拼音获得关键词
        /// </summary>
        /// <param name="py"></param>
        /// <returns></returns>
        string GetWordByPinyin(string py);

        /// <summary>
        /// 获得热门关键词
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetWordList();


        #endregion

        #region 首页热门专业

        /// <summary>
        /// 获得热门专业的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetHotMajorList();


        /// <summary>
        /// 获得所有热门
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetAllTagList();

        #endregion

        #region 发布日期 Publish

        /// <summary>
        /// 获得发布日期的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetPublishList();


        /// <summary>
        /// 通过数值返回天数
        /// </summary>
        /// <param name="dtime"></param>
        /// <returns></returns>
        int GetDTimeByValue(string dtime);

        #endregion

        #region 职位类型 PositionType
        /// <summary>
        /// 获得职能类型的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetPositionTypeList();

        #endregion

        #region 薪资 Salary

        /// <summary>
        /// 获得求职意向薪资的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetObjectiveSalaryList();

        /// <summary>
        /// 通过salaryid获得薪水数
        /// </summary>
        /// <param name="SalaryId"></param>
        /// <returns></returns>
        int GetObjectiveSalaryNum(int SalaryId);


        /// <summary>
        /// 根据求职意向的薪资id获得薪资的名称
        /// </summary>
        /// <param name="ObjectiveSalaryId"></param>
        /// <returns></returns>
        string GetObjectiveSalaryName(int ObjectiveSalaryId);


        /// <summary>
        /// 获得所有薪资的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetSalaryList();


        /// <summary>
        /// 获得薪资mingc
        /// </summary>
        /// <param name="SalaryId"></param>
        /// <returns></returns>
        string GetSalaryName(int SalaryId);
        string GetSalaryName(string SalaryId);
        /// <summary>
        /// 获得薪资范围
        /// </summary>
        /// <param name="SalaryMin"></param>
        /// <param name="SalaryMax"></param>
        /// <returns></returns>
        string GetSalaryRange(int SalaryMin, int SalaryMax, int InternSalaryType = 0);

        /// <summary>
        /// 获得薪资id
        /// </summary>
        /// <param name="SalaryMin"></param>
        /// <param name="SalaryMax"></param>
        /// <returns></returns>
        int GetSalaryIdBySalaryMinMax(int SalaryMin, int SalaryMax);

        /// <summary>
        /// 通过薪资id获得薪资的最大小值
        /// </summary>
        /// <param name="SalaryId"></param>
        /// <returns></returns>
        int[] GetSalaryMinMax(int SalaryId);

        #endregion

        #region Certification

        /// <summary>
        /// 获得证书的List
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetCertificationList();

        IList<DictItem> GetCertificationList(string certName);

        string GetCertificationName(int Id);

        int GetCertificationId(string Name);

        IList<DictItem> GetCertificationList(int parentId);

        #endregion

        #region 专业 Major
        /// <summary>
        /// 获得专业名称
        /// </summary>
        /// <param name="idStr"></param>
        /// <param name="joinCharacter"></param>
        /// <returns></returns>
        string GetMajorNames(string idStr, string joinCharacter);

        /// <summary>根据专业拼音获取专业职位信息
        /// 
        /// </summary>
        /// <param name="majorPinYin"></param>
        /// <returns></returns>
        MajorPosition GetMajorPositionByPY(string majorPinYin);

        /// <summary>
        /// 根据专业Name获取专业职位信息
        /// </summary>
        /// <param name="majorPinYin"></param>
        /// <returns></returns>
        MajorPosition GetMajorPositionByName(string majorName);


        #endregion

        #region 学年制 SchoolTime

        /// <summary>
        /// 获得学年制
        /// </summary>
        /// <param name="SchoolTimeId"></param>
        /// <returns></returns>
        string GetSchoolTimeName(int SchoolTimeId);

        #endregion

        #region 政治面貌 Politics
        /// <summary>
        /// 获得政治面貌
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetPolitics();


        /// <summary>
        /// 获得政治面貌名称
        /// </summary>
        /// <param name="PoliticsId"></param>
        /// <returns></returns>
        string GetPoliticsName(int PoliticsId);

        #endregion

        #region 性别 Sex

        /// <summary>
        /// 获得性别名称
        /// </summary>
        /// <param name="SexId"></param>
        /// <returns></returns>
        string GetSexName(int SexId);

        #endregion

        #region 竞争技能 ComputeSkill

        /// <summary>
        /// 获得竞争技能
        /// </summary>
        /// <param name="ComputeSkillId"></param>
        /// <returns></returns>
        string GetComputeSkillName(int ComputeSkillId);

        /// <summary>
        /// 获得竞争技能
        /// </summary>
        /// <param name="idStr"></param>
        /// <returns></returns>
        string GetSkillName(string idStr);





        #endregion

        #region 语言熟练等级 LanguageLevel

        /// <summary>
        /// 获得语言等级
        /// </summary>
        /// <param name="LanguageLevelId"></param>
        /// <returns></returns>
        string GetLanguageLevelName(int LanguageLevelId);

        IList<DictItem> GetLanguageLevelList();

        #endregion

        #region 语言类型 LanguageType

        /// <summary>
        /// 获得语言类型
        /// </summary>
        /// <param name="LanguageTypeId"></param>
        /// <returns></returns>
        string GetLanguageTypeName(int LanguageTypeId);
        string GetLanguageTypeName(string LanguageTypeId);
        /// <summary>
        /// 获得语言类型
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        IList<string> GetPositionLanguage(string languageCode);

        string GetPositionLanguageName(string LanMaster);
        /// <summary>
        /// 获得所有语言类型
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetLanguageTypeList();

        #endregion

        #region 语言证书 LanguageCertification

        /// <summary>
        /// 获得语言证书
        /// </summary>
        /// <param name="LanguageCertificationId"></param>
        /// <returns></returns>
        string GetLanguageCertificationName(int LanguageCertificationId);


        //原systemitem
        /// <summary>
        /// 获得语言证书名称
        /// </summary>
        /// <param name="idStr"></param>
        /// <returns></returns>
        string GetLanguageName(string idStr);


        //原systemitem
        /// <summary>
        /// 获得语言证书的List
        /// </summary>
        /// <param name="langId"></param>
        /// <returns></returns>
        IList<DictItem> GetLanguageCerts(int langId);


        //原systemitem
        /// <summary>
        /// 获得语言证书
        /// </summary>
        /// <returns></returns>
        IList<DictItem> GetLanguageCertificationList(int parentId);


        //原systemitem
        /// <summary>
        /// 通过id获得语言证书
        /// </summary>
        /// <param name="idStr"></param>
        /// <returns></returns>
        string GetLanguageCert(string idStr);


        //原systemitem
        /// <summary>
        /// 获得语言证书
        /// </summary>
        /// <param name="certStr"></param>
        /// <returns></returns>
        string GetLanguageCertifications(string certStr);

        /// <summary>
        /// 获得语言证书 201411 供 新版 个人中心-用户简历管理 调用
        /// </summary>
        /// <param name="certStr"></param>
        /// <returns></returns>
        string GetLanguageCertificationsV1411(string certStr);

        #endregion

        #region 职位类型 PositionType

        /// <summary>
        /// 获得职能类型
        /// </summary>
        /// <param name="PositionTypeId"></param>
        /// <returns></returns>
        string GetPositionTypeName(int PositionTypeId);

        #endregion

        #region DictionaryItemHelper

        string GetIndustryName(int id);

        string GetFunctionName(int id);

        #endregion

        IList<DictItem> GetIndexMajorWordList();

        IList<ProfessionalMajor> GetProfessionalMajorList();

        string GetHotWordNameByPinyin(string pyName);

        IList<DictItem> GetStandardPositionList();

        string GetIndustryIdsByPY(string pyIndustry);

        IList<DictItem> GetClassRankingList();



        string GetObjectiveSalaryName(string strId);

        //string GetPositionTypeName(string strId);


        int GetTagCount();

        IQueryable<DictItem> GetAllTag(int page, int skipNumber);

        int GetCertificationParentID(string p);
        string  GetCertificationParentName(string p);

        bool IsPositionWord(string q);

        string GetSearchIndustryIds(string searchIndustryId);

        string GetDegreeName(string strId);

        string GetClassRankingName(int id);
        string GetClassRankingName(string strId);


        IList<DictItem> GetRelatedMajors(string major);

        IList<DictItem> GetHotWordList();

        IList<DictItem> GetIndustryListByPid(int p);
    }

    /// <summary>两个城市是否属同一省份比较类
    /// 
    /// </summary>
    public class CityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            if (x.Length > 4 && y.Length > 4)
            {
                return x.Substring(0, 4) == y.Substring(0, 4);
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(string cityID)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(cityID, null)) return 0;

            //只做非直辖市比较 
            if (cityID.Length > 4)
            {
                return cityID.Substring(0, 4).GetHashCode();
            }
            else
            {
                return 0;
            }
        }

    }

}
