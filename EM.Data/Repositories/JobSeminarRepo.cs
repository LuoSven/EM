using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Common;
using Topuc22Top.Data.ResultModel;
using Topuc.Framework.Cache;

namespace Topuc22Top.Data.Repositories
{
    public class JobSeminarRepo : RepositoryBase<TB_JobSeminar>, IJobSeminarRepo
    {
        private readonly ICache cache;
#if Debug
        private readonly int  cacheMinutes=1;
#else
        private readonly int cacheMinutes = 60;
#endif

        public JobSeminarRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public async Task<IList<TB_JobSeminar>> GetListAsync(int enterpriseId, int? count)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from m in context.TB_JobSeminar
                            join n in context.TB_Enterprise_JobSeminar
                            on m.SeminarId equals n.SeminarId
                            where n.EnterpriseId == enterpriseId && m.OccurDate >= DateTime.Today
                            select m;
                if (count.HasValue)
                {
                    return await query.OrderBy(x => x.OccurDate).Take(count.Value).ToListAsync();
                }

                return await query.OrderBy(x => x.OccurDate).ToListAsync();
            }
        }

        public IList<TB_JobSeminar> GetCampusTalks(string keyword, string cityName, string schoolName, string dtime, int page, int pageSize, out int recCount)
        {
            DateTime now = DateTime.Now.Date;
            var query = from f in DataContext.TB_JobSeminar
                        where f.OccurDate >= now
                        select f;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(f => f.SeminarName.Contains(keyword) || f.College.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(cityName))
            {
                query = query.Where(f => f.LocatedCity == cityName);
            }
            if (!string.IsNullOrEmpty(schoolName))
            {
                if (schoolName.Contains('（') || schoolName.Contains('）'))
                {
                    schoolName = schoolName.Replace("（", "(").Replace("）", ")");
                }
                query = query.Where(f => f.College.Contains(schoolName));
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

        public IList<CampusTalkCityResult> GetCampusTalkCitys()
        {
            return cache.Get(Settings.Key_CampusTalkCity, () =>
            {
                var campusTalk = from f in DataContext.TB_JobSeminar
                                 where f.OccurDate >= DateTime.Today
                                 group f by f.LocatedCity into g
                                 select new
                                 {
                                     CityName = g.Key,
                                     Cnt = g.Count()
                                 };
                var city = from ct in campusTalk
                           join di in DataContext.DictItem
                           on ct.CityName equals di.ItemName
                           where di.Type == "City"
                           && di.ItemId != 2005 //排除吉林省，因为吉林省和吉林市的名称都是吉林
                           select new CampusTalkCityResult
                           {
                               CityId = di.ItemId,
                               CityName = ct.CityName,
                               CityPinyin = di.ItemValue,
                               Cnt = ct.Cnt
                           };
                var list = city.OrderByDescending(f => f.Cnt).ToList();
                foreach (var i in list)
                {
                    //if (i.CityName == "长沙" || i.CityName == "重庆" || i.CityName == "长春")
                    //{
                    //    i.FirstAlphabet = "C";
                    //}
                    //else if (i.CityName == "厦门")
                    //{
                    //    i.FirstAlphabet = "X";
                    //}
                    //else
                    //{
                    //    i.FirstAlphabet = Pinyin.GetFirstAlphabet(i.CityName);
                    //}
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
            var query = from f in DataContext.TB_JobSeminar
                        where f.LocatedCity == cityName && f.OccurDate >= DateTime.Today
                        group f by f.College into g
                        select new
                        {
                            College = g.Key,
                            Cnt = g.Count()
                        };
            var school = from q in query
                         join s in DataContext.School
                         on q.College equals s.Name
                         select new CollegeResult
                         {
                             SchoolId = s.ID,
                             SchoolName = s.Name,
                             SchoolPinyin = s.Pinyin,
                             EtpCount = q.Cnt   //这个EtpCount是每个学校的宣讲会个数 仅用于排序大学
                         };
            return school.OrderByDescending(f => f.EtpCount).ToList();
        }

    }
    public interface IJobSeminarRepo : IRepository<TB_JobSeminar>
    {
        /// <summary>
        /// 20150817 sven 定位到执行时间较长的sql，目前已经没地方调用不用，先留着，万一以后要用
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IList<TB_JobSeminar>> GetListAsync(int enterpriseId, int? count);

        IList<TB_JobSeminar> GetCampusTalks(string keyword, string cityName, string schoolName, string dtime, int page, int pageSize, out int recCount);

        IList<CampusTalkCityResult> GetCampusTalkCitys();

        IList<CollegeResult> GetSchoolListByCity(string cityName);
    }
}
