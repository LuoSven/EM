using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    class SEOMajorNewsRepo : RepositoryBase<SEO_Major_News>, ISEOMajorNewsRepo
    {
        public SEOMajorNewsRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IList<SEO_Major_News> GetNewsListByMajorId(int majorId, int takeNo = 10) 
        {
            if (takeNo <= 0) takeNo = 10;
            return (from entity in DataContext.SEO_Major_News
                    where entity.MajorId == majorId
                    select entity).OrderByDescending(p => p.PublishTime).Take(takeNo).ToList();
        }

    }

    public interface ISEOMajorNewsRepo : IRepository<SEO_Major_News>
    {
        IList<SEO_Major_News> GetNewsListByMajorId(int majorId, int takeNo = 10) ;

    }
}
