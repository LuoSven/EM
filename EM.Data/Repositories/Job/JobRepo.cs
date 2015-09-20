using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Data.SearchModels;
using Topuc.Framework.Cache;
using Topuc.Framework.Logger;
using Topuc22Top.Model.DTOs;
using Dapper;
using System.Diagnostics;
using Topuc22Top.Data.Dapper;

namespace Topuc22Top.Data.Repositories
{

    public class JobRepo : RepositoryBase<TB_Position_Element>, IJobRepo
    {
        private readonly ICache cache;
#if Debug
        private readonly int  cacheMinutes=1;
#else
        private readonly int cacheMinutes = 60;
#endif
        public JobRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public int GetPositionType(int posId) 
        {
            return (from entity in DataContext.TB_Position_Element
                    where entity.PositionId == posId
                    select entity.PositionType).FirstOrDefault();
        }

        public V_PositionDetails GetPositionDetail(int positionId)
        {

            var query = from p in DataContext.V_PositionDetails
                        where p.PositionId == positionId
                        select p;
            return query.FirstOrDefault();
        }

        public async Task<V_PositionDetails> GetPositionDetailAsync(int positionId)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                var query = from p in DataContext.V_PositionDetails
                            where p.PositionId == positionId
                            select p;
                return await query.FirstOrDefaultAsync();
            }
        }

        public IList<TB_Position_Element> GetActivePosListByEtpId(int enterpriseId, int count)
        {
            return DataContext.TB_Position_Element.Where(m => m.EnterpriseId == enterpriseId && m.PositionStatus == 1 && m.Deadline >= DateTime.Today)
                .OrderByDescending(x => x.DeployTime).ThenByDescending(x => x.PositionId).Take(count).ToList();
        }

        public async Task<IList<ActivePositionModel>> GetActivePosListByEtpIdAsync(int enterpriseId, int count)
        {
            using (TopucDB db = new TopucDB())
            {
                var query = from m in db.TB_Position_Element
                            where m.EnterpriseId == enterpriseId && m.PositionStatus == (int)PositionStatus.Publish && m.Deadline >= DateTime.Today
                            select new ActivePositionModel()
                            {
                                PositionId = m.PositionId,
                                Position = m.Position,
                                EnterpriseId = m.EnterpriseId,
                                PositionType = m.PositionType,
                                CityId = m.CityId,
                                RecruitCount = m.RecruitCount,
                                SalaryMin = m.SalaryMin,
                                SalaryMax = m.SalaryMax,
                                DeployTime = m.DeployTime,
                                DegreeIds = m.DegreeIds,
                                InternSalaryType = m.InternSalaryType
                            };
                return await query.OrderByDescending(x => x.DeployTime).ThenByDescending(x => x.PositionId).Take(count).ToListAsync();
            }
        }


        public PagedResult<TB_Position_Element> GetActivePosListByEtpId(int enterpriseId, string postype, string cityid, string funcid, string degreeid, string dtime, int page, int pageSize)
        {
            return GetActivePosListByEtpId(enterpriseId, null, postype, cityid, funcid, degreeid, dtime, page, pageSize);
        }

        public PagedResult<TB_Position_Element> GetActivePosListByEtpId(int enterpriseId, string posname, string postype, string cityid, string funcid, string degreeid, string dtime, int page, int pageSize)
        {
            PagedResult<TB_Position_Element> result = new PagedResult<TB_Position_Element>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            var query = from m in DataContext.TB_Position_Element
                        where m.EnterpriseId == enterpriseId && m.PositionStatus == 1 && m.Deadline >= DateTime.Today
                        select m;

            if (!string.IsNullOrEmpty(posname))
            {
                query = query.Where(a => a.Position.Contains(posname));
            }

            if (!string.IsNullOrEmpty(postype))
            {
                if (postype == "1")
                {
                    query = query.Where(a => a.PositionType == 1);

                }
                else
                {
                    query = query.Where(a => a.PositionType != 1);

                }

            }

            if (!string.IsNullOrEmpty(cityid))
            {
                int cityint = int.Parse(cityid);
                query = query.Where(a => a.CityId == cityint);

            }

            if (!string.IsNullOrEmpty(funcid))
            {
                query = query.Where(a => ("," + a.FunctionIds + ",").Contains("," + funcid + ","));

            }

            if (!string.IsNullOrEmpty(degreeid) && degreeid != "0")
            {
                query = query.Where(a => (a.DegreeIds ?? "").Contains(degreeid));
            }

            if (!string.IsNullOrEmpty(dtime))
            {
                if (dtime == "1")
                {
                    DateTime dt = DateTime.Now.AddDays(-7);
                    query = query.Where(a => a.DeployTime >= dt);
                }

                if (dtime == "2")
                {
                    DateTime dt = DateTime.Now.AddDays(-15);
                    query = query.Where(a => a.DeployTime >= dt);
                }

                if (dtime == "3")
                {
                    DateTime dt = DateTime.Now.AddMonths(-1);
                    query = query.Where(a => a.DeployTime >= dt);
                }


            }



            result.RowCount = query.Count();
            var rquery = query.OrderByDescending(x => x.DeployTime).Skip(pageSize * (page - 1)).Take(pageSize);
            result.Results = rquery.ToList();
            return result;
        }



        public PagedResult<ActivePositionModel> GetActivePosListByEtpId(int enterpriseId, int page, int pageSize)
        {
            PagedResult<ActivePositionModel> result = new PagedResult<ActivePositionModel>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            var query = from m in DataContext.TB_Position_Element
                        join a in DataContext.TB_S_Position on m.PositionId equals a.PositionId into ag
                        where m.EnterpriseId == enterpriseId && m.PositionStatus == 1 && m.Deadline >= DateTime.Today
                        select new ActivePositionModel
                        {
                            PositionId = m.PositionId,
                            Position = m.Position,
                            PositionType = m.PositionType,
                            CityId = m.CityId,
                            RecruitCount = m.RecruitCount,
                            SalaryMin = m.SalaryMin,
                            SalaryMax = m.SalaryMax,
                            DeployTime = m.DeployTime,
                            DegreeIds = m.DegreeIds,
                            InternSalaryType = m.InternSalaryType,
                            ApplyCnt = ag.Count()

                        };
            result.RowCount = query.Count();
            result.Results = query.OrderByDescending(x => x.DeployTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return result;
        }

        public IList<ActivePositionModel> GetJobsByEtpId(int enterpriseId)
        {
            var query = from m in DataContext.TB_Position_Element
                        join a in DataContext.TB_S_Position on m.PositionId equals a.PositionId into ag
                        where m.EnterpriseId == enterpriseId && m.PositionStatus == 1 && m.Deadline >= DateTime.Today
                        select new ActivePositionModel
                        {
                            PositionId = m.PositionId,
                            Position = m.Position,
                            PositionType = m.PositionType,
                            CityId = m.CityId,
                            RecruitCount = m.RecruitCount,
                            SalaryMin = m.SalaryMin,
                            SalaryMax = m.SalaryMax,
                            DeployTime = m.DeployTime,
                            ApplyCnt = ag.Count()

                        };

            return query.ToList();
        }

        public PositionGroup GetActivePosGroupByEtpId(int enterpriseId)
        {
            return GetActivePosGroupByEtpId(enterpriseId, null);
        }


        public PositionGroup GetActivePosGroupByEtpId(int enterpriseId, string posname)
        {
            var dicts = (from d in DataContext.DictItem
                         where d.Type == "City" || d.Type == "Function" || d.Type == "Degree"
                         select d).ToList();

            var query = from p in DataContext.TB_Position_Element
                        where p.EnterpriseId == enterpriseId && p.PositionStatus == 1 && p.Deadline >= DateTime.Today
                        select new
                        {
                            p.Position,
                            p.PositionType,
                            p.CityId,
                            p.FunctionIds,
                            p.DegreeIds,
                            p.DeployTime
                        };
            if (!string.IsNullOrEmpty(posname))
            {
                query = query.Where(a => a.Position.Contains(posname));
            }


            var list = query.ToList();
            PositionGroup pg = new PositionGroup();
            var positiontypeg = string.Join(",", list.Select(a => a.PositionType)).Trim(',').Split(',').Distinct();
            foreach (var p in positiontypeg)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    int typeid = int.Parse(p);
                    if (typeid == 1)
                    {
                        pg.PositionType.Add(1, "全职");
                    }
                    else if ((typeid != 1) && !pg.PositionType.ContainsKey(2))
                    {
                        pg.PositionType.Add(2, "实习");
                    }
                }
            }

            var cityg = list.Select(a => a.CityId).Distinct();
            foreach (var cityid in cityg)
            {
                if (!pg.City.ContainsKey(cityid))
                {
                    pg.City.Add(cityid, dicts.Where(a => a.Type == "City" && a.ItemId == cityid).Select(a => a.ItemName).FirstOrDefault() ?? "");
                }
            }

            var funcg = string.Join(",", list.Select(a => a.FunctionIds)).Trim(',').Split(',').Distinct();

            foreach (var f in funcg)
            {
                int funcid = 0;
                int.TryParse(f, out funcid);

                if (funcid != 0)
                {
                    if (!pg.Func.ContainsKey(funcid))
                    {
                        pg.Func.Add(funcid, dicts.Where(a => a.Type == "Function" && a.ItemId == funcid).Select(a => a.ItemName).FirstOrDefault() ?? "");
                    }
                }
            }

            var degreeg = new List<int>() { };
            list.Select(p => p.DegreeIds).ToList().ForEach(p =>
            {
                foreach (var item in (p??"").Split(','))
                {
                    int i = 0;
                    int.TryParse(item, out i);
                    if (i > 0) 
                    {
                        degreeg.Add(i);
                    }
                }
            });
            degreeg = degreeg.Distinct().ToList();

            foreach (var degreeid in degreeg)
            {
                if (!pg.Degree.ContainsKey(degreeid))
                {
                    string value = dicts.Where(a => a.Type == "Degree" && a.ItemId == degreeid).Select(a => a.ItemName).FirstOrDefault() ?? "";
                    if (!string.IsNullOrEmpty(value))
                    {
                        pg.Degree.Add(degreeid, value);
                    }
                }
            }

            if (list.Where(a => a.DeployTime >= DateTime.Now.AddDays(-7)).Any())
            {
                pg.PublishTime.Add(1, "一周内");
            };

            if (list.Where(a => a.DeployTime >= DateTime.Now.AddDays(-15)).Any())
            {
                pg.PublishTime.Add(2, "半月内");

            };

            if (list.Where(a => a.DeployTime >= DateTime.Now.AddMonths(-1)).Any())
            {
                pg.PublishTime.Add(3, "一月内");

            };

            return pg;
        }



        public int GetActivePosCountByEtpId(int enterpriseId)
        {
            var list = from a in DataContext.TB_Position_Element
                       where a.EnterpriseId == enterpriseId && a.Deadline >= DateTime.Today && a.PositionStatus == (int)PositionStatus.Publish
                       select a;
            return list.Count();
        }


        public IList<JobSimpleResult> GetOtherPositions(int enterpriseid, int positionid, out int totalPosCnt)
        {

            var list = from a in DataContext.TB_Position_Element
                       where a.EnterpriseId == enterpriseid
                       && a.Deadline >= DateTime.Today
                       && a.PositionStatus == (int)PositionStatus.Publish
                       && a.PositionId != positionid
                       select new JobSimpleResult()
                       {
                           CityId = a.CityId,
                           CreateDate = a.CreateDate,
                           EtpId = a.EnterpriseId,
                           InternSalaryType = a.InternSalaryType,
                           Position = a.Position,
                           PositionId = a.PositionId,
                           PositionType = a.PositionType,
                           SalaryMax = a.SalaryMax,
                           SalaryMin = a.SalaryMin,
                           DeployTime = a.DeployTime
                       };
            totalPosCnt = list.Count() + 1;
            return list.OrderByDescending(a => a.DeployTime).Take(8).ToList();
        }

        public IList<JobSimpleResult> GetOtherPositions(int enterpriseid, int positionid)
        {

            var list = from a in DataContext.TB_Position_Element
                       where a.EnterpriseId == enterpriseid
                       && a.Deadline >= DateTime.Today
                       && a.PositionStatus == (int)PositionStatus.Publish
                       && a.PositionId != positionid
                       select new JobSimpleResult()
                       {
                           CityId = a.CityId,
                           CreateDate = a.CreateDate,
                           EtpId = a.EnterpriseId,
                           InternSalaryType = a.InternSalaryType,
                           Position = a.Position,
                           PositionId = a.PositionId,
                           PositionType = a.PositionType,
                           SalaryMax = a.SalaryMax,
                           SalaryMin = a.SalaryMin,
                           DeployTime = a.DeployTime
                       };
            return list.OrderByDescending(a => a.DeployTime).Take(8).ToList();
        }

        public async Task<IList<JobSimpleResult>> GetOtherPositionsForJobDetailAsync(int enterpriseId, int positionId, int cityId)
        {
            return await cache.Get(string.Format(Settings.ParaKey_OtherPositionsForJobDetailAsync, enterpriseId, positionId, cityId), async () =>
            {
                using (TopucDB DataContext = new TopucDB())
                {
                    var list = from a in DataContext.TB_Position_Element
                               where a.EnterpriseId == enterpriseId
                               && a.Deadline >= DateTime.Today
                               && a.PositionStatus == (int)PositionStatus.Publish
                               && a.PositionId != positionId
                               select new JobSimpleResult()
                               {
                                   CityId = a.CityId,
                                   CreateDate = a.CreateDate,
                                   EtpId = a.EnterpriseId,
                                   InternSalaryType = a.InternSalaryType,
                                   Position = a.Position,
                                   PositionId = a.PositionId,
                                   PositionType = a.PositionType,
                                   SalaryMax = a.SalaryMax,
                                   SalaryMin = a.SalaryMin,
                                   DeployTime = a.DeployTime
                               };

                    var sameCityList = list.Where(m => m.CityId == cityId);
                    if (await sameCityList.CountAsync() >= 6)
                    {
                        return await sameCityList.OrderByDescending(a => a.DeployTime).Take(6).ToListAsync();
                    }
                    else
                    {
                        return await list.OrderByDescending(a => a.DeployTime).Take(6).ToListAsync();
                    }
                }
            }, cacheMinutes);
        }


        public TB_Position_Element GetNextPosition(int posid, string func, int city)
        {

            var pos = (from p in DataContext.TB_Position_Element
                       where ("," + p.FunctionIds + ",").Contains("," + func + ",")
                       && p.CityId == city && p.Deadline > DateTime.Now && p.PositionStatus == 1 && p.PositionId < posid
                       orderby p.PositionId descending
                       select p).FirstOrDefault();
            return pos;
        }

        public bool ExistPositionToMigrate(int enterpriseId)
        {
            var query = from m in DataContext.TB_Position_Element
                        where m.EnterpriseId == enterpriseId && m.PositionStatus == (int)PositionStatus.ToMigrate
                        select m;
            return query.Any();
        }

        public IList<TB_Position_Element> GetPositionListToMigrate(int enterpriseId)
        {
            var query = from m in DataContext.TB_Position_Element
                        where m.EnterpriseId == enterpriseId && m.PositionStatus == (int)PositionStatus.ToMigrate
                        select m;
            return query.ToList();
        }

        public IList<TB_Position_Element> GetPositionList(int[] positionIdList)
        {
            return DataContext.TB_Position_Element.Where(x => positionIdList.Contains(x.PositionId)).ToList();
        }
        public IList<TB_Position_Element> GetPositionList(int enterpriseId)
        {
            return DataContext.TB_Position_Element.Where(x => x.EnterpriseId == enterpriseId).ToList();
        }
        public PagedResult<PositionResult> GetPositionList(PositionSearchModel sm, int page, int pageSize)
        {
            PagedResult<PositionResult> result = new PagedResult<PositionResult>();
            result.CurrentPage = page;
            result.PageSize = pageSize;

            var query = from m in DataContext.TB_Position_Element
                        where m.EnterpriseId == sm.enterpriseId
                        select m;

            if (sm.currentStatus == (int)PositionStatus.Publish)
            {
                query = query.Where(a => a.Deadline >= DateTime.Today && a.PositionStatus == (int)PositionStatus.Publish);
            }
            else if (sm.currentStatus == (int)PositionStatus.WaitingApprove)
            {
                query = query.Where(a => a.PositionStatus == (int)PositionStatus.WaitingApprove);
            }
            else if (sm.currentStatus == (int)PositionStatus.Expired)
            {
                query = query.Where(a => a.Deadline < DateTime.Today || a.PositionStatus == (int)PositionStatus.Expired);
            }
            else if (sm.currentStatus == (int)PositionStatus.Paused)
            {
                query = query.Where(x => x.PositionStatus == (int)PositionStatus.Paused);
            }

            if (!string.IsNullOrEmpty(sm.posName))
            {
                query = query.Where(x => x.Position.Contains(sm.posName));
            }
            if (!string.IsNullOrEmpty(sm.posType))
            {
                if (sm.posType == ((int)PositionType.FullTime).ToString())
                {
                    query = query.Where(x => x.PositionType == (int)PositionType.FullTime);
                }
                else
                {
                    query = query.Where(x => x.PositionType != (int)PositionType.FullTime);
                }
            }
            if (!string.IsNullOrEmpty(sm.func))
            {
                query = query.Where(x => ("," + x.FunctionIds + ",").Contains("," + sm.func + ","));
            }
            if (!string.IsNullOrEmpty(sm.city))
            {
                int id = Int32.Parse(sm.city);
                query = query.Where(x => x.CityId == id);
            }

            var groupApply = from a in DataContext.TB_S_Position
                             where a.DeleteDate == null
                             group a by a.PositionId into g
                             select new GroupResult
                             {
                                 Key = g.Key,
                                 ApplyCount = g.Count(),
                                 UnreadApplyCount = g.Where(a => a.ApplyStatus == (int)ApplyStatus.Apply || a.ApplyStatus == (int)ApplyStatus.Read).Count(),
                                 PushCount = 0
                             };

            var groupPush = from p in DataContext.Job_Push
                            group p by p.JobId into g
                            select new GroupResult
                            {
                                Key = g.Key,
                                ApplyCount = 0,
                                UnreadApplyCount = 0,
                                PushCount = g.Count()
                            };


            var resultquery = from m in query
                              join n in groupApply on m.PositionId equals n.Key into jn
                              from nn in jn.DefaultIfEmpty()
                              join p in groupPush on m.PositionId equals p.Key into jp
                              from pp in jp.DefaultIfEmpty()
                              select new PositionResult()
                              {
                                  PositionId = m.PositionId,
                                  Position = m.Position,
                                  PositionStatus = m.PositionStatus,
                                  Deadline = m.Deadline,
                                  DeployTime = m.DeployTime,
                                  RecruitCount = m.RecruitCount,
                                  CityId = m.CityId,
                                  FunctionIds = m.FunctionIds,

                                  SalaryMin = m.SalaryMin,
                                  SalaryMax = m.SalaryMax,
                                  InternSalaryType = m.InternSalaryType,
                                  DegreeIds = m.DegreeIds,
                                  PositionType = m.PositionType,
                                  PushDate = m.PushDate,
                                  AppliedCount = nn.ApplyCount ?? 0,
                                  UnreadApplyCount = nn.UnreadApplyCount ?? 0,
                                  PushCount = pp.PushCount ?? 0,
                              };

            result.RowCount = resultquery.Count();
            result.Results = resultquery.OrderByDescending(f => f.DeployTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return result;
        }
        public IList<PositionResult> GetPositionList(string positionIdStr)
        {
            var intIds = ArrayConvertor.Convert(positionIdStr);
            if (intIds.Length > 0)
            {
                var query = from m in DataContext.TB_Position_Element
                            join n in DataContext.TB_S_Position
                            on m.PositionId equals n.PositionId into g
                            where intIds.Contains(m.PositionId)
                            select new PositionResult()
                            {
                                PositionId = m.PositionId,
                                Position = m.Position,
                                PositionStatus = m.PositionStatus,
                                Deadline = m.Deadline,
                                DeployTime = m.DeployTime,
                                RecruitCount = m.RecruitCount,
                                CityId = m.CityId,
                                FunctionIds = m.FunctionIds,
                                AppliedCount = g.Count(),
                                //--------
                                SalaryMin = m.SalaryMin,
                                SalaryMax = m.SalaryMax,
                                DegreeIds = m.DegreeIds,
                                PositionType = m.PositionType,
                                PushDate = m.PushDate,
                                UnreadApplyCount = g.Where(x => x.ApplyStatus == (int)ApplyStatus.Apply || x.ApplyStatus == (int)ApplyStatus.Read).Count(),
                            };
                return query.ToList();
            }
            return new List<PositionResult>();
        }
        public Dictionary<int, string> GetPositionCitys(int enterpriseId, int? positionStatus = null)
        {
            var dict = new Dictionary<int, string>();
            var query = from m in DataContext.TB_Position_Element
                        join n in DataContext.DictItem
                        on m.CityId equals n.ItemId
                        where m.EnterpriseId == enterpriseId && n.Type == "City"
                        select new
                        {
                            CityId = m.CityId,
                            CityName = n.ItemName,
                            PositionStatus = m.PositionStatus,
                            Deadline = m.Deadline
                        };
            if (positionStatus.HasValue)
            {
                DateTime today = DateTime.Now.Date;
                if (positionStatus.Value == (int)PositionStatus.Expired)
                {
                    query = query.Where(a => a.Deadline < today);
                }
                else
                {
                    query = query.Where(a => a.PositionStatus == positionStatus.Value && a.Deadline >= today);
                }

            }

            foreach (var m in query.ToList())
            {
                if (!dict.Keys.Contains(m.CityId))
                {
                    dict.Add(m.CityId, m.CityName);
                }
            }
            return dict;
        }
        public Dictionary<int, string> GetPositionKeyValuePairs(int enterpriseId, ApplicationSearchModel sm)
        {
            var query = from f in DataContext.TB_Position_Element
                        join c in DataContext.DictItem.Where(c => c.Type == "City")
                        on f.CityId equals c.ItemId
                        where f.EnterpriseId == enterpriseId
                        && f.PositionStatus != (int)PositionStatus.Deleted
                        select new
                        {
                            PositionId = f.PositionId,
                            Position = f.Position + " (" + c.ItemName + ")"
                        };
            return query.ToDictionary(x => x.PositionId, x => x.Position);
        }
        public IList<PositionAndApplyStatus> GetPositionAndApplyStatusList(int enterpriseId)
        {
            var query = from position in DataContext.TB_Position_Element
                        //join m in DataContext.TB_S_Position.Where(p => p.DeleteDate == null)
                        //on position.PositionId equals m.PositionId into gApply
                        join city in DataContext.DictItem.Where(p => p.Type == "City")
                        on position.CityId equals city.ItemId
                        where position.EnterpriseId == enterpriseId
                        && position.PositionStatus != (int)PositionStatus.Deleted
                        select new PositionAndApplyStatus()
                        {
                            PositionId = position.PositionId,
                            PositionName = position.Position,
                            PositionStatus = position.PositionStatus,
                            ExpiredDate = position.Deadline,
                            CityId = position.CityId,
                            CityName = city.ItemName,
                            SalaryMin = position.SalaryMin,
                            SalaryMax = position.SalaryMax,
                            InternSalaryType = position.InternSalaryType,
                            PositionType = position.PositionType,
                            RecruitCount = position.RecruitCount,
                            DegreeIds = position.DegreeIds
                        };
            return query.ToList();

        }
        public Dictionary<int, string> GetPositionFuncs(int enterpriseId, int? positionStatus = null)
        {
            var query = from m in DataContext.TB_Position_Element
                        where m.EnterpriseId == enterpriseId
                        select m;

            if (positionStatus.HasValue)
            {
                DateTime today = DateTime.Now.Date;
                if (positionStatus.Value == (int)PositionStatus.Expired)
                {
                    query = query.Where(a => a.Deadline < today);
                }
                else
                {
                    query = query.Where(a => a.PositionStatus == positionStatus.Value && a.Deadline >= today);
                }
            }

            var funcs = query.Select(a => a.FunctionIds).Distinct().ToList();

            string[] funstrArray = string.Join(",", funcs).Split(',').Distinct().ToArray();

            if (funstrArray.Length == 0)
            {
                return new Dictionary<int, string>();
            }

            int[] funcintArray = Array.ConvertAll<string, int>(funstrArray, delegate(string s) { int resultint = 0; int.TryParse(s, out  resultint); return resultint; });

            var result = from m in funcintArray
                         join n in DataContext.DictItem
                         on m equals n.ItemId
                         where n.Type == "Function"
                         select new
                         {
                             FunctionId = m,
                             FunctionName = n.ItemName,
                         };

            return result.ToDictionary(x => x.FunctionId, x => x.FunctionName);
        }
        public Dictionary<string, string> GetPositionNames(int enterpriseId, int? positionStatus = null)
        {
            var query = from m in DataContext.TB_Position_Element
                        where m.EnterpriseId == enterpriseId
                        select new
                        {
                            Position = m.Position,
                            PositionStatus = m.PositionStatus,
                            Deadline = m.Deadline
                        };
            if (positionStatus.HasValue)
            {
                DateTime today = DateTime.Now.Date;
                if (positionStatus.Value == (int)PositionStatus.Expired)
                {
                    query = query.Where(x => x.Deadline < today);
                }
                else
                {
                    query = query.Where(x => x.PositionStatus == positionStatus.Value && x.Deadline >= today);
                }
            }
            return query.Select(x => new { name = x.Position }).Distinct().ToDictionary(x => x.name, x => x.name);
        }
        public string GetPositionName(int positionId)
        {
            var query = from m in DataContext.TB_Position_Element
                        where m.PositionId == positionId
                        select m.Position;
            return query.FirstOrDefault();
        }
        public int GetNewPublishGroup(int enterpriseId)
        {
            var query = from p in DataContext.TB_Position_Element
                        where p.EnterpriseId == enterpriseId
                        select p;

            return -1 * (enterpriseId * 10000 + query.Count());
        }

        public V_PositionDetails GetUserLastAppliedJob(int userId)
        {
            var query = from a in DataContext.TB_S_Position
                        where a.UserId == userId
                        select a.PositionId;
            if (query.Any())
            {
                int posId = query.FirstOrDefault();
                return DataContext.V_PositionDetails.Where(v => v.PositionId == posId).FirstOrDefault();
            }
            return null;
        }


        #region 异步方法
        public async Task<IList<TB_Position_Element>> GetEtpActiveJobsAsync(int enterpriseId, int? count = null)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from e in context.TB_Position_Element
                            where e.EnterpriseId == enterpriseId && e.PositionStatus == 1 && e.Deadline >= DateTime.Today
                            select e;
                if (count.HasValue)
                {

                    return await query.OrderByDescending(x => x.DeployTime).ThenByDescending(x => x.PositionId).Take(count.Value).ToListAsync();
                }
                else
                {
                    return await query.OrderByDescending(x => x.DeployTime).ThenByDescending(x => x.PositionId).ToListAsync();
                }
            }
        }

        public async Task<int> GetEtpActiveJobsCountAsync(int enterpriseId)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from e in context.TB_Position_Element
                            where e.EnterpriseId == enterpriseId && e.PositionStatus == 1 && e.Deadline >= DateTime.Today
                            select e;
                return await query.CountAsync();
            }
        }

        #endregion

        /// <summary>
        /// 获取所有有效的职位的ID
        /// </summary>
        /// <returns></returns>
        public IList<int> GetEffectionJobId()
        {
            DateTime now = DateTime.Now.Date;
            var query = from m in this.DataContext.TB_Position_Element
                        where m.PositionStatus == 1 && m.Deadline >= now
                        select m.PositionId;

            return query.ToList<int>();
        }

        /// <summary>
        /// 此处为定时服务调用，需要使用 new TopucDB()
        /// </summary>
        /// <param name="isActiveEtpPositon"></param>
        /// <returns></returns>
        public async Task<IList<int>> GetPositionIdsAsync(bool? isActiveEtpPositon)
        {
            using(TopucDB context = new TopucDB())
            {
                DateTime now = DateTime.Now.Date;
                var query = from m in context.TB_Position_Element
                            where m.PositionStatus == (int)PositionStatus.Publish && m.Deadline >= now
                            select m;
                if (isActiveEtpPositon.HasValue)
                {
                    var activeEtpIds = context.TB_Enterprise.Where(e => e.ProcessStatus == (int)EtpProcessStatus.AccountApproved).Select(e => e.EnterpriseId);
                    if (isActiveEtpPositon.Value)
                    {
                        query = query.Where(m => activeEtpIds.Contains(m.EnterpriseId));
                    }
                    else
                    {
                        DateTime startDate = DateTime.Now.Date.AddMonths(-3); //对于非激活企业只返回近三个月的有效职位ID
                        query = query.Where(m => m.CreateDate > startDate && !activeEtpIds.Contains(m.EnterpriseId));
                    }

                }
                return await query.Select(m => m.PositionId).ToListAsync();
            }
            
        }



        public async Task<Dictionary<string, int>> GetStatSummaryAsync()
        {
            return cache.Get(Settings.ParaKey_StatSummary, () =>
            {
                using (TopucDB context = new TopucDB())
                {
                    Dictionary<string, int> summary = new Dictionary<string, int>();
                    var exceptetp = from e in context.TB_Enterprise
                                    where e.Status == (int)EtpStatus.Shadow || e.Status == (int)EtpStatus.Invalid
                                    select e.EnterpriseId;

                    var poslist = from p in context.TB_Position_Element
                                  where p.PositionStatus == 1 && p.Deadline >= DateTime.Now
                                  && !exceptetp.Contains(p.EnterpriseId)
                                  select p;

                    int poscount = poslist.CountAsync().Result;
                    int etpcount = (from e in poslist
                                    select e.EnterpriseId).Distinct().CountAsync().Result;


                    int stuCnt = DataContext.TB_S_Account.CountAsync().Result;

                    summary.Add("posCnt", poscount);
                    summary.Add("etpCnt", etpcount);
                    summary.Add("stuCnt", stuCnt);
                    return summary;
                }
            }, cacheMinutes);

        }

        public async Task<bool> IsJobApplied(int userId, int posId)
        {
            return DataContext.TB_S_Position.Where(s => s.UserId == userId
                   && s.PositionId == posId).Any();
        }

        #region Dapper
        /// <summary>
        /// 职位详情页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JobDetailsDTO> FindJobDetailAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"
select a.PositionId, a.Position PositionName, a.PositionType, a.CityId, a.DegreeIds
,a.RecruitCount,a.DeployTime,b.Name EnterpriseName, c.PosDescription, a.EnterpriseId EnterpriseId
,a.SalaryMin, a.SalaryMax, a.Tags, a.Additional, b.Industry, b.Scale, a.MajorIds, a.SchoolLevelIds,
a.SchoolCityIds, a.InternSalaryType
from TB_Position_Element a
left join TB_Enterprise b
on a.EnterpriseId = b.EnterpriseId
left join TB_Position_BigTxt c
on a.PositionId = c.PositionId
where a.PositionId = @id
and isnull(b.EnterpriseId,0) > 0
and isnull(c.PositionId,0) > 0
and PositionStatus <> 99";
                var model = await conn.QueryAsync<JobDetailsDTO>(sql, new { @id = id });
                return model.FirstOrDefault();
            }
        }

        public async Task<IList<EtpPositionInfoDTO>> GetEtpPositionDynamicList(string companyIds, int days) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var startDate = DateTime.Now.AddDays((days > 0 ? -days : days)).Date;
                string sql = string.Format(@"
select a.EnterpriseId, b.Name EnterpriseName, a.Position PositionName, a.PositionId, a.CityId,a.SalaryMin,a.SalaryMax,a.InternSalaryType
,a.DeployTime
from TB_Position_Element a
left join TB_Enterprise b
on a.EnterpriseId = b.EnterpriseId
where a.EnterpriseId in ({0}) and a.CreateDate >= @startDate
and a.PositionStatus = 1
and b.Status <> 99
and a.Deadline > getdate()
", companyIds);
                var list = await conn.QueryAsync<EtpPositionInfoDTO>(sql, new { @startDate = startDate });
                return list.ToList();
            }
        }


        public int[] GetSamePublishGroupPosIds(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select PositionId from TB_Position_Element
where PublishGroup = (select PublishGroup from TB_Position_Element where PositionId = {0})
", id);
                var list = conn.Query<int>(sql);
                return list.ToArray();
            }
        }

        public async Task<PagedResult<EtpPositionInfoDTO>> GetEtpActivePositionList(int companyId, int page, int pageSize)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select a.EnterpriseId, b.Name EnterpriseName, a.Position PositionName, a.PositionId, a.CityId,a.SalaryMin,a.SalaryMax,a.InternSalaryType
,a.DeployTime
from TB_Position_Element a
left join TB_Enterprise b
on a.EnterpriseId = b.EnterpriseId
where a.EnterpriseId = {0}
and a.PositionStatus = 1
and b.Status <> 99
and a.Deadline > getdate()
", companyId);
                var result = await conn.QueryWithPageAsync<EtpPositionInfoDTO>(sql, null, "DeployTime desc", page, pageSize);
                return result;
            }
        }

        #endregion


    }
    public interface IJobRepo : IRepository<TB_Position_Element>
    {

        int GetPositionType(int posId);

        /// <summary>
        /// 获取指定企业的所有有效职位的分页列表
        /// </summary>
        PagedResult<TB_Position_Element> GetActivePosListByEtpId(int enterpriseId, string postype, string cityid, string funcid, string degreeid, string dtime, int page, int pageSize);

        /// <summary>
        /// 获取指定企业的所有有效职位的分页列表
        /// </summary>
        PagedResult<TB_Position_Element> GetActivePosListByEtpId(int enterpriseId, string posname, string postype, string cityid, string funcid, string degreeid, string dtime, int page, int pageSize);


        /// <summary>
        /// 通过企业id获取存在的的属性用于筛选
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        PositionGroup GetActivePosGroupByEtpId(int enterpriseId);


        /// <summary>
        /// 通过企业id获取存在的的属性用于筛选
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        PositionGroup GetActivePosGroupByEtpId(int enterpriseId, string posname);

        /// <summary>
        /// 获取职位详情（数据来自视图）
        /// </summary>
        V_PositionDetails GetPositionDetail(int positionId);
        Task<V_PositionDetails> GetPositionDetailAsync(int positionId);
        /// <summary>
        /// 获取企业发布中的职位数
        /// </summary>
        int GetActivePosCountByEtpId(int enterpriseId);
        /// <summary>
        /// 获取企业发布中的所有职位
        /// </summary>
        IList<TB_Position_Element> GetActivePosListByEtpId(int enterpriseId, int count);
        Task<IList<ActivePositionModel>> GetActivePosListByEtpIdAsync(int enterpriseId, int count);

        /// <summary>
        /// 获取指定企业的所有有效职位的分页列表
        /// </summary>
        PagedResult<ActivePositionModel> GetActivePosListByEtpId(int enterpriseId, int page, int pageSize);
        /// <summary>
        /// 获取该企业其他职位
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="positionid"></param>
        /// <returns></returns>
        IList<JobSimpleResult> GetOtherPositions(int enterpriseid, int positionid, out int totalPosCnt);
        /// <summary>
        /// 获取该企业其他职位Job->Detail页面
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="positionid"></param>
        /// <returns></returns>
        IList<JobSimpleResult> GetOtherPositions(int enterpriseid, int positionid);
        /// <summary>
        /// 获取下一个推荐职位
        /// </summary>
        /// <param name="posid"></param>
        /// <param name="func"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        TB_Position_Element GetNextPosition(int posid, string func, int city);

        /// <summary>
        /// 异步获取公司指定个数的职位列表
        /// </summary>
        /// <param name="companyId">公司Id</param>
        /// <param name="count">职位列表数</param>
        Task<IList<TB_Position_Element>> GetEtpActiveJobsAsync(int enterpriseId, int? count = null);


        /// <summary>
        /// 异步获取公司指定个数的职位数目
        /// </summary>
        /// <param name="companyId">公司Id</param>
        /// <param name="count">职位列表数</param>
        Task<int> GetEtpActiveJobsCountAsync(int enterpriseId);

        //------数据同步发布 2014-7-17 Green
        bool ExistPositionToMigrate(int enterpriseId);
        /// <summary>
        /// 获取待迁移数据
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        IList<TB_Position_Element> GetPositionListToMigrate(int enterpriseId);
        IList<TB_Position_Element> GetPositionList(int[] positionIdList);
        IList<TB_Position_Element> GetPositionList(int enterpriseId);
        //------整改企业职位管理
        PagedResult<PositionResult> GetPositionList(PositionSearchModel sm, int page, int pageSize);
        IList<PositionResult> GetPositionList(string positionIdStr);
        Dictionary<int, string> GetPositionCitys(int enterpriseId, int? positionStatus = null);
        Dictionary<int, string> GetPositionFuncs(int enterpriseId, int? positionStatus = null);
        Dictionary<int, string> GetPositionKeyValuePairs(int enterpriseId, ApplicationSearchModel sm);
        IList<PositionAndApplyStatus> GetPositionAndApplyStatusList(int enterpriseId);
        Dictionary<string, string> GetPositionNames(int enterpriseId, int? positionStatus = null);
        //----------------------------------

        int GetNewPublishGroup(int enterpriseId);
        string GetPositionName(int positionId);

        IList<ActivePositionModel> GetJobsByEtpId(int enterpriseId);

        V_PositionDetails GetUserLastAppliedJob(int userId);

        /// <summary>
        /// 获取所有有效的职位的ID
        /// </summary>
        /// <returns></returns>
        IList<int> GetEffectionJobId();

        Task<IList<JobSimpleResult>> GetOtherPositionsForJobDetailAsync(int enterpriseId, int positionId, int cityId);

        Task<IList<int>> GetPositionIdsAsync(bool? isActiveEtpPositon);

        Task<Dictionary<string, int>> GetStatSummaryAsync();

        /// <summary>
        /// check 用户是否已申请过该职位，已申请过返回true
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="posId"></param>
        /// <returns></returns>
        Task<bool> IsJobApplied(int userId, int posId);

        #region Dapper
        /// <summary>
        /// 获取JobDetail （M端职位详情页）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<JobDetailsDTO> FindJobDetailAsync(int id);

        Task<IList<EtpPositionInfoDTO>> GetEtpPositionDynamicList(string companyIds, int days);

        int[] GetSamePublishGroupPosIds(int id);
        #endregion

        /// <summary>
        /// 在招职位
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedResult<EtpPositionInfoDTO>> GetEtpActivePositionList(int companyId, int page, int pageSize);

    }


    public class GroupResult
    {
        public int Key { get; set; }
        public int? ApplyCount { get; set; }

        public int? UnreadApplyCount { get; set; }

        public int? PushCount { get; set; }
    }

}
