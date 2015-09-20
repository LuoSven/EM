using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using Topuc22Top.Common;
using Topuc.Framework.Cache;
using System.Data.Entity;
using System.Diagnostics;
using Topuc.Framework.Logger;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyRequirementRepo : RepositoryBase<TB_Enterprise_Requirement>, ICompanyRequirementRepo
    {
        public CompanyRequirementRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string GetRequirement(int etpId)
        {
            var rslt = "";

            var entity = DataContext.TB_Enterprise_Requirement.Where(p => p.EnterpriseId == etpId).FirstOrDefault();
            if (entity != null)
            {
                var comment = entity.ConsultantComment;

                var reqStr = "";
                var headCountStr = "";
                if (entity.Headcount.HasValue && entity.Headcount.Value > 0)
                {
                    headCountStr = string.Format("<b>{0}</b>位", entity.Headcount);
                }
                var degreeReq = "";
                if (!string.IsNullOrEmpty(entity.Degrees))
                {
                    try
                    {
                        var degreeNames = ((Topuc22Top.Common.Degree)Convert.ToInt32(entity.Degrees)).GetEnumDescription();
                        degreeReq = string.Format("<b>{0}及以上学历</b>", degreeNames);
                    }
                    catch { }
                }
                reqStr += string.Format("该公司今年计划招聘{0}{1}的大学生{2}", headCountStr, degreeReq
, string.IsNullOrEmpty(headCountStr) && string.IsNullOrEmpty(degreeReq) ? "，" : "。");

                var reqStrpart2 = "";

                var dictRepo = new DictItemRepo(new DatabaseFactory(), new InMemoryCache());

                if (!string.IsNullOrEmpty(entity.SchoolLevels))
                {
                    var schoolReq = "";
                    if (entity.SchoolLevels == "1")
                    {
                        schoolReq = "普通本科及以上级别";
                    }
                    else if (entity.SchoolLevels == "2")
                    {
                        schoolReq = "一本及以上级别";
                    }
                    else if (entity.SchoolLevels == "3")
                    {
                        schoolReq = "211及以上级别";
                    }
                    else if (entity.SchoolLevels == "4")
                    {
                        schoolReq = "985";
                    }
                    if (!string.IsNullOrEmpty(schoolReq))
                        reqStrpart2 += string.Format("{1}<b>{0}</b>的高校，", schoolReq, string.IsNullOrEmpty(reqStrpart2) ? "主要面对" : "");
                }

                if (!string.IsNullOrEmpty(entity.Majors))
                {
                    var majorNames = dictRepo.GetMajorNames(entity.Majors, "、");
                    if (!string.IsNullOrEmpty(majorNames))
                        reqStrpart2 += string.Format("{1}对口专业为<b>{0}</b>等，", majorNames, string.IsNullOrEmpty(reqStrpart2) ? "" : "");
                }

                reqStr += reqStrpart2;

                //去逗号，改为句号
                if (reqStr.EndsWith("，"))
                {
                    reqStr = reqStr.Substring(0, reqStr.Length - 1) + "。";
                }

                if (!string.IsNullOrEmpty(entity.Functions))
                {
                    reqStr += string.Format("招聘职位主要为<b>{0}</b>等类职位，", entity.Functions.Replace(",", "、"));
                }

                if (!string.IsNullOrEmpty(entity.Cities))
                {
                    var cityNames = dictRepo.GetCityNames(entity.Cities, "、");
                    if (!string.IsNullOrEmpty(cityNames))
                        reqStr += string.Format("工作城市为<b>{0}</b>等，", cityNames);
                }

                //去逗号，改为句号
                if (reqStr.EndsWith("，"))
                {
                    reqStr = reqStr.Substring(0, reqStr.Length - 1) + "。";
                }

                if (reqStr == "该公司今年计划招聘的大学生。")
                    reqStr = "";

                if (!string.IsNullOrEmpty(comment) && !string.IsNullOrEmpty(reqStr))
                {
                    rslt = comment + "<br/><br/>" + reqStr;
                }
                else if (!string.IsNullOrEmpty(comment) || !string.IsNullOrEmpty(reqStr))
                {
                    rslt = comment + reqStr;
                }
                else rslt = "";
            }

            return rslt;
        }

        public string GetReqConsultantComment(int etpId) 
        {
            return (from entity in DataContext.TB_Enterprise_Requirement where entity.EnterpriseId == etpId select entity.ConsultantComment).FirstOrDefault();
        }

        public void Inquire(int etpId)
        {
            var entity = DataContext.TB_Enterprise_Requirement.Where(p => p.EnterpriseId == etpId).FirstOrDefault();
            if (entity == null)
            {
                entity = new TB_Enterprise_Requirement()
                {
                    EnterpriseId = etpId,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    InquireCount = 1
                };
                DataContext.TB_Enterprise_Requirement.Add(entity);
                DataContext.SaveChanges();
                return;
            }
            else
            {
                entity.InquireCount = (entity.InquireCount ?? 0) + 1;
                DataContext.SaveChanges();
            }
        }

        public async Task<string> GetRequirementAsync(int etpId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (TopucDB DataContext = new TopucDB())
            {
                var rslt = "";

                var entity = await DataContext.TB_Enterprise_Requirement.Where(p => p.EnterpriseId == etpId).FirstOrDefaultAsync();
                if (entity != null)
                {
                    var comment = entity.ConsultantComment;

                    var reqStr = "";
                    var headCountStr = "";
                    if (entity.Headcount.HasValue && entity.Headcount.Value > 0)
                    {
                        headCountStr = string.Format("<b>{0}</b>位", entity.Headcount);
                    }
                    var degreeReq = "";
                    if (!string.IsNullOrEmpty(entity.Degrees))
                    {
                        try
                        {
                            var degreeNames = ((Topuc22Top.Common.Degree)Convert.ToInt32(entity.Degrees)).GetEnumDescription();
                            degreeReq = string.Format("<b>{0}及以上学历</b>", degreeNames);
                        }
                        catch { }
                    }
                    reqStr += string.Format("该公司今年计划招聘{0}{1}的大学生{2}", headCountStr, degreeReq
    , string.IsNullOrEmpty(headCountStr) && string.IsNullOrEmpty(degreeReq) ? "，" : "。");

                    var reqStrpart2 = "";

                    if (!string.IsNullOrEmpty(entity.SchoolLevels))
                    {
                        var schoolReq = "";
                        if (entity.SchoolLevels == "1")
                        {
                            schoolReq = "普通本科及以上级别";
                        }
                        else if (entity.SchoolLevels == "2")
                        {
                            schoolReq = "一本及以上级别";
                        }
                        else if (entity.SchoolLevels == "3")
                        {
                            schoolReq = "211及以上级别";
                        }
                        else if (entity.SchoolLevels == "4")
                        {
                            schoolReq = "985";
                        }
                        if (!string.IsNullOrEmpty(schoolReq))
                            reqStrpart2 += string.Format("{1}<b>{0}</b>的高校，", schoolReq, string.IsNullOrEmpty(reqStrpart2) ? "主要面对" : "");
                    }

                    if (!string.IsNullOrEmpty(entity.Majors))
                    {
                        string[] idlist = entity.Majors.Split(',');
                        var majors = await DataContext.MajorStandard.Where(a => idlist.Contains(a.Code)).Select(a => a.Major).ToListAsync();
                        var majorNames = string.Join("、", majors.ToArray());
                        if (!string.IsNullOrEmpty(majorNames))
                            reqStrpart2 += string.Format("{1}对口专业为<b>{0}</b>等，", majorNames, string.IsNullOrEmpty(reqStrpart2) ? "" : "");
                    }

                    reqStr += reqStrpart2;

                    //去逗号，改为句号
                    if (reqStr.EndsWith("，"))
                    {
                        reqStr = reqStr.Substring(0, reqStr.Length - 1) + "。";
                    }

                    if (!string.IsNullOrEmpty(entity.Functions))
                    {
                        reqStr += string.Format("招聘职位主要为<b>{0}</b>等类职位，", entity.Functions.Replace(",", "、"));
                    }

                    if (!string.IsNullOrEmpty(entity.Cities))
                    {
                        int[] idlist = ArrayConvertor.Convert(entity.Cities);
                        var query = await DataContext.DictItem.Where(s => s.Type == "City").Where(it => idlist.Contains(it.ItemId)).Select(it => it.ItemName).ToListAsync();
                        var cityNames = string.Join("、", query);

                        if (!string.IsNullOrEmpty(cityNames))
                            reqStr += string.Format("工作城市为<b>{0}</b>等，", cityNames);
                    }

                    //去逗号，改为句号
                    if (reqStr.EndsWith("，"))
                    {
                        reqStr = reqStr.Substring(0, reqStr.Length - 1) + "。";
                    }

                    if (reqStr == "该公司今年计划招聘的大学生。")
                        reqStr = "";

                    if (!string.IsNullOrEmpty(comment) && !string.IsNullOrEmpty(reqStr))
                    {
                        rslt = comment + "<br/><br/>" + reqStr;
                    }
                    else if (!string.IsNullOrEmpty(comment) || !string.IsNullOrEmpty(reqStr))
                    {
                        rslt = comment + reqStr;
                    }
                    else rslt = "";
                }
                AppLogger.Info("查询公司职位需求共花费：" + sw.ElapsedMilliseconds + "ms, 公司id:" + etpId.ToString());
                return rslt;
            }
        }
    }

    public interface ICompanyRequirementRepo : IRepository<TB_Enterprise_Requirement>
    {
        /// <summary>
        /// 获取招聘需求（包括顾问点评）
        /// </summary>
        /// <param name="etpId"></param>
        /// <returns></returns>
        string GetRequirement(int etpId);
        /// <summary>
        /// 获取顾问点评
        /// </summary>
        /// <param name="etpId"></param>
        /// <returns></returns>
        string GetReqConsultantComment(int etpId);
        void Inquire(int etpId);
        Task<string> GetRequirementAsync(int etpId);
    }
}
