using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using Topuc22Top.Common;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Data.Dapper;
using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public class CampusTalkRepo : RepositoryBase<TB_JobSeminar>, ICampusTalkRepo
    {
        private string[] invalidPeriod = { "", "0:00", "00:00", "待定" };
        public CampusTalkRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        /// <summary>
        /// 获取宣讲会信息列表
        /// </summary>
        /// <param name="cityName">宣讲会所在城市</param>
        /// <param name="schoolName">宣讲会所在学校</param>
        /// <param name="dtime">宣讲会时间：today，tomorrow，week，选择日期（自定义日期）</param>
        /// <param name="page">请求的页号</param>
        /// <param name="recCount">总记录条数</param>
        /// <returns>宣讲会信息列表</returns>
        public IList<TB_JobSeminar> GetCampusTalks(string keyword, string cityName, string schoolName, int dtime, int page, out int recCount, int pageSize = 20)
        {
            DateTime now = DateTime.Now.Date;
            var query = from f in DataContext.TB_JobSeminar
                        where f.OccurDate >= now
                        select f;
            //add 20150121 0点或待定不显示
            query = query.Where(f => !string.IsNullOrEmpty(f.OccurDuration) && !invalidPeriod.Contains(f.OccurDuration));
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
                    schoolName = schoolName.Replace("（", "(").Replace("）", ")");
                query = query.Where(f => f.College == schoolName);
            }

            if (dtime!=0)
            {
                DateTime toDate = DateTime.Now.Date;
                if (dtime ==(int)QuickDateType.Today)
                {
                    query = query.Where(f => f.OccurDate == now);
                }
                else if (dtime == (int)QuickDateType.Tomorrow)
                {
                    toDate = now.AddDays(1);
                    query = query.Where(f => f.OccurDate == toDate);
                }
                else if (dtime == (int)QuickDateType.Week)
                {
                    toDate = now.AddDays(7);
                    query = query.Where(f => f.OccurDate <= toDate);
                }
                else if (dtime == (int)QuickDateType.AfterWeek)
                {
                    toDate = now.AddDays(7);
                    query = query.Where(f => f.OccurDate >= toDate);
                }

            }

            recCount = query.Count();
            return query.OrderBy(f => f.OccurDate).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        /// 得到有宣讲会的城市
        /// </summary>
        /// <returns></returns>
        public IList<CampusTalkCityResult> GetCampusTalkCity()
        {
            var campusTalk = from f in DataContext.TB_JobSeminar
                             where f.OccurDate >= DateTime.Today && f.OccurDuration != null && !invalidPeriod.Contains(f.OccurDuration)
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
                       && di.ItemId != 2005 && di.ParentItemId != 2 //排除省份
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
                if (!string.IsNullOrEmpty(i.CityPinyin))
                {
                    i.FirstAlphabet = i.CityPinyin.ToUpper().Substring(0, 1);
                }
            }

            return list;
        }

        public IList<CollegeResult> GetSchoolNameByCity(string cityName)
        {
            DateTime now = DateTime.Now.Date;
            var query = from f in DataContext.TB_JobSeminar
                        where f.LocatedCity == cityName && f.OccurDate >= now && f.OccurDuration != null && !invalidPeriod.Contains(f.OccurDuration)
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

        public IList<TB_JobSeminar> GetJobSeminarByCollege(string college)
        {
            if (string.IsNullOrEmpty(college))
                return null;

            var query = from m in DataContext.TB_JobSeminar
                        where m.College == college && m.OccurDate >= DateTime.Today && m.OccurDuration != null && !invalidPeriod.Contains(m.OccurDuration)
                        select m;
            return query.OrderBy(n => n.OccurDate).ToList();
        }

        public IList<TB_JobSeminar> GetJobSeminarByJobSeminarName(string seminarName)
        {
            if (string.IsNullOrEmpty(seminarName))
                return null;

            var query = from m in DataContext.TB_JobSeminar
                        where m.OccurDate >= DateTime.Today && (m.SeminarName.Contains(seminarName.Replace("宣讲会", "")) || seminarName.Contains(m.SeminarName.Replace("宣讲会", "")))
                        select m;
            return query.OrderBy(n => n.OccurDate).ToList();
        }

        public IList<TB_JobSeminar> GetJobSeminar(int? cityId, DateTime? dtime)
        {
            var jobSeminarList = from f in DataContext.TB_JobSeminar
                                 where f.OccurDate >= DateTime.Now && f.OccurDuration != null && !invalidPeriod.Contains(f.OccurDuration)
                                 select f;
            if (cityId.HasValue)
            {
                var cities = from c in (DataContext.DictItem.Where(a => a.Type == "City"))
                             where c.ParentItemId == cityId || c.ItemId == cityId
                             select c;

                jobSeminarList = from c in cities
                                 join j in jobSeminarList
                                 on c.ItemName equals j.LocatedCity
                                 select j;
            }
            if (dtime.HasValue)
            {
                jobSeminarList = jobSeminarList.Where(f => f.OccurDate <= dtime.Value);
            }
            return jobSeminarList.ToList();
        }

        public int GetRecentCampusTalkCnt()
        {
            var query = from a in DataContext.TB_JobSeminar
                        where a.OccurDate >= DateTime.Today && a.OccurDuration != null && !invalidPeriod.Contains(a.OccurDuration)
                        select a;

            return query.Count();
        }

       public   IList<string> GetSchoolNameByList(IList<TB_JobSeminar> list)
        {
            var listresult = new List<string>();
            foreach (var item in list)
            {
                if (listresult.Where(o => o == item.College).Count() == 0 &&!string.IsNullOrEmpty(item.College))
                {
                    listresult.Add(item.College);
                }
            }
            return listresult;
        }


       public async Task<CampusTalkDTO> GetAsync(int id)
       {
           using(var conn = DapperHelper.Get22Connection())
           {
               string sql = "select * from TB_JobSeminar where SeminarId=@id";//select*
               var model = await conn.QueryAsync<CampusTalkDTO>(sql, new { id = id });
               return model.FirstOrDefault();
           }
       }
    }

    public interface ICampusTalkRepo : IRepository<TB_JobSeminar>
    {

        IList<TB_JobSeminar> GetCampusTalks(string keyword, string cityName, string schoolName, int dtime, int page, out int recCount, int pageSize = 20);
        IList<CampusTalkCityResult> GetCampusTalkCity();
        IList<CollegeResult> GetSchoolNameByCity(string cityName);
        IList<string> GetSchoolNameByList(  IList<TB_JobSeminar> list);
        IList<TB_JobSeminar> GetJobSeminarByCollege(string college);
        IList<TB_JobSeminar> GetJobSeminarByJobSeminarName(string seminarName);
        IList<TB_JobSeminar> GetJobSeminar(int? cityId, DateTime? dtime);

        int GetRecentCampusTalkCnt();

        Task<CampusTalkDTO> GetAsync(int id);
    }
}
