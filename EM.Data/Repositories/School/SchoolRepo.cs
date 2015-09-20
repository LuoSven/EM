using System.Collections.Generic;
using System.Linq;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using Topuc.Framework.Cache;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Data.Dapper;
using System;

using Dapper;
using Dapper.Contrib.Extensions;

namespace Topuc22Top.Data.Repositories
{
    public class SchoolRepo: RepositoryBase<School>, ISchoolRepo
    {
        private readonly ICache cache;

        public SchoolRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public int GetIdByName(string name)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select ID from School where Name = @name and DeleteBy is null and DeleteDate is null";
                var result = conn.Query<int>(sql, new { @name = name });
                return result.FirstOrDefault();
            }
        }

        public IList<School> GetList(string city, string level, string major)
        {
            var query = GetSchoolQuery(city, level, major);
            //暂时先取前50所符合条件的学校
            //return query.Take(50).ToList();
            return query.ToList(); 
        }

        public int GetCityByName(string name) 
        {
            return (from m in DataContext.School
                      where m.Name == name
                    select m.CityID)
                      .FirstOrDefault();
        }

        public int GetLevelByName(string name)
        {
            return (from m in DataContext.School
                    where m.Name == name
                    select (m.SchoolLevel ?? 0))
                      .FirstOrDefault();
        }

        public string GetSchoolNameByPinyin(string schoolPinyin)
        {
            if (string.IsNullOrEmpty(schoolPinyin)) return string.Empty;
            var query = from f in DataContext.School
                        where f.Pinyin == schoolPinyin
                        select f.Name;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return string.Empty;
        }

        public int GetSchoolsCount(string city, string level, string major)
        {
            var query = GetSchoolQuery(city, level,major);
            return query.Count();
        }

        private IQueryable<School> GetSchoolQuery(string city, string level, string major = "0")
        {
            var query = from s in DataContext.School
                        select s;
            if (!string.IsNullOrEmpty(city) && city != "0")
            {
                var cityIdArr = city.Split(',').Select(cityItemStr =>
                {
                    int cityItemId = 0;
                    int.TryParse(cityItemStr, out cityItemId);
                    return cityItemId;
                }).Where(p => p > 0).Distinct().ToArray();
                if (cityIdArr != null && cityIdArr.Length > 0)
                {
                    query = query.Where(s => cityIdArr.Contains(s.CityID) || cityIdArr.Contains(s.CityID / 1000));
                }
                else
                {
                    return (from s in DataContext.School where s.ID == -1 select s); //参数格式不对
                }
            }
            if (!string.IsNullOrEmpty(level) && level != "0")
            {
                var levelIdArr = level.Split(',').Select(levelItemStr =>
                {
                    int levelItemId = 0;
                    int.TryParse(levelItemStr, out levelItemId);
                    return levelItemId;
                }).Where(p => p > 0).Distinct().ToArray();
                if (levelIdArr != null && levelIdArr.Count() > 0)
                {
                    query = query.Where(s => s.SchoolLevel.HasValue && levelIdArr.Contains(s.SchoolLevel.Value));
                }
                else 
                {
                    return (from s in DataContext.School where s.ID == -1 select s); //参数格式不对
                }
            }
            if (!string.IsNullOrEmpty(major) && major != "0")
            {
                var ids = new List<int>() { };
                major.Split(',').ToList().ForEach(majorItemStr =>
                {
                    if (!string.IsNullOrEmpty(majorItemStr))
                    {
                        //三级专业需要转换为二级专业查询
                        if(majorItemStr.Length>=6)
                        {
                            majorItemStr = majorItemStr.Substring(0, 4);
                        }
                        var schoolIds = (from m in DataContext.School_Major where m.MajorCode.StartsWith(majorItemStr) select m.SchoolId).ToList();
                        ids = ids.Union(schoolIds).ToList();
                    }
                });
                query = query.Where(s => ids.Contains(s.ID));
            }

            if ((string.IsNullOrEmpty(city) || city == "0")
                && (string.IsNullOrEmpty(level) || level == "0")
                && (string.IsNullOrEmpty(major) || major == "0"))
                return query.Take(50);

            return query;
        }


        #region Dapper

        public PagedResult<SchoolSimpleInfoDTO> GetSimpleInfoList(string name, int cityId, int schoolLevel, int schoolType, bool? hasContact, int page, int pageSize)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select ID, Name, CityName, schoolLevels, SchoolType  from School
where 
DeleteBy is null and DeleteDate is null
and (isnull('{0}','') = '' or Name like '%{0}%')
and (@cityId = 0 or CityID = @cityId)
and (@schoolLevel = '0' or SchoolLevel like '%' + @schoolLevel + '%')
and (@schoolType = 0 or isnull(SchoolType,0) = @schoolType)
and (@hasContact = 0 or (ID in (select SchoolId from School_Contact)))
", (name ?? "").ToSqlLink());

                var result = conn.QueryWithPage<SchoolSimpleInfoDTO>(sql, new
                {
                    @cityId = cityId
                    ,
                    @schoolLevel = schoolLevel.ToString(),
                    @schoolType = schoolType,
                    @hasContact = (hasContact ?? false) ? 1 : 0
                }, "Name", page, pageSize);

                return result;
            }
        }

        public string GetNameById(int id) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Name from School where ID = @id and DeleteBy is null and DeleteDate is null";
                var result = conn.Query<string>(sql, new{@id = id});
                return result.FirstOrDefault();
            }
        }

        public School GetScoolById(int id) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select * from School where ID = @id and DeleteBy is null and DeleteDate is null";
                var result = conn.Query<School>(sql, new { @id = id });
                return result.FirstOrDefault();
            }
        }

        public SchoolDTO GetScoolDTOById(int id) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select * from School where ID = @id and DeleteBy is null and DeleteDate is null";
                var result = conn.Query<SchoolDTO>(sql, new { @id = id });
                return result.FirstOrDefault();
            }
        }

        public int AddSchool(SchoolDTO school) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Insert<SchoolDTO>(school);
                return (int)result;
            }
        }

        public bool UpdateSchool(SchoolDTO school)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Update<SchoolDTO>(school);
                return result;
            }
        }

        public string GetCommentById(int id) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var sql = @"select Comment from School where ID = @id and DeleteBy is null and DeleteDate is null";
                var result = conn.Query<string>(sql, new { @id = id });
                return result.FirstOrDefault();
            }
        }

        public void UpdateComment(int id, string comment) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var sql = "update School set Comment = @comment, LastUpdateTime = getdate() where ID = @id and DeleteBy is null and DeleteDate is null";
                var result = conn.Execute(sql, new {@id = id, @comment = comment });
            }
        }

        public void DeleteSchool(int id, string deleteBy)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var sql = "update School set DeleteBy = @deleteBy, DeleteDate = @deleteDate where ID = @id";
                var result = conn.Execute(sql, new { @id = id, @deleteBy = deleteBy, @deleteDate = DateTime.Now });
            }
        }

        #endregion

    }

    public interface ISchoolRepo : IRepository<School>
    {

        int GetIdByName(string name);

        IList<School> GetList(string city, string level, string major);

        int GetCityByName(string name);
        int GetLevelByName(string name);

        string GetSchoolNameByPinyin(string schoolPinyin);

        /// <summary>
        /// 获取符合条件的学校数量
        /// </summary>
        /// <param name="city"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        int GetSchoolsCount(string city, string level, string major);
        /// <summary>
        /// Admin获取学校列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cityId"></param>
        /// <param name="schoolLevel"></param>
        /// <param name="schoolType"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedResult<SchoolSimpleInfoDTO> GetSimpleInfoList(string name, int cityId, int schoolLevel, int schoolType, bool? hasContact, int page, int pageSize);

        string GetNameById(int id);

        School GetScoolById(int id);
        SchoolDTO GetScoolDTOById(int id);

        int AddSchool(SchoolDTO school);
        bool UpdateSchool(SchoolDTO school);
        string GetCommentById(int id);
        void UpdateComment(int id, string comment);

        void DeleteSchool(int id, string deleteby);
    }
}
