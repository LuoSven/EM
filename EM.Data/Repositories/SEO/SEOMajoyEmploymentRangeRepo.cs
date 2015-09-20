using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    class SEOMajoyEmploymentRangeRepo : RepositoryBase<SEO_Majoy_EmploymentRange>, ISEOMajoyEmploymentRangeRepo
    {
        public SEOMajoyEmploymentRangeRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string GetMajorERangeByYear(int majorId, int year)
        {
            return (from entity in DataContext.SEO_Majoy_EmploymentRange
                    where entity.MajorId == majorId && entity.Year == year
                    select entity.Range).FirstOrDefault();
        }
    }


    public interface ISEOMajoyEmploymentRangeRepo : IRepository<SEO_Majoy_EmploymentRange>
    {
        string GetMajorERangeByYear(int majorId, int year);
    }
}
