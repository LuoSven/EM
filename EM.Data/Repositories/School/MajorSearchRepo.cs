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
    public class MajorSearchRepo : RepositoryBase<MajorSearch>, IMajorSearchRepo
    {
        private readonly ICache cache;

        public MajorSearchRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public string GetMajorJobs(string major)
        {
            var majorCode = (from m in DataContext.MajorStandard
                             where m.Code.Length >= 6 && m.Major == major
                             select m.Code).FirstOrDefault();
            if (!string.IsNullOrEmpty(majorCode))
            {
                string secondCode = majorCode.Substring(0, 4);
                var query = from m in DataContext.MajorSearch
                            join n in DataContext.MajorStandard
                            on m.Major equals n.Major
                            where n.Code == secondCode
                            select m.MajorJobs;
                return query.FirstOrDefault();
            }
            return string.Empty;
        }
    }
    public interface IMajorSearchRepo : IRepository<MajorSearch>
    {
        string GetMajorJobs(string major);
    }
}
