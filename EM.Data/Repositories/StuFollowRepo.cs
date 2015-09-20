using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Logger;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Data.Dapper;

using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public class StuFollowRepo : RepositoryBase<Stu_Follow>, IStuFollowRepo
    {
        public StuFollowRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        /// <summary>
        /// 学生关注企业
        /// </summary>
        public OperationResult FollowEtp(int etpID, int stuID)
        {
            try
            {
                var query = from f in DataContext.Stu_Follow.Where(a => a.TargetType == "Enterprise")
                            where f.UserId == stuID && f.TargetId == etpID
                            select f;
                if (query.Any())
                {
                    return OperationResult.RepeatOperation;//重复操作
                }
                else
                {
                    Stu_Follow follow = new Stu_Follow();
                    follow.UserId = stuID;
                    follow.TargetId = etpID;
                    follow.TargetType = "Enterprise";
                    follow.FollowDate = DateTime.Now;
                    DataContext.Stu_Follow.Add(follow);

                }
                DataContext.SaveChanges();
                return OperationResult.Success;//操作成功
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message);
                return OperationResult.Fail;//操作失败
            }
        }


        /// <summary>
        /// 学生取消关注企业
        /// </summary>
        public int UnFollowEtp(int etpID, int stuID)
        {
            try
            {

                var query = from f in DataContext.Stu_Follow.Where(a => a.TargetType == "Enterprise")
                            where f.UserId == stuID && f.TargetId == etpID
                            select f;
                if (query.Any())
                {
                    Stu_Follow follow = query.FirstOrDefault();
                    DataContext.Stu_Follow.Remove(follow);
                    DataContext.SaveChanges();
                    return 1;
                }
                return 2;
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message);
                return 0;//重复失败
            }
        }


        /// <summary>
        /// 获取该学生关注的所有企业
        /// </summary>
        public PagedResult<EtpInFollow> GetStuFollowing(int stuID, int page, int pageSize)
        {
            var query = from f in DataContext.Stu_Follow
                        join e in DataContext.TB_Enterprise on f.TargetId equals e.EnterpriseId
                        join p in DataContext.TB_Position_Element.Where(a => a.Deadline > DateTime.Today && a.PositionStatus == (int)PositionStatus.Publish) on e.EnterpriseId equals p.EnterpriseId into pg
                        where f.TargetType == "Enterprise" && f.UserId == stuID
                        select new EtpInFollow
                        {
                            EtpID = e.EnterpriseId,
                            EtpName = string.IsNullOrEmpty(e.Abbr) ? e.Name : e.Abbr,
                            Industry = e.Industry,
                            PosCnt = pg.Count(),
                            FollowDate = f.FollowDate
                        };

            return new PagedResult<EtpInFollow>()
            {
                Results = query.OrderByDescending(f => f.FollowDate).Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

        }

        /// <summary>
        /// 获取该学生关注的所有企业的数目（学生关注）
        /// </summary>
        public int GetStuFollowingCount(int stuID)
        {

            var query = from f in DataContext.Stu_Follow.Where(a => a.TargetType == "Enterprise")
                        join e in DataContext.TB_Enterprise
                        on f.TargetId equals e.EnterpriseId
                        where f.UserId == stuID
                        select f;
            return query.Count();
        }



        public int GetEtpFollowersCount(int enterpriseId)
        {
            var query = from f in DataContext.Stu_Follow.Where(a => a.TargetType == "Enterprise")
                        where f.TargetId == enterpriseId
                        select f;
            return query.Count();
        }
        public bool IsEtpFollowed(int etpID, int stuID)
        {
            if (etpID <= 0 || stuID <= 0)
                return false;
            var stufollow = from s in DataContext.Stu_Follow.Where(a => a.TargetType == "Enterprise")
                            where s.UserId == stuID && s.TargetId == etpID
                            select s;
            if (stufollow.Any())
            {
                return true;
            }
            return false;
        }

        public PagedResult<EtpActivity> GetEtpActivities(int userId, int page, int pageSize)
        {
            var etpIds = DataContext.Stu_Follow.Where(f=>f.TargetType == "Enterprise" && f.UserId == userId).Select(f=>f.TargetId);

            var sdate = DateTime.Now.AddMonths(-3);
            var posQuery = from p in DataContext.TB_Position_Element
                           join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                           where p.PositionStatus == (int)PositionStatus.Publish && p.Deadline >= DateTime.Today
                           && etpIds.Contains(p.EnterpriseId) && p.CreateDate > sdate
                           select new EtpActivity()
                           {
                               EtpId = p.EnterpriseId,
                               EtpName = e.Name,
                               ActivityType = (int)EtpActivityType.Position,
                               Position = p,
                               Life = null,
                               CreateDate = p.CreateDate
                           };

            var lifeQuery = from l in DataContext.TB_Enterprise_Life
                            join e in DataContext.TB_Enterprise on l.EnterpriseId equals e.EnterpriseId
                            where etpIds.Contains(l.EnterpriseId) && l.CreateTime > sdate
                            select new EtpActivity()
                            {
                                EtpId = l.EnterpriseId,
                                EtpName = e.Name,
                                ActivityType = (int)EtpActivityType.Life,
                                Position = null,
                                Life = l,
                                CreateDate = l.CreateTime
                            };
            var query = posQuery.Union(lifeQuery);

            return new PagedResult<EtpActivity>()
            {
                Results = query.OrderByDescending(f => f.CreateDate).Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };
        }

        public int GetLatestEtpActivitiesCount(int userId,DateTime sdate)
        {
            if (userId == 0) return 0;
            var etpIds = DataContext.Stu_Follow.Where(f => f.TargetType == "Enterprise" && f.UserId == userId).Select(f => f.TargetId);
            var posQuery = from p in DataContext.TB_Position_Element
                           join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                           where p.PositionStatus == (int)PositionStatus.Publish && p.Deadline >= DateTime.Today
                           && etpIds.Contains(p.EnterpriseId) && p.CreateDate > sdate
                           select new EtpActivity()
                           {
                               EtpId = p.EnterpriseId,
                               EtpName = e.Name,
                               ActivityType = (int)EtpActivityType.Position,
                               Position = p,
                               Life = null,
                               CreateDate = p.CreateDate
                           };

            var lifeQuery = from l in DataContext.TB_Enterprise_Life
                            join e in DataContext.TB_Enterprise on l.EnterpriseId equals e.EnterpriseId
                            where etpIds.Contains(l.EnterpriseId) && l.CreateTime > sdate
                            select new EtpActivity()
                            {
                                EtpId = l.EnterpriseId,
                                EtpName = e.Name,
                                ActivityType = (int)EtpActivityType.Life,
                                Position = null,
                                Life = l,
                                CreateDate = l.CreateTime
                            };
            var query = posQuery.Union(lifeQuery);

            return query.Count();
        }

        #region 异步方法

        public async Task<int> GetEtpFansCountAsync(int enterpriseId)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from f in context.Stu_Follow
                            where f.TargetType == "Enterprise" && f.TargetId == enterpriseId
                            select f;
                return await query.CountAsync();
            }
        }

        public async Task<bool> IsEtpFollowedAsync(int etpID, int stuID)
        {
            if (stuID == 0) return false;
            using (TopucDB context = new TopucDB())
            {
                var query = from s in context.Stu_Follow
                            where s.TargetType == "Enterprise" && s.UserId == stuID && s.TargetId == etpID
                            select s;
                return await query.AnyAsync();
            }
        }
        #endregion

        public int[] GetStuFollowingIds(int stuID) 
        {
            return (from s in DataContext.Stu_Follow.Where(a => a.TargetType == "Enterprise")
                    where s.UserId == stuID
                    select s.TargetId).ToArray();
        }

        public async Task<IList<EtpInFollowDTO>> GetFollowedCompanyListAsync(int stuId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"
select b.EnterpriseId EnterpriseId, b.Name EnterpriseName,  dbo.fun_getCityName(b.City) CityNames
, dbo.fun_getIndustryName(b.Industry) IndustryNames
, COUNT(c.PositionId) ActiveJobCount 
, CONVERT(varchar(100), a.FollowDate, 20) FollowDate
from Stu_Follow a
left join TB_Enterprise b 
on a.TargetId = b.EnterpriseId and b.status <> 99
left join TB_Position_Element c 
on a.TargetId = c.EnterpriseId and c.Deadline >= getdate() and c.PositionStatus = 1
where a.UserId <> 0 and a.UserId = @stuId
and a.TargetType = 'Enterprise'
and ISNULL(b.EnterpriseId,0) > 0
group by b.EnterpriseId, b.Name, b.City, b.Industry,a.FollowDate
order by a.FollowDate desc";
                var list = (await conn.QueryAsync<EtpInFollowDTO>(sql, new { @stuId = stuId })).ToList();
                return list;
            }
        }

    }

    public interface IStuFollowRepo : IRepository<Stu_Follow>
    {
        /// <summary>
        /// 学生关注企业
        /// </summary>
        OperationResult FollowEtp(int etpID, int stuID);

        /// <summary>
        /// 学生取消关注企业
        /// </summary>
        int UnFollowEtp(int etpID, int stuID);


        /// <summary>
        /// 获取该学生关注的所有企业
        /// </summary>
        PagedResult<EtpInFollow> GetStuFollowing(int stuID, int page, int pageSize);


        /// <summary>
        /// 获取该学生关注的所有企业的数目（学生关注）
        /// </summary>
        int GetStuFollowingCount(int stuID);



        /// <summary>
        /// 获取关注该企业的所有学生的数目（企业粉丝）
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        int GetEtpFollowersCount(int enterpriseId);
        /// <summary>
        /// 学生是否关注了该企业
        /// </summary>
        /// <param name="etpID"></param>
        /// <param name="stuID"></param>
        /// <returns></returns>
        bool IsEtpFollowed(int etpID, int stuID);


        /// <summary>
        /// 获取关注该企业的所有学生的数目（企业粉丝）
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        Task<int> GetEtpFansCountAsync(int enterpriseId);

        /// <summary>
        /// 学生是否关注了该企业
        /// </summary>
        Task<bool> IsEtpFollowedAsync(int etpID, int stuID);


        PagedResult<EtpActivity> GetEtpActivities(int userId, int page, int pageSize);

        int GetLatestEtpActivitiesCount(int userId,DateTime sdate);

        int[] GetStuFollowingIds(int stuID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stuId"></param>
        /// <returns></returns>
        Task<IList<EtpInFollowDTO>> GetFollowedCompanyListAsync(int stuId);

    }
}
