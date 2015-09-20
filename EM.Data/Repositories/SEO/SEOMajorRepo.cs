using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
namespace Topuc22Top.Data.Repositories
{
    class SEOMajorRepo : RepositoryBase<SEO_Major>, ISEOMajorRepo
    {
        public SEOMajorRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int DoFavor(int id)
        {
            if (id == 0) return -1;
            var seoMajor = DataContext.SEO_Major.Where(a => a.Id == id).FirstOrDefault();
            if (seoMajor != null)
            {
                if (seoMajor.FavorCount.HasValue)
                    seoMajor.FavorCount += 1;
                else
                    seoMajor.FavorCount = 1;
            }
            else
            {
                return -1;
            }
            DataContext.SaveChanges();
            return seoMajor.FavorCount.Value;
        }

    }

    public interface ISEOMajorRepo : IRepository<SEO_Major>
    {
        int DoFavor(int id);
    }
}
