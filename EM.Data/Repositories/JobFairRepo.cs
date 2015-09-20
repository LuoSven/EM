using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;
using Dapper;

namespace Topuc22Top.Data.Repositories
{

    public class JobFairRepo : RepositoryBase<TB_JobFair>, IJobFairRepo
    {
        private readonly ICache cache;
#if Debug
        private readonly int  cacheMinutes=1;
#else
        private readonly int cacheMinutes = 60;
#endif

        public JobFairRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public IList<TB_JobFair> GetJobFairList(string keyword, string cityName, string schoolName, string dtime, int page, int pageSize, out int recCount)
        {
            DateTime now = DateTime.Now.Date;
            var query = from f in DataContext.TB_JobFair
                        where f.OccurDate >= now
                        select f;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(f => f.JobFairName.Contains(keyword) || f.College.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(cityName))
            {
                query = query.Where(f => f.LocatedCity == cityName);
            }
            if (!string.IsNullOrEmpty(schoolName) && schoolName != "other") //选择大学
            {
                if (schoolName.Contains('（') || schoolName.Contains('）'))
                {
                    schoolName = schoolName.Replace("（", "(").Replace("）", ")");
                }
                query = query.Where(f => f.College.Contains(schoolName));
            }
            else if (!string.IsNullOrEmpty(schoolName)) //选择其他
            {
                query = query.Where(f => string.IsNullOrEmpty(f.College));
            }
            if (!string.IsNullOrEmpty(dtime))
            {
                DateTime toDate = DateTime.Now.Date;
                if (dtime == "today")
                {
                    query = query.Where(f => f.OccurDate == now);
                }
                else if (dtime == "tomorrow")
                {
                    toDate = now.AddDays(1);
                    query = query.Where(f => f.OccurDate == toDate);
                }
                else if (dtime == "week")
                {
                    toDate = now.AddDays(7);
                    query = query.Where(f => f.OccurDate <= toDate);
                }
                else if (dtime == "afterWeek")
                {
                    toDate = now.AddDays(7);
                    query = query.Where(f => f.OccurDate >= toDate);
                }
                else   //选择的是时间
                {
                    DateTime dt;
                    if (DateTime.TryParse(dtime, out dt))
                    {
                        DateTime date = dt.Date;
                        query = query.Where(f => f.OccurDate == date.Date);
                    }
                }
            }
            recCount = query.Count();
            return query.OrderBy(f => f.OccurDate).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        public IList<JobFairCityResult> GetJobFairCitys()
        {
            return cache.Get(Settings.Key_JobFair, () =>
            {
                DateTime now = DateTime.Now.Date;
                var query = from f in DataContext.TB_JobFair
                            where f.OccurDate >= now
                            group f by f.LocatedCity into g
                            select new
                            {
                                CityName = g.Key,
                                Cnt = g.Count()
                            };
                var city = from ct in query
                           join di in DataContext.DictItem
                           on ct.CityName equals di.ItemName
                           where di.Type == "City"
                           select new JobFairCityResult
                           {
                               CityId = di.ItemId,
                               CityName = ct.CityName,
                               CityPinyin = di.ItemValue,
                               Cnt = ct.Cnt
                           };
                var list = city.OrderByDescending(f => f.Cnt).ToList();
                foreach (var i in list)
                {
                    if (!string.IsNullOrEmpty(i.CityPinyin))
                    {
                        i.FirstAlphabet = i.CityPinyin.Substring(0, 1).ToUpper();
                    }
                }

                return list;
            }, cacheMinutes);
        }

        public IList<CollegeResult> GetSchoolListByCity(string cityName)
        {
            var temp = from f in DataContext.TB_JobFair
                       where f.LocatedCity == cityName && f.OccurDate >= DateTime.Today
                       group f by f.College into g
                       select new
                       {
                           College = g.Key,
                           Cnt = g.Count()
                       };
            var query = from t in temp
                        join s in DataContext.School
                        on t.College equals s.Name
                        select new CollegeResult
                        {
                            SchoolId = s.ID,
                            SchoolName = s.Name,
                            SchoolPinyin = s.Pinyin,
                            EtpCount = t.Cnt   //这里EtpCount当做每个学校招聘会的个数, 仅仅用于倒序排列
                        };
            return query.OrderByDescending(f => f.EtpCount).ToList();
        }

        public async Task<JobFairDTO> GetAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = "select * from TB_JobFair where JobFairId=@id";//select*
                var model = await conn.QueryAsync<JobFairDTO>(sql, new { id = id });
                return model.FirstOrDefault();
            }
        }

    }
    public interface IJobFairRepo : IRepository<TB_JobFair>
    {
        IList<TB_JobFair> GetJobFairList(string keyword, string cityName, string schoolName, string dtime, int page, int pageSize, out int recCount);

        IList<JobFairCityResult> GetJobFairCitys();

        IList<CollegeResult> GetSchoolListByCity(string cityName);

        Task<JobFairDTO> GetAsync(int id);
    }
}
