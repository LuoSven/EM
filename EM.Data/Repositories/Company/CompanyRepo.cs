using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Topuc.Framework.Cache;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Data.Dapper;
using Dapper;
using System.Diagnostics;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyRepo : RepositoryBase<TB_Enterprise>, ICompanyRepo
    {
        private readonly ICache cache;
        public CompanyRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        //新首页推荐的公司处使用
        //同城市，同行业，指定个数的企业
        public IList<TB_Enterprise> GetRecommendEtps(int enterpriseID, int cityID, string industry, int count = 5)
        {
            string city = cityID.ToString();

            DateTime lastmonth = DateTime.Today.AddMonths(-1);
            var query = from e in DataContext.TB_Enterprise
                        join p in DataContext.TB_Position_Element on e.EnterpriseId equals p.EnterpriseId into pg
                        where e.CustomerLevel != (int)CustomerLevel.InternalTest && e.ProcessStatus == (int)EtpProcessStatus.AccountApproved && e.EnterpriseId != enterpriseID
                        && pg.Where(a => a.PositionStatus == 1 && a.Deadline >= DateTime.Today && a.DeployTime > lastmonth).Count() != 0
                        orderby (!e.IsFamous.HasValue ? 0 : e.IsFamous.Value) descending, pg.Where(a => a.PositionStatus == 1 && a.Deadline >= DateTime.Today && a.DeployTime > lastmonth).Count() descending
                        select e;

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(m => ("," + m.City + ",").Contains("," + city + ","));
            }

            if (!string.IsNullOrEmpty(industry))
            {
                string[] indu = industry.Split(',');
                string val = indu[0];
                query = query.Where(m => m.Industry.Contains(val));
            }

            return query.Take(count).ToList();
        }
        public TB_Enterprise GetEffectiveEtpByID(int enterpriseID)
        {
            var query = from m in DataContext.TB_Enterprise
                        where m.EnterpriseId == enterpriseID && m.Status != (int)EtpStatus.Shadow && m.Status != (int)EtpStatus.Invalid
                        select m;
            return query.FirstOrDefault();
        }
        public async Task<TB_Enterprise> GetEffectiveEtpByIDAsync(int enterpriseID)
        {
            return await DataContext.TB_Enterprise.Where(m => m.EnterpriseId == enterpriseID && m.Status != (int)EtpStatus.Shadow && m.Status != (int)EtpStatus.Invalid).FirstOrDefaultAsync();
        }

        public CompanyInfoResult GetCompanyInfo(int enterpriseID)
        {

            var query = from m in DataContext.TB_Enterprise
                        join n in DataContext.TB_Enterprise_Contact
                        on m.EnterpriseId equals n.EnterpriseId into jn
                        from c in jn.DefaultIfEmpty()
                        where m.EnterpriseId == enterpriseID
                        select new CompanyInfoResult()
                        {
                            EnterpriseId = m.EnterpriseId,
                            Name = m.Name,
                            Abbr = m.Abbr,
                            Industry = m.Industry,
                            Mode = m.Mode,
                            Scale = m.Scale,
                            City = m.City,
                            WebSite = m.WebSite,
                            DescText = m.DescText,
                            ContactId = c.ContactId,
                            ContactMan = c.ContactMan,
                            ContactEmail = c.ContactEmail,
                            ContactAreaCode = c.ContactAreaCode,
                            ContactTelephone = c.ContactTelephone,
                            ContactExt = c.ContactExt,
                            ContactMobile = c.ContactMobile,
                            PostCode = c.PostCode,
                            Address = c.Address,
                            ProcessStatus = m.ProcessStatus,
                            Tags = m.Tags,
                            CertificationStatus = m.CertificationStatus
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 判断待审核的账号和审核已通过的账号是否存在同名企业
        /// </summary>
        public bool IsNameExist(string name)
        {
            var query = from e in DataContext.TB_Enterprise
                        join a in DataContext.TB_Enterprise_Account
                        on e.EnterpriseId equals a.EnterpriseId
                        where e.Name == name &&
                        (e.ProcessStatus == (int)EtpProcessStatus.AccountApproved
                        || e.ProcessStatus == (int)EtpProcessStatus.AccountGenerated)
                        select e;
            return query.Any();
        }


        public bool IsNameExist(int enterpriseId, string name)
        {
            var query = from e in DataContext.TB_Enterprise
                        join a in DataContext.TB_Enterprise_Account
                        on e.EnterpriseId equals a.EnterpriseId
                        where e.EnterpriseId != enterpriseId && e.Name == name && e.Status != (int)EtpStatus.Invalid
                        select e;
            return query.Any();
        }

        public string GetCompanyIntro(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise
                         where m.EnterpriseId == enterpriseId
                         select m.DescText).FirstOrDefault();

            return query ?? string.Empty;
        }
        public string GetCompanyName(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise
                         where m.EnterpriseId == enterpriseId
                         select m.Name).FirstOrDefault();

            return query ?? string.Empty;
        }

        public string GetCompanyShotDesc(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise
                         where m.EnterpriseId == enterpriseId
                         select m.ShortDesc).FirstOrDefault();

            return query ?? string.Empty;
        }

        public IList<CompanyPreviewResult> GetCompanyPreviewList(int[] enterpriseids)
        {
            var query = from e in DataContext.TB_Enterprise
                        join q in DataContext.TB_Enterprise_QA on e.EnterpriseId equals q.EnterpriseId into vq
                        where enterpriseids.Contains(e.EnterpriseId)
                        select new CompanyPreviewResult
                        {
                            EnterpriseId = e.EnterpriseId,
                            EnterpriseName = string.IsNullOrEmpty(e.Abbr) ? e.Name : e.Abbr,
                            Industry = e.Industry,
                            Mode = e.Mode,
                            Scale = e.Scale,
                            Tags = e.Tags,
                            Status = e.Status,
                            QACnt = vq.Count(),
                            ShortDesc = e.ShortDesc,
                            Abbr = e.Abbr
                        };
            var list = query.ToList();

            var enterpriseIdArr = (from m in list
                                   select m.EnterpriseId).ToArray<int>();

            var positions = (from p in this.DataContext.TB_Position_Element
                             where
                             enterpriseIdArr.Contains(p.EnterpriseId)
                             && p.PositionStatus == 1
                             && p.Deadline >= DateTime.Today
                             orderby p.DeployTime descending
                             select new PositionShortInfo()
                                   {
                                       Position = p.Position,
                                       PositionId = p.PositionId,
                                       EnterpriseId = p.EnterpriseId
                                   });

            foreach (var m in list)
            {
                m.PositionList = (
                    from p in positions
                    where p.EnterpriseId == m.EnterpriseId
                    select p
                    ).Take(2).ToList<PositionShortInfo>();
            }

            return list;

        }


        public CompanyPreviewResult GetCompanyPreview(int enterpriseid)
        {
            return cache.Get(string.Format(Settings.ParaKey_CompanyPreviewResult, enterpriseid.ToString()), () =>
            {
                var sql = string.Format(@"select EnterpriseId,Name as EnterpriseName,Industry
,(select COUNT(1) from TB_Enterprise_QA b where b.EnterpriseId = {0}) as QACnt
,Status,Scale,Mode
 from TB_Enterprise
where EnterpriseId = {0}", enterpriseid);
                var ss = DataContext.Database.SqlQuery<CompanyPreviewResult>(sql);
                return ss.FirstOrDefault();
            });

        }

        public IList<CompanyNameAndIdResult> GetEnterpriseNameAndIdBy(string nameInitial, int page, int count)
        {
            if (!string.IsNullOrEmpty(nameInitial))
            {
                nameInitial = nameInitial.ToUpper();
            }
            if (page <= 0)
            {
                page = 1;
            }
            var query = from e in DataContext.TB_Enterprise
                        where e.Status != (int)EtpStatus.Shadow && e.Status != (int)EtpStatus.Invalid && e.NameInitial == nameInitial
                        select new CompanyNameAndIdResult
                        {
                            EnterpriseId = e.EnterpriseId,
                            Name = e.Name
                        };
            return query.OrderBy(v => v.EnterpriseId).Skip(count * (page - 1)).Take(count).ToList();
        }

        public int GetEnterpriseCountBy(string nameInitial)
        {
            if (!string.IsNullOrEmpty(nameInitial))
            {
                nameInitial = nameInitial.ToUpper();
            }
            var query = from e in DataContext.TB_Enterprise
                        where e.Status != (int)EtpStatus.Shadow && e.Status != (int)EtpStatus.Invalid && e.NameInitial == nameInitial
                        select e;
            return query.Count();
        }

        public IList<CompanyNameAndIdResult> GetEnterpriseNameAndIdBy(int page, int count)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var query = from e in DataContext.TB_Enterprise
                        where e.Status != (int)EtpStatus.Shadow && e.Status != (int)EtpStatus.Invalid
                        select new CompanyNameAndIdResult
                        {
                            EnterpriseId = e.EnterpriseId,
                            Name = e.Name
                        };
            return query.OrderBy(v => v.EnterpriseId).Skip(count * (page - 1)).Take(count).ToList();
        }

        public int GetEnterpriseCountBy()
        {
            var query = from e in DataContext.TB_Enterprise
                        where e.Status != (int)EtpStatus.Shadow && e.Status != (int)EtpStatus.Invalid
                        select e;
            return query.Count();
        }

        public string GetEnterpriseTags(int enterpriseId)
        {
            return DataContext.TB_Enterprise.Where(x => x.EnterpriseId == enterpriseId).Select(x => x.Tags).FirstOrDefault();
        }


        public int? GetEtpProcessStatus(int enterpriseId)
        {
            return DataContext.TB_Enterprise.Where(m => m.EnterpriseId == enterpriseId).FirstOrDefault().ProcessStatus;
        }

        public string GetRejectReason(int enterpriseId)
        {
            return DataContext.TB_Enterprise.Where(m => m.EnterpriseId == enterpriseId).FirstOrDefault().Tags;
        }
        //20150817 sven 找到时间较长的sql，因为用缓存，故暂时不动
        //推荐的公司必须是激活公司，有logo，有在招职位 
        //优先条件：同城市（职位的城市）、同行业
        //三元组 Item1表示Id，Item2表示Name（or Abbr），Item3表示在招职位个数
        public IList<Tuple<int, string, int>> GetRecommendEtps(int enterpriseID, string city, string industry, int count = 5)
        {
            var etpList = GetEtpRecommandNecessary();

            var list =
                cache.Get(string.Format("CompanyIndexGetRecommendEtps_{0}_{1}", city, industry), () =>
                {
                    return etpList.OrderByDescending(p => new Tuple<string, string, string, string>(p.City, p.Industry, city, industry), 
                        new RecommendEtpComparer()).Take(count + 1)
                        .Select(p => new Tuple<int, string, int>(p.EnterpriseId, (string.IsNullOrEmpty(p.Abbr) ? p.Name : p.Abbr), p.PosCount))
                        .ToList();
                },1800);

            return list.Where(p => p.Item1 != enterpriseID).Take(count).ToList();
        }

        private IList<CompanyIndexRecommandEtp> GetEtpRecommandNecessary()
        {
            return cache.Get("GetEtpRecommandNecessary", () =>
            {
                using (var conn = DapperHelper.Get22Connection())
                {
                    string sql = string.Format(@"
select a.EnterpriseId,a.Name,a.Abbr,a.City,a.Industry,count(c.PositionId) PosCount
from TB_Enterprise a
left join TB_Enterprise_Display b
on a.EnterpriseId = b.EnterpriseId
left join (
	select PositionId, EnterpriseId from TB_Position_Element
	where PositionStatus = 1 and Deadline > convert(varchar(10),getdate(),120) 
) as c
on a.EnterpriseId = c.EnterpriseId
where isnull(b.LogoPic,0)<>0 and ISNULL(c.EnterpriseId,0)<>0
and a.CustomerLevel != 99 and a.ProcessStatus = 4 
group by a.EnterpriseId,a.Name,a.Abbr,a.City,a.Industry
");
                    var result = conn.Query<CompanyIndexRecommandEtp>(sql);
                    return result.ToList();
                }
            }, 3600);
        }


        public int GetEnterpriseDecoratePrecent(int enterpriseid)
        {
            //- Logo 10% 
            //- 公司名称 3% 
            //- 简称 2% 
            //- 行业、规模、性质 5% 
            //- 简介 10% 
            //- 亮点 5% 
            //- 地址 5% （城市+地址，至少有一条完整记录） 
            //- 网址 2% 
            //- 第三方主页 3% (至少一条记录)

            //相册 15% （至少创建一个相册，且相册中至少有一张照片） 
            //视频 10% （至少添加一条视频记录） 
            //产品与服务 15% （至少添加一个产品） 
            //新闻报道 5% （至少添加一条新闻报道） 
            //员工成长 10% （至少添加一个案例）

            int completion = 0;//25
            var display = DataContext.TB_Enterprise_Display.Where(a => a.EnterpriseId == enterpriseid).FirstOrDefault();
            if (display != null)
            {
                completion += display.LogoPic.HasValue ? 10 : 0;
            }
            var corpbasic = DataContext.TB_Enterprise.Where(a => a.EnterpriseId == enterpriseid).FirstOrDefault();
            if (corpbasic != null)
            {
                completion += string.IsNullOrEmpty(corpbasic.Name) ? 0 : 3;
                completion += string.IsNullOrEmpty(corpbasic.Abbr) ? 0 : 2;
                if (!string.IsNullOrEmpty(corpbasic.Industry) || corpbasic.Scale.HasValue || corpbasic.Mode.HasValue)
                {
                    completion += 5;
                }
                completion += !string.IsNullOrEmpty(corpbasic.DescText) ? 10 : 0;
                completion += !string.IsNullOrEmpty(corpbasic.WebSite) ? 2 : 0;
                completion += string.IsNullOrEmpty(corpbasic.Tags) ? 0 : 5;
            }

            completion += DataContext.TB_Enterprise_Address.Any(x => x.EnterpriseId == enterpriseid && !string.IsNullOrEmpty(x.City) && !string.IsNullOrEmpty(x.Address)) ? 5 : 0;
            completion += HasThirdLink(enterpriseid) ? 3 : 0;


            completion += DataContext.TB_Enterprise_OnlineVideo.Where(a => a.EnterpriseId == enterpriseid).Count() > 0 ? 10 : 0;
            completion += DataContext.Photo.Where(a => a.EnterpriseId == enterpriseid).Count() > 0 ? 15 : 0;
            completion += DataContext.TB_Enterprise_Business.Where(a => a.EnterpriseId == enterpriseid).Count() > 0 ? 15 : 0;
            completion += DataContext.TB_Enterprise_StaffGrowth.Where(a => a.EnterpriseId == enterpriseid).Count() > 0 ? 10 : 0;
            completion += DataContext.TB_Enterprise_News.Where(a => a.EnterpriseId == enterpriseid).Count() > 0 ? 5 : 0;

            return completion > 100 ? 100 : completion;
        }

        private bool HasThirdLink(int enterpriseid)
        {
            bool result = false;

            var link = DataContext.TB_Enterprise_Link.Where(a => a.EnterpriseId == enterpriseid).FirstOrDefault();

            if (link != null)
            {
                if (!string.IsNullOrEmpty(link.LinkXml))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(link.LinkXml);

                    var root = xmldoc.ChildNodes;
                    if (root.Count > 0)
                    {
                        var items = root[0].ChildNodes;

                        for (int i = 0; i < items.Count; i++)
                        {
                            var name = items[i]["name"] != null ? items[i]["name"].InnerText : "";
                            var url = items[i]["url"] != null ? items[i]["url"].InnerText : "";

                            switch (name)
                            {
                                case "Sina":
                                    result = !string.IsNullOrEmpty(url);
                                    break;
                                case "Tencent":
                                    result = !string.IsNullOrEmpty(url);
                                    break;
                                case "RenRen":
                                    result = !string.IsNullOrEmpty(url);
                                    break;
                                case "Facebook":
                                    result = !string.IsNullOrEmpty(url);
                                    break;
                                case "Twitter":
                                    result = !string.IsNullOrEmpty(url);
                                    break;
                                case "Google":
                                    result = !string.IsNullOrEmpty(url);
                                    break;
                            }


                        }


                    }

                }
            }

            return result;


        }

        public void UpdateDecoratePercent(int enterpriseId)
        {
            var etp = DataContext.TB_Enterprise.Where(p => p.EnterpriseId == enterpriseId).FirstOrDefault();
            if (etp != null)
            {
                etp.DecoratePercent = GetEnterpriseDecoratePrecent(enterpriseId);
            }
        }


        //四元组：Item1：比较企业的City，Item2：比较企业的Industry，
        //Item3：当做常量 当前访问的企业的City，Item4：当做常量 当前访问的企业的Industry
        private class RecommendEtpComparer : IComparer<Tuple<string, string, string, string>>
        {
            public int Compare(Tuple<string, string, string, string> x, Tuple<string, string, string, string> y)
            {
                return GetRelativity(x) - GetRelativity(y);
            }
            /// <summary>
            /// 获取相关性
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            private int GetRelativity(Tuple<string, string, string, string> tuple)
            {
                int rlty = 0;
                if (!string.IsNullOrEmpty(tuple.Item3) && !string.IsNullOrEmpty(tuple.Item1))
                {
                    //计算城市相关性
                    var arr1 = tuple.Item3.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arr1)
                    {
                        if (tuple.Item1.Contains("," + item + ",") || tuple.Item1.StartsWith(item + ",") || tuple.Item1.EndsWith("," + item))
                            rlty++;
                    }
                }
                if (!string.IsNullOrEmpty(tuple.Item4) && !string.IsNullOrEmpty(tuple.Item2))
                {
                    //计算行业相关性
                    var arr1 = tuple.Item4.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arr1)
                    {
                        if (tuple.Item2.Contains("," + item + ",") || tuple.Item2.StartsWith(item + ",") || tuple.Item2.EndsWith("," + item))
                            rlty++;
                    }
                }
                return rlty;
            }
        }


        public IList<TB_Enterprise> GetSameNameList(int etpId, string name)
        {
            var query = from e in DataContext.TB_Enterprise
                        join a in DataContext.TB_Enterprise_Account on e.EnterpriseId equals a.EnterpriseId into ja
                        from aa in ja.DefaultIfEmpty()
                        where e.Name == name && e.EnterpriseId != etpId && aa.EnterpriseId == null
                        select e;
            return query.ToList();
        }

        /// <summary>
        /// 获取所有有效的企业列表
        /// （CustomerLevel！=99 and Status not in(31,99)）
        /// </summary>
        /// <returns></returns>
        public IList<int> GetEffectionEnterpriseId()
        {
            var exceptArr = new int[] { 31, 99 };
            DateTime dt = DateTime.Now.Date;
            var query = from e in DataContext.TB_Enterprise
                        where
                         e.CustomerLevel != 99
                        && e.CustomerLevel != null
                       && !exceptArr.Contains(e.Status)
                        select e.EnterpriseId;

            return query.ToList<int>();
        }

        public EtpAccountInfoResult GetAccountInfo(int id)
        {
            var query = from m in DataContext.TB_Enterprise
                        join n in DataContext.TB_Enterprise_Contact
                        on m.EnterpriseId equals n.EnterpriseId into jn
                        from c in jn.DefaultIfEmpty()
                        join act in DataContext.TB_Enterprise_Account
                        on m.EnterpriseId equals act.EnterpriseId into acgrp
                        from actinfo in acgrp.DefaultIfEmpty()
                        where m.EnterpriseId == id
                        select new EtpAccountInfoResult()
                        {
                            EnterpriseId = m.EnterpriseId,
                            EnterpriseName = m.Name,
                            ContactMan = c.ContactMan,
                            ContactAreaCode = c.ContactAreaCode,
                            ContactTelephone = c.ContactTelephone,
                            ContactExt = c.ContactExt,
                            ContactMobile = c.ContactMobile,
                            ContactEmail = (c.ContactEmail == null || c.ContactEmail == "") ? actinfo.LoginEmail : c.ContactEmail,
                            AccountName = actinfo.UserName,
                            CertificationStatus = m.CertificationStatus
                        };
            return query.FirstOrDefault();
        }

        public async Task<TB_Enterprise> GetByIdAsync(int id)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Enterprise.FindAsync(id);
            }
        }


        public async Task<Dictionary<string, bool>> GetSummaryAsync(int id)
        {
            Dictionary<string, bool> summary = new Dictionary<string, bool>();
            using (TopucDB db = new TopucDB())
            {
                summary.Add("bussiness", await db.TB_Enterprise_Business.Where(m => m.EnterpriseId == id).AnyAsync());
                summary.Add("sfaff", await db.TB_Enterprise_StaffGrowth.Where(m => m.EnterpriseId == id).AnyAsync());
                summary.Add("album", await db.TB_Enterprise_Album.Where(a => a.EnterpriseId == id && a.PhotoCount > 0).AnyAsync());
                summary.Add("contact", (await db.TB_Enterprise_Contact.Where(m => m.EnterpriseId == id).AnyAsync())
                    || (await db.TB_Enterprise_Address.Where(m => m.EnterpriseId == id).AnyAsync()));
                summary.Add("jobs", await db.TB_Position_Element.Where(m => m.EnterpriseId == id && m.PositionStatus == (int)PositionStatus.Publish && m.Deadline >= DateTime.Today).AnyAsync());
                summary.Add("qa", await db.TB_Enterprise_Display.Where(d => d.EnterpriseId == id && d.QAStatus.HasValue && d.QAStatus.Value).AnyAsync());
                summary.Add("news", await db.TB_Enterprise_News.Where(m => m.EnterpriseId == id).AnyAsync());
                summary.Add("video", await db.TB_Enterprise_OnlineVideo.Where(m => m.EnterpriseId == id).AnyAsync());
            }
            return summary;
        }


        public Task<string> GetCompanyWebSiteAsync(int id)
        {
            string website = string.Empty;
            var query = from e in DataContext.TB_Enterprise
                        where e.EnterpriseId == id
                        select e.WebSite;
            if (query.Any())
            {
                website = query.FirstOrDefault();
            }
            return Task.FromResult(website);
        }

        public int GetEtpCertificationStatus(int id)
        {
            return ((from e in DataContext.TB_Enterprise
                     where e.EnterpriseId == id
                     select e.CertificationStatus).FirstOrDefault() ?? ((int)EtpCertificationStatus.NoHandle));
        }

        public TB_Enterprise GetEnterpriseByAccountId(int accountId)
        {
            return (from etp in DataContext.TB_Enterprise
                    where (from etpAccount in DataContext.TB_Enterprise_Account where etpAccount.AccountId == accountId select etpAccount.EnterpriseId).Contains(etp.EnterpriseId)
                    select etp).FirstOrDefault();
        }

        public IList<Tuple<int, DateTime?>> GetEtpIdLastLoginAtTupleList()
        {
            using (TopucDB context = new TopucDB())
            {
                var dtNow = DateTime.Now;
                var etpIds = (from pos in context.TB_Position_Element
                              where pos.Deadline >= dtNow && pos.PositionStatus == (int)PositionStatus.Publish
                              select pos.EnterpriseId).Distinct().ToList();
                var entList = (from etp in context.TB_Enterprise
                               join account in context.TB_Enterprise_Account
                               on etp.EnterpriseId equals account.EnterpriseId
                               where etp.ProcessStatus == (int)EtpProcessStatus.AccountApproved
                               && etpIds.Contains(etp.EnterpriseId)
                               && account.LastLoginAt.HasValue
                               select new
                               {
                                   EnterpriseId = etp.EnterpriseId,
                                   LastLoginAt = account.LastLoginAt
                               }
                        ).ToList();
                return entList.Select(p => new Tuple<int, DateTime?>(p.EnterpriseId, p.LastLoginAt)).ToList();
            }

        }

        #region Dapper

        public async Task<CompanyDTO> GetAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"select a.EnterpriseId, a.Name, a.Industry, a.Mode, a.Scale,a.Status 
from TB_Enterprise a
where a.EnterpriseId = {0}
", id);
                var list = await conn.QueryAsync<CompanyDTO>(sql);
                return list.FirstOrDefault();
            }
        }

        public CompanyDTO Get(int id)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from e in context.TB_Enterprise
                            where e.EnterpriseId == id
                            select new CompanyDTO()
                            {
                                EnterpriseId = e.EnterpriseId,
                                Name = e.Name,
                                Industry = e.Industry,
                                Mode = e.Mode,
                                Scale = e.Scale,
                                Status = e.Status,
                                WebSite = e.WebSite
                            };
                return query.FirstOrDefault();
            }
        }


        public async Task<CompanyInfoDTO> GetCompanyInfoDTO(int companyId, int userId = -1)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select top 1 a.EnterpriseId, a.Name EnterpriseName, a.Industry, a.Mode, a.Scale
,case isnull(b.ID,0) when 0 then 0 else 1 end IsFollowed
from TB_Enterprise a
left join Stu_Follow b
on a.EnterpriseId = b.TargetId and {1} > 0 and b.UserId = {1} and b.TargetType = 'Enterprise'
where a.EnterpriseId = {0}
order by b.FollowDate
", companyId, userId);
                var list = await conn.QueryAsync<CompanyInfoDTO>(sql);
                return list.FirstOrDefault();
            }
        }

        public Task<string> GetCompanyDescText(int companyId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select DescText from TB_Enterprise
where EnterpriseId = {0}
", companyId);
                return Task.FromResult<string>((conn.Query<string>(sql)).FirstOrDefault());
            }
        }

        public PagedResult<string> GetDuplicatedNames(string etpName, int page, int pageSize)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                /*
                 * 
                 * 
    WITH OrderedOrders AS 
    (
    select Name ,ROW_NUMBER()OVER(ORDER BY RCount DESC) AS 'RowNumber'
    from (
    select RTRIM(LTRIM(Name)) Name, COUNT(1) RCount
    from TB_Enterprise
    where 
    Status <> 99
    group by RTRIM(LTRIM(Name))
    having COUNT(1) > 1
    ) as t
    )
    SELECT Name
    FROM OrderedOrders  
    WHERE RowNumber between 1 and 10
                 */
                string sql = string.Format(@"
select RTRIM(LTRIM(Name)) Name from TB_Enterprise
where 
Status <> 99 and isnull(Name,'') <> ''
and (isnull(@etpName,'') = '' or Name like ('%' + @etpName + '%'))
group by RTRIM(LTRIM(Name))
having COUNT(1) > 1
order by COUNT(1) desc,Name
");
                var result = conn.Query<string>(sql, new { etpName = etpName });
                var totalCnt = result.Count();
                var pageResult = result.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var r = new PagedResult<string>(pageResult, page, pageSize, totalCnt);
                return r;
            }
        }

        public IList<DuplicatedEtpInfoDTO> GetDuplicatedEtpListByName(string name)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select a.EnterpriseId, dbo.fun_getCityName(a.City) as CityName, a.Status, a.ProcessStatus
,a.CreateDate
, case isnull(b.AccountId,0) when 0 then 0 else 1 end IsHasAccount
, (select g.UserName from UserProfile g where g.UserId = c.AssignTo) UserName
--,c.AssignResult
from TB_Enterprise a
left join TB_Enterprise_Account b
on a.EnterpriseId = b.EnterpriseId
left join AssignList c
on a.EnterpriseId  = c.EtpID
where a.Status <> 99 and RTRIM(LTRIM(a.Name)) = @name
");
                var result = conn.Query<DuplicatedEtpInfoDTO>(sql, new { @name = name });
                return result.ToList();
            }
        }

        #endregion

    }

    public interface ICompanyRepo : IRepository<TB_Enterprise>
    {
        //新首页推荐的公司处使用
        //同城市，同行业，指定个数的企业

        IList<TB_Enterprise> GetRecommendEtps(int enterpriseID, int cityID, string industry, int count = 5);

        /// <summary>
        /// 获取有效企业
        /// </summary>
        /// <param name="enterpriseID"></param>
        /// <returns></returns>
        TB_Enterprise GetEffectiveEtpByID(int enterpriseID);
        Task<TB_Enterprise> GetEffectiveEtpByIDAsync(int enterpriseID);

        CompanyInfoResult GetCompanyInfo(int enterpriseID);

        /// <summary>
        /// 判断正在更新或创建的企业是否已有重名
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsNameExist(int enterpriseId, string name);
        bool IsNameExist(string name);
        /// <summary>
        /// 获取企业简介
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        string GetCompanyIntro(int enterpriseId);

        string GetCompanyName(int enterpriseId);
        string GetCompanyShotDesc(int enterpriseId);

        IList<CompanyPreviewResult> GetCompanyPreviewList(int[] enterpriseids);

        CompanyPreviewResult GetCompanyPreview(int enterpriseid);

        /// <summary>
        /// 按首字母获取有效企业的Name和ID
        /// </summary>
        /// <param name="nameInitial">首字母</param>
        /// <param name="page">页数</param>
        /// <param name="count">每页显示数量</param>
        /// <returns></returns>
        IList<CompanyNameAndIdResult> GetEnterpriseNameAndIdBy(string nameInitial, int page, int count);

        IList<CompanyNameAndIdResult> GetEnterpriseNameAndIdBy(int page, int count);

        int GetEnterpriseCountBy(string nameInitial);

        int GetEnterpriseCountBy();

        string GetEnterpriseTags(int enterpriseId);

        /// <summary>
        /// 查询企业处理状态
        /// </summary>
        int? GetEtpProcessStatus(int enterpriseId);


        /// <summary>
        /// 获取企业账号审核未通过的原因
        /// </summary>
        string GetRejectReason(int enterpriseId);

        /// <summary>
        /// 20150817 sven 找到时间较长的sql，因为用缓存，故暂时不动
        /// 推荐企业
        /// 城市 和 行业 作为 了 优先条件 ， 非必要
        /// 返回三元组 三元组 Item1表示Id，Item2表示Name（or Abbr），Item3表示在招职位个数
        /// 参数分别 是 当前企业Id，当前企业的City字段 当前企业Industry字段
        /// </summary>
        IList<Tuple<int, string, int>> GetRecommendEtps(int enterpriseID, string city, string industry, int count = 5);

        /// <summary>
        /// 获取装饰完整度
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <returns></returns>
        int GetEnterpriseDecoratePrecent(int enterpriseid);

        void UpdateDecoratePercent(int enterpriseId);

        IList<TB_Enterprise> GetSameNameList(int etpId, string name);


        /// <summary>
        /// 获取所有有效的企业列表
        /// CustomerLevel!=99 and Status not in(31,99)
        /// </summary>
        /// <returns></returns>
        IList<int> GetEffectionEnterpriseId();

        EtpAccountInfoResult GetAccountInfo(int id);

        Task<TB_Enterprise> GetByIdAsync(int id);

        Task<CompanyDTO> GetAsync(int id);

        CompanyDTO Get(int id);

        Task<Dictionary<string, bool>> GetSummaryAsync(int id);

        Task<string> GetCompanyWebSiteAsync(int id);

        /// <summary>
        /// 获取企业认证状态
        /// </summary>
        /// <returns></returns>
        int GetEtpCertificationStatus(int id);

        TB_Enterprise GetEnterpriseByAccountId(int accountId);

        IList<Tuple<int, DateTime?>> GetEtpIdLastLoginAtTupleList();

        #region Dapper

        Task<CompanyInfoDTO> GetCompanyInfoDTO(int companyId, int userId = -1);
        Task<string> GetCompanyDescText(int companyId);

        PagedResult<string> GetDuplicatedNames(string etpName, int page, int pageSize);
        IList<DuplicatedEtpInfoDTO> GetDuplicatedEtpListByName(string name);
        #endregion
    }


    class CompanyIndexRecommandEtp 
    {
        public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
        public string City { get; set; }
        public string Industry { get; set; }
        public int PosCount { get;set; }
    }

}
