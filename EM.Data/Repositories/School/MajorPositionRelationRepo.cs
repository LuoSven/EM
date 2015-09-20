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
    class MajorPositionRelationRepo : RepositoryBase<MajorPositionRelation>, IMajorPositionRelationRepo
    {
        private readonly ICache cache;

        public MajorPositionRelationRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public int GetMajorMaxSortScore(string majorCode)
        {
            return (from entity in DataContext.MajorPositionRelation
                    where entity.MajorCode == majorCode
                    select entity.SortScore).OrderByDescending(p => p).FirstOrDefault() ?? 0;
        }

        public object GetMajorPositionCheckDate(string majorCode, string posIds)
        {
            var posIdArr = posIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
            if (string.IsNullOrEmpty(majorCode))
                return null;
            return (from entity in DataContext.MajorPositionRelation
                    where entity.MajorCode == majorCode
                    && entity.CheckDate.HasValue
                    && posIdArr.Contains(entity.PositionId)
                    select new {
                        PositionId = entity.PositionId,
                        CheckDate = entity.CheckDate
                    }).ToList();
        }

    }

    public interface IMajorPositionRelationRepo : IRepository<MajorPositionRelation>
    {
        int GetMajorMaxSortScore(string majorCode);

        object GetMajorPositionCheckDate(string majorCode, string posIds);

    }

}
