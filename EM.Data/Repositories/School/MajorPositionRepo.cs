using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class MajorPositionRepo : RepositoryBase<MajorPosition>, IMajorPositionRepo
    {
        private readonly ICache cache;

        public MajorPositionRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public string GetRelatedPositions(string majorCode)
        {
            var query = from m in DataContext.MajorPosition
                        where m.MajorCode == majorCode
                        select m;
            if (query.Any())
            {
                var major = query.FirstOrDefault();
                return major.RelatedPositions;
            }
            return string.Empty;
        }


         public string GetRelatedMajors(string majorCode)
        {
            var query = from m in DataContext.MajorPosition
                        where m.MajorCode == majorCode
                        select m;
            if (query.Any())
            {
                var major = query.FirstOrDefault();
                return major.RelatedMajors;
            }
            return string.Empty;
        }

        private string GetPositions(MajorPosition major)
        {
            IList<string> result = new List<string>();
            string postions = (major.FitPosition1 ?? "")
                + "," + (major.FitPosition2 ?? "")
                 + "," + (major.FitPosition3 ?? "")
                  + "," + (major.FitPosition4 ?? "")
                   + "," + (major.FitPosition5 ?? "")
                    + "," + (major.FitPosition6 ?? "")
                     + "," + (major.FitPosition7 ?? "")
                      + "," + (major.FitPosition8 ?? "")
                       + "," + (major.FitPosition9 ?? "")
                        + "," + (major.FitPosition10 ?? "")
                         + "," + (major.OtherPosition ?? "");
            string[] posList = ArrayConvertor.Convert(postions, ',');
            for (int i = 0; i < posList.Length; i++)
            {
                if (!string.IsNullOrEmpty(posList[i]))
                {
                    result.Add(posList[i]);
                }
            }
            return string.Join(" ", result);
        }

        public MajorPosition GetByMajorCode(string majorCode)
        {
            var query = from m in DataContext.MajorPosition
                        where m.MajorCode == majorCode
                        select m;
            if (query.Any())
            {
                var major = query.FirstOrDefault();
                return major;
            }
            return null;
        }

        public MajorPosition GetByMajorId(int majorId)
        {
            return (from entity in DataContext.MajorPosition
                    where entity.MajorID == majorId
                    select entity).FirstOrDefault();
        }

        public IList<MajorPosition> GetList(string majorCode, string majorName, int pagesize, int page, out int totalCnt)
        {
          var query = from m in DataContext.MajorPosition
                        select m;
            if (!string.IsNullOrEmpty(majorCode))
            {
                query = query.Where(m => m.MajorCode == majorCode);
            }
            if (!string.IsNullOrEmpty(majorName))
            {
                query = query.Where(m => m.Major == majorName);
            }
            totalCnt = query.Count();
            return query.OrderBy(m => m.MajorCode).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

    }

    public interface IMajorPositionRepo : IRepository<MajorPosition>
    {
        /// <summary>
        /// 获取专业相关职位关键词，以英文逗号分隔
        /// </summary>
        string GetRelatedPositions(string majorCode);

        /// <summary>
        /// 获取专业相关的专业关键词，以英文逗号分隔
        /// </summary>
        string GetRelatedMajors(string majorCode);

        MajorPosition GetByMajorCode(string majorCode);


        MajorPosition GetByMajorId(int majorId);

        IList<MajorPosition> GetList(string majorCode, string majorName, int pagesize, int page, out int totalCnt);

    }
}
