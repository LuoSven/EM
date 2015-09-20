using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class MajorRelatedRepo : RepositoryBase<MajorRelated>, IMajorRelatedRepo
    {
        public MajorRelatedRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string GetMajorRelated(string major)
        {
            var query = from m in DataContext.MajorRelated
                        where m.Keyword == major
                        select m.KeywordsRelated;
            return query.FirstOrDefault();
        }
    }
    public interface IMajorRelatedRepo : IRepository<MajorRelated>
    {
        string GetMajorRelated(string major);
    }
}
