using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

using Dapper;

namespace Topuc22Top.Data.Repositories
{

    public class BDStatisticDailyRepo : RepositoryBase<BDStatistic_Daily>, IBDStatisticDailyRepo
    {
        public BDStatisticDailyRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IList<BDStatistic_Daily> GetList(int page, int pageSize, out int totalCnt) 
        {
            var query =  (from entity in DataContext.BDStatistic_Daily select entity);

            totalCnt = query.Count();

            return query.OrderByDescending(p => p.CreateDate).ThenBy(p => p.UserName)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();

        }

        public IList<BDStatistic_Daily> GetDayList(DateTime dt) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
declare @UserName varchar(128),
		@today datetime,
		@followCnt int,
		@activeCnt int,
		@processedApplyCnt int,
		@communicateCnt int,
		@posCnt int
		
declare @curDate date
set @curDate = '{0}'
declare @date date
select @date = DATEADD(DD,0,@curDate)
declare @date2 date
select @date2 = DATEADD(DD,1,@date)

	select t2.UserName UserName, t.today CreateDate, m.followCnt FollowCnt, n.activeCnt ActiveCnt, o.processedApplyCnt ProcessedApplyCnt, p.communicateCnt CommunicateCnt
, q.posCnt PosCnt

	from
	(
		select UserId AssignTo, @date today  from [22TopWeb].dbo.UserProfile
	) t

	left join 
	(
		--跟进数量
		select AssignTo,COUNT(distinct EtpID) followCnt from [22TopWeb].dbo.AssignList
		where FollowDate is not null and FollowDate >= @date and FollowDate < @date2
		group by AssignTo
	) m
	on t.AssignTo = m.AssignTo

	left join
	(
		--激活数量
		select a.AssignTo AssignTo,COUNT(distinct b.EnterpriseId) activeCnt
		from [22TopWeb].dbo.AssignList a join [22TopWeb].dbo.TB_Enterprise_Account b
		on a.EtpID = b.EnterpriseId
		where b.CreateDate is not null and b.CreateDate >= @date and b.CreateDate < @date2
		group by a.AssignTo
	) n
	on t.AssignTo=n.AssignTo

	left join
	(
		--简历处理数
		select a.AssignTo AssignTo,COUNT(distinct b.ApplyId) processedApplyCnt
		from [22TopWeb].dbo.AssignList a join [22TopWeb].dbo.TB_S_Position b
		on a.EtpID = b.EnterpriseId
		where b.ApplyStatus not in (1,21)
		and b.CreateDate is not null and b.CreateDate >= @date and b.CreateDate < @date2
		group by a.AssignTo
	 ) o
	on t.AssignTo=o.AssignTo

	left join 
	(
		--沟通企业数
		select userId AssignTo, count(distinct EnterpriseId) communicateCnt 
		from [22TopWeb].dbo.TB_Enterprise_Communication where ThenStatus >= 2
		and CreateDate is not null and CreateDate >= @date and CreateDate < @date2
		group by userId
	) p 
	on t.AssignTo = p.AssignTo

	left join
	(
		--职位发布数
		select a.AssignTo AssignTo,COUNT(distinct b.PositionId) posCnt
		from [22TopWeb].dbo.AssignList a join [22TopWeb].dbo.TB_Position_Element b
		on a.EtpID = b.EnterpriseId
		where b.PositionStatus <> 99
		and b.CreateDate is not null and b.CreateDate >= @date and b.CreateDate < @date2
		group by a.AssignTo
	) q
	on t.AssignTo = q.AssignTo

	left join [22TopWeb].dbo.UserProfile t2 on t.AssignTo = t2.UserId

	where 
	len(isnull(UserName,'')) > 0 and
	(ISNULL(followCnt,0)<>0 or ISNULL(activeCnt,0)<>0 or ISNULL(processedApplyCnt,0)<>0 or ISNULL(communicateCnt,0)<>0 or ISNULL(posCnt,0)<>0)
	order by UserName", dt.ToString("yyyy-MM-dd"));
                return conn.Query<BDStatistic_Daily>(sql).ToList();
            } 
        }

        public IList<BDStatistic_Daily> GetList(string userName, string userIds, DateTime startDate, DateTime endDate) 
        {
            var date1 = startDate.Date;
            var date2 = endDate.AddDays(1).Date;
            if (date2 > DateTime.Now.Date) date2 = DateTime.Now.Date;
            var query = from entity in DataContext.BDStatistic_Daily
                        where entity.CreateDate >= date1 && entity.CreateDate < date2
                        select entity;
            if (!string.IsNullOrEmpty(userName))
                query = query.Where(p => p.UserName.Contains(userName));
            if (!string.IsNullOrEmpty(userIds))
            {
                var userIdArr = userIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p =>
                {
                    int i = 0;
                    int.TryParse(p, out i);
                    return i;
                }).Where(p => p > 0).Distinct();
                var userNameArr = (from u in DataContext.UserProfile where userIdArr.Contains(u.UserId) select u.UserName);
                query = query.Where(p => userNameArr.Contains(p.UserName));
            }
            return query.OrderBy(p => p.UserName).ThenByDescending(p => p.CreateDate).ToList();
        }

    }

    public interface IBDStatisticDailyRepo : IRepository<BDStatistic_Daily>
    {
        IList<BDStatistic_Daily> GetList(int page, int pageSize, out int totalCnt);
        /// <summary>
        /// 获取某天的统计数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        IList<BDStatistic_Daily> GetDayList(DateTime dt);

        IList<BDStatistic_Daily> GetList(string userName, string userIds, DateTime startDate, DateTime endDate);

    }

}
