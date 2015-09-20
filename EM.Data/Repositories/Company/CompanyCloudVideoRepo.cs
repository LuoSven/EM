using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    class CompanyCloudVideoRepo : RepositoryBase<TB_Enterprise_CloudVideo>, ICompanyCloudVideoRepo
    {
        public CompanyCloudVideoRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

    }

    public interface ICompanyCloudVideoRepo : IRepository<TB_Enterprise_CloudVideo>
    {

    }
}
