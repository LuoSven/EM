using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class ZwdsRepo : RepositoryBase<TB_Activity_ZWDS>, IZwdsRepo
    {
        public ZwdsRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
    public interface IZwdsRepo : IRepository<TB_Activity_ZWDS>
    {
    }
}
