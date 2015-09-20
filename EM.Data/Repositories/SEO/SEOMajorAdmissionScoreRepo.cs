using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    class SEOMajorAdmissionScoreRepo : RepositoryBase<SEO_Major_AdmissionScore>, ISEOMajorAdmissionScoreRepo
    {
        public SEOMajorAdmissionScoreRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int GetAverageAvgAdmissionScore(int majorId, string klType = "", int[] years = null)
        {
            var query = from entity in DataContext.SEO_Major_AdmissionScore
                        where entity.MajorId == majorId
                        select entity;
            if (!string.IsNullOrEmpty(klType))
            {
                query = query.Where(p => p.KLType == klType);
            }
            if (years != null && years.Count() > 0)
            {
                query = query.Where(p => p.Year != null && years.Contains(p.Year.Value));
            }
            var rslt = query.Select(p => p.AverageScore).Average();
            return (int)(rslt ?? 0);
        }

        public int GetMaxAvgAdmissionScore(int majorId, string klType = "", int[] years = null)
        {
            var query = from entity in DataContext.SEO_Major_AdmissionScore
                        where entity.MajorId == majorId
                        select entity;
            if (!string.IsNullOrEmpty(klType))
            {
                query = query.Where(p => p.KLType == klType);
            }
            if (years != null && years.Count() > 0)
            {
                query = query.Where(p => p.Year != null && years.Contains(p.Year.Value));
            }
            var rslt = query.Select(p => p.AverageScore).Max();
            return rslt ?? 0;
        }


        public SEO_Major_AdmissionScore GetHiggerAdmissionScoreEntity(int majorId, int year, int skipCount = 0, string klType = "")
        {
            var query = from entity in DataContext.SEO_Major_AdmissionScore
                        where entity.MajorId == majorId && entity.Year == year
                        select entity;
            if (!string.IsNullOrEmpty(klType))
            {
                query = query.Where(p => p.KLType == klType);
            }
            return query.OrderByDescending(p => p.AverageScore).Skip(skipCount).Take(1).FirstOrDefault();
        }
    }

    public interface ISEOMajorAdmissionScoreRepo : IRepository<SEO_Major_AdmissionScore>
    {
        /// <summary>
        /// 各高校平均分 再平均
        /// </summary>
        /// <param name="majorId"></param>
        /// <returns></returns>
        int GetAverageAvgAdmissionScore(int majorId, string klType = "", int[] years = null);
        /// <summary>
        /// 各高校平均分 最高的平均分
        /// </summary>
        /// <param name="majorId"></param>
        /// <returns></returns>
        int GetMaxAvgAdmissionScore(int majorId, string klType = "", int[] years = null);

        /// <summary>
        /// 获取某专业，录取平均分高的院校
        /// </summary>
        /// <param name="year">此参数作为required</param>
        SEO_Major_AdmissionScore GetHiggerAdmissionScoreEntity(int majorId, int year, int skipCount = 0, string klType = "");
    }
}
