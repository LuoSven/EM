using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class MajorStandardRepo : RepositoryBase<MajorStandard>, IMajorStandardRepo
    {
        private readonly ICache cache;

        public MajorStandardRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public IList<MajorStandard> GetRelatedMajors(string major)
        {
            return cache.Get(string.Format(Settings.ParaKey_RelatedMajorList, major), () =>
            {
                IList<MajorStandard> list = new List<MajorStandard>();
                var majorList = from it in DataContext.MajorStandard
                                 where it.Major == major
                                 select it;
                if (majorList.Any())
                {
                    string pcode = majorList.First().Code.Substring(0, 2);
                    var query = from it in DataContext.MajorStandard
                                where it.Code.StartsWith(pcode) && it.Code.Length >= 6
                                && it.Major != major && it.DisplayLevel==2
                                select it;
                    list = query.ToList();
                }
                return list;
            });
        }

        public IList<MajorStandard> GetTopMajors()
        {
            var query = from it in DataContext.MajorStandard
                        where it.ParentCode == null
                        select it;
            return query.OrderBy(it => it.DisplayOrder).ToList();
        }

        public IList<MajorStandard> GetSubMajors(string pcode)
        {
            if (string.IsNullOrEmpty(pcode)) return new List<MajorStandard>();

            var query = from it in DataContext.MajorStandard
                        where it.ParentCode == pcode
                        select it;
            return query.OrderBy(it => it.Code).ToList();
        }


        public string GetMajorNames(string MajorIds, string joinCharacter)
        {
            if (string.IsNullOrEmpty(MajorIds)) return string.Empty;
            if (MajorIds == "0") return "不限";
            string[] ids = MajorIds.Split(',');
            var query = from it in DataContext.MajorStandard
                        where ids.Contains(it.Code)
                        select it.Major;
            return string.Join(joinCharacter, query.ToArray());
        }

        /// <summary>
        /// 获取专业三级Code
        /// </summary>
        /// <param name="major"></param>
        /// <returns></returns>
        public string GetMajorL3Code(string major)
        {
            if (string.IsNullOrEmpty(major)) return string.Empty;

            var query = from m in DataContext.MajorStandard
                        where m.Major == major&&m.Code.Length>=6
                        select m.Code;
            return query.FirstOrDefault();
        }

        public IList<MajorStandard> GetHomeMajors()
        {
            var query = from m in DataContext.MajorStandard
                        where m.DisplayLevel==2
                        select m;
            return query.ToList();
        }


        public IList<MajorStandard> GetHomeSubMajors(string pcode)
        {
            var query = from m in DataContext.MajorStandard
                        where m.DisplayLevel == 2 &&m.Code.Length>=6
                        select m;
            if(!string.IsNullOrEmpty(pcode))
            {
                query = query.Where(m => m.Code.StartsWith(pcode));
            }
            return query.ToList();
        }


        public IList<MajorStandard> GetL2Majors()
        {
            var query = from m in DataContext.MajorStandard
                        where m.Code.Length==4
                        select m;
            return query.ToList();
        }

        public IList<MajorStandard> GetL3Majors()
        {
            var query = from m in DataContext.MajorStandard
                        where m.Code.Length > 4
                        select m;
            return query.ToList();
        }


        public string GetMajorCodeByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            name = name.Trim();
            var query = from m in DataContext.MajorStandard
                        where m.Major == name
                        select m.Code;
            return query.FirstOrDefault();

        }

        public string GetMajorPYByName(string name) 
        {
            name = name.Trim();
            var query = from m in DataContext.MajorStandard
                        where m.Major == name
                        select m.PY;
            return query.FirstOrDefault();
        }

        public IList<MajorStandard> GetMatchedL3Majors(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return null;
            keyword = keyword.Trim();
            var query = from m in DataContext.MajorStandard
                        where m.Code.Length > 4 && m.Major.Contains(keyword) || (keyword.Length >= 4 && m.PY.Contains(keyword))
                        select m;
            return query.ToList();
        }

    }
    public interface IMajorStandardRepo : IRepository<MajorStandard>
    {
        IList<MajorStandard> GetHomeMajors();
        IList<MajorStandard> GetRelatedMajors(string major);

        IList<MajorStandard> GetTopMajors();

        IList<MajorStandard> GetL2Majors();


        IList<MajorStandard> GetL3Majors();

        IList<MajorStandard> GetSubMajors(string pcode);

        string GetMajorNames(string MajorIds, string joinCharacter);

        string GetMajorL3Code(string major);

        /// <summary>
        /// 获取专业所在分类
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetMajorCodeByName(string name);

        IList<MajorStandard> GetHomeSubMajors(string pcode);

        string GetMajorPYByName(string name);

        IList<MajorStandard> GetMatchedL3Majors(string keyword);

    }

}
